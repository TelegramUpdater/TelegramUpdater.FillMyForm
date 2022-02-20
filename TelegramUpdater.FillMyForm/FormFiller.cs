using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Telegram.Bot.Types;
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

    private readonly IEnumerable<Type> _convertersByType;
    private readonly List<IFormPropertyConverter> _converters;

    private bool _validated = false;

    public FormFiller(
        Action<CrackerContext<TForm>>? buildCrackers = default,
        IEnumerable<Type>? additionalConverters = default)
    {
        _object = new TForm();
        _propertyCrackers = new();

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

                var requiredAttr = x.GetCustomAttribute<RequiredAttribute>();
                fillingInfo.Required = requiredAttr != null;
                fillingInfo.RequiredErrorMessage = requiredAttr?.ErrorMessage;

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

    public TForm Form
    {
        get
        {
            if (_validated) return _object;
            throw new InvalidOperationException("Form not validated!");
        }
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

    public async Task<bool> FillAsync<T>(
        User user,
        IContainer<T> container, CancellationToken cancellationToken)
        where T : class
    {
        foreach (var property in propertyFillingInfo)
        {
            await _object.OnBeginAskAsync(
                container.Updater, user, property.PropertyInfo.Name, cancellationToken);

            var cracker = GetCracker(property.PropertyInfo.Name);

            while (true)
            {
                var update = await container.ShiningInfo.ReadNextAsync(
                    cracker.UpdateChannel.TimeOut, cancellationToken);

                if (update == null) // timed out!
                {
                    var timeOutRetry = property.GetRetryOption(FillingError.TimeoutError);

                    await _object.OnTimeOutAsync(
                        container.Updater, user, property.PropertyInfo.Name, new TimeoutContext(
                            timeOutRetry == null? null: new RetryContext(
                                timeOutRetry.Tried,
                                timeOutRetry.RetryCount,
                                timeOutRetry.CanTry),
                            property.TimeOut),
                        cancellationToken);

                    if (timeOutRetry != null)
                    {
                        if (timeOutRetry.CanTry)
                        {
                            timeOutRetry.Try();
                            continue;
                        }
                    }
                }

                var cancelled = false;
                object? input;

                if (cracker.CancelTrigger != null)
                {
                    cancelled = cracker.CancelTrigger.ShouldCancel(update!.Value);
                }

                // Do not try converting if it's time out or cancelled
                // Since there's no suitable update to convert in here!
                // Go directly to the set value and validations.
                if (!cancelled && update != null)
                {
                    // Update not null here.
                    if (cracker.UpdateChannel.UpdateType != update!.Value.Type)
                    {
                        await UnRelated(
                            user, container, property.PropertyInfo.Name, update, cancellationToken);
                        continue;
                    }

                    if (!cracker.UpdateChannel.ShouldChannel(update.Value))
                    {
                        await UnRelated(
                            user, container, property.PropertyInfo.Name, update, cancellationToken);
                        continue;
                    }

                    var cracked = false;

                    if (cracker is DefaultCracker @default)
                    {
                        var converter = GetConverterForType(property.Type);
                        if (converter == null)
                        {
                            throw new InvalidOperationException($"No converter for {property.Type}");
                        }

                        cracked = @default.TryReCrack(update.Value, converter, out input);
                    }
                    else
                    {
                        cracked = cracker.TryCrack(update!.Value, out input);
                    }

                    // Converting fase
                    if (!cracked)
                    {
                        var convertOption = property.GetRetryOption(FillingError.ConvertingError);
                        var notCRetry = convertOption == null;

                        await _object.OnConversationErrorAsync(
                            container.RebaseAsRaw(update), user, property.PropertyInfo.Name, new ConversationErrorContext(
                                notCRetry ? null : new RetryContext(
                                    convertOption!.Tried,
                                    convertOption.RetryCount,
                                    convertOption.CanTry),
                                property.Type),
                            cancellationToken);

                        // TODO: refactor
                        if (!notCRetry)
                        {
                            if (convertOption!.CanTry)
                            {
                                convertOption.Try();
                                continue;
                            }
                        }

                        input = null;
                    }
                }
                else
                {
                    // Cancelled or timed out
                    if (cancelled)
                    {
                        // Update can't be null.

                        await _object.OnCancelAsync(
                            container.RebaseAsRaw(update!),
                            askingFrom: user,
                            property.PropertyInfo.Name,
                            cancellationToken);
                    }

                    input = null;
                }

                // Validating fase
                var retryOption = property.GetRetryOption(FillingError.ValidationError);
                var noRetry = retryOption == null;

                if (input != null)
                {
                    var setResult = TrySetPropertyValue(property, input, out var validationResults);

                    if (setResult != FillingError.NoError)
                    {
                        if (setResult == FillingError.ValidationError)
                        {
                            await _object.OnValidationErrorAsync(
                                container.Updater, update, user, property.PropertyInfo.Name, new ValidationErrorContext(
                                    noRetry ? null : new RetryContext(
                                        retryOption!.Tried,
                                        retryOption.RetryCount,
                                        retryOption.CanTry),
                                    validationResults),
                                cancellationToken);
                        }

                        // TODO: refactor
                        if (!noRetry)
                        {
                            if (retryOption!.CanTry)
                            {
                                retryOption.Try();
                                continue;
                            }
                        }

                        return false;
                    }
                }
                else
                {
                    if (property.Required)
                    {
                        await _object.OnValidationErrorAsync(
                            container.Updater, null, user, property.PropertyInfo.Name, new ValidationErrorContext(
                                noRetry ? null : new RetryContext(
                                    retryOption!.Tried,
                                    retryOption.RetryCount,
                                    retryOption.CanTry),
                                new List<ValidationResult>
                                {
                                    new ValidationResult(property.RequiredErrorMessage,
                                        new string[] {property.PropertyInfo.Name})
                                }),
                            cancellationToken);

                        if (!noRetry)
                        {
                            if (retryOption!.CanTry)
                            {
                                retryOption.Try();
                                continue;
                            }
                        }

                        return false;
                    }
                }

                // TODO: need to validate object

                // if it's timeout or cancel but not required then the update is null but success.
                await _object.OnSuccessAsync(
                    update == null ? null : container.RebaseAsRaw(update),
                    user, property.PropertyInfo.Name,
                    new OnSuccessContext(input), cancellationToken);
                break;
            }
        }

        _validated = true;
        return true;
    }

    private IUpdateCracker GetCracker(string propertyName)
    {
        return _propertyCrackers[propertyName];
    }

    private async Task UnRelated<T>(
        User user,
        IContainer<T> container,
        string propertyName,
        ShiningInfo<long, Update> arg2,
        CancellationToken cancellationToken) where T : class
    {
        await _object.OnUnrelatedUpdateAsync(
            container.RebaseAsRaw(arg2), user, propertyName, new OnUnrelatedUpdateContext(arg2), cancellationToken);
    }

    public bool InPlaceValidate(
        Queue<(bool timedOut, string? input)> inputs, out List<ValidationResult> validationResults)
    {
        foreach (var property in propertyFillingInfo)
        {
            while (true)
            {
                var (timedOut, input) = inputs.Dequeue();

                if (timedOut)
                {
                    var timeOutRetry = property.GetRetryOption(FillingError.TimeoutError);
                    if (timeOutRetry != null)
                    {
                        if (timeOutRetry.CanTry)
                        {
                            timeOutRetry.Try();
                            continue;
                        }
                    }
                }

                var setResult = TrySetPropertyValue(property, input, out validationResults);

                if (setResult != FillingError.NoError)
                {
                    var retryOption = property.GetRetryOption(setResult);
                    if (retryOption != null)
                    {
                        if (retryOption.CanTry)
                        {
                            retryOption.Try();
                            continue;
                        }
                    }

                    if (property.Required)
                    {
                        return false;
                    }
                }

                break;
            }
        }

        validationResults = new List<ValidationResult>();
        _validated = true;
        return true;
    }

    internal FillingError TrySetPropertyValue(
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
        return result? FillingError.NoError: FillingError.ValidationError;
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
