using System.Linq.Expressions;
using System.Reflection;

namespace TelegramUpdater.FillMyForm.UpdateCrackers;

/// <summary>
/// A context class to initialize your crackers to be added to the <see cref="FormFiller{TForm}"/>.
/// </summary>
/// <typeparam name="TForm">Your form.</typeparam>
public class CrackerContext<TForm> where TForm : IForm, new()
{
    private readonly Dictionary<string, IUpdateCracker> _propertyCrackers;

    /// <summary>
    /// Creates an instance of cracker instance. Use
    /// <see cref="AddCracker{TProperty, TUpdate}(Expression{Func{TForm, TProperty}}, AbstractUpdateCracker{TProperty, TUpdate})"/>
    /// to add your crackers.
    /// </summary>
    public CrackerContext()
    {
        _propertyCrackers = [];
    }

    /// <summary>
    /// Add your cracker.
    /// </summary>
    /// <typeparam name="TProperty">Type of property you're creating a cracker for.</typeparam>
    /// <typeparam name="TUpdate">Type of update you wanna crack it.</typeparam>
    /// <param name="propertySelector">A selector to select a property from your <typeparamref name="TForm"/>.</param>
    /// <param name="cracker">Add a suitable ( with <typeparamref name="TUpdate"/> ) cracker instance.</param>
    /// <returns></returns>
    public CrackerContext<TForm> AddCracker<TProperty, TUpdate>(
        Expression<Func<TForm, TProperty>> propertySelector,
        AbstractUpdateCracker<TProperty, TUpdate> cracker)
        where TUpdate: class
    {
        var prop = (PropertyInfo)((MemberExpression)propertySelector.Body).Member;
        if (_propertyCrackers.ContainsKey(prop.Name))
        {
            _propertyCrackers[prop.Name] = cracker;
        }
        else
        {
            _propertyCrackers.Add(prop.Name, cracker);
        }
        return this;
    }

    internal void Build(FormFiller<TForm> formFiller)
    {
        foreach (var cracker in _propertyCrackers)
        {
            formFiller.AddCracker(cracker.Key, cracker.Value);
        }
    }
}
