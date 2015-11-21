using System.IO;
using SyslogNet.Client;

namespace Logmew.Service.InfluxDb
{
	/// <summary>
	/// InfluxDB log builder interface
	/// </summary>
	///
	public interface IInfluxDbLogBuilder
	{
		/// <summary>
		/// Convert IPaperTrailLogEntry to SyslogMessage.
		/// </summary>
		/// <returns>The SyslogMessage instance.</returns>
		/// <param name="writer">Stream writer.</param>
		/// <param name="logEntry">Log entry.</param>
		void Append(StreamWriter writer, ILogEntry logEntry);
	}
}
