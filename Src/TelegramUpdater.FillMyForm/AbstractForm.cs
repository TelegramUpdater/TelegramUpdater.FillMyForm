namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Your form may inherit from this.
/// </summary>
public abstract class AbstractForm : IForm
{
    /// <inheritdoc/>
    public abstract Task OnBeginAskAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <inheritdoc/>
    public abstract Task OnSuccessAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        OnSuccessContext onSuccessContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <inheritdoc/>
    public virtual Task OnCancelAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        OnCancelContext onCancelContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnConversationErrorAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        ConversationErrorContext conversationErrorContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnTimeOutAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        TimeoutContext timeoutContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnUnrelatedUpdateAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        OnUnrelatedUpdateContext onUnrelatedUpdateContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnValidationErrorAsync<TForm>(
        FormFillerContext<TForm> fillerContext,
        ValidationErrorContext validationErrorContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;
}