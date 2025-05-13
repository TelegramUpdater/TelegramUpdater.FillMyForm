using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.FillMyForm.CancelTriggers.Triggers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace SurveyBot.UpdateHandlers.Messages;

[ApplyFilter(typeof(FormStartFilter))]
internal class FormHandler : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        var filler = container.CreateFormFiller<MySimpleForm>(
            defaultCancelTrigger: new MessageCancelTextTrigger());

        var form = await filler.StartFilling(container.Sender()!);

        if (form is not null)
        {
            await container.Response($"Thank you, {form}");
        }
        else
        {
            await container.Response($"Please try again later.");
        }
    }
}

class FormStartFilter : UpdaterFilter<Message>
{
    public FormStartFilter() : base(ReadyFilters.OnCommand("form")) { }
}
