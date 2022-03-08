namespace TelegramUpdater.FillMyForm
{
    /// <summary>
    /// Different error kinds while filling a form.
    /// </summary>
    public enum FillingError
    {
        /// <summary>
        /// There was no errors.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// An error occured while converting user input to the excepted type.
        /// </summary>
        ConvertingError = 1,

        /// <summary>
        /// An error occured while validating input value.
        /// </summary>
        ValidationError = 2,

        /// <summary>
        /// Waiting timed out.
        /// </summary>
        TimeoutError = 3,

        /// <summary>
        /// Received an unrelated update.
        /// </summary>
        UnrelatedAnswer = 4
    }
}
