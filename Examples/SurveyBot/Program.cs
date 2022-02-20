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
    var filler = new FormFiller<SimpleSurvey>(
        ctx => ctx
        .AddCracker(
            x => x.HowLovelyWeAre, new CallbackQueryCracker<HowLovelyWeAre>(
                new CallbackQueryChannel(
                    TimeSpan.FromSeconds(30),
                    FilterCutify.DataMatches(@"^HLWA_")),

                x => x.ToHowLovelyWeAre(),

                new CallbackQueryCancelTrigger(
                    FilterCutify.DataMatches("^cancel$")
                )
            )
        )
        .AddCracker(
            x => x.FoundFromWhere, new CallbackQueryCracker<FoundFromWhere>(
                new CallbackQueryChannel(
                    TimeSpan.FromSeconds(30),
                    FilterCutify.DataMatches(@"^FFW_")),

                x => x.ToFoundFromWhere(),

                new CallbackQueryCancelTrigger(
                    FilterCutify.DataMatches("^cancel$")
                )
            )
        )
    );

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