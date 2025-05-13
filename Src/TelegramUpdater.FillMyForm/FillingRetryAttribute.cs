namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Apply retry options for your property.
/// </summary>
/// <remarks>
/// Apply retry options for your property.
/// </remarks>
/// <param name="fillingError">Type of error to retry on.</param>
/// <param name="retryCount">Possible available tries.</param>
[AttributeUsage(AttributeTargets.Property)]
public class FillingRetryAttribute(FillingError fillingError, int retryCount) : Attribute
{
    private int tries = 0;

    /// <summary>
    /// Error popped up during filling.
    /// </summary>
    public FillingError FillingError { get; } = fillingError;

    /// <summary>
    /// Retry count.
    /// </summary>
    public int RetryCount { get; } = retryCount;

    internal void Try()
    {
        if (!CanTry)
        {
            throw new InvalidOperationException("Can't try anymore.");
        }

        tries++;
    }

    internal bool CanTry => tries < RetryCount;

    internal int Tried => tries;
}
