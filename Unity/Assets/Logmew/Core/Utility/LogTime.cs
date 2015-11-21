using System;

namespace Logmew
{
	public static class LogTime
	{
		public static readonly long EpocTicks = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).Ticks;

		public static DateTimeOffset FromUnixTimestampSec(double sec)
		{
			long ticks = (long)(sec * 1000 * 1000 * 10) + EpocTicks;
			return new DateTimeOffset(new DateTime(ticks), TimeSpan.Zero);
		}

		public static DateTimeOffset FromUnixTimestampMilliSec(long msec)
		{
			long ticks = (msec * 1000 * 10) + EpocTicks;
			return new DateTimeOffset(new DateTime(ticks), TimeSpan.Zero);
		}
	}
}
