namespace TelegramUpdater.FillMyForm;

/// <summary>
/// Use this attribute on a property so that the <see cref="FormFiller{TForm}"/> will ignore it.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FillerIgnoreAttribute : Attribute { }
