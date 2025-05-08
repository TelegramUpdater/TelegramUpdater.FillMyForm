namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Your form may inherit from this.
/// </summary>
public abstract class AbstractForm : IForm
{
    /// <inheritdoc/>
    public abstract Task OnBeginAskAsync<TForm>(
        FormFillingContext<TForm> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <inheritdoc/>
    public abstract Task OnSuccessAsync<TForm>(
        FormFillingContext<TForm, OnSuccessContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <inheritdoc/>
    public virtual Task OnCancelAsync<TForm>(
        FormFillingContext<TForm, OnCancelContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnConversationErrorAsync<TForm>(
        FormFillingContext<TForm, ConversationErrorContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnTimeOutAsync<TForm>(
        FormFillingContext<TForm, TimeoutContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnUnrelatedUpdateAsync<TForm>(
        FormFillingContext<TForm, OnUnrelatedUpdateContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnValidationErrorAsync<TForm>(
        FormFillingContext<TForm, ValidationErrorContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;
}