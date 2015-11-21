using System;
using System.Runtime.InteropServices;
using SyslogNet.Client;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_ANDROID
namespace Logmew
{
	/// <summary>
	/// Receives logs from Android codebase.
	/// </summary>
	public static class AndroidLogManager
	{
		private static bool active;

		/// <summary>
		/// Occurs when new log comes.
		/// </summary>
		public static event Action<AndroidLogEntry> OnLog = delegate { };

		public static bool GeneratesStackTrace;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Logmew.LogmewAndroidBridge"/> is active.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public static bool Active
		{
			get { return active; }
			set
			{
				if (active == value) {
					return;
				}
				active = value;

				var bridge = new AndroidJavaClass("logmew.bridge.LogBridge");
				if (active) {
					bridge.CallStatic("enableLogmewBridge");
					bridge.CallStatic("setCallback", new AndroidJavaRunnable(fetchLoop));
				} else {
					bridge.CallStatic("disableLogmewBridge");
				}
			}
		}

		/// <summary>
		/// Sets the minimum log level.
		/// </summary>
		/// <value>The minimum log level.</value>
		public static LogLevel MinLogLevel
		{
			set
			{
				var bridge = new AndroidJavaClass("logmew.bridge.LogBridge");
				bridge.CallStatic("setMinLogLevel", (int)value);
			}
		}

		/// <summary>
		/// To include stacktrace in log or not.
		/// </summary>
		/// <value><c>true</c> to provide stack trace; otherwise, <c>false</c>.</value>
		public static bool ProvidesStackTrace
		{
			set
			{
				var bridge = new AndroidJavaClass("logmew.bridge.LogBridge");
				bridge.CallStatic("providesStackTrace", value);
			}
		}

		private static void fetchLoop()
		{
			var bridge = new AndroidJavaClass("logmew.bridge.LogBridge");
			while (active) {
				var androidLogEntry = bridge.CallStatic<AndroidJavaObject>("getLogEntry");
				if (!isNullOrEmpty(androidLogEntry)) {
					var logEntry = new AndroidLogEntry(androidLogEntry);
					OnLog(logEntry);
				}
			}
			bridge = null;
		}

		private static bool isNullOrEmpty(AndroidJavaObject androidLogEntry)
		{
			return (androidLogEntry == null) || (androidLogEntry.Get<long>("timestamp") < 0);
		}
	}
}
#endif
