using System;
using UnityEngine;

namespace Logmew
{
	/// <summary>
	/// UnityEngine.Debug compatible logging behaviour.
	/// </summary>
	public class UnityLoggerBase<T> where T : UnityLoggerBase<T>, new()
	{
		protected string tag = string.Empty;
		protected int stackTraceSkipFrameCount = 2;

		private static T instance;

		protected static UnityLoggerBase<T> getInstance()
		{
			return instance = instance ?? new T();
		}

		public static void Log(object message)
		{
			log(LogType.Log, message);
		}

		public static void LogWarning(object message)
		{
			log(LogType.Warning, message);
		}

		public static void LogError(object message)
		{
			log(LogType.Error, message);
		}

		public static void LogException(Exception e)
		{
			if (e != null) {
				log(LogType.Exception, e.Message, e.StackTrace);
			} else {
				log(LogType.Exception, null);
			}
		}

		public static void Assert(bool condition)
		{
			if (!condition) {
				log(LogType.Assert, "Assertion failed!");
			}
		}

		public static void Assert(bool condition, object message)
		{
			if (!condition) {
				log(LogType.Assert, message);
			}
		}

		public static void LogFormat(string format, params object[] args)
		{
			log(LogType.Log, string.Format(format, args));
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			log(LogType.Warning, string.Format(format, args));
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			log(LogType.Error, string.Format(format, args));
		}

		public static void Assert(bool condition, string format, params object[] args)
		{
			if (!condition) {
				log(LogType.Assert, string.Format(format, args));
			}
		}

		private static void log(LogType logType, object message)
		{
			var instance = getInstance();

			string stackTrace;
			if (UnityLogManager.ProvidesStackTrace) {
				var trace = new System.Diagnostics.StackTrace(instance.stackTraceSkipFrameCount, true);
				stackTrace = trace.ToString();
			} else {
				stackTrace = string.Empty;
			}

			var logEntry = new UnityLogEntry(logType, instance.tag, message, stackTrace);
			UnityLogManager.Log(logEntry);
		}

		private static void log(LogType logType, object message, string stackTrace)
		{
			var instance = getInstance();
			var logEntry = new UnityLogEntry(logType, instance.tag, message, stackTrace);
			UnityLogManager.Log(logEntry);
		}
	}

	public class Logger : UnityLoggerBase<Logger>
	{
	}
}
