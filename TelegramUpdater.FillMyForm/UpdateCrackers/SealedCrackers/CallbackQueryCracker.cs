using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.SealedCrackers
{
    public class CallbackQueryCracker<T> : AnyCracker<T, CallbackQuery>
    {
        public CallbackQueryCracker(
            AbstractChannel<CallbackQuery> updateChannel,
            Func<CallbackQuery, T> cracker,
            CancelTriggerAbs<CallbackQuery>? cancelTrigger = null)
            : base(x => x.CallbackQuery, updateChannel, cracker, cancelTrigger)
        { }

        protected override T Crack(CallbackQuery update)
        {
            throw new NotImplementedException();
        }
    }
}
