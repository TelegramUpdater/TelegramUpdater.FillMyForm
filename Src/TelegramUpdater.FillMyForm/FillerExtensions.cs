using Telegram.Bot.Types;
using TelegramUpdater.FillMyForm.CancelTriggers;
using TelegramUpdater.FillMyForm.UpdateCrackers;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateContainer.Tags;

namespace TelegramUpdater.FillMyForm;

/// <summary>
/// A set of extension methods for forms and fillers.
/// </summary>
public static class FillerExtensions
{
    /// <summary>
    /// Create an instance of form filler.
    /// </summary>
    /// <param name="updater">Updater instance.</param>
    /// <param name="buildCrackers">Add crackers to the filler.</param>
    /// <param name="additionalConverters">Add converters for other data types, int long float supported.</param>
    /// <param name="defaultCancelTrigger">A default cancel trigger to use for all.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static FormFiller<TForm> CreateFormFiller<TForm>(
        this IUpdater updater,
        Action<CrackerContext<TForm>>? buildCrackers = default,
        IEnumerable<Type>? additionalConverters = default,
        ICancelTrigger? defaultCancelTrigger = default)
        where TForm : IForm, new()
        => new(updater, buildCrackers, additionalConverters, defaultCancelTrigger);

    /// <inheritdoc cref="CreateFormFiller{TForm}(IUpdater, Action{CrackerContext{TForm}}?, IEnumerable{Type}?, ICancelTrigger?)"/>
    public static FormFiller<TForm> CreateFormFiller<TForm>(
        this IContainer container,
        Action<CrackerContext<TForm>>? buildCrackers = default,
        IEnumerable<Type>? additionalConverters = default,
        ICancelTrigger? defaultCancelTrigger = default)
        where TForm : IForm, new()
        => new(container.Updater, buildCrackers, additionalConverters, defaultCancelTrigger);

    /// <summary>
    /// Create an instance of form filler.
    /// </summary>
    /// <remarks>
    /// Make sure the <typeparamref name="C"/> has a non-null <see cref="User"/> inside it.
    /// </remarks>
    /// <param name="container">The update container.</param>
    /// <param name="buildCrackers">Add crackers to the filler.</param>
    /// <param name="additionalConverters">Add converters for other data types, int long float supported.</param>
    /// <param name="defaultCancelTrigger">A default cancel trigger to use for all.</param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task<TForm?> GetAndFillForm<TForm, C>(
        this C container,
        Action<CrackerContext<TForm>>? buildCrackers = default,
        IEnumerable<Type>? additionalConverters = default,
        ICancelTrigger? defaultCancelTrigger = default,
        CancellationToken cancellationToken = default)
        where TForm : IForm, new()
        where C : IContainer, ISenderUserExtractable
    {
        var filler = new FormFiller<TForm>(container.Updater, buildCrackers, additionalConverters, defaultCancelTrigger);

        return filler.StartFilling(
            container.GetSenderUser()?? throw new ArgumentNullException(
                nameof(container), "A User object cannot be extracted from this container."),
            cancellationToken);
    }
}
