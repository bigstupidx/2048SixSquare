//
//  GameConfig.h
//  Unity-iPhone
//
//  Created by KHOA on 2/6/15.
//
//

#ifndef Unity_iPhone_GameConfig_h
#define Unity_iPhone_GameConfig_h


// plist online
#define PLIST_LINK_FILE "https://www.dropbox.com/s/havevhah522rffo/config.plist?dl=1"
// plist check new version
#define PLIST_NEW_VERSION "https://www.dropbox.com/s/3g52jty4wepqz89/CheckUpdateNewVersion_BirdzyFree_1.0%281.0%29.plist?dl=1"

#define IS_IPAD (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
#define IS_IPHONE (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone)
#define IS_RETINA ([[UIScreen mainScreen] scale] >= 2.0)

#define SCREEN_WIDTH ([[UIScreen mainScreen] bounds].size.width)
#define SCREEN_HEIGHT ([[UIScreen mainScreen] bounds].size.height)
#define SCREEN_MAX_LENGTH (MAX(SCREEN_WIDTH, SCREEN_HEIGHT))
#define SCREEN_MIN_LENGTH (MIN(SCREEN_WIDTH, SCREEN_HEIGHT))

#define IS_IPHONE_4_OR_LESS (IS_IPHONE && SCREEN_MAX_LENGTH < 568.0)
#define IS_IPHONE_5 (IS_IPHONE && SCREEN_MAX_LENGTH == 568.0)   
#define IS_IPHONE_6 (IS_IPHONE && SCREEN_MAX_LENGTH == 667.0)
#define IS_IPHONE_6P (IS_IPHONE && SCREEN_MAX_LENGTH == 736.0)

#endif
