namespace TelegramUpdater.FillMyForm;

public interface IForm
{
    /// <summary>
    /// Callback function to be called before waiting for a user input.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillterContext">Filler context containing data about filling process.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnBeginAskAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                       CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when waiting for an input is timed out.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillterContext">Filler context containing data about filling process.</param>
    /// <param name="timeoutContext">Inforamtion about a timed out.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnTimeOutAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                      TimeoutContext timeoutContext,
                                      CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when filler fails to convert user input into the excepted type.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillterContext">Filler context containing data about filling process.</param>
    /// <param name="conversationErrorContext">Inforamtion about a failed conversation.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnConversationErrorAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                ConversationErrorContext conversationErrorContext,
                                                CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when validations failed for an input.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillterContext">Filler context containing data about filling process.</param>
    /// <param name="validationErrorContext">Inforamtion about a failed validation.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnValidationErrorAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                              ValidationErrorContext validationErrorContext,
                                              CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when an unrelated update received from user.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillterContext">Filler context containing data about filling process.</param>
    /// <param name="onUnrelatedUpdateContext">Inforamtion about an unrelated update.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnUnrelatedUpdateAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                              OnUnrelatedUpdateContext onUnrelatedUpdateContext,
                                              CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when a propery value set successfully.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillterContext">Filler context containing data about filling process.</param>
    /// <param name="onSuccessContext">Inforamtion about a success.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnSuccessAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                      OnSuccessContext onSuccessContext,
                                      CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <summary>
    /// Callback function to be called when a cancellation trigger, triggered.
    /// </summary>
    /// <typeparam name="TForm">The form.</typeparam>
    /// <param name="fillterContext">Filler context containing data about filling process.</param>
    /// <param name="onCancelContext">Inforamtion about a cancel.</param>
    /// <param name="cancellationToken">To cancel the job.</param>
    /// <returns></returns>
    public Task OnCancelAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                     OnCancelContext onCancelContext,
                                     CancellationToken cancellationToken)
        where TForm : IForm, new();
}
