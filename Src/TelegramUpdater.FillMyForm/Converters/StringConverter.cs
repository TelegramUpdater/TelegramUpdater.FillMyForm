using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.FillMyForm.Converters
{
    public class StringConverter : FormPropertyConverter<string>
    {
        protected override bool TryConvert(string value, [NotNullWhen(true)] out string convertedValue)
        {
            convertedValue = value;
            return true;
        }
    }
}
