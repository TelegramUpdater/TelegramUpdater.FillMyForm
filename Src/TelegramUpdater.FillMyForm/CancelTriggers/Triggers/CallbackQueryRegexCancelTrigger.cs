using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using TelegramUpdater.Filters;

namespace TelegramUpdater.FillMyForm.CancelTriggers.Triggers;

/// <summary>
/// Cancel trigger checks <see cref="CallbackQuery.Data"/> against a regex pattern.
/// </summary>
public class CallbackQueryRegexCancelTrigger : CallbackQueryCancelTrigger
{
    /// <summary>
    /// Creates a new instance of <see cref="CallbackQueryRegexCancelTrigger"/>.
    /// </summary>
    public CallbackQueryRegexCancelTrigger(Regex regex)
        : base(new BasicRegexFilter<CallbackQuery>(x=> x.Data, regex))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="CallbackQueryRegexCancelTrigger"/>.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> Use an overload with <see cref="Regex.MatchTimeout"/> is set.
    /// </remarks>
    /// <param name="pattern"></param>
    public CallbackQueryRegexCancelTrigger(string pattern)
        : base(new BasicRegexFilter<CallbackQuery>(
#pragma warning disable MA0009 // Add regex evaluation timeout
            x => x.Data, new Regex(pattern)))
#pragma warning restore MA0009 // Add regex evaluation timeout
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="CallbackQueryRegexCancelTrigger"/>.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> Use an overload with <see cref="Regex.MatchTimeout"/> is set.
    /// </remarks>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    public CallbackQueryRegexCancelTrigger(string pattern, RegexOptions options)
        : base(new BasicRegexFilter<CallbackQuery>(
#pragma warning disable MA0009 // Add regex evaluation timeout
            x => x.Data, new Regex(pattern, options)))
#pragma warning restore MA0009 // Add regex evaluation timeout
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="CallbackQueryRegexCancelTrigger"/>.
    /// </summary>
    /// <param name="pattern"></param>
    /// <param name="options"></param>
    /// <param name="timeOut"></param>
    public CallbackQueryRegexCancelTrigger(string pattern, RegexOptions options, TimeSpan timeOut)
        : base(new BasicRegexFilter<CallbackQuery>(
            x => x.Data, new Regex(pattern, options, timeOut)))
    {
    }
}
