using System;

namespace Logmew
{
	/// <summary>
	/// Log entry interface.
	/// </summary>
	public interface ILogEntry
	{
		/// <summary>
		/// Log origin name.
		/// </summary>
		/// <value>Log origin name.</value>
		string Origin { get; }

		/// <summary>
		/// Log timestamp.
		/// </summary>
		/// <value>Log timestamp.</value>
		DateTimeOffset TimeStamp { get; }

		/// <summary>
		/// Log level.
		/// </summary>
		/// <value>Log level.</value>
		LogLevel LogLevel { get; }

		/// <summary>
		/// Tag.
		/// </summary>
		/// <value>Tag.</value>
		string Tag { get; }

		/// <summary>
		/// Log message.
		/// </summary>
		/// <value>Log message.</value>
		string Message { get; }

		/// <summary>
		/// Stack trace.
		/// </summary>
		/// <value>stack trace.</value>
		string StackTrace { get; }

	}
}
