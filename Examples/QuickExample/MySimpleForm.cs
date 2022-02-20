using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace QuickExample;

internal class MySimpleForm : AbstractForm
{
    [Required]
    [FormProperty(CancelTriggerType = typeof(MessageCancelTextTrigger))]
    public string FirstName { get; set; } = null!;

    [FormProperty(CancelTriggerType = typeof(MessageCancelTextTrigger))]
    public string? LastName { get; set; } // can be null, It's Nullable!

    [Required]
    [FormProperty(CancelTriggerType = typeof(MessageCancelTextTrigger))]
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
}
