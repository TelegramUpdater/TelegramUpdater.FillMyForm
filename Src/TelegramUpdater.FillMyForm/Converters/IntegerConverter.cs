namespace TelegramUpdater.FillMyForm.Converters;

/// <inheritdoc/>
public class IntegerConverter : AbstractFormPropertyConverter<int>
{
    /// <inheritdoc/>
    protected override bool TryConvert(string value, out int convertedValue)
        => int.TryParse(value, out convertedValue);
}
