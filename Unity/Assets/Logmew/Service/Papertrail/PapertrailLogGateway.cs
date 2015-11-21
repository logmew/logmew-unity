using System;
using System.Collections.Generic;
using System.Threading;
using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;
using UnityEngine;
using System.Net.Sockets;

namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// Send log to Papertrail.
	/// </summary>
	public class PapertrailLogGateway
	{
		private const int MAX_QUEUE_COUNT = 1000;

		/// <summary>
		/// The converterMap maps [type of LogEntry] to [ILogEntryConverter].
		/// </summary>
		private Dictionary<Type, ILogEntryConverter> converterMap;

		private Queue<ILogEntry> queue;
		private AutoResetEvent queueActiveEvent;

		private SyslogUdpSender udpSender;
		private ManualResetEvent workerActiveEvent;

		/// <summary>
		/// The papertrail server host.
		/// </summary>
		private string host;

		/// <summary>
		/// The papertrail server port.
		/// </summary>
		private int port;

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.Papertrail.PapertrailGateway"/> class.
		/// </summary>
		public PapertrailLogGateway()
		{
			converterMap = new Dictionary<Type, ILogEntryConverter>();
			queue = new Queue<ILogEntry>(MAX_QUEUE_COUNT);
			queueActiveEvent = new AutoResetEvent(false);
			workerActiveEvent = new ManualResetEvent(false);
			new Thread(worker).Start();
		}

		/// <summary>
		/// Registers converter instance which will convert specific log entry to syslogMessage.
		/// </summary>
		/// <param name="logEntryType">Type of Log entry.</param>
		/// <param name="converter">Converter instance.</param>
		public void RegisterConverter(Type logEntryType, ILogEntryConverter converter)
		{
			converterMap[logEntryType] = converter;
		}

		/// <summary>
		/// Establish a connection to the Papertrail server.
		/// </summary>
		/// <param name="host">Papertrail server host.</param>
		/// <param name="port">Papertrail server port.</param>
		public void Connect(string host, int port)
		{
			if (this.host == host &&
				this.port == port) {
				return;
			}

			this.host = host;
			this.port = port;
			if (udpSender != null) {
				udpSender.Dispose();
			}
			workerActiveEvent.Set();
		}

		/// <summary>
		/// Disconnect the connection to Papertrail server.
		/// </summary>
		public void Disconnect()
		{
			if (udpSender != null) {
				udpSender.Dispose();
			}
			workerActiveEvent.Reset();
		}

		/// <summary>
		/// Send log to Papertrail asynchronously.
		/// </summary>
		/// <param name="syslogMessageSource">SyslogMessage generator.</param>
		internal void SendAsync(ILogEntry logEntry)
		{
			lock (queue) {
				// discard overflown message
				if (MAX_QUEUE_COUNT <= queue.Count) {
					queue.Dequeue();
				}

				queue.Enqueue(logEntry);
			}

			if (workerActiveEvent.WaitOne(0)) {
				queueActiveEvent.Set();
			}
		}

		private void worker()
		{
			while (workerActiveEvent.WaitOne()) {
				if (connect()) {
					try {
						processLogQueue();
					} catch (Exception e) {
						if (!(e is ThreadAbortException)) {
							// FIXME needs more appropriate reporting
							UnityEngine.Debug.LogException(e);
						}
					}
				}
			}
		}

		private const int RECONNECT_DELAY_DEFAULT = 300;
		private const int RECONNECT_DELAY_MAX = 5 * 1000;
		private int reconnectDelay = RECONNECT_DELAY_DEFAULT;

		private bool connect()
		{
			try {
				if (udpSender != null) {
					try {
						udpSender.Dispose();
					} finally {
						udpSender = null;
					}
				}

				udpSender = new SyslogUdpSender(host, port);
				reconnectDelay = RECONNECT_DELAY_DEFAULT;
				// set queueActiveEvent while the queue may have any entry.
				queueActiveEvent.Set();
				return true;
			} catch (Exception e) {
				if (!(e is ThreadAbortException)) {
					// FIXME needs more appropriate reporting
					UnityEngine.Debug.LogException(e);
				}
				Thread.Sleep(reconnectDelay);
				reconnectDelay = Math.Min(reconnectDelay * 2, RECONNECT_DELAY_MAX);
				return false;
			}
		}

		private void processLogQueue()
		{
			var syslogSerializer = new SyslogRfc5424MessageSerializer();

			while (queueActiveEvent.WaitOne()) {
				while (true) {
					ILogEntry logEntry = null;
					lock (queue) {
						if (queue.Count == 0) {
							break;
						}
						logEntry = queue.Peek();
					}

					ILogEntryConverter converter;
					if (converterMap.TryGetValue(logEntry.GetType(), out converter)) {
						SyslogMessage message = converter.ToSyslogMessage(logEntry);
						try {
							udpSender.Send(message, syslogSerializer);
						} catch (SocketException e) {
							if (e.SocketErrorCode == SocketError.MessageSize) {
								UnityEngine.Debug.LogWarning("[Logmew] Logmew.Service.Papertrail.PapertrailGateway: message is too long to send\n" +
									logEntry.ToString().Substring(0, 200));
							} else {
								throw e;
							}
						}

						lock (queue) {
							queue.Dequeue();
						}
					}
				}
			}
		}
	}
}
