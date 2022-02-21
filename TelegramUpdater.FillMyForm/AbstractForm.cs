namespace TelegramUpdater.FillMyForm
{
    /// <summary>
    /// Your form may inherit from this.
    /// </summary>
    public abstract class AbstractForm : IForm
    {
        /// <inheritdoc/>
        public abstract Task OnBeginAskAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                    CancellationToken cancellationToken)
            where TForm : IForm, new();

        /// <inheritdoc/>
        public abstract Task OnSuccessAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                   OnSuccessContext onSuccessContext,
                                                   CancellationToken cancellationToken)
            where TForm : IForm, new();

        /// <inheritdoc/>
        public virtual Task OnCancelAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                  OnCancelContext onCancelContext,
                                                  CancellationToken cancellationToken)
            where TForm : IForm, new() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task OnConversationErrorAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                             ConversationErrorContext conversationErrorContext,
                                                             CancellationToken cancellationToken)
            where TForm : IForm, new() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task OnTimeOutAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                   TimeoutContext timeoutContext,
                                                   CancellationToken cancellationToken)
            where TForm : IForm, new() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task OnUnrelatedUpdateAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                           OnUnrelatedUpdateContext onUnrelatedUpdateContext,
                                                           CancellationToken cancellationToken)
            where TForm : IForm, new() => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task OnValidationErrorAsync<TForm>(FormFillterContext<TForm> fillterContext,
                                                           ValidationErrorContext validationErrorContext,
                                                           CancellationToken cancellationToken)
            where TForm : IForm, new() => Task.CompletedTask;
    }
}