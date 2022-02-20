using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers
{
    public class MessageCancelTrigger : CancelTrigger<Message>
    {
        public MessageCancelTrigger(Filter<Message> shouldCancel)
            : base(x => x.Message, UpdateType.Message, shouldCancel)
        {
        }
    }
}
