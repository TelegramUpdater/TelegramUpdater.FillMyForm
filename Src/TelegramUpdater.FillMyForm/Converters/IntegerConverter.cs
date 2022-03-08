namespace TelegramUpdater.FillMyForm.Converters
{
    public class IntegerConverter : FormPropertyConverter<int>
    {
        protected override bool TryConvert(string value, out int convertedValue)
            => int.TryParse(value, out convertedValue);
    }
}
