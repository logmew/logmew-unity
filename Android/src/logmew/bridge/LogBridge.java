package logmew.bridge;

import java.util.concurrent.LinkedBlockingQueue;

import logmew.bridge.Log.Printer;

public class LogBridge {
	private final static String TAG = "Logmew";

	private static LinkedBlockingQueue<LogEntry> queue;
	private static Thread callbackThread;

	static {
		queue = new LinkedBlockingQueue<LogEntry>(1024);
	}

	/**
	 * Append logEntry to the queue.
	 *
	 * @param logEntry
	 */
	public static void enqueue(LogEntry logEntry) {
		queue.offer(logEntry);
	}

	/**
	 * Set callback of C# space.
	 *
	 * @param aCallback, which will be invoked whenever the queue is populated.
	 */
	public static void setCallback(final Runnable aCallback) {
		if (callbackThread != null) return;
		callbackThread = new Thread(new Runnable() {

			@Override
			public void run() {
				aCallback.run();
			}
		});
		callbackThread.start();
		Log.d(TAG, "activated logmew bridge between Android code and Unity code");
	}

	/**
	 * Get a LogEntry instance.
	 *
	 * @return LogEntry
	 */
	public static LogEntry getLogEntry() {
		try {
			return queue.take();
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
		return LogEntry.Empty;
	}

	private static class LogmewPrinter implements Printer {

		public void print(int level, String tag, String msg) {
			LogEntry logEntry = new LogEntry(LogLevel.fromLevel(level), tag, msg);
			LogBridge.enqueue(logEntry);
		}
	}

	public final static LogmewPrinter LOGMEW = new LogmewPrinter();

	public static boolean providesStackTrace = true;

	/**
	 * Enable Logmew sending.
	 */
	public static void enableLogmewBridge() {
		Log.usePrinter(LOGMEW, true);
		Log.d(TAG, "enabled logmew bridge.");
	}

	/**
	 * Disable Logmew sending.
	 */
	public static void disableLogmewBridge() {
		Log.d(TAG, "deactivated logmew bridge.");
		Log.usePrinter(LOGMEW, false);
	}

	/**
	 * Enable system log output.
	 */
	public static void enableSystemLog() {
		if (Log.ANDROID.loaded()) {
			Log.usePrinter(Log.ANDROID, true);
		} else {
			Log.usePrinter(Log.SYSTEM, true);
		}
	}

	/**
	 * Disable system log output.
	 */
	public static void disbleSystemLog() {
		if (Log.ANDROID.loaded()) {
			Log.usePrinter(Log.ANDROID, false);
		} else {
			Log.usePrinter(Log.SYSTEM, false);
		}
	}

	/**
	 * Enable/disable stack trace creation.
	 */
	public static void providesStackTrace(boolean provides) {
		LogEntry.providesStackTrace = provides;
	}

	/**
	 * Enable/disable stack trace creation.
	 */
	public static void setMinLogLevel(int logLevel) {
		int level = LogLevel.valueOf(logLevel).toLevel();
		Log.level(level);
	}
}
