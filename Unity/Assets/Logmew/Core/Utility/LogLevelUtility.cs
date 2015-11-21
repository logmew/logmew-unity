using System;
using UnityEngine;

namespace Logmew
{
	public static class LogLevelUtility
	{
		public static LogLevel FromLogType(LogType logType)
		{
			switch (logType) {
				case LogType.Error:
					return LogLevel.Error;
				case LogType.Assert:
					return LogLevel.Assert;
				case LogType.Exception:
					return LogLevel.Exception;
				case LogType.Warning:
					return LogLevel.Warning;
				case LogType.Log:
				default:
					return LogLevel.Debug;
			}
		}

		public static LogType ToLogType(LogLevel logLevel)
		{
			switch (logLevel) {
				case LogLevel.Error:
					return LogType.Error;
				case LogLevel.Assert:
					return LogType.Assert;
				case LogLevel.Exception:
					return LogType.Exception;
				case LogLevel.Warning:
					return LogType.Warning;
				case LogLevel.Debug:
				default:
					return LogType.Log;
			}
		}
	}
}
