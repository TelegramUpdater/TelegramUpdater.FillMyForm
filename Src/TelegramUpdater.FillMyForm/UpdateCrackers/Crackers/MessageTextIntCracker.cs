using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels.ReadyToUse;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;

/// <summary>
/// Cracks out an integer from a <see cref="Message.Text"/>.
/// </summary>
/// <param name="timeOut"></param>
/// <param name="cancelTrigger"></param>
public sealed class MessageTextIntCracker(
    TimeSpan timeOut,
    AbstractCancelTrigger<Message>? cancelTrigger = default)
    : AbstractUpdateCracker<int, Message>(x => x.Message, new MessageChannel(timeOut, ReadyFilters.Text()), cancelTrigger)
{
    /// <inheritdoc/>
    protected override int Crack(Message update)
    {
        return int.Parse(update.Text!);
    }
}
