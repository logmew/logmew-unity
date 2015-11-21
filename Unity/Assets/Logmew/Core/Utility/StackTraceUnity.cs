using System;
using System.Text.RegularExpressions;

namespace Logmew
{
	public static class StackTraceUnity
	{
		private static string baseFilePath;

		private static Regex reUnityDebugLog = new Regex(@"^[\S\s]*?UnityEngine\.Debug:(Log|Assert)[^\n]+\n");
		private static Regex reUnityMethod = new Regex(@":(?=\D)");
		private static Regex reMethod = new Regex(@"^\s+at ", RegexOptions.Multiline);
		private static Regex reLocation = new Regex(@" in ([^:]+):line (\d+)\s*$", RegexOptions.Multiline);
		private static Regex reLocation2 = new Regex(@" in ([^:]+):(\d+)\s*$", RegexOptions.Multiline);

		/// <summary>
		/// Static constructor.
		/// </summary>
		static StackTraceUnity()
		{
			baseFilePath = determineBaseFilePath();
		}

		public static string Normalize(string stackTrace)
		{
			if (string.IsNullOrEmpty(stackTrace)) {
				return stackTrace;
			}

			stackTrace = reUnityDebugLog.Replace(stackTrace, "");
			if (stackTrace.Length != stackTrace.Length) {
				// trace string is passed by Unity.
				stackTrace = reUnityMethod.Replace(stackTrace, ".");
			} else {
				// trace string is generated from System.Diagnostics.StackTrace.
				if (baseFilePath != null) {
					stackTrace = stackTrace.Replace(baseFilePath, "");
				}
				stackTrace = reMethod.Replace(stackTrace, "");
				stackTrace = reLocation.Replace(stackTrace, "@$1:$2 ");
				stackTrace = reLocation2.Replace(stackTrace, "@$1:$2 ");
			}
			stackTrace = stackTrace.TrimEnd();

			return stackTrace;
		}

		private static string determineBaseFilePath()
		{
#if LOGMEW_STRIP_BASEPATH
			// Determine base file directory.
			var stackFrame = new System.Diagnostics.StackFrame(1, true);
			if (stackFrame != null) {
				var filePath = stackFrame.GetFileName();
				if (filePath != null) {
					var indexToRelPath = Math.Max(0, filePath.LastIndexOf("/Assets/"));
					if (0 <= indexToRelPath) {
						return filePath.Substring(0, indexToRelPath + 1);
					}
				}
			}
#endif
			return null;
		}
	}
}
