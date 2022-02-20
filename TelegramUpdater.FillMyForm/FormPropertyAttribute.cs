using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;

namespace TelegramUpdater.FillMyForm;

[AttributeUsage(AttributeTargets.Property)]
public class FormPropertyAttribute : Attribute
{
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Time out in seconds.
    /// </summary>
    public int TimeOut { get; set; } = 30;

    /// <summary>
    /// Get or set cancel trigger type for this poropety.
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
                if (trigger is CancelTriggerAbs<Message> cancelTrigger)
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

    public CancelTriggerAbs<Message>? CancelTrigger { get; private set; }
}
