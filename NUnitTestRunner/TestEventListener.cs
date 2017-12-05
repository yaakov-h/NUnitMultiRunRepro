using System;
using NUnit.Engine;

namespace NUnitTestRunner
{
	[Serializable]
	class TestEventListener : ITestEventListener
	{
		public void OnTestEvent(string report)
		{
			// Console.WriteLine("[EVENT]: {0}", report);
		}
	}
}
