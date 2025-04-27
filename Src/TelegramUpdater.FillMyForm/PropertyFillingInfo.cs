using System.Reflection;

namespace TelegramUpdater.FillMyForm;

internal sealed class PropertyFillingInfo(
    PropertyInfo propertyInfo,
    int priority,
    TimeSpan timeOut)
{
    internal bool Required { get; set; } = false;

    internal Type Type => PropertyInfo.PropertyType;

    internal PropertyInfo PropertyInfo { get; } = propertyInfo;

    internal int Priority { get; } = priority;

    internal TimeSpan TimeOut { get; } = timeOut;

    internal List<FillPropertyRetryAttribute> RetryAttributes { get; set; } = [];

    internal void SetValue(object? obj, object? value)
    {
        PropertyInfo.SetValue(obj, value, index: null);
    }

    internal FillPropertyRetryAttribute? GetRetryOption(FillingError fillingError)
        => RetryAttributes.Find(x => x.FillingError == fillingError);
}
