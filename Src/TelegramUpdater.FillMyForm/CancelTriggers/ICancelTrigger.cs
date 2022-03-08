using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers
{
    public interface ICancelTrigger
    {
        public UpdateType UpdateType { get; }

        public bool ShouldCancel(Update update);
    }
}
