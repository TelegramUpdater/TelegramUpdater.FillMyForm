using TelegramUpdater;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.FillMyForm.CancelTriggers.Triggers;
using TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;
using TelegramUpdater.UpdateChannels.ReadyToUse;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace SurveyBot.UpdateHandlers.Messages;

internal class Survey : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        var callbackCancelTrigger =
            new CallbackQueryRegexCancelTrigger("^cancel$"); // Cancellation will trigger on "cancel" callback data.

        var filler = container.CreateFormFiller<SimpleSurvey>(
            ctx => ctx
            // Add custom cracker for each property
            .AddCracker(
                x => x.HowLovelyWeAre,                          // 1. Select the targeted property.
                new CallbackQueryCracker<HowLovelyWeAre>(       // 2. Create a cracker
                    new CallbackQueryChannel(                   //      2-1. Use channels to get matching update
                        TimeSpan.FromSeconds(5),               //          2-1-1. A time out for waiting time
                        ReadyFilters.DataMatches(@"^HLWA_")),   //          2-1-2. A filter to match callback data with given regex
                    x => x.ToHowLovelyWeAre(),                  //      2-2. Extract a value for the propery from update.
                    callbackCancelTrigger)                      //      2-3. Add a cancel trigger.
                )

            .AddCracker(
                x => x.FoundFromWhere,
                new CallbackQueryCracker<FoundFromWhere>(       // Same. But different! but still same.
                    timeOut: TimeSpan.FromSeconds(5),
                    cracker: x => x.ToFoundFromWhere(),
                    filter: ReadyFilters.DataMatches(@"^FFW_"),
                    cancelTrigger: callbackCancelTrigger)));


        var form = await filler.StartFilling(container.Sender()!); // I'm sure the sender is not null, are you?

        if (form is not null) // Form got filled.
        {
            await container.Response($"Thank you, {form}");
        }
        else // Something is wrong
        {
            await container.Response($"Please try again later.");
        }
    }
}
