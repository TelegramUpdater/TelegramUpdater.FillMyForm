using System.ComponentModel.DataAnnotations;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUpdater.FillMyForm;

namespace SurveyBot;

internal class MySimpleForm : AbstractForm
{
    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    [FillingRetry(FillingError.ValidationError, 2)]
    public string FirstName { get; set; } = null!;

    [MinLength(3)]
    [MaxLength(32)]
    [FillingRetry(FillingError.ValidationError, 2)]
    public string? LastName { get; set; } // can be null, It's Null-able!

    [Required]
    [Range(13, 120)]
    [FillingRetry(FillingError.ValidationError, 2)]
    public int Age { get; set; }

    public override string ToString()
    {
        return string.Format("{0} {1}, {2} years old.", FirstName, LastName ?? "", Age);
    }

    public override async Task OnBeginAsk<TForm>(FormFillingContext<TForm> ctx, CancellationToken cancellationToken)
    {
        await ctx.SendMessage(
            $"Please send me a value for {ctx.PropertyName}",
            replyMarkup: new ForceReplyMarkup(),
            cancellationToken: cancellationToken);
    }

    public override Task OnSuccess<TForm>(FormFillingContext<TForm, OnSuccessContext> ctx, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public override async Task OnValidationError<TForm>(FormFillingContext<TForm, ValidationErrorContext> ctx, CancellationToken cancellationToken)
    {
        if (ctx.Context.RequiredItemNotSupplied)
        {
            await ctx.SendMessage($"{ctx.PropertyName} was required! You can't just leave it.", cancellationToken: cancellationToken);
        }
        else
        {
            await ctx.SendMessage($"You input is invalid for {ctx.PropertyName}.\n" +
                string.Join("\n", ctx.Context.ValidationResults.Select(
                    x => x.ErrorMessage)), cancellationToken: cancellationToken);
        }
    }
}
