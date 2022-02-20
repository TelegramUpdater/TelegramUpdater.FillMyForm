using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers
{
    public sealed class MessageCancelTextTrigger : CancelTriggerAbs<Message>
    {
        public MessageCancelTextTrigger() : base(x=> x.Message, UpdateType.Message)
        {
        }

        protected override bool ShouldCancel(Message resolved)
        {
            return resolved switch
            {
                { Text: { } txt } => txt.ToLower() == "/cancel",
                _ => false,
            };
        }
    }
}
