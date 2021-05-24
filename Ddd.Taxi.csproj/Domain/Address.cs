﻿using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
	public class Address : ValueType<Address>
	{
		public Address(string street, string building)
		{
			Street = street;
			Building = building;
		}

		public string Building { get; }

		public string Street { get; }

	}
}