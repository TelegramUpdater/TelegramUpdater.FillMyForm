using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.RainbowUtilities;

namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Full context about a field of form begin filled.
/// </summary>
/// <typeparam name="TForm">Type of the form.</typeparam>
/// <param name="filler">The filler responsible for filling this form.</param>
/// <param name="askingFrom">Information about a user being asked from.</param>
/// <param name="propertyName">The name of form property being filled.</param>
public class FormFillerContext<TForm>(
    FormFiller<TForm> filler, User askingFrom, string propertyName)
    where TForm: IForm, new()
{
    /// <summary>
    /// The filler responsible for filling this form.
    /// </summary>
    public FormFiller<TForm> Filler { get; } = filler;

    /// <summary>
    /// Information about a user being asked from.
    /// </summary>
    public User AskingFrom { get; } = askingFrom;

    /// <summary>
    /// The name of form property being filled.
    /// </summary>
    public string PropertyName { get; } = propertyName;

    /// <summary>
    /// The updater.
    /// </summary>
    public IUpdater Updater => Filler.Updater;

    /// <summary>
    /// The bot client.
    /// </summary>
    public ITelegramBotClient TelegramBotClient => Updater.BotClient; 
}

/// <summary>
/// Context about a retry operation begin done.
/// </summary>
/// <param name="TriedCount"></param>
/// <param name="MaximumTries"></param>
/// <param name="CanTryAgain"></param>
public record RetryContext(int TriedCount, int MaximumTries, bool CanTryAgain);

/// <summary>
/// Context about an operation begin timed out.
/// </summary>
/// <param name="RetryContext"></param>
/// <param name="ExceptedTimeout"></param>
public record TimeoutContext(RetryContext? RetryContext, TimeSpan ExceptedTimeout);

/// <summary>
/// Context about an unrelated updated received.
/// </summary>
/// <param name="ShiningInfo"></param>
public record OnUnrelatedUpdateContext(ShiningInfo<long, Update> ShiningInfo);

/// <summary>
/// Context about a field value set with success.
/// </summary>
/// <param name="Value"></param>
/// <param name="ShiningInfo"></param>
public record OnSuccessContext(object? Value, ShiningInfo<long, Update>? ShiningInfo);

/// <summary>
/// Context about an operation being canceled.
/// </summary>
/// <param name="ShiningInfo"></param>
public record OnCancelContext(ShiningInfo<long, Update> ShiningInfo);

/// <summary>
/// Context about a conversation to expected type failed.
/// </summary>
/// <param name="RetryContext"></param>
/// <param name="ExceptedType"></param>
/// <param name="ShiningInfo"></param>
public record ConversationErrorContext(
    RetryContext? RetryContext,
    Type ExceptedType,
    ShiningInfo<long, Update>? ShiningInfo);

/// <summary>
/// Context about validations on a specified field has been failed.
/// </summary>
/// <param name="RetryContext"></param>
/// <param name="ShiningInfo"></param>
/// <param name="RequiredItemNotSupplied"></param>
/// <param name="ValidationResults"></param>
public record ValidationErrorContext(
    RetryContext? RetryContext,
    ShiningInfo<long, Update>? ShiningInfo,
    bool RequiredItemNotSupplied,
    IEnumerable<ValidationResult> ValidationResults);

/// <summary>
/// Extension methods for form filler.
/// </summary>
public static class FormFillerContextExtensions
{
    /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage(ITelegramBotClient, ChatId, string, ParseMode, ReplyParameters?, ReplyMarkup?, LinkPreviewOptions?, int?, IEnumerable{MessageEntity}?, bool, bool, string?, string?, bool, CancellationToken)"/>
    public static async Task<Message> SendMessage<TForm>(
        this FormFillerContext<TForm> formFiller,
        string text,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? entities = null,
        bool disableWebPagePreview = default,
        bool disableNotification = default,
        int replyToMessageId = default,
        bool allowSendingWithoutReply = default,
        ReplyMarkup? replyMarkup = null,
        CancellationToken cancellationToken = default) where TForm : IForm, new()
    {
        return await formFiller.TelegramBotClient.SendMessage(
            chatId: formFiller.AskingFrom.Id,
            text: text,
            parseMode: parseMode,
            entities: entities,
            linkPreviewOptions:disableWebPagePreview,
            disableNotification: disableNotification,
            replyParameters: new ReplyParameters
            {
                MessageId = replyToMessageId,
                AllowSendingWithoutReply = allowSendingWithoutReply,
            },
            replyMarkup: replyMarkup,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
