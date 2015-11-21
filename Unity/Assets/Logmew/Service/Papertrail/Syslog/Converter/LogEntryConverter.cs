using System;
using System.Collections.Generic;
using SyslogNet.Client;
using UnityEngine;

namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// Converts ILogEntry to SyslogMessage.
	/// </summary>
	internal class LogEntryConverter : ILogEntryConverter
	{
		private static StructuredDataElement emptyStructuredElement = new StructuredDataElement("empty", new Dictionary<string, string>());

		private ILogEntryMessageFormatter[] formatters;

		/// <summary>
		/// The host name. Papertrail treat this as its `system` name.
		/// </summary>
		private string hostName;

		/// <summary>
		/// The app name. This is `APP-NAME` in syslog format.
		/// </summary>
		private string appName;


		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="formatters">Formatters.</param>
		public LogEntryConverter(ILogEntryMessageFormatter[] formatters)
		{
			if (formatters == null) {
				this.formatters = new ILogEntryMessageFormatter[0];
			} else {
				this.formatters = formatters;
			}
		}

		/// <summary>
		/// Gets or sets the host name. Papertrail treat this as its `system` name.
		/// </summary>
		/// <value>The host name.</value>
		public string HostName
		{
			get
			{
				if (string.IsNullOrEmpty(hostName)) {
					try {
						hostName = System.Net.Dns.GetHostName();
					} catch {
						hostName = "unknown";
					}
				}
				return hostName;
			}
			set { hostName = value; }
		}

		/// <summary>
		/// Gets or sets the app name. This is `APP-NAME` in syslog format.
		/// </summary>
		/// <value>The name of the app.</value>
		public string AppName
		{
			get
			{
				if (string.IsNullOrEmpty(appName)) {
					appName = "Unity";
				}
				return appName;
			}
			set { appName = value; }
		}

		/// <summary>
		/// Convert ILogEntry to SyslogMessage.
		/// </summary>
		/// <returns>The SyslogMessage instance.</returns>
		/// <param name="logEntry">Log entry.</param>
		public SyslogMessage ToSyslogMessage(ILogEntry logEntry)
		{
			var message = logEntry.Message;
			foreach (var formatter in formatters) {
				message = formatter.FormatMessage(message, logEntry);
			}

			return new SyslogMessage(
				logEntry.TimeStamp,
				Facility.UserLevelMessages,
				toSeverity(logEntry.LogLevel),
				HostName,
				AppName,
				null,
				logEntry.Tag,
				message,
				emptyStructuredElement
			);
		}

		private Severity toSeverity(LogLevel logLevel)
		{
			switch (logLevel) {
				case LogLevel.Exception:
				case LogLevel.Assert:
				case LogLevel.Error:
					return Severity.Error;
				case LogLevel.Warning:
					return Severity.Warning;
				case LogLevel.Info:
					return Severity.Informational;
				case LogLevel.Debug:
				default:
					return Severity.Debug;
			}
		}

	}
}
