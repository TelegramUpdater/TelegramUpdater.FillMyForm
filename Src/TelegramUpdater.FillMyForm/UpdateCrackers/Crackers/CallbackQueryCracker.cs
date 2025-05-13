using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;
using TelegramUpdater.UpdateChannels.ReadyToUse;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;

/// <summary>
/// Cracks out something out of a <see cref="CallbackQuery"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CallbackQueryCracker<T> : AnyCracker<T, CallbackQuery>
{
    /// <summary>
    /// Creates a new instance of <see cref="CallbackQueryCracker{T}"/>.
    /// </summary>
    /// <param name="updateChannel"></param>
    /// <param name="cracker"></param>
    /// <param name="cancelTrigger"></param>
    public CallbackQueryCracker(
        AbstractChannel<CallbackQuery> updateChannel,
        Func<CallbackQuery, T> cracker,
        AbstractCancelTrigger<CallbackQuery>? cancelTrigger = null) : base(x => x.CallbackQuery, updateChannel, cracker, cancelTrigger)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="CallbackQueryCracker{T}"/>.
    /// </summary>
    /// <param name="timeOut"></param>
    /// <param name="cracker"></param>
    /// <param name="filter"></param>
    /// <param name="cancelTrigger"></param>
    public CallbackQueryCracker(
        TimeSpan timeOut,
        Func<CallbackQuery, T> cracker,
        IFilter<UpdaterFilterInputs<CallbackQuery>>? filter = default,
        AbstractCancelTrigger<CallbackQuery>? cancelTrigger = null)
        : base(
            x => x.CallbackQuery,
            new CallbackQueryChannel(timeOut, filter),
            cracker,
            cancelTrigger)
    {
    }
}
