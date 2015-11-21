using UnityEngine;
using System.Collections;

namespace Logmew.Service.InfluxDb
{
	public class InfluxDbLogService
	{
		private readonly InfluxDbLogGateway gateway;

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.Service.InfluxDb.LogCollectorService"/> class.
		/// </summary>
		public InfluxDbLogService()
		{
			gateway = new InfluxDbLogGateway();
		}

		/// <summary>
		/// Gets the gateway.
		/// </summary>
		/// <value>The gateway.</value>
		public InfluxDbLogGateway Gateway { get { return gateway; } }

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
		public void ActivateUnityLogCollector()
		{
			var builder = new InfluxDbLogBuilder();
			gateway.RegisterConverter(typeof(UnityLogEntry), builder);

			UnityLogManager.OnLog -= gateway.SendAsync;
			UnityLogManager.OnLog += gateway.SendAsync;
			UnityLogManager.Active = true;
		}

		/// <summary>
		/// Activates the android log collector.
		/// </summary>
		[System.Diagnostics.Conditional("UNITY_ANDROID")]
		public void ActivateAndroidLogCollector()
		{
			#if !UNITY_EDITOR && UNITY_ANDROID
			var builder = new InfluxDbLogBuilder();
			gateway.RegisterConverter(typeof(AndroidLogEntry), builder);

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
		public void ActivateIosLogCollector()
		{
			#if !UNITY_EDITOR && UNITY_IOS
			var builder = new InfluxDbLogBuilder();
			gateway.RegisterConverter(typeof(IosLogEntry), builder);

			IosLogManager.OnLog -= gateway.SendAsync;
			IosLogManager.OnLog += gateway.SendAsync;
			IosLogManager.Active = true;
			#endif
		}
	}
}
