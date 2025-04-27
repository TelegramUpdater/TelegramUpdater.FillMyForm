using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels.ReadyToUse;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;

/// <summary>
/// Cracks out <see cref="Message.Text"/>.
/// </summary>
/// <param name="timeOut"></param>
/// <param name="cancelTrigger"></param>
public sealed class MessageTextStrCracker(TimeSpan timeOut, AbstractCancelTrigger<Message>? cancelTrigger = default)
    : AbstractUpdateCracker<string, Message>(x => x.Message, new MessageChannel(timeOut, FilterCutify.Text()), cancelTrigger)
{
    /// <inheritdoc/>
    protected override string Crack(Message update) => update.Text!;
}
