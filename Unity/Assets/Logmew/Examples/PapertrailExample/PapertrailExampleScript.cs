using System;
using UnityEngine;
using UnityEngine.UI;
using Logmew;
using Logmew.Service.Papertrail;

namespace LogmewExamples.ServicePapertrail
{
	internal class PapertrailExampleScript : MonoBehaviour
	{
		#pragma warning disable 0649
		public string PapertrailHost;
		public int PapertrailPort;
		public string HostName;
		public string AppName;
		#pragma warning restore 0649

		PapertrailLogService logCollectorService;

		void Start()
		{
			GameObject.Find("LogButton").GetComponent<Button>().onClick.AddListener(onLogClick);
			GameObject.Find("WarningButton").GetComponent<Button>().onClick.AddListener(onWarningClick);
			GameObject.Find("ErrorButton").GetComponent<Button>().onClick.AddListener(onErrorClick);
			GameObject.Find("ExceptionButton").GetComponent<Button>().onClick.AddListener(onExceptionClick);
			GameObject.Find("AssertButton").GetComponent<Button>().onClick.AddListener(onAssertClick);
			GameObject.Find("LogLevel").GetComponent<Slider>().onValueChanged.AddListener(onLogLevelChanged);
			GameObject.Find("StackTrace").GetComponent<Toggle>().onValueChanged.AddListener(onStackTraceChanged);
			GameObject.Find("EnableLog").GetComponent<Toggle>().onValueChanged.AddListener(onEnableLogChanged);

			if (string.IsNullOrEmpty(PapertrailHost) || PapertrailPort == 0) {
				throw new ArgumentException("Fill PapertrailHost and PapertrailPort in UnityEditor");
			}

			logCollectorService = new PapertrailLogService(HostName, AppName);
			logCollectorService.ConnectToServer(PapertrailHost, PapertrailPort);

			logCollectorService.ActivateUnityLogCollector();
			logCollectorService.ActivateIosLogCollector();
			logCollectorService.ActivateAndroidLogCollector();
		}

		private void onLogClick()
		{
			Debug.Log("This is a log level message.");
		}

		private void onWarningClick()
		{
			Action action = () => {
				Debug.LogWarning("This is a warning level message.");
			};
			action();
		}

		private void onErrorClick()
		{
			Debug.LogError("This is an error level message.");
		}

		private void onExceptionClick()
		{
			try {
				throw new Exception("This is an exception sample.");
			} catch (Exception e) {
				Debug.LogException(e);
			}
		}

		private void onAssertClick()
		{
			Debug.Assert(false, "This is a sample assert.");
		}

		void onLogLevelChanged(float value)
		{
			LogLevel logLevel = (LogLevel)(5 - value);
			GameObject.Find("LogLevelLabel").GetComponent<Text>().text =
				string.Format("Minimum Log Level: {0}", logLevel);
			UnityLogManager.MinLogLevel = logLevel;
#if !UNITY_EDITOR && UNITY_ANDROID
			AndroidLogManager.MinLogLevel = logLevel;
#elif !UNITY_EDITOR && UNITY_IOS
			IosLogManager.MinLogLevel = logLevel;
#endif
		}

		void onStackTraceChanged(bool value)
		{
			UnityLogManager.ProvidesStackTrace = value;
#if !UNITY_EDITOR && UNITY_ANDROID
			AndroidLogManager.ProvidesStackTrace = value;
#elif !UNITY_EDITOR && UNITY_IOS
			IosLogManager.ProvidesStackTrace = value;
#endif
		}

		void onEnableLogChanged(bool value)
		{
			UnityLogManager.Active = value;
#if !UNITY_EDITOR && UNITY_ANDROID
			AndroidLogManager.Active = value;
#elif !UNITY_EDITOR && UNITY_IOS
			IosLogManager.Active = value;
#endif
		}
	}
}
