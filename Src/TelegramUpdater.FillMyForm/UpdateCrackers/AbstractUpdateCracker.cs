using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers
{
    public abstract class AbstractUpdateCracker<T, TUpdate> : IUpdateCracker
        where TUpdate : class
    {
        protected readonly Func<Update, TUpdate?> _updateResolver;

        protected AbstractUpdateCracker(
            Func<Update, TUpdate?> updateResolver,
            AbstractChannel<TUpdate> updateChannel,
            CancelTriggerAbs<TUpdate>? cancelTrigger = default)
        {
            _updateResolver = updateResolver ??
                throw new ArgumentNullException(nameof(updateResolver));
            UpdateChannel = updateChannel ??
                throw new ArgumentNullException(nameof(updateChannel));
            CancelTrigger = cancelTrigger;
        }

        public IUpdateChannel UpdateChannel { get; }

        public ICancelTrigger? CancelTrigger { get; }

        protected abstract T Crack(TUpdate update);

        public object? CrackerExpression(Update update)
        {
            return Crack(_updateResolver(update) ??
                throw new InvalidOperationException("Inner update is null."));
        }
    }
}
