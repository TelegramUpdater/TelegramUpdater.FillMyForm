namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Base interface for your forms. you better use <see cref="AbstractForm"/> to inherit from.
/// </summary>
public interface IForm
{
    /// <summary>
    /// Callback function to be called before waiting for a user input.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnBeginAskAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when waiting for an input is timed out.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="timeoutContext">Information about a timed out.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnTimeOutAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        TimeoutContext timeoutContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when filler fails to convert user input into the excepted type.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="conversationErrorContext">Information about a failed conversation.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnConversationErrorAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        ConversationErrorContext conversationErrorContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when validations failed for an input.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="validationErrorContext">Information about a failed validation.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnValidationErrorAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        ValidationErrorContext validationErrorContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when an unrelated update received from user.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="onUnrelatedUpdateContext">Information about an unrelated update.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnUnrelatedUpdateAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        OnUnrelatedUpdateContext onUnrelatedUpdateContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when a property value set successfully.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="onSuccessContext">Information about a success.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnSuccessAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        OnSuccessContext onSuccessContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when a cancellation trigger, triggered.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="onCancelContext">Information about a cancel.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnCancelAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        OnCancelContext onCancelContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();
}
