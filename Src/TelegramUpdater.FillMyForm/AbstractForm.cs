namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Your form may inherit from this.
/// </summary>
public abstract class AbstractForm : IForm
{
    /// <inheritdoc/>
    public abstract Task OnBeginAsk<TForm>(
        FormFillingContext<TForm> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <inheritdoc/>
    public abstract Task OnSuccess<TForm>(
        FormFillingContext<TForm, OnSuccessContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new();

    /// <inheritdoc/>
    public virtual Task OnCancel<TForm>(
        FormFillingContext<TForm, OnCancelContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnConversationError<TForm>(
        FormFillingContext<TForm, ConversationErrorContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnTimeOut<TForm>(
        FormFillingContext<TForm, TimeoutContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnUnrelatedUpdate<TForm>(
        FormFillingContext<TForm, OnUnrelatedUpdateContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task OnValidationError<TForm>(
        FormFillingContext<TForm, ValidationErrorContext> fillerContext,
        CancellationToken cancellationToken)
        where TForm : IForm, new() => Task.CompletedTask;
}