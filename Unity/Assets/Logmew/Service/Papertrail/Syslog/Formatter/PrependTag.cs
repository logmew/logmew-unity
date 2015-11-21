using UnityEngine;
using System.Collections;

namespace Logmew.Service.Papertrail
{
	public class PrependTag : ILogEntryMessageFormatter
	{
		private string delimiter;

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name="delimiter">Delimiter.</param>
		public PrependTag(string delimiter = " $$ ")
		{
			this.delimiter = delimiter;
		}

		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="message">Message.</param>
		/// <param name="logEntry">Log entry.</param>
		public string FormatMessage(string message, ILogEntry logEntry)
		{
			return string.Format("{0}{1}{2}", logEntry.Tag, delimiter, message);
		}
	}
}
