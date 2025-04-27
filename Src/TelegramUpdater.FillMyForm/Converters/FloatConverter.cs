namespace TelegramUpdater.FillMyForm.Converters;

/// <inheritdoc/>
public class FloatConverter : AbstractFormPropertyConverter<float>
{
    /// <inheritdoc/>
    protected override bool TryConvert(string value, out float convertedValue)
        => float.TryParse(value, out convertedValue);
}
