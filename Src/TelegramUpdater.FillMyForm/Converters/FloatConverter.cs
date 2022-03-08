namespace TelegramUpdater.FillMyForm.Converters
{
    internal class FloatConverter : FormPropertyConverter<float>
    {
        protected override bool TryConvert(string value, out float convertedValue)
            => float.TryParse(value, out convertedValue);
    }
}
