using System.Diagnostics.CodeAnalysis;

namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Converts an input string to a desired type.
/// </summary>
public interface IFormPropertyConverter
{
    /// <summary>
    /// The type to convert string to.
    /// </summary>
    public Type ConvertTo { get; }

    /// <summary>
    /// Tries to convert the input string into <see cref="IFormPropertyConverter.ConvertTo"/> type.
    /// </summary>
    /// <param name="value">Input string.</param>
    /// <param name="convertedValue">Resulting converted output value.</param>
    /// <returns></returns>
    public bool TryConvert(string value, [NotNullWhen(true)] out object? convertedValue);
}
