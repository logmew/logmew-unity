//
//  LGMLog.m
//
//  Created by HANAI Tohru aka pokehanai <hanai@pokelabo.co.jp> on 2015/11/21.
//

#import "LGMLog.h"

typedef void (*LogDelegate)(
    double timestamp,
    LGMLogLevel logLevel,
    char *tag,
    char *message,
    char *stackTrace
);

static LogDelegate sendLogAsyncProc = NULL;
static LGMLogLevel minLogLevel = LGMLogLevel_Debug;
static BOOL providesStackTrace = YES;

/**
 * Set or reset Logmew log callback proc
 *
 * If aSendLogAsyncProc is nil, logs will be not sent to C# codebase.
 */
extern "C" void LGMSetCallback(LogDelegate aSendLogAsyncProc)
{
    if (sendLogAsyncProc == aSendLogAsyncProc) return;

    if (!aSendLogAsyncProc) {
        if (aSendLogAsyncProc) {
            LGMSendLog1(LGMLogLevel_Debug,
                        @"logmew",
                        @"deactivate log bridge between iOS code and Unity code");
        }
    }

    sendLogAsyncProc = aSendLogAsyncProc;

    if (aSendLogAsyncProc) {
        LGMSendLog1(LGMLogLevel_Debug,
                    @"logmew",
                    @"activate log bridge between iOS code and Unity code");
    }
}

/**
 * Set minimum log level.
 */
extern "C" void LGMSetMinLogLevel(int logLevel)
{
    minLogLevel = (LGMLogLevel)logLevel;
}

/**
 * Set minimum log level.
 */
extern "C" void LGMEnableStackTrace(BOOL enable)
{
    providesStackTrace = enable;
}

@implementation LGMLogBridge

+ (void)sendLog:(LGMLogLevel)logLevel
            tag:(NSString *)tag
         format:(NSString *)format, ...
{
    if (!sendLogAsyncProc) return;
    if (minLogLevel < logLevel) return;
    
    va_list args;
    va_start(args, format);
    NSString *message = [[NSString alloc] initWithFormat:format arguments:args];
    va_end(args);

    NSString *stackTrace = nil;
    if (providesStackTrace) {
        NSArray *callStackSymbols = [NSThread callStackSymbols];
        stackTrace = [callStackSymbols componentsJoinedByString:@"\n"];
    }
    sendLogAsyncProc([[NSDate date]  timeIntervalSince1970],
                     logLevel,
                     tag ? strdup([tag UTF8String]) : NULL,
                     message ? strdup([message UTF8String]) : NULL,
                     stackTrace ? strdup([stackTrace UTF8String]) : NULL);

}

@end
