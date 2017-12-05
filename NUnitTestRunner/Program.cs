using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using NUnit.Engine;

namespace NUnitTestRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("NUnitTestRunner Starting...");
			Console.WriteLine("Using NUnit Engine {0}", GetNUnitEngineVersion());

			using (var activator = new CustomTestEngineActivator())
			using (var engine = activator.CreateInstance())
			{
				Console.WriteLine("Created engine instance.");

				var assemblyToTest = @"..\..\..\NUnitGuineaPig\bin\Debug\NUnitGuineaPig.dll";
				Console.WriteLine("Will run tests on {0} (NUnit Framework {1})", Path.GetFileName(assemblyToTest), GetNUnitFrameworkVersion(assemblyToTest));

				var builder = engine.Services.GetService<ITestFilterService>().GetTestFilterBuilder();
				var filter = builder.GetFilter();

				for (var i = 1; i <= 5; i++)
				{
					Console.WriteLine("[Iteration #{0}]: Starting...", i);
					using (var runner = engine.GetRunner(new TestPackage(assemblyToTest)))
					{
						Console.WriteLine("[Iteration #{0}]: Created runner.", i);
						Console.WriteLine("[Iteration #{0}]: Starting tests...", i);

						Console.WriteLine("[Iteration #{0}]: Starting...", i);

						var xml = runner.Run(new TestEventListener(), filter);
						var text = XmlToText(xml);

						var cases = xml.SelectNodes("//test-case");
						Console.WriteLine("[Iteration #{0}]: Ran {1} tests.", i,cases.Count);

						foreach (XmlNode testCase in cases)
						{
							var fullName = testCase.Attributes["fullname"].Value;
							var result = testCase.Attributes["result"].Value;

							Console.WriteLine("[Iteration #{0}]: {1}: {2}", i, fullName, result);
						}
					}
					Console.WriteLine("[Iteration #{0}]: Completed.", i);
				}

				Console.WriteLine("NUnitTestRunner Finished. Disposing of engine...");
			}
			Console.WriteLine("Completed.");
		}

		static string GetNUnitEngineVersion() => FileVersionInfo.GetVersionInfo(typeof(ITestEngine).Assembly.Location).ProductVersion;

		static string GetNUnitFrameworkVersion(string assemblyToTest)
		{
			var directory = Path.GetDirectoryName(assemblyToTest);
			var frameworkAssembly = Path.Combine(directory, "nunit.framework.dll");
			return FileVersionInfo.GetVersionInfo(frameworkAssembly).ProductVersion;
		}

		static string XmlToText(XmlNode node)
		{
			using (var ms = new MemoryStream())
			using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
			{
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 2;
				writer.IndentChar = ' ';

				node.WriteTo(writer);
				writer.Flush();

				return Encoding.UTF8.GetString(ms.GetBuffer());
			}
		}
	}
}