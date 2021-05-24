using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ddd.Infrastructure
{
	/// <summary>
	/// Базовый класс для всех Value типов.
	/// </summary>
	public class ValueType<T>
	{
		public override bool Equals(object obj)
        {
			if (obj is null ||!obj.GetType().Equals( typeof(T)))
				return false;
			var objT = obj.GetType();
			var type = typeof(T);

			if (object.ReferenceEquals(obj, this))
				return true;
			return type.GetProperties().All(prop => prop.GetValue(this).Equals(prop.GetValue(obj)));
        }
			

        public override int GetHashCode()
        {
			int basePrimal = unchecked(0);
			foreach (var prop in typeof(T).GetFields())
			unchecked
			{
				basePrimal += prop.GetValue(this).GetHashCode();
			};
			return basePrimal;
        }

        public override string ToString()
        {
			var fields = typeof(T).GetProperties();
			return $"{typeof(T).Name}({string.Join("; ", fields.Select(s => $"{s.Name}: {s.GetValue(this,null)}"))})";
		}
    }
}