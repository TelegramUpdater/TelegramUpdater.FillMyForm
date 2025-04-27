using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.FillMyForm.Converters;

/// <inheritdoc/>
public class StringConverter : AbstractFormPropertyConverter<string>
{
    /// <inheritdoc/>
    protected override bool TryConvert(string value, [NotNullWhen(true)] out string convertedValue)
    {
        convertedValue = value;
        return true;
    }
}
