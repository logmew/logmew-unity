using UnityEngine;
using System.Collections;

namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// Escape message.
	/// </summary>
	public class EscapeMessage : ILogEntryMessageFormatter
	{
		/// <summary>
		/// Formats the message.
		/// </summary>
		/// <returns>The message.</returns>
		/// <param name="message">Message.</param>
		/// <param name="logEntry">Log entry.</param>
		public string FormatMessage(string message, ILogEntry logEntry)
		{
			return message
				.Replace("\n", @"\n");
		}
	}
}
