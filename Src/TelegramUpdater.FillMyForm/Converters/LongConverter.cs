namespace TelegramUpdater.FillMyForm.Converters;

/// <inheritdoc/>
public class LongConverter : AbstractFormPropertyConverter<long>
{
    /// <inheritdoc/>
    protected override bool TryConvert(string value, out long convertedValue)
        => long.TryParse(value, out convertedValue);
}
