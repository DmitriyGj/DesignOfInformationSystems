using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Reflection.Randomness
{
    public class Generator<T> where T:new()
    {
        Type type;
        Dictionary<PropertyInfo, FromDistribution> propertiesWithAttributes;
        public T Generate(Random rnd)
        {
            var resultDistribution = new T();
            foreach (var pwa in propertiesWithAttributes)
                pwa.Key.SetValue(resultDistribution, pwa.Value.Distribution.Generate(rnd));
            return resultDistribution;
        }

        public Generator()
        {
            type = typeof(T);
            propertiesWithAttributes = type.GetProperties()
                .Where(prop => prop.GetCustomAttributes()
                .OfType<FromDistribution>().Count() > 0)
                .ToDictionary(prop => prop, prop => prop.GetCustomAttributes().OfType<FromDistribution>().First());
        }
    }

    public class FromDistribution : Attribute
    {
        public IContinuousDistribution Distribution { get; set; }
        public FromDistribution(Type distributionType, params object[] parametrs)
        {
            if (!distributionType.GetConstructors().Any(ctor=>ctor.GetParameters().Count() == parametrs.Length))
                throw new ArgumentException(distributionType.ToString());
            Distribution = Activator.CreateInstance(distributionType,parametrs) as IContinuousDistribution;
        }
    }
}
