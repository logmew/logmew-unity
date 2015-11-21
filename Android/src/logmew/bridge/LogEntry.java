package logmew.bridge;

public class LogEntry {
	public long timestamp;
	public int logLevel;
	public String tag;
	public String message;
	public String stackTrace;

	private LogEntry() {
		timestamp = -1;
	}

	public static LogEntry Empty = new LogEntry();
	public static boolean providesStackTrace = true;

	/**
	 * Constructor
	 *
	 * @param logLevel
	 * @param tag
	 * @param message
	 */
	public LogEntry(LogLevel logLevel, String tag, String message) {
		this.timestamp = System.currentTimeMillis();
		this.logLevel = logLevel.getValue();
		this.tag = tag;
		this.message = message;
		if (providesStackTrace) {
			this.stackTrace = getStackTrace(8);
		} else {
			this.stackTrace = "";
		}
	}

	private String getStackTrace(int skipCount) {
		StackTraceElement[] stackTraceElements = Thread.currentThread().getStackTrace();

		if (stackTraceElements == null) {
			return "";
		}

		StringBuilder stringBuilder = new StringBuilder();
		for (StackTraceElement element : stackTraceElements) {
			if (0 < skipCount--) continue;
			stringBuilder.append(element.toString()).append("\n");
		}
		return stringBuilder.toString();
	}
}
