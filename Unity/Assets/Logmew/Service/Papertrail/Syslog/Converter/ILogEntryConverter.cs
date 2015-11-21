using SyslogNet.Client;

namespace Logmew.Service.Papertrail
{
	/// <summary>
	/// ILogEntry to SyslogMessage converter interface.
	/// </summary>
	///
	public interface ILogEntryConverter
	{
		/// <summary>
		/// Convert ILogEntry to SyslogMessage.
		/// </summary>
		/// <returns>The SyslogMessage instance.</returns>
		/// <param name="logEntry">Log entry.</param>
		SyslogMessage ToSyslogMessage(ILogEntry logEntry);
	}
}
