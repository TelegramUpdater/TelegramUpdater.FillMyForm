using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Converts an input <see cref="string"/> to a desired type of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type to convert string input to.</typeparam>
public abstract class AbstractFormPropertyConverter<T> : IFormPropertyConverter
{
    /// <inheritdoc/>
    public Type ConvertTo => typeof(T);

    /// <summary>
    /// Tries to convert the input string into <see cref="IFormPropertyConverter.ConvertTo"/> type of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">Input string.</param>
    /// <param name="convertedValue">Resulting converted output value.</param>
    /// <returns></returns>
    protected abstract bool TryConvert(string value, [NotNullWhen(true)] out T convertedValue);

    /// <inheritdoc/>
    public bool TryConvert(string value, [NotNullWhen(true)] out object? convertedValue)
    {
        var result = TryConvert(value, out T converted);
        convertedValue = converted;
        return result;
    }
}
