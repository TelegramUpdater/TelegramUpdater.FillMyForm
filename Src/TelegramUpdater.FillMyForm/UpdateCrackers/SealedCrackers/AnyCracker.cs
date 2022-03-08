using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers.SealedCrackers
{
    public class AnyCracker<T, TUpdate> : AbstractUpdateCracker<T, TUpdate> where TUpdate
        : class
    {
        private readonly Func<TUpdate, T> _cracker;

        public AnyCracker(
            Func<Update, TUpdate?> updateResolver,
            AbstractChannel<TUpdate> updateChannel,
            Func<TUpdate, T> cracker,
            CancelTriggerAbs<TUpdate>? cancelTrigger = default)
            : base(updateResolver, updateChannel, cancelTrigger)
        {
            _cracker = cracker;
        }

        protected override T Crack(TUpdate update)
        {
            return _cracker(update);
        }
    }
}
