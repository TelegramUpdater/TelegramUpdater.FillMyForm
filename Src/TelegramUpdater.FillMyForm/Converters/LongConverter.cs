namespace TelegramUpdater.FillMyForm.Converters
{
    internal class LongConverter : FormPropertyConverter<long>
    {
        protected override bool TryConvert(string value, out long convertedValue)
            => long.TryParse(value, out convertedValue);
    }
}
