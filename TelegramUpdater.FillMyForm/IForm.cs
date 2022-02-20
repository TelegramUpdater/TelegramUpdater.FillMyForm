using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace TelegramUpdater.FillMyForm
{
    public interface IForm
    {
        public Task OnBeginAskAsync(IUpdater updater,
                                    User askingFrom,
                                    string propertyName,
                                    CancellationToken cancellationToken);

        public Task OnTimeOutAsync(IUpdater updater,
                                   User askingFrom,
                                   string propertyName,
                                   TimeoutContext timeoutContext,
                                   CancellationToken cancellationToken);

        public Task OnConversationErrorAsync(RawContainer? container,
                                             User askingFrom,
                                             string propertyName,
                                             ConversationErrorContext conversationErrorContext,
                                             CancellationToken cancellationToken);

        public Task OnValidationErrorAsync(IUpdater updater,
                                           ShiningInfo<long, Update>? shiningInfo,
                                           User askingFrom,
                                           string propertyName,
                                           ValidationErrorContext validationErrorContext,
                                           CancellationToken cancellationToken);

        public Task OnUnrelatedUpdateAsync(RawContainer container,
                                           User askingFrom,
                                           string propertyName,
                                           OnUnrelatedUpdateContext onUnrelatedUpdateContext,
                                           CancellationToken cancellationToken);

        public Task OnSuccessAsync(RawContainer? container,
                                   User askingFrom,
                                   string propertyName,
                                   OnSuccessContext onSuccessContext,
                                   CancellationToken cancellationToken);

        public Task OnCancelAsync(RawContainer? container,
                                  User askingFrom,
                                  string propertyName,
                                  CancellationToken cancellationToken);
    }
}
