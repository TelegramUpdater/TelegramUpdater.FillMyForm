﻿using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using TelegramUpdater.FillMyForm;

namespace SurveyBot;

enum FoundFromWhere
{
    Google = 4,
    Friend = 3,
    Company = 2,
    None = 1
}

enum HowLovelyWeAre
{
    SuperLovely = 4,
    NotBadLovely = 3,
    NotLovely = 2,
    IHateYou = 1
}

internal class SimpleSurvey : AbstractForm
{
    [Required]
    public HowLovelyWeAre HowLovelyWeAre { get; set; }

    public FoundFromWhere FoundFromWhere { get; set; } = FoundFromWhere.None;

    public override string ToString()
    {
        return $"Survey result: 1. {HowLovelyWeAre}, 2. {FoundFromWhere}";
    }

    public override async Task OnBeginAsk<TForm>(
        FormFillingContext<TForm> fillerContext, CancellationToken cancellationToken)
    {
        if (fillerContext.PropertyName == "HowLovelyWeAre")
        {
            await fillerContext.SendMessage(
                "How much do you love us?",
                replyMarkup: Extensions.HowLovelyWeAreButtons(),
                cancellationToken: cancellationToken);
        }
        else
        {
            await fillerContext.SendMessage(
                "Where did you find us?",
                replyMarkup: Extensions.HowFoundFromWhereButtons(),
                cancellationToken: cancellationToken);
        }
    }

    public override async Task OnSuccess<TForm>(
        FormFillingContext<TForm, OnSuccessContext> fillerContext, CancellationToken cancellationToken)
    {
        switch (fillerContext.Context)
        {
            case { ShiningInfo.Value.CallbackQuery.Message: { } msg }:
                {
                    await fillerContext.TelegramBotClient.DeleteMessage(
                        msg.Chat.Id, msg.MessageId, cancellationToken: cancellationToken);
                    break;
                }
        }
    }

    public override async Task OnCancel<TForm>(
        FormFillingContext<TForm, OnCancelContext> fillerContext, CancellationToken cancellationToken)
    {
        switch (fillerContext.Context)
        {
            case { ShiningInfo.Value.CallbackQuery.Message: { } msg }:
                {
                    await fillerContext.TelegramBotClient.EditMessageText(
                        msg.Chat.Id, msg.Id, "Ok.", replyMarkup: null, cancellationToken: cancellationToken);
                    break;
                }
        }
    }
}
