using System.ComponentModel.DataAnnotations;
using Telegram.Bot.Types;
using TelegramUpdater.RainbowUtlities;

namespace TelegramUpdater.FillMyForm
{
    public record RetryContext(int TriedCount,
                               int MaximumTries,
                               bool CanTryAgain);

    public record TimeoutContext(RetryContext? RetryContext,
                                 TimeSpan ExceptedTimeout);

    public record ConversationErrorContext(RetryContext? RetryContext,
                                           Type ExceptedType);

    public record ValidationErrorContext(RetryContext? RetryContext,
                                         IEnumerable<ValidationResult> ValidationResults);

    public record OnUnrelatedUpdateContext(ShiningInfo<long, Update> ShiningInfo);

    public record OnSuccessContext(object? Value);
}
