using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;

/// <summary>
/// Cracks out something out of a <see cref="CallbackQuery"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="updateChannel"></param>
/// <param name="cracker"></param>
/// <param name="cancelTrigger"></param>
public class CallbackQueryCracker<T>(
    AbstractChannel<CallbackQuery> updateChannel,
    Func<CallbackQuery, T> cracker,
    AbstractCancelTrigger<CallbackQuery>? cancelTrigger = null)
    : AnyCracker<T, CallbackQuery>(x => x.CallbackQuery, updateChannel, cracker, cancelTrigger)
{
}
