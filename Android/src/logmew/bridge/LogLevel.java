package logmew.bridge;

public enum LogLevel {
	Error(0),
	Assert(1),
	Exception(2),
	Warning(3),
	Info(4),
	Debug(5);

	private final int value;

	private LogLevel(int value) {
		this.value = value;
	}

	public int getValue() {
		return value;
	}

    public static LogLevel valueOf(final int anIntValue) {
        for (LogLevel d : values()) {
            if (d.getValue() == anIntValue) {
                return d;
            }
        }
        return LogLevel.Error;
    }

	public static LogLevel fromLevel(int level) {
		switch (level) {
		case Log.V:
		case Log.D:
		default:
			return LogLevel.Debug;
		case Log.I:
			return LogLevel.Info;
		case Log.W:
			return LogLevel.Warning;
		case Log.E:
			return LogLevel.Error;
		}
	}

	public int toLevel() {
		switch (this.value) {
		case 5:
			return Log.D;
		case 4:
			return Log.I;
		case 3:
			return Log.W;
		default:
			return Log.E;
		}
	}
}
