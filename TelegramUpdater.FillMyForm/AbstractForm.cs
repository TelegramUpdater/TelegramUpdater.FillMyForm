using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.FillMyForm
{
    public abstract class AbstractForm : IForm
    {
        public abstract Task OnBeginAskAsync(IUpdater updater,
                                                      User askingFrom,
                                                      string propertyName,
                                                      CancellationToken cancellationToken);

        public abstract Task OnSuccessAsync(RawContainer? container,
                                            User askingFrom,
                                            string propertyName,
                                            OnSuccessContext onSuccessContext,
                                            CancellationToken cancellationToken);

        public virtual Task OnConversationErrorAsync(RawContainer? rawContainer,
                                                              User askingFrom,
                                                              string propertyName,
                                                              ConversationErrorContext conversationErrorContext,
                                                              CancellationToken cancellationToken)
            => Task.CompletedTask;

        public virtual Task OnTimeOutAsync(IUpdater updater,
                                                    User askingFrom,
                                                    string propertyName,
                                                    TimeoutContext timeoutContext,
                                                    CancellationToken cancellationToken)
            => Task.CompletedTask;


        public virtual Task OnUnrelatedUpdateAsync(RawContainer rawContainer,
                                                   User askingFrom,
                                                   string propertyName,
                                                   OnUnrelatedUpdateContext onUnrelatedUpdateContext,
                                                   CancellationToken cancellationToken)
            => Task.CompletedTask;

        public virtual Task OnValidationErrorAsync(IUpdater updater,
                                                   ShiningInfo<long, Update>? shiningInfo,
                                                   User user,
                                                   string propertyName,
                                                   ValidationErrorContext validationErrorContext,
                                                   CancellationToken cancellationToken)
            => Task.CompletedTask;

        public virtual Task OnCancelAsync(RawContainer? container,
                                          User askingFrom,
                                          string propertyName,
                                          CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}