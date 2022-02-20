using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.FillMyForm
{
    public interface IFormPropertyConverter
    {
        public Type ConvertTo { get; }

        public bool TryConvert(string value, [NotNullWhen(true)] out object? convertedValue);
    }
}
