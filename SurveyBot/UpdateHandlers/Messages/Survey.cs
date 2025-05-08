using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers;
using TelegramUpdater.FillMyForm.UpdateCrackers.Crackers;
using TelegramUpdater.Filters;
using TelegramUpdater.UpdateChannels.ReadyToUse;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.UpdateContainers;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;

namespace SurveyBot.UpdateHandlers.Messages;

internal class Survey : MessageHandler
{
    protected override async Task HandleAsync(MessageContainer container)
    {
        var callbackCancelTrigger = new CallbackQueryCancelTrigger(
             new MyCallbackQueryRegexFilter("^cancel$")); // Cancellation will trigger on "cancel" callback data.

        var filler = new FormFiller<SimpleSurvey>(
            container.Updater,
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
                new CallbackQueryCracker<FoundFromWhere>(       // Same.
                    new CallbackQueryChannel(
                        TimeSpan.FromSeconds(5),
                        ReadyFilters.DataMatches(@"^FFW_")),
                    x => x.ToFoundFromWhere(),
                    callbackCancelTrigger)));


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

class MyCallbackQueryRegexFilter(string pattern) : Filter<CallbackQuery>()
{
    public override bool TheyShellPass(CallbackQuery input)
    {
        if (input.Data is null) return false;

        return new StringRegex(pattern).TheyShellPass(input.Data);
    }
}