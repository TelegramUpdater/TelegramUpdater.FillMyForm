using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels.SealedChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.SealedCrackers
{
    public sealed class DefaultCracker : AbstractUpdateCracker<object, Message>
    {
        public DefaultCracker(TimeSpan timeOut, CancelTriggerAbs<Message>? cancelTrigger = default)
            : base(x => x.Message, new MessageChannel(timeOut, FilterCutify.Text()), cancelTrigger)
        { }

        public bool TryReCrack(
            Update update, IFormPropertyConverter converter, out object? converted)
        {
            var input = Crack(_updateResolver(update)!);
            if (input != null)
            {
                return converter.TryConvert(input, out converted);
            }
            else
            {
                converted = null;
                return false;
            }
        }

        protected override string Crack(Message update) => update.Text!;

    }
}
