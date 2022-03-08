using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.UpdateChannels;

namespace TelegramUpdater.FillMyForm.UpdateCrackers
{
    public interface IUpdateCracker
    {
        public IUpdateChannel UpdateChannel { get; }

        public object? CrackerExpression(Update update);

        public ICancelTrigger? CancelTrigger { get; }

        internal bool TryCrack(Update update, out object? cracked)
        {
            try
            {
                cracked = CrackerExpression(update);
                return true;
            }
            catch
            {
                cracked = null;
                return false;
            }
        }
    }
}
