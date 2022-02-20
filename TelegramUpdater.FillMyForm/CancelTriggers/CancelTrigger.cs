using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers
{
    public class CancelTrigger<T> : CancelTriggerAbs<T> where T : class
    {
        private readonly Func<T, bool> _shouldCancel;

        public CancelTrigger(
            Func<Update, T?> updateResolver,
            UpdateType updateType,
            Filter<T> shouldCancel) : base(updateResolver, updateType)
        {
            _shouldCancel = shouldCancel;
        }

        protected override bool ShouldCancel(T resolved)
        {
            return _shouldCancel(resolved);
        }
    }
}
