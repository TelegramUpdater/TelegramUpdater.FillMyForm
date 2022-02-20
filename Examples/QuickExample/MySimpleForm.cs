using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.RainbowUtlities;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace QuickExample;

internal class MySimpleForm : AbstractForm
{
    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    public string FirstName { get; set; } = null!;

    [MinLength(3)]
    [MaxLength(32)]
    public string? LastName { get; set; } // can be null, It's Nullable!

    [Required]
    [Range(13, 120)]
    public int Age { get; set; }

    public override string ToString()
    {
        return string.Format("{0} {1}, {2} years old.", FirstName, LastName?? "", Age);
    }

    public override async Task OnBeginAskAsync(IUpdater updater,
                                               User askingFrom,
                                               string propertyName,
                                               CancellationToken cancellationToken)
    {
        await updater.BotClient.SendTextMessageAsync(
            askingFrom.Id, $"Please send me a value for {propertyName}",
            replyMarkup: new ForceReplyMarkup(),
            cancellationToken: cancellationToken);
    }

    public override Task OnSuccessAsync(RawContainer? container,
                                        User askingFrom,
                                        string propertyName,
                                        OnSuccessContext onSuccessContext,
                                        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override async Task OnValidationErrorAsync(IUpdater updater,
                                                ShiningInfo<long, Update>? shiningInfo,
                                                User user,
                                                string propertyName,
                                                ValidationErrorContext validationErrorContext,
                                                CancellationToken cancellationToken)
    {
        if (validationErrorContext.RequiredItemNotSupplied)
        {
            await updater.BotClient.SendTextMessageAsync(
                user.Id, $"{propertyName} was required! You can't just leave it.");
        }
        else
        {
            await updater.BotClient.SendTextMessageAsync(
                user.Id,
                $"You input is invalid for {propertyName}.\n" +
                string.Join("\n", validationErrorContext.ValidationResults.Select(
                    x=> x.ErrorMessage)));
        }
    }
}
