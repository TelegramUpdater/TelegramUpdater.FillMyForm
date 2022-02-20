using System.Linq.Expressions;
using System.Reflection;

namespace TelegramUpdater.FillMyForm.UpdateCrackers
{
    public class CrackerContext<TForm> where TForm : IForm, new()
    {
        private readonly Dictionary<string, IUpdateCracker> _propertyCrackers;

        public CrackerContext()
        {
            _propertyCrackers = new();
        }

        public CrackerContext<TForm> AddCracker<TProperty>(
            Expression<Func<TForm, TProperty>> propertySelector, IUpdateCracker cracker)
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
}
