using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SyslogNet.Client;
using UnityEngine;

namespace Logmew.Service.InfluxDb
{
	/// <summary>
	/// Provides a set of services that convert a IPapertrailLogEntry to other form.
	/// </summary>
	public class InfluxDbLogBuilder : IInfluxDbLogBuilder
	{
		private static Regex reTagKeyValue = new Regex("([, =])");

		protected readonly Dictionary<string, string> tags;
		protected List<string> sortedTagKeys;
		protected bool tagsUpdated;

		public InfluxDbLogBuilder()
		{
			tags = new Dictionary<string, string>();
			tagsUpdated = true;
		}

		public void AddTag(string key, string value)
		{
			var escapedKey = escapeTagKeyValue(key);
			var escapedValue = escapeTagKeyValue(value);

			tags[escapedKey] = escapedValue;
			tagsUpdated = true;
		}

		public void RemoveTag(string key)
		{
			var escapedKey = escapeTagKeyValue(key);

			if (tags.Remove(escapedKey)) {
				tagsUpdated = true;
			}
		}

		/// <summary>
		/// Convert ILogEntry to SyslogMessage.
		/// </summary>
		/// <returns>The SyslogMessage instance.</returns>
		/// <param name="writer">Stream writer.</param>
		/// <param name="logEntry">Log entry.</param>
		public virtual void Append(StreamWriter writer, ILogEntry logEntry)
		{
			// Key
			writer.Write("events");

			// tags
			tags["origin"] = escapeTagKeyValue(logEntry.Origin);
			tags["logLevel"] = logEntry.LogLevel.ToString();
			tags["tag"] = escapeTagKeyValue(logEntry.Tag ?? string.Empty);

			if (tagsUpdated) {
				sortTagKeys();
			}

			for (int i = 0, end = sortedTagKeys.Count; i < end; ++i) {
				var escapedTagKey = sortedTagKeys[i];
				appendTag(writer, escapedTagKey, tags[escapedTagKey]);
			}

			// Fiels

			// logEntry.Message
			appendField(writer, " message", logEntry.Message);

			// logEntry.StackTrace
			var stackTrace = logEntry.StackTrace;
			if (!string.IsNullOrEmpty(stackTrace)) {
				appendField(writer, ",stackTrace", stackTrace);
			}

			// Timestamp
			appendTimestamp(writer, logEntry.TimeStamp);
		}

		protected void sortTagKeys()
		{
			sortedTagKeys = new List<string>(tags.Keys);
			if (1 < sortedTagKeys.Count) {
				sortedTagKeys.Sort();
			}
		}

		protected string escapeTagKeyValue(string s)
		{
			return reTagKeyValue.Replace(s, @"\$1");
		}

		/// <summary>
		/// Appends the tag key and value.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="escapedTagKey">Escaped tag key.</param>
		/// <param name="escapedTagValue">Escaped tag value.</param>
		protected void appendTag(StreamWriter writer, string escapedTagKey, string escapedTagValue)
		{
			// Skip tag with empty value.
			// (InfluxDB does not accept it.)
			if (string.IsNullOrEmpty(escapedTagValue)) {
				return;
			}

			writer.Write(",");
			writer.Write(escapedTagKey);
			writer.Write("=");
			writer.Write(tags[escapedTagKey]);
		}

		/// <summary>
		/// Appends the timestamp.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="timestamp">Timestamp.</param>
		protected void appendTimestamp(StreamWriter writer, DateTimeOffset timestamp)
		{
			writer.Write(" ");
			writer.WriteLine((timestamp.UtcTicks - LogTime.EpocTicks) * 100);
		}

		/// <summary>
		/// Append int field
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="fieldKey">Field key.</param>
		/// <param name="value">Value.</param>
		protected void appendField(StreamWriter writer, string fieldKey, int value)
		{
			writer.Write(" logLevel=");
			writer.Write(value);
			writer.Write("i");
		}

		/// <summary>
		/// Appends string field.
		/// </summary>
		/// <param name="writer">Writer.</param>
		/// <param name="fieldKey">Field key.</param>
		/// <param name="value">Value.</param>
		protected void appendField(StreamWriter writer, string fieldKey, string value)
		{
			var escapedValue = value
				.Replace("\"", "\\\"")
				.Replace("\n", "\\n");

			writer.Write(fieldKey);
			writer.Write("=\"");
			writer.Write(escapedValue);
			writer.Write("\"");
		}
	}
}
