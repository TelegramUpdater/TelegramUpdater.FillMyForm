// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using SurveyBot;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.ExceptionHandlers;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.FillMyForm.CancelTriggers.SealedTriggers;
using TelegramUpdater.FillMyForm.UpdateCrackers.SealedCrackers;
using TelegramUpdater.UpdateChannels.SealedChannels;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.SealedHandlers;


await new Updater(new TelegramBotClient("BOT_TOKEN"))
    .AddExceptionHandler(new ExceptionHandler<Exception>(HandleException, inherit: true))
    .AddUpdateHandler(new MessageHandler(HandleUpdate, FilterCutify.OnCommand("survey")))
    .StartAsync();


async Task HandleUpdate(IContainer<Message> ctnr)
{
    var callbackCancelTrigger = new CallbackQueryCancelTrigger(FilterCutify.DataMatches("^cancel$"));

    var filler = new FormFiller<SimpleSurvey>(
        ctx => ctx
        // Add custom cracker for each property
        .AddCracker(
            x => x.HowLovelyWeAre, new CallbackQueryCracker<HowLovelyWeAre>(
                new CallbackQueryChannel(TimeSpan.FromSeconds(30), FilterCutify.DataMatches(@"^HLWA_")), // 1. Get matching update
                x => x.ToHowLovelyWeAre(),                                                               // 2. Extract value from update
                callbackCancelTrigger))                                                                  // 3. Add a cancel trigger

        .AddCracker(
            x => x.FoundFromWhere, new CallbackQueryCracker<FoundFromWhere>(
                new CallbackQueryChannel(TimeSpan.FromSeconds(30), FilterCutify.DataMatches(@"^FFW_")),
                x => x.ToFoundFromWhere(),
                callbackCancelTrigger)));

    var ok = await filler.FillAsync(ctnr.Sender()!, ctnr);

    if (ok)
    {
        await ctnr.Response($"Thank you, {filler.Form}");
    }
    else
    {
        await ctnr.Response($"Please try again later.");
    }
}

static Task HandleException(IUpdater updater, Exception exception)
{
    updater.Logger.LogError(exception: exception, "Error in handlers");
    return Task.CompletedTask;
}