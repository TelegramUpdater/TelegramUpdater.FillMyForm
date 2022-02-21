using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.FillMyForm
{
    public class FormFillterContext<TForm> where TForm: IForm, new()
    {
        public FormFillterContext(
            FormFiller<TForm> filler, User askingFrom, string propertyName)
        {
            Filler = filler;
            AskingFrom = askingFrom;
            PropertyName = propertyName;
        }

        public FormFiller<TForm> Filler { get; }

        public User AskingFrom { get; }

        public string PropertyName { get; }

        public IUpdater Updater => Filler.Updater;

        public ITelegramBotClient TelegramBotClient => Updater.BotClient; 
    }

    public record RetryContext(int TriedCount, int MaximumTries, bool CanTryAgain);

    public record TimeoutContext(RetryContext? RetryContext, TimeSpan ExceptedTimeout);

    public record OnUnrelatedUpdateContext(ShiningInfo<long, Update> ShiningInfo);

    public record OnSuccessContext(object? Value, ShiningInfo<long, Update>? ShiningInfo);

    public record OnCancelContext(ShiningInfo<long, Update> ShiningInfo);

    public record ConversationErrorContext(RetryContext? RetryContext,
                                           Type ExceptedType,
                                           ShiningInfo<long, Update>? ShiningInfo);

    public record ValidationErrorContext(RetryContext? RetryContext,
                                         ShiningInfo<long, Update>? ShiningInfo,
                                         bool RequiredItemNotSupplied,
                                         IEnumerable<ValidationResult> ValidationResults);
}
