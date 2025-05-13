using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers.Triggers;

/// <inheritdoc />
public class CallbackQueryCancelTrigger(Filter<CallbackQuery> shouldCancel)
    : CancelTrigger<CallbackQuery>(x => x.CallbackQuery, UpdateType.CallbackQuery, shouldCancel)
{
}
