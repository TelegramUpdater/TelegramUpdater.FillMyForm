using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers;

/// <summary>
/// A cancel trigger
/// </summary>
public interface ICancelTrigger
{
    /// <summary>
    /// Desired update type/
    /// </summary>
    public UpdateType UpdateType { get; }

    /// <summary>
    /// Indicates if we should cancel on the kind of update
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public bool ShouldCancel(Update update);
}
