using System;
using System.Text.RegularExpressions;

namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// Append Unity stack trace to tail of ILogEntryUnity.Message.
	/// </summary>
	public class AppendUnityStackTrace : ILogEntryMessageFormatter
	{
		private static string baseFilePath;

		private static Regex RegexUnityDebugLog;
		private static Regex RegexUnityMethod;
		private static Regex RegexLocation;
		private static Regex RegexMethod;

		private string delimiter;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static AppendUnityStackTrace()
		{
			// Determine base file directory.
			var stackFrame = new System.Diagnostics.StackFrame(1, true);
			if (stackFrame != null) {
				var filePath = stackFrame.GetFileName();
				if (filePath != null) {
					var indexToRelPath = Math.Max(0, filePath.LastIndexOf("/Assets/"));
					if (0 <= indexToRelPath) {
						baseFilePath = filePath.Substring(0, indexToRelPath);
					}
				}
			}

			RegexUnityDebugLog = new Regex(@"^[\S\s]*?UnityEngine\.Debug:(Log|Assert)[^\n]+\n");
			RegexUnityMethod = new Regex(@":(?=\D)");
			RegexMethod = new Regex(@"^   at ", RegexOptions.Multiline);
			RegexLocation = new Regex(@" in ([^:]+):line (\d+)$", RegexOptions.Multiline);
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="delimiter">Delimiter string, used between message and stacktrace. Default is " @@ "</param>
		public AppendUnityStackTrace(string delimiter = " @@ ")
		{
			this.delimiter = delimiter;
		}

		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="message">Message.</param>
		/// <param name="logEntry">Log entry.</param>
		public string FormatMessage(string message, ILogEntry logEntry)
		{
			var trace = logEntry.StackTrace;
			if (string.IsNullOrEmpty(trace)) {
				return message + this.delimiter;
			}

			trace = RegexUnityDebugLog.Replace(trace, "");
			if (trace.Length != trace.Length) {
				// trace string is passed by Unity.
				trace = RegexUnityMethod.Replace(trace, ".");
			} else {
				// trace string is generated from System.Diagnostics.StackTrace.
				if (baseFilePath != null) {
					trace = trace.Replace(baseFilePath, "");
				}
				trace = RegexMethod.Replace(trace, "");
				trace = RegexLocation.Replace(trace, " " + "(at $1:$2)");
			}
			trace = trace.TrimEnd();

			return string.Format("{0}{1}{2}", message, delimiter, trace);
		}
	}
}
