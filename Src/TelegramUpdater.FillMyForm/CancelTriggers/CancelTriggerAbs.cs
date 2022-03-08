using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers
{
    public abstract class CancelTriggerAbs<T> : ICancelTrigger where T : class
    {
        protected readonly Func<Update, T?> _updateResolver;

        public UpdateType UpdateType { get; }

        protected CancelTriggerAbs(Func<Update, T?> updateResolver, UpdateType updateType)
        {
            _updateResolver = updateResolver;
            UpdateType = updateType;
        }

        protected abstract bool ShouldCancel(T resolved);

        public bool ShouldCancel(Update update)
        {
            if (UpdateType == update.Type)
            {

                return ShouldCancel(_updateResolver(update) ??
                    throw new NullReferenceException("Inner update is null?"));
            }

            return false;
        }
    }
}
