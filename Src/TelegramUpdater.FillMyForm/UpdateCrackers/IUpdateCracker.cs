using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers;

/// <summary>
/// Cracks out something out of an update.
/// </summary>
public interface IUpdateCracker
{
    /// <summary>
    /// The update channel to wait for the update.
    /// </summary>
    public IUpdateChannel UpdateChannel { get; }

    /// <summary>
    /// Defines how we crack an update.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    public object? CrackerExpression(Update update);

    /// <summary>
    /// Defines when we cancel cracking.
    /// </summary>
    public ICancelTrigger? CancelTrigger { get; }

    internal bool TryCrack(Update update, out object? cracked)
    {
        try
        {
            cracked = CrackerExpression(update);
            return true;
        }
        catch
        {
            cracked = null;
            return false;
        }
    }
}
