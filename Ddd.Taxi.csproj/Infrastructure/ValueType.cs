using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit;

namespace Ddd.Infrastructure
{
    public class ValueType<T>
    {
        static IEnumerable<PropertyInfo> properties;
        static ValueType() => properties = typeof(T)
                              .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .OrderBy(prop => prop.Name);

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != typeof(T))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return properties.All(prop => Equals(prop.GetValue(this), (prop.GetValue(obj))));
        }

        public bool Equals(T obj) => Equals((object)obj);

        public override int GetHashCode()
        {
            var a = new ValueType<int>();
            var res = unchecked((int)2166136261);
            var prime = 16777619;
            unchecked
            {
                foreach (var prop in properties)
                    res = (res ^ prop.GetValue(this).GetHashCode()) * prime;
            }
            return res;
        }

        public override string ToString() =>
            $"{typeof(T).Name}({string.Join("; ", properties.Select(s => $"{s.Name}: {s.GetValue(this, null)}"))})";
    }
}