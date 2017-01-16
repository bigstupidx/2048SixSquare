//
//  InhouseSDK.h
//  Unity-iPhone
//
//  Created by KHOA on 2/1/15.
//
//

#import <Foundation/Foundation.h>

@interface InhouseSDK : NSObject
+(InhouseSDK*)sharedInstance;
-(void)showRateAlertWithTitle:(NSString*)title
                      message:(NSString*)message
                       cancel:(NSString*)cancel
                           ok:(NSString*)ok
                          url:(NSString*)url
                     identify:(NSString*)identify;
- (void)showPaidAlert:(float)delay
                   id:(int)ID
                title:(NSString*)title
              message:(NSString*)message
               cancel:(NSString*)cancel
                   ok:(NSString*)ok
                  url:(NSString*)url;
@end
