//
//  LGMLog_h
//
//  Created by HANAI Tohru aka pokehanai <hanai@pokelabo.co.jp> on 2015/11/21.

#ifndef LGMLog_h
#define LGMLog_h

#import <Foundation/Foundation.h>

typedef NS_ENUM(NSInteger, LGMLogLevel)
{
    LGMLogLevel_Error = 0,
    LGMLogLevel_Assert = 1,
    LGMLogLevel_Exception = 2,
    LGMLogLevel_Warning = 3,
    LGMLogLevel_Info = 4,
    LGMLogLevel_Debug = 5,
};

@interface LGMLogBridge : NSObject

+ (void)sendLog:(LGMLogLevel)logLevel
            tag:(NSString *)tag
         format:(NSString *)format, ...NS_FORMAT_FUNCTION(3, 4);

@end

#define LGMSendLog1(logLevel, logtag, fmt, ...)     \
    [LGMLogBridge sendLog:logLevel \
                      tag:logtag \
                   format:fmt, ##__VA_ARGS__];

#define LGMSendLog(logLevel, logtag, fmt, ...) LGMSendLog1(logLevel, @"iOS", fmt, ##__VA_ARGS__)

#define LGMLogError(fmt, ...)    LGMSendLog(LGMLogLevel_Error,    fmt, ##__VA_ARGS__)
#define LGMLogWarn(fmt, ...)     LGMSendLog(LGMLogLevel_Warning,  fmt, ##__VA_ARGS__)
#define LGMLogInfo(fmt, ...)     LGMSendLog(LGMLogLevel_Info,     fmt, ##__VA_ARGS__)
#define LGMLogDebug(fmt, ...)    LGMSendLog(LGMLogLevel_Debug,    fmt, ##__VA_ARGS__)
#define LGMLog(fmt, ...)         LGMSendLog(LGMLogLevel_Debug,    fmt, ##__VA_ARGS__)

#endif /* LGMLog_h */
