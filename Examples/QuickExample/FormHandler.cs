using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.ScopedHandlers.ReadyToUse;

namespace QuickExample;

[ApplyFilter(typeof(FormStartFilter))]
internal class FormHandler : ScopedMessageHandler
{
    protected override async Task HandleAsync(IContainer<Message> updateContainer)
    {
        var filler = new FormFiller<MySimpleForm>(
            defaultCancelTrigger: new MessageCancelTextTrigger());

        var ok = await filler.FillAsync(updateContainer.Sender()!, updateContainer);

        if (ok)
        {
            await updateContainer.Response($"Thank you, {filler.Form}");
        }
        else
        {
            await updateContainer.Response($"Please try again later.");
        }
    }
}

class FormStartFilter : Filter<Message>
{
    public FormStartFilter() : base(FilterCutify.OnCommand("form")) { }
}
