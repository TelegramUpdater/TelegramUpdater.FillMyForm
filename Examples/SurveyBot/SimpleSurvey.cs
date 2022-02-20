﻿using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUpdater;
using TelegramUpdater.FillMyForm;
using TelegramUpdater.UpdateContainer.UpdateContainers;

namespace SurveyBot
{
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

        public override async Task OnBeginAskAsync(IUpdater updater, User askingFrom, string propertyName, CancellationToken cancellationToken)
        {
            if (propertyName == "HowLovelyWeAre")
            {
                await updater.BotClient.SendTextMessageAsync(
                    askingFrom.Id, "How much do you love us?",
                    replyMarkup: Extensions.HowLovelyWeAreButtons(),
                    cancellationToken: cancellationToken);
            }
            else
            {
                await updater.BotClient.SendTextMessageAsync(
                    askingFrom.Id, "Where did you find us?",
                    replyMarkup: Extensions.HowFoundFromWhereButtons(),
                    cancellationToken: cancellationToken);
            }
        }

        public override async Task OnSuccessAsync(RawContainer? container, User askingFrom, string propertyName, OnSuccessContext onSuccessContext, CancellationToken cancellationToken)
        {
            if (container is null) return;

            switch (container)
            {
                case { BotClient: { } bot, ShiningInfo: { Value: { CallbackQuery: { Message: { } msg } } update } }:
                    {
                        await bot.EditMessageTextAsync(
                            msg.Chat.Id, msg.MessageId,
                            $"Got it {onSuccessContext.Value}",
                            cancellationToken: cancellationToken);
                        break;
                    }
            }
        }
    }
}
