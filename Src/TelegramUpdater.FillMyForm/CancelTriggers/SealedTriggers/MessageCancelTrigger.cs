using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers;

public class MessageCancelTrigger(Filter<Message> shouldCancel)
    : CancelTrigger<Message>(x => x.Message, UpdateType.Message, shouldCancel)
{
}
