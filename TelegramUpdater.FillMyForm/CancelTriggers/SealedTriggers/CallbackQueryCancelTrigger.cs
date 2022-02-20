using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers
{
    public class CallbackQueryCancelTrigger : CancelTrigger<CallbackQuery>
    {
        public CallbackQueryCancelTrigger(Filter<CallbackQuery> shouldCancel)
            : base(x => x.CallbackQuery, UpdateType.CallbackQuery, shouldCancel)
        {
        }
    }
}
