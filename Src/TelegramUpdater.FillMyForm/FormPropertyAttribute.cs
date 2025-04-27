using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;

namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Indicates options for a from property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FormPropertyAttribute : Attribute
{
    /// <summary>
    /// Get or set input priority for this property.
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Time out in seconds.
    /// </summary>
    public int TimeOut { get; set; } = 30;

    /// <summary>
    /// Get or set cancel trigger type for this property.
    /// </summary>
    /// <remarks>
    /// Should be type of <see cref="ICancelTrigger"/>
    /// </remarks>
    public Type? CancelTriggerType
    { 
        get => CancelTriggerType;
        set
        {
            if (typeof(ICancelTrigger).IsAssignableFrom(value))
            {
                var trigger = Activator.CreateInstance(value);
                if (trigger is AbstractCancelTrigger<Message> cancelTrigger)
                {
                    CancelTrigger = cancelTrigger;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Cancel trigger type should an instance of CancelTriggerAbs<Message>. "
                        + "Use crackers for more complex and customized cancel trigger.");
                }
            }
            else
            {
                throw new InvalidOperationException("Cancel trigger type should implement ICancelTrigger.");
            }
        }
    }

    /// <summary>
    /// The trigger that cancels filling this property.
    /// </summary>
    public AbstractCancelTrigger<Message>? CancelTrigger { get; private set; }
}
