using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels.ReadyToUse;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.SealedCrackers
{
    public sealed class MessageTextIntCracker : AbstractUpdateCracker<int, Message>
    {
        public MessageTextIntCracker(TimeSpan timeOut, CancelTriggerAbs<Message>? cancelTrigger = default)
            : base(x => x.Message, new MessageChannel(timeOut, FilterCutify.Text()), cancelTrigger)
        {
        }

        protected override int Crack(Message update)
        {
            return int.Parse(update.Text!);
        }
    }
}
