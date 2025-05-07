using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers;

/// <inheritdoc />
public class CancelTrigger<T>(
    Func<Update, T?> updateResolver,
    UpdateType updateType,
    Filter<T> shouldCancel) : AbstractCancelTrigger<T>(updateResolver, updateType) where T : class
{
    /// <inheritdoc />
    protected override bool ShouldCancel(T resolved) => shouldCancel.TheyShellPass(resolved);
}
