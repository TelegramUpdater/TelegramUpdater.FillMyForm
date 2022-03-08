using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.FillMyForm
{
    public abstract class FormPropertyConverter<T> : IFormPropertyConverter
    {
        public Type ConvertTo => typeof(T);

        protected abstract bool TryConvert(string value, [NotNullWhen(true)] out T convertedValue);

        public bool TryConvert(string value, [NotNullWhen(true)] out object? convertedValue)
        {
            var result = TryConvert(value, out T converted);
            convertedValue = converted;
            return result;
        }
    }
}
