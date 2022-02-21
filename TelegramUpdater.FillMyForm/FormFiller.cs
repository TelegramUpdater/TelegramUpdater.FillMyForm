using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.FillMyForm.Converters;
using TelegramUpdater.FillMyForm.UpdateCrackers;
using TelegramUpdater.FillMyForm.UpdateCrackers.SealedCrackers;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer;

namespace TelegramUpdater.FillMyForm;

public sealed class FormFiller<TForm> where TForm : IForm, new()
{
    private readonly Dictionary<string, IUpdateCracker> _propertyCrackers;
    private readonly string[] validProps;
    private readonly PropertyFillingInfo[] propertyFillingInfo;
    private readonly TForm _object;
    private readonly ICancelTrigger? _defaultCancelTrigger;
    private readonly IEnumerable<Type> _convertersByType;
    private readonly List<IFormPropertyConverter> _converters;

    private bool _validated = false;

    public FormFiller(
        IUpdater updater,
        Action<CrackerContext<TForm>>? buildCrackers = default,
        IEnumerable<Type>? additionalConverters = default,
        ICancelTrigger? defaultCancelTrigger = default)
    {
        Updater = updater ?? throw new ArgumentNullException(nameof(updater));

        _object = new TForm();
        _propertyCrackers = new();
        _defaultCancelTrigger = defaultCancelTrigger;

        // Default converters.
        _convertersByType = new List<Type>()
        {
            typeof(StringConverter),
            typeof(IntegerConverter),
            typeof(LongConverter),
            typeof(FloatConverter),
        };

        if (additionalConverters != null)
        {
            _convertersByType = _convertersByType.Union(additionalConverters);
        }

        _converters = new List<IFormPropertyConverter>();

        InitConverters();

        var validProperties = GetValidProperties();
        validProps = validProperties.Select(x=>x.Name).ToArray();

        if (buildCrackers != null)
        {
            var ctx = new CrackerContext<TForm>();
            buildCrackers(ctx);

            ctx.Build(this);
        }

        propertyFillingInfo = validProperties
            .Select(x =>
            {
                var attributeInfo = x.GetCustomAttribute<FormPropertyAttribute>();

                // get retry options
                var retryOptions = x.GetCustomAttributes<FillPropertyRetryAttribute>()
                    .DistinctBy(x => x.FillingError);

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
            .OrderBy(x=> x.Priority)
            .ToArray();
    }

    /// <summary>
    /// Get your form which is validated and it's an instance of <typeparamref name="TForm"/>.
    /// </summary>
    public TForm Form
    {
        get
        {
            if (_validated) return _object;
            throw new InvalidOperationException("Form not validated!");
        }
    }

    /// <summary>
    /// Get the form without checking for validity! you may use <see cref="Form"/> if you're
    /// not sure.
    /// </summary>
    public TForm MayInvalidForm => _object;

    public IUpdater Updater { get; init; }

    public async Task<bool> FillAsync<T>(
        User user, IContainer<T> container, CancellationToken cancellationToken = default)
        where T : class
    {
        FormFillterContext<TForm> FillerCtx(string propertyName)
            => new(this, user, propertyName);

        if (user is null)
            throw new ArgumentNullException(nameof(user));

        if (container is null)
            throw new ArgumentNullException(nameof(container));

        foreach (var property in propertyFillingInfo)
        {
            await _object.OnBeginAskAsync(FillerCtx(property.PropertyInfo.Name), cancellationToken);

            var cracker = GetCracker(property.PropertyInfo.Name);

            while (true)
            {
                var update = await container.ShiningInfo.ReadNextAsync(
                    cracker.UpdateChannel.TimeOut, cancellationToken);

                var timedOut = update == null;

                if (timedOut) // timed out!
                {
                    var timeOutRetry = property.GetRetryOption(FillingError.TimeoutError);

                    await _object.OnTimeOutAsync(FillerCtx(property.PropertyInfo.Name),
                        new TimeoutContext(
                            CreateRetryContext(timeOutRetry),
                            property.TimeOut),
                        cancellationToken);

                    if (CheckRetryOptions(timeOutRetry))
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

                // NOTE: smth that is cancelled, should not do retry.

                // Do not try converting if it's time out or cancelled
                // Since there's no suitable update to convert in here!
                // Go directly to the set value and validations.
                if (timedOut || cancelled)
                {
                    // Cancelled or timed out
                    if (cancelled)
                    {
                        // Update can't be null.
                        await _object.OnCancelAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new OnCancelContext(update!),
                            cancellationToken);
                    }

                    input = null;
                }
                else // Not timedout and not cancelled.
                {
                    // Update not null here.
                    if (!cracker.UpdateChannel.ShouldChannel(update!.Value))
                    {
                        await UnRelated(user, property.PropertyInfo.Name, update, cancellationToken);
                        continue;
                    }

                    // Can't crack it? it's an invalid input then.
                    if (!TryCrackingIt(cracker, property.Type, update.Value, out input))
                    {
                        var convertOption = property.GetRetryOption(FillingError.ConvertingError);

                        await _object.OnConversationErrorAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new ConversationErrorContext(
                                CreateRetryContext(convertOption),
                                property.Type,
                                update),
                            cancellationToken);

                        if (CheckRetryOptions(convertOption))
                            continue;

                        // Set input value to null since we can't convert it to requested type.
                        input = null;
                    }
                }

                // Validating fase
                var retryOption = property.GetRetryOption(FillingError.ValidationError);

                if (input != null)
                {
                    // Failed to set value? then it's a validation error
                    if (!TrySetPropertyValue(property, input, out var validationResults))
                    {
                        await _object.OnValidationErrorAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new ValidationErrorContext(
                                CreateRetryContext(retryOption),
                                update,
                                false,
                                validationResults),
                            cancellationToken);

                        if (CheckRetryOptions(retryOption))
                            continue;

                        // Still invalid? no chance.
                        return false;
                    }
                }
                else
                {
                    // It can't be null if it's required.
                    if (property.Required)
                    {
                        await _object.OnValidationErrorAsync(
                            FillerCtx(property.PropertyInfo.Name),
                            new ValidationErrorContext(
                                CreateRetryContext(retryOption),
                                update,
                                true,
                                Array.Empty<ValidationResult>()),
                            cancellationToken);

                        if (!cancelled) // Don't retry if it's cancelled.
                        {
                            if (CheckRetryOptions(retryOption))
                                continue;
                        }

                        // Still invalid? no chance.
                        return false;
                    }
                }

                // if it's timeout or cancel but not required then the update is null but success.
                await _object.OnSuccessAsync(
                    FillerCtx(property.PropertyInfo.Name),
                    new OnSuccessContext(input, update),
                    cancellationToken);
                break;
            }
        }

        _validated = true;
        return true;
    }

    public bool InPlaceValidate(
        Queue<(bool timedOut, string? input)> inputs, out List<ValidationResult> validationResults)
    {
        throw new NotImplementedException();
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
        else
        {
            return cracker.TryCrack(update, out input);
        }
    }

    private bool CheckRetryOptions(FillPropertyRetryAttribute? retryAttribute)
    {
        if (retryAttribute is not null)
        {
            if (retryAttribute.CanTry)
            {
                retryAttribute.Try();
                return true;
            }
        }

        return false;
    }

    private RetryContext? CreateRetryContext(FillPropertyRetryAttribute? retryAttribute)
    {
        if (retryAttribute is not null)
        {
            return new(retryAttribute.Tried,
                       retryAttribute.RetryCount,
                       retryAttribute.CanTry);
        }

        return null;
    }

    private IUpdateCracker GetCracker(string propertyName)
    {
        return _propertyCrackers[propertyName];
    }

    private async Task UnRelated(User user,
                                 string propertyName,
                                 ShiningInfo<long, Update> arg2,
                                 CancellationToken cancellationToken)
    {
        await _object.OnUnrelatedUpdateAsync(
            new FormFillterContext<TForm>(this, user, propertyName),
            new OnUnrelatedUpdateContext(arg2),
            cancellationToken);
    }

    internal FormFiller<TForm> AddCracker(string propName, IUpdateCracker cracker)
    {
        if (!validProps.Any(x => x == propName))
        {
            throw new InvalidOperationException("Selected property not found.");
        }

        if (_propertyCrackers.ContainsKey(propName))
        {
            _propertyCrackers[propName] = cracker;
        }
        else
        {
            _propertyCrackers.Add(propName, cracker);
        }
        return this;
    }

    private bool TrySetPropertyValue(
        PropertyFillingInfo fillingInfo, object input,
        out List<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        var result = ValidateProperty(fillingInfo.PropertyInfo.Name, input, out var compValidations);
        if (result)
        {
            fillingInfo.SetValue(_object, input);
        }

        validationResults = validationResults.Concat(compValidations).ToList();
        return result;
    }

    private IFormPropertyConverter? GetConverterForType(Type type)
        => _converters.FirstOrDefault(x => x.ConvertTo == type);

    private void InitConverters()
    {
        var converterBase = typeof(IFormPropertyConverter);

        foreach (var converterType in _convertersByType)
        {
            if (!converterBase.IsAssignableFrom(converterType))
            {
                throw new InvalidOperationException("All converters should implement IFormPropertyConverter.");
            }

            var converter = (IFormPropertyConverter?)Activator.CreateInstance(converterType);
            if (converter == null)
            {
                throw new InvalidOperationException($"Can't create an instance of {converterType}");
            }

            // Delete prev converters on same type.
            _converters.RemoveAll(x => x.ConvertTo == converter.ConvertTo);

            _converters.Add(converter);
        }
    }

    private bool ValidateProperty(
        string propertyName, object? value, out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        ValidationContext valContext = new(_object, null, null)
        {
            MemberName = propertyName
        };

        return Validator.TryValidateProperty(value, valContext, validationResults);
    }

    private static IEnumerable<PropertyInfo> GetValidProperties()
        => typeof(TForm).GetProperties(
            BindingFlags.Public | BindingFlags.Instance)
            .Where(x=> x.CanWrite && x.CanRead)
            .Where(x => x.GetCustomAttribute<FillerIgnoreAttribute>() is null);
}
