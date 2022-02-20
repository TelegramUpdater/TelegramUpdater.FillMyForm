namespace TelegramUpdater.FillMyForm
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FillPropertyRetryAttribute : Attribute
    {
        private int tries = 0;

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
