using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;

/// <inheritdoc/>
public class AnyCracker<T, TUpdate>(
    Func<Update, TUpdate?> updateResolver,
    AbstractChannel<TUpdate> updateChannel,
    Func<TUpdate, T> cracker,
    AbstractCancelTrigger<TUpdate>? cancelTrigger = default) : AbstractUpdateCracker<T, TUpdate>(updateResolver, updateChannel, cancelTrigger) where TUpdate
    : class
{
    private readonly Func<TUpdate, T> _cracker = cracker;

    /// <inheritdoc/>
    protected override T Crack(TUpdate update)
    {
        return _cracker(update);
    }
}
