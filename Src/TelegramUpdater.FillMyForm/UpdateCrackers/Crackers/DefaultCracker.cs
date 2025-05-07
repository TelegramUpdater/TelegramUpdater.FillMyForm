using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels.ReadyToUse;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;

/// <summary>
/// Default cracker cracks out an string <see cref="Message.Text"/> out of a <see cref="Message"/>.
/// </summary>
/// <param name="timeOut"></param>
/// <param name="cancelTrigger"></param>
public sealed class DefaultCracker(TimeSpan timeOut, AbstractCancelTrigger<Message>? cancelTrigger = default)
    : AbstractUpdateCracker<object, Message>(x => x.Message, new MessageChannel(timeOut, ReadyFilters.Text()), cancelTrigger)
{
    /// <summary>
    /// Tries to re crack the update. (Idk know why yet!)
    /// </summary>
    /// <param name="update"></param>
    /// <param name="converter"></param>
    /// <param name="converted"></param>
    /// <returns></returns>
    public bool TryReCrack(
        Update update, IFormPropertyConverter converter, out object? converted)
    {
        var input = Crack(_updateResolver(update)!);
        if (input != null)
        {
#if NET8_0_OR_GREATER
            return converter.TryConvert(input, out converted);
#else
            return converter.TryConvert((string)input, out converted);
#endif
        }

        converted = null;
        return false;
    }

#if NET8_0_OR_GREATER
    /// <inheritdoc/>
    protected override string Crack(Message update) => update.Text!;
#else
    /// <inheritdoc/>
    protected override object Crack(Message update) => update.Text!;
#endif
}
