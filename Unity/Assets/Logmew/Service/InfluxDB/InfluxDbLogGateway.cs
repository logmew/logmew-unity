using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

namespace Logmew.Service.InfluxDb
{
	/// <summary>
	/// Send log to InfluxDb.
	/// </summary>
	public class InfluxDbLogGateway
	{
		private const int MAX_QUEUE_COUNT = 1000;
		private const int SUITABLE_MAX_BUFFER_SIZE = 65536;

		/// <summary>
		/// The builderMap maps [type of LogEntry] to [IInfluxDbLogBuilder].
		/// </summary>
		private Dictionary<Type, IInfluxDbLogBuilder> builderMap;

		private Queue<ILogEntry> queue;
		private AutoResetEvent queueActiveEvent;

		private UdpClient udpClient;
		private ManualResetEvent workerActiveEvent;

		/// <summary>
		/// The InfluxDB server host.
		/// </summary>
		private string host;

		/// <summary>
		/// The InfluxDB server port.
		/// </summary>
		private int port;

		/// <summary>
		/// Initializes a new instance of the <see cref="Logmew.InfluxDb.InfluxDbGateway"/> class.
		/// </summary>
		public InfluxDbLogGateway()
		{
			builderMap = new Dictionary<Type, IInfluxDbLogBuilder>();
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
		public void RegisterConverter(Type logEntryType, IInfluxDbLogBuilder builder)
		{
			builderMap[logEntryType] = builder;
		}

		/// <summary>
		/// Establish a connection to the InfluxDb server.
		/// </summary>
		/// <param name="host">InfluxDb host.</param>
		/// <param name="port">InfluxDb port.</param>
		public void Connect(string host, int port)
		{
			if (this.host == host &&
				this.port == port) {
				return;
			}

			this.host = host;
			this.port = port;
			if (udpClient != null) {
				udpClient.Close();
			}
			workerActiveEvent.Set();
		}

		/// <summary>
		/// Disconnect the connection to InfluxDb server.
		/// </summary>
		public void Disconnect()
		{
			if (udpClient != null) {
				udpClient.Close();
			}
			workerActiveEvent.Reset();
		}

		/// <summary>
		/// Send log to InfluxDb asynchronously.
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
				if (udpClient != null) {
					try {
						udpClient.Close();
					} finally {
						udpClient = null;
					}
				}

				udpClient = new UdpClient(host, port);
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
			var stream = new MemoryStream(1024);
			var writer = new StreamWriter(stream);

			while (queueActiveEvent.WaitOne()) {

				// populate stream(send data buffer) with all log entries in the queue
				while (true) {
					ILogEntry logEntry = null;
					lock (queue) {
						if (queue.Count == 0) {
							break;
						}
						logEntry = queue.Dequeue();
					}

					IInfluxDbLogBuilder builder;
					if (builderMap.TryGetValue(logEntry.GetType(), out builder)) {
						builder.Append(writer, logEntry);
						writer.Flush();
					}

					// If the concatinated log length exceeds suitable length,
					// send it now
					if (SUITABLE_MAX_BUFFER_SIZE < stream.Length) {
						break;
					}
				}

				if (stream.Length == 0) {
					continue;
				}

				// send
				try {
					udpClient.Send(stream.GetBuffer(), (int)stream.Length);
				} catch (SocketException e) {
					if (e.SocketErrorCode == SocketError.MessageSize) {
						UnityEngine.Debug.LogWarningFormat("[Logmew] Logmew.Service.InfluxDb.InfluxDbGateway: message is too long to send ({0} bytes)",
							stream.Length);
					} else {
						throw e;
					}
				}

				// reset stream
				if (SUITABLE_MAX_BUFFER_SIZE < stream.Length) {
					stream.Capacity = SUITABLE_MAX_BUFFER_SIZE;
				}
				stream.SetLength(0);
			}
		}
	}
}
