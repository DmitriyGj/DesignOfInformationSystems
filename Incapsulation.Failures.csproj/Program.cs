using System;
using System.Collections.Generic;
using System.Linq;
using Incapsulation.Failures;
using NUnitLite;

class Program
{
	static void Main(string[] args)
	{
		new AutoRun().Execute(args);
		Console.ReadKey();
	}
}
