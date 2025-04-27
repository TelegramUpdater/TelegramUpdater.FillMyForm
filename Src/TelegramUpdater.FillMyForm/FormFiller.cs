using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.FillMyForm.Converters;
using TelegramUpdater.FillMyForm.UpdateCrackers;
using TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;
using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Your form filler class.
/// </summary>
/// <typeparam name="TForm">
/// Your form that has some readable and writable properties as inputs.
/// <para><typeparamref name="TForm"/> <b>should have a parameterless ctor</b>.</para>
/// </typeparam>
public sealed class FormFiller<TForm> where TForm : IForm, new()
{
    private readonly Dictionary<string, IUpdateCracker> _propertyCrackers;
    private readonly string[] validProps;
    private readonly PropertyFillingInfo[] propertyFillingInfo;
    private readonly ICancelTrigger? _defaultCancelTrigger;
    private readonly IEnumerable<Type> _convertersByType;
    private readonly List<IFormPropertyConverter> _converters;

    /// <summary>
    /// Create an instance of form filler.
    /// </summary>
    /// <param name="updater">Updater instance.</param>
    /// <param name="buildCrackers">Add crackers to the filler.</param>
    /// <param name="additionalConverters">Add converters for other data types, int long float supported.</param>
    /// <param name="defaultCancelTrigger">A default cancel trigger to use for all.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public FormFiller(
        IUpdater updater,
        Action<CrackerContext<TForm>>? buildCrackers = default,
        IEnumerable<Type>? additionalConverters = default,
        ICancelTrigger? defaultCancelTrigger = default)
    {
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));

        _propertyCrackers = new(StringComparer.Ordinal);
        _defaultCancelTrigger = defaultCancelTrigger;

        // Default converters.
        _convertersByType =
        [
            typeof(StringConverter),
            typeof(IntegerConverter),
            typeof(LongConverter),
            typeof(FloatConverter),
        ];

        if (additionalConverters != null)
        {
            _convertersByType = _convertersByType.Union(additionalConverters);
        }

        _converters = [];

        InitConverters();

        var validProperties = GetValidProperties();
        validProps = validProperties.Select(x=>x.Name).ToArray();

        if (buildCrackers != null)
        {
            var ctx = new CrackerContext<TForm>();
            buildCrackers(ctx);

            ctx.Build(this);
        }

        propertyFillingInfo = [.. validProperties
            .Select(x =>
            {
                var attributeInfo = x.GetCustomAttribute<FormPropertyAttribute>();

#if NET8_0_OR_GREATER
                // get retry options
                var retryOptions = x.GetCustomAttributes<FillPropertyRetryAttribute>()
                    .DistinctBy(x => x.FillingError);
#else
                // get retry options
                var retryOptions = x.GetCustomAttributes<FillPropertyRetryAttribute>()
                    .GroupBy(x => x.FillingError).Select(x=> x.First());
#endif
                var fillingInfo = new PropertyFillingInfo(
                    x,
                    attributeInfo?.Priority?? 0,
                    TimeSpan.FromSeconds(attributeInfo?.TimeOut??30));

                fillingInfo.RetryAttributes.AddRange(retryOptions);

                fillingInfo.Required = x.GetCustomAttribute<RequiredAttribute>() != null;

                if (!_propertyCrackers.ContainsKey(x.Name))
                {
                    if (GetConverterForType(x.PropertyType) == null)
                        throw new InvalidOperationException($"No converter for type {x.PropertyType}!");

                    AddCracker(x.Name, new DefaultCracker(
                        fillingInfo.TimeOut, attributeInfo?.CancelTrigger));
                }

                return fillingInfo;
            })
            .OrderBy(x=> x.Priority),];
    }

    /// <summary>
    /// The updater.
    /// </summary>
    public IUpdater Updater { get; init; }

    /// <summary>
    /// Start filling the form.
    /// </summary>
    /// <param name="user">The user to ask from.</param>
    /// <param name="cancellationToken">Cancel the filling process.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TForm?> FillAsync(User user, CancellationToken cancellationToken = default)
    {
        FormFillerContext<TForm> FillerCtx(string propertyName)
            => new(this, user, propertyName);

#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(user);
#else
        if (user is null)
            throw new ArgumentNullException(nameof(user));
#endif

        // TODO: get user's queue id from user id and rainbow.
        //      Check if provided user id owns any queue.
        ushort queueId = Updater.Rainbow.GetOwnersQueue(user.Id)
            ?? throw new InvalidOperationException("This user has no active queue.");

        // Create a new instance of form
        TForm _form = new();

        foreach (var property in propertyFillingInfo)
        {
            await _form.OnBeginAskAsync(FillerCtx(property.PropertyInfo.Name), cancellationToken).ConfigureAwait(false);

            var cracker = GetCracker(property.PropertyInfo.Name);

            while (true)
            {
                var update = await Updater.Rainbow.ReadNextAsync(
                    queueId, cracker.UpdateChannel.TimeOut, cancellationToken).ConfigureAwait(false);

                var timedOut = update == null;

                if (timedOut) // timed out!
                {
                    var timeOutRetry = property.GetRetryOption(FillingError.TimeoutError);

                    await _form.OnTimeOutAsync(FillerCtx(property.PropertyInfo.Name),
                        new TimeoutContext(
                            FormFiller<TForm>.CreateRetryContext(timeOutRetry),
                            property.TimeOut),
                        cancellationToken).ConfigureAwait(false);

                    if (FormFiller<TForm>.CheckRetryOptions(timeOutRetry))
                        continue;
                }

                var cancelled = false;
                object? input;

                if (cracker.CancelTrigger != null)
                {
                    cancelled = cracker.CancelTrigger.ShouldCancel(update!.Value);
                }
                else // Check default cancel trigger
                {
                    if (_defaultCancelTrigger is not null)
                    {
                        cancelled = _defaultCancelTrigger.ShouldCancel(update!.Value);
                    }
                }

                // NOTE: something that is canceled, should not do retry.

                // Do not try converting if it's time out or canceled
                // Since there's no suitable update to convert in here!
                // Go directly to the set value and validations.
                if (timedOut || cancelled)
                {
                    // Canceled or timed out
                    if (cancelled)
                    {
                        // Update can't be null.
                        await _form.OnCancelAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new OnCancelContext(update!),
                            cancellationToken).ConfigureAwait(false);
                    }

                    input = null;
                }
                else // Not timed out and not canceled.
                {
                    // Update not null here.
                    if (!cracker.UpdateChannel.ShouldChannel(Updater, update!.Value))
                    {
                        await UnRelated(_form, user, property.PropertyInfo.Name, update, cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    // Can't crack it? it's an invalid input then.
                    if (!TryCrackingIt(cracker, property.Type, update.Value, out input))
                    {
                        var convertOption = property.GetRetryOption(FillingError.ConvertingError);

                        await _form.OnConversationErrorAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new ConversationErrorContext(
                                FormFiller<TForm>.CreateRetryContext(convertOption),
                                property.Type,
                                update),
                            cancellationToken).ConfigureAwait(false);

                        if (FormFiller<TForm>.CheckRetryOptions(convertOption))
                            continue;

                        // Set input value to null since we can't convert it to requested type.
                        input = null;
                    }
                }

                // Validating phase
                var retryOption = property.GetRetryOption(FillingError.ValidationError);

                if (input != null)
                {
                    // Failed to set value? then it's a validation error
                    if (!FormFiller<TForm>.TrySetPropertyValue(_form, property, input, out var validationResults))
                    {
                        await _form.OnValidationErrorAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new ValidationErrorContext(
                                FormFiller<TForm>.CreateRetryContext(retryOption),
                                update,
                                RequiredItemNotSupplied: false,
                                validationResults),
                            cancellationToken).ConfigureAwait(false);

                        if (FormFiller<TForm>.CheckRetryOptions(retryOption))
                            continue;

                        // Still invalid? no chance.
                        return default;
                    }
                }
                else
                {
                    // It can't be null if it's required.
                    if (property.Required)
                    {
                        await _form.OnValidationErrorAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new ValidationErrorContext(
                                FormFiller<TForm>.CreateRetryContext(retryOption),
                                update,
                                RequiredItemNotSupplied: true,
                                []),
                            cancellationToken).ConfigureAwait(false);

                        if (!cancelled) // Don't retry if it's canceled.
                        {
                            if (FormFiller<TForm>.CheckRetryOptions(retryOption))
                                continue;
                        }

                        // Still invalid? no chance.
                        return default;
                    }
                }

                // if it's timeout or cancel but not required then the update is null but success.
                await _form.OnSuccessAsync(
                    FillerCtx(property.PropertyInfo.Name),
                    new OnSuccessContext(input, update),
                    cancellationToken).ConfigureAwait(false);
                break;
            }
        }

        return _form;
    }

    private bool TryCrackingIt(
        IUpdateCracker cracker, Type propertyType, Update update, out object? input)
    {
        if (cracker is DefaultCracker defaultCracker)
        {
            var converter = GetConverterForType(propertyType);

            // Converter can't be null! since it checked in ctor.
            return defaultCracker.TryReCrack(update, converter!, out input);
        }

        return cracker.TryCrack(update, out input);
    }

    private static bool CheckRetryOptions(FillPropertyRetryAttribute? retryAttribute)
    {
        if (retryAttribute is not null && retryAttribute.CanTry)
        {
            retryAttribute.Try();
            return true;
        }

        return false;
    }

    private static RetryContext? CreateRetryContext(FillPropertyRetryAttribute? retryAttribute)
    {
        if (retryAttribute is not null)
        {
            return new(retryAttribute.Tried,
                       retryAttribute.RetryCount,
                       retryAttribute.CanTry);
        }

        return null;
    }

    private IUpdateCracker GetCracker(string propertyName) => _propertyCrackers[propertyName];

    private async Task UnRelated(
        TForm form,
        User user,
        string propertyName,
        ShiningInfo<long, Update> arg2,
        CancellationToken cancellationToken)
    {
        await form.OnUnrelatedUpdateAsync(
            new FormFillerContext<TForm>(this, user, propertyName),
            new OnUnrelatedUpdateContext(arg2),
            cancellationToken).ConfigureAwait(false);
    }

    internal FormFiller<TForm> AddCracker(string propName, IUpdateCracker cracker)
    {
        if (!validProps.Any(x => string.Equals(x, propName, StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Selected property not found.");
        }

        if (!_propertyCrackers.TryAdd(propName, cracker))
        {
            _propertyCrackers[propName] = cracker;
        }

        return this;
    }

    private static bool TrySetPropertyValue(
        TForm form,
        PropertyFillingInfo fillingInfo, object input,
        out List<ValidationResult> validationResults)
    {
        validationResults = [];
        var result = FormFiller<TForm>.ValidateProperty(form, fillingInfo.PropertyInfo.Name, input, out var compValidations);
        if (result)
        {
            fillingInfo.SetValue(form, input);
        }

        validationResults = [.. validationResults, .. compValidations];
        return result;
    }

    private IFormPropertyConverter? GetConverterForType(Type type)
        => _converters.Find(x => x.ConvertTo == type);

    private void InitConverters()
    {
        var converterBase = typeof(IFormPropertyConverter);

        foreach (var converterType in _convertersByType)
        {
            if (!converterBase.IsAssignableFrom(converterType))
            {
                throw new InvalidOperationException("All converters should implement IFormPropertyConverter.");
            }

            var converter = (IFormPropertyConverter?)Activator.CreateInstance(converterType) ?? throw new InvalidOperationException($"Can't create an instance of {converterType}");

            // Delete prev converters on same type.
            _converters.RemoveAll(x => x.ConvertTo == converter.ConvertTo);

            _converters.Add(converter);
        }
    }

    private static bool ValidateProperty(
        TForm form, string propertyName,
        object? value, out ICollection<ValidationResult> validationResults)
    {
        validationResults = [];
        ValidationContext valContext = new(form, serviceProvider: null, items: null)
        {
            MemberName = propertyName,
        };

        return Validator.TryValidateProperty(value, valContext, validationResults);
    }

    private static IEnumerable<PropertyInfo> GetValidProperties()
        => typeof(TForm).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.CanWrite && x.CanRead && x.GetCustomAttribute<FillerIgnoreAttribute>() is null);
}
