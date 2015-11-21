using UnityEngine;
using System.Collections;

namespace Logmew.Service.Papertrail
{
	public class PapertrailLogService
	{
		private readonly PapertrailLogGateway gateway;
		private readonly string hostName;
		private readonly string appName;

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.Service.Papertrail.LogCollectorService"/> class.
		/// </summary>
		/// <param name="hostName">Host name.</param>
		/// <param name="appName">App name.</param>
		public PapertrailLogService(string hostName, string appName)
		{
			this.hostName = hostName;
			this.appName = appName;
			gateway = new PapertrailLogGateway();
		}

		/// <summary>
		/// Gets the gateway.
		/// </summary>
		/// <value>The gateway.</value>
		public PapertrailLogGateway Gateway { get { return gateway; } }

		/// <summary>
		/// Connects to server.
		/// </summary>
		/// <param name="serverHost">Server host.</param>
		/// <param name="serverPort">Server port.</param>
		public void ConnectToServer(string serverHost, int serverPort)
		{
			gateway.Connect(serverHost, serverPort);
		}

		/// <summary>
		/// Activates the unity log collector.
		/// </summary>
		/// <param name="formatters">Formatters.</param>
		public void ActivateUnityLogCollector(ILogEntryMessageFormatter[] formatters = null)
		{
			formatters = formatters ?? getDefaultFormatters();

			var converter = new LogEntryConverter(formatters);
			converter.HostName = hostName;
			converter.AppName = appName;
			gateway.RegisterConverter(typeof(UnityLogEntry), converter);

			UnityLogManager.OnLog -= gateway.SendAsync;
			UnityLogManager.OnLog += gateway.SendAsync;
			UnityLogManager.Active = true;
		}

		/// <summary>
		/// Activates the android log collector.
		/// </summary>
		/// <param name="formatters">Formatters.</param>
		[System.Diagnostics.Conditional("UNITY_ANDROID")]
		public void ActivateAndroidLogCollector(ILogEntryMessageFormatter[] formatters = null)
		{
#if !UNITY_EDITOR && UNITY_ANDROID
			formatters = formatters ?? getDefaultFormatters();

			var converter = new LogEntryConverter(formatters);
			converter.HostName = hostName;
			converter.AppName = appName;
			gateway.RegisterConverter(typeof(AndroidLogEntry), converter);

			AndroidLogManager.OnLog -= gateway.SendAsync;
			AndroidLogManager.OnLog += gateway.SendAsync;
			AndroidLogManager.Active = true;
#endif
		}

		/// <summary>
		/// Activates the android log collector.
		/// </summary>
		/// <param name="formatters">Formatters.</param>
		[System.Diagnostics.Conditional("UNITY_IOS")]
		public void ActivateIosLogCollector(ILogEntryMessageFormatter[] formatters = null)
		{
#if !UNITY_EDITOR && UNITY_IOS
			formatters = formatters ?? getDefaultFormatters();

			var converter = new LogEntryConverter(formatters);
			converter.HostName = hostName;
			converter.AppName = appName;
			gateway.RegisterConverter(typeof(IosLogEntry), converter);

			IosLogManager.OnLog -= gateway.SendAsync;
			IosLogManager.OnLog += gateway.SendAsync;
			IosLogManager.Active = true;
#endif
		}

		protected virtual ILogEntryMessageFormatter[] getDefaultFormatters()
		{
			return new ILogEntryMessageFormatter[] {
				new AnsiColorlizeMessage(),
				new AppendStackTrace(),
				new PrependTag(),
				new EscapeMessage(),
			};
		}
	}
}
