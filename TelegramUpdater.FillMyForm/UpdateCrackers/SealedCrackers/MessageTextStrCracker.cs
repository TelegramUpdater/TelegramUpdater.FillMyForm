using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels.SealedChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.SealedCrackers
{
    public sealed class MessageTextStrCracker : AbstractUpdateCracker<string, Message>
    {
        public MessageTextStrCracker(TimeSpan timeOut, CancelTriggerAbs<Message>? cancelTrigger = default)
            : base(x => x.Message, new MessageChannel(timeOut, FilterCutify.Text()), cancelTrigger)
        { }

        protected override string Crack(Message update) => update.Text!;
    }
}
