﻿using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramUpdater.FillMyForm.CancelTriggers.Triggers;

/// <inheritdoc />
public class MessageCancelTextTrigger()
    : AbstractCancelTrigger<Message>(x => x.Message, UpdateType.Message)
{
    /// <inheritdoc />
    protected override bool ShouldCancel(Message resolved)
    {
        return resolved switch
        {
            { Text: { } txt } => string.Equals(txt, "/cancel", StringComparison.OrdinalIgnoreCase),
            _ => false,
        };
    }
}
