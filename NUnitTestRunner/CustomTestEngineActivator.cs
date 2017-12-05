using System;
using System.Diagnostics.Tracing;
using System.Reflection;
using NUnit.Engine;
using NUnit.Engine.Services;

namespace NUnitTestRunner
{
	class CustomTestEngineActivator : IDisposable
	{
		public ITestEngine CreateInstance()
		{
			DisableEventListenerShutdownEvents();
			var engine = new TestEngine();
			InitalizeEngineServices(engine.Services);
			return engine;
		}

		static void InitalizeEngineServices(ServiceContext services)
		{
			services.Add(new SettingsService(false));
			services.Add(new DomainManager());
			services.Add(new ExtensionService());
			services.Add(new DriverService());
			services.Add(new RecentFilesService());
			services.Add(new ProjectService());
			services.Add(new RuntimeFrameworkService());
			services.Add(new DefaultTestRunnerFactory());
			services.Add(new TestAgency());
			services.Add(new ResultService());
			services.Add(new TestFilterService());

			services.ServiceManager.StartServices();
		}

		/* Prevent the following error from occuring:
			System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
			   at System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
			   at System.Collections.Generic.List`1.Enumerator.MoveNextRare()
			   at System.Collections.Generic.List`1.Enumerator.MoveNext()
			   at System.Diagnostics.Tracing.EventListener.DisposeOnShutdown(Object sender, EventArgs e)
			Internally EventListener maintains a weak reference list of all EventSource objects, hooks the AppDomain.Current.ProcessExit and disposes them all.
			Looks like a bug in the .NET framework.
			Prevent the hook by setting the s_EventSourceShutdownRegistered field, and dispose all sources manually.
		*/

		static void DisableEventListenerShutdownEvents()
		{
			var eventSourceShutdownRegisteredField = typeof(EventListener).GetField("s_EventSourceShutdownRegistered", BindingFlags.NonPublic | BindingFlags.Static);
			eventSourceShutdownRegisteredField?.SetValue(null, true);
		}

		public void Dispose()
		{
			foreach (var source in EventSource.GetSources())
			{
				source.Dispose();
			}
		}
	}
}
