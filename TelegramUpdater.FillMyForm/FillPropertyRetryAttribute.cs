namespace TelegramUpdater.FillMyForm
{
    /// <summary>
    /// Apply retry options for your property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FillPropertyRetryAttribute : Attribute
    {
        private int tries = 0;

        /// <summary>
        /// Apply retry options for your property.
        /// </summary>
        /// <param name="fillingError">Type of error to retry on.</param>
        /// <param name="retryCount">Possible available tries.</param>
        public FillPropertyRetryAttribute(FillingError fillingError, int retryCount)
        {
            FillingError = fillingError;
            RetryCount = retryCount;
        }

        public FillingError FillingError { get; }

        public int RetryCount { get; }

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
}
