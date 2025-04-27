using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers;

/// <summary>
/// A cancel trigger for actual updates of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The actual inner update.</typeparam>
/// <param name="updateResolver">Resolve actual <typeparamref name="T"/> update out of <see cref="Update"/>.</param>
/// <param name="updateType">The target UpdateType.</param>
public abstract class AbstractCancelTrigger<T>(
    Func<Update, T?> updateResolver,
    UpdateType updateType) : ICancelTrigger where T : class
{
    /// <summary>
    /// Resolve actual <typeparamref name="T"/> update out of <see cref="Update"/>.
    /// </summary>
    protected Func<Update, T?> UpdateResolver { get; } = updateResolver;

    /// <inheritdoc/>
    public UpdateType UpdateType { get; } = updateType;

    /// <summary>
    /// Indicates if we should cancel on the kind of update
    /// </summary>
    /// <returns></returns>
    protected abstract bool ShouldCancel(T resolved);

    /// <inheritdoc/>
    public bool ShouldCancel(Update update)
    {
        if (UpdateType == update.Type)
        {
            return ShouldCancel(UpdateResolver(update) ??
                throw new ArgumentNullException(nameof(update), $"Inner update is null?"));
        }

        return false;
    }
}
