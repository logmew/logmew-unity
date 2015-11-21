using Logmew;

namespace LogmewExamples.ServiceInfluxDb
{
	// Sample of log snatching.
	//
	// This Debug class should eats(overrides) `Debug.xxx()` call
	// within the same namespace and its child namespces.
	//
	// With the combination of #if !UNITY_EDITOR macro,
	// this results
	//   - If the app runs on the Unity Editor:
	//     The default UnityEngine.xxx() will be called.
	//     So normal log will be output in the Console,
	//     the default behaviour should be available.
	//     (double click on the row to jump to the exact line, etc.)
	//   - If the app runs app on gadgets:
	//     Our Debug class eats logs.
	//     It enables to control Unity logs will be suppressed, never sent to gadget's
	//     Console.
	//
#if !UNITY_EDITOR
	internal class Debug : UnityLoggerBase<Debug>
	{
		public Debug()
		{
			tag = "PapertrailLogExample";
		}
	}
#endif
}
