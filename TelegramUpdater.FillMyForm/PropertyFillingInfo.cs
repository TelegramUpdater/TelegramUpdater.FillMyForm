using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TelegramUpdater.FillMyForm;

internal sealed class PropertyFillingInfo
{
    public PropertyFillingInfo(
        PropertyInfo propertyInfo,
        int priority,
        TimeSpan timeOut)
    {
        PropertyInfo = propertyInfo;
        Priority = priority;
        TimeOut = timeOut;
        RetryAttributes = new();
    }

    [MemberNotNullWhen(true, "RequiredErrorMessage")]
    internal bool Required { get; set; } = false;
    
    internal string? RequiredErrorMessage { get; set; }

    internal Type Type => PropertyInfo.PropertyType;

    internal PropertyInfo PropertyInfo { get; }

    internal int Priority { get; }

    internal TimeSpan TimeOut { get; }

    internal List<FillPropertyRetryAttribute> RetryAttributes { get; set; }

    internal void SetValue(object? obj, object? value)
    {
        PropertyInfo.SetValue(obj, value, null);
    }

    internal FillPropertyRetryAttribute? GetRetryOption(FillingError fillingError)
        => RetryAttributes.FirstOrDefault(x => x.FillingError == fillingError);
}
