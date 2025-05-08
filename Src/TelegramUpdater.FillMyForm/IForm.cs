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
        FormFillingContext<TForm> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when waiting for an input is timed out.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnTimeOutAsync<TForm>(
        FormFillingContext<TForm, TimeoutContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when filler fails to convert user input into the excepted type.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnConversationErrorAsync<TForm>(
        FormFillingContext<TForm, ConversationErrorContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when validations failed for an input.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnValidationErrorAsync<TForm>(
        FormFillingContext<TForm, ValidationErrorContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when an unrelated update received from user.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnUnrelatedUpdateAsync<TForm>(
        FormFillingContext<TForm, OnUnrelatedUpdateContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when a property value set successfully.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnSuccessAsync<TForm>(
        FormFillingContext<TForm, OnSuccessContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when a cancellation trigger, triggered.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillerContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnCancelAsync<TForm>(
        FormFillingContext<TForm, OnCancelContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();
}
