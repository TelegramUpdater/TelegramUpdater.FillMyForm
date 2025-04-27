using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers;

/// <summary>
/// Cracks something out of an update
/// </summary>
/// <typeparam name="T">The target item to crack out.</typeparam>
/// <typeparam name="TUpdate">The inner actual update.</typeparam>
public abstract class AbstractUpdateCracker<T, TUpdate>(
    Func<Update, TUpdate?> updateResolver,
    AbstractChannel<TUpdate> updateChannel,
    AbstractCancelTrigger<TUpdate>? cancelTrigger = default) : IUpdateCracker
    where TUpdate : class
{
    /// <summary>
    /// Resolve actual update from an <see cref="Update"/>.
    /// </summary>
    protected readonly Func<Update, TUpdate?> _updateResolver = updateResolver ??
            throw new ArgumentNullException(nameof(updateResolver));


    /// <inheritdoc />
    public IUpdateChannel UpdateChannel { get; } = updateChannel ??
        throw new ArgumentNullException(nameof(updateChannel));

    /// <inheritdoc />
    public ICancelTrigger? CancelTrigger { get; } = cancelTrigger;

    /// <summary>
    /// Defines how we crack an update.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    protected abstract T Crack(TUpdate update);

    /// <inheritdoc />
    public object? CrackerExpression(Update update)
    {
        return Crack(_updateResolver(update) ??
            throw new InvalidOperationException("Inner update is null."));
    }
}
