//
//  InhouseSDK.m
//  Unity-iPhone
//
//  Created by KHOA on 2/1/15.
//
//

#import "InhouseSDK.h"
#import "GameConfig.h"
#import "FFIAPManager.h"
#import "UIAlertView+Blocks.h"
#import "CaptureViewController.h"
#import "ShareCaptureViewController.h"

static InhouseSDK* _sharedInhouseSDK = nil;


@interface UIActivityViewController (__UIActivityViewController)

- (BOOL)_shouldExcludeActivityType:(UIActivity *)activity;



@end

@implementation UIActivityViewController (__UIActivityViewController)

- (BOOL)_shouldExcludeActivityType:(UIActivity *)activity{
    
    NSArray *acceptActivities= @[UIActivityTypePostToFacebook,UIActivityTypeMail, UIActivityTypePostToTwitter, UIActivityTypeMessage];
    if([acceptActivities containsObject:[activity activityType]]) {
        return NO;
    }
    
    return YES;
}

@end

@implementation InhouseSDK{
    NSString* _alert_url;
    NSMutableArray* _listIdCallback;
}

+(InhouseSDK*)sharedInstance{
    if(_sharedInhouseSDK == nil)
        _sharedInhouseSDK = [[InhouseSDK alloc] init];
    return _sharedInhouseSDK;
}

- (id)init {
    self = [super init];
    if (self) {
        [[NSNotificationCenter defaultCenter] addObserver: self
                                                 selector: @selector(handleBecomeActive:)
                                                     name: UIApplicationWillEnterForegroundNotification
                                                   object: nil];
        [[NSNotificationCenter defaultCenter] addObserver: self
                                                 selector: @selector(handleBecomeDeactive:)
                                                     name: UIApplicationWillResignActiveNotification
                                                   object: nil];
        _listIdCallback = [NSMutableArray new];
    }
    return self;
}

- (void)handleBecomeActive:(NSNotification*)noti {
    UnitySendMessage("InhouseSDKManager", "BecomeActiveCallback", "ok");
}

- (void)handleBecomeDeactive:(NSNotification*)noti {
    UnitySendMessage("InhouseSDKManager", "BecomeDeactiveCallback", "ok");
}

- (NSString*) getCaptureImage:(int) score {
    CaptureViewController *vc = [CaptureViewController new];
    UIImage* image = [vc imageWithScore:score];
    NSData* data = UIImagePNGRepresentation(image);
    NSString* result = [data base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
    return result;
}

- (NSString*) getCaptureImage:(int) type score:(int)score mode:(NSString*)mode {
    ShareCaptureViewController *vc = [ShareCaptureViewController new];
    UIImage* image = [vc imageWithScore:score type:type mode:mode];
    NSData* data = UIImagePNGRepresentation(image);
    NSString* result = [data base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
    return result;
}

-(void)showRateAlertWithTitle:(NSString*)title
                      message:(NSString*)message
                       cancel:(NSString*)cancel
                           ok:(NSString*)ok
                          url:(NSString*)url
                           identify:(NSString*)identify
{
    UIAlertView* alert = [[UIAlertView alloc] initWithTitle:![title  isEqual: @""] ? title : nil
                                                    message:![message  isEqual: @""] ? message : nil
                                                   delegate:self
                                          cancelButtonTitle:![cancel  isEqual: @""] ? cancel : nil
                                          otherButtonTitles:![ok  isEqual: @""] ? ok : nil, nil];
    [alert show];
    [_listIdCallback addObject:identify];
    _alert_url = url;
}

-(void)showRateAlertWithTitle:(NSString*)title
                      message:(NSString*)message
                       cancel:(NSString*)cancel
                           ok1:(NSString*)ok1
                          ok2:(NSString*)ok2
                          url:(NSString*)url
                     identify:(NSString*)identify
{
    UIAlertView* alert = [[UIAlertView alloc] initWithTitle:![title  isEqual: @""] ? title : nil
                                                    message:![message  isEqual: @""] ? message : nil
                                                   delegate:self
                                          cancelButtonTitle:![cancel  isEqual: @""] ? cancel : nil
                                          otherButtonTitles:![ok1  isEqual: @""] ? ok1 : nil,
                                          ![ok2  isEqual: @""] ? ok2 : nil,
                          nil];
    [alert show];
    [_listIdCallback addObject:identify];
    _alert_url = url;
}

-(void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex{
//    if (buttonIndex == 0) {
//        if (![_alert_url  isEqual: @""])
//            [self openURL:_alert_url];
//    }
    _alert_url = nil;
    NSString* ident = [_listIdCallback lastObject];
    [_listIdCallback removeObject:ident];
    NSString* params = [NSString stringWithFormat:@"%ld|%@", (long)buttonIndex, ident];
    UnitySendMessage("InhouseSDKManager", "RatePopupCallback", [params UTF8String]);
}

-(void)showRate:(NSString*)url{
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:url]];
}


-(bool)rated{
    return [[NSUserDefaults standardUserDefaults] boolForKey:@"RATED"];
}

typedef void (^MSTAPIManagerBlock)(NSString *title, NSString *messageUpdate, NSString *ok, NSString *cancel, NSString *urlToUpdate);

void MSTAPIManagerCheckUpdateFeature(NSURL *url, MSTAPIManagerBlock block1) {
    NSURLRequest *request= [NSURLRequest requestWithURL:url];
    [NSURLConnection sendAsynchronousRequest:request queue:[NSOperationQueue mainQueue] completionHandler:^(NSURLResponse *response, NSData *data, NSError *connectionError) {
        
        if (!connectionError && data.length > 0) {
            NSError*error;
            NSPropertyListFormat format;
            NSDictionary *detailAppConfig = [NSPropertyListSerialization propertyListWithData:data options:(NSPropertyListReadCorruptError) format:&format error:&error];
            if (!error && detailAppConfig) {
                
                NSString * version = [[NSBundle mainBundle] objectForInfoDictionaryKey: @"CFBundleShortVersionString"];
                
                NSDictionary*languageDict = detailAppConfig[@"language"];
                NSString *onlineVersion = detailAppConfig[@"newversion"];
                
                NSString *tempStr123 = [NSLocale preferredLanguages].firstObject;
                NSArray *array123 = [tempStr123 componentsSeparatedByString:@"-"];
                NSString *strLocation = [array123 firstObject];
                
                NSString *currentLocation = strLocation;
                if (!languageDict[currentLocation]) {
                    currentLocation = @"en";
                    
                    if (!languageDict[currentLocation]) {
                        currentLocation = [[languageDict allKeys] firstObject];
                    }
                }
                
                if ([onlineVersion compare:version options:NSNumericSearch] == NSOrderedDescending) {
                    block1(languageDict[currentLocation][@"Title"],languageDict[currentLocation][@"message"],languageDict[currentLocation][@"OK"],languageDict[currentLocation][@"Cancel"], detailAppConfig[@"url"]);
                } else {
                    block1(nil,nil,nil,nil,nil);
                }
            }
        } else {
            block1(nil,nil,nil,nil,nil);
        }
    }];
}

- (void)checkNewVersion {
    MSTAPIManagerCheckUpdateFeature([NSURL URLWithString:@PLIST_NEW_VERSION], ^(NSString *title, NSString *messageUpdate, NSString *ok, NSString *cancel, NSString *urlToUpdate) {
        if (title && messageUpdate && ok && cancel && urlToUpdate) {
            
            [UIAlertView showWithTitle:title
                               message:messageUpdate
                     cancelButtonTitle:cancel
                     otherButtonTitles:@[ok?ok:@"OK"]
                              tapBlock:^(UIAlertView *alertView, NSInteger buttonIndex) {
                                  if (buttonIndex == [alertView cancelButtonIndex]) {
                                  } else if ([[alertView buttonTitleAtIndex:buttonIndex] isEqualToString:ok]) {
                                      [[UIApplication sharedApplication] openURL:[NSURL URLWithString:urlToUpdate]];
                                  }
                              }];
        }
        
    });
}

-(NSString*)currentLanguage{
    NSString * language = [NSLocale preferredLanguages].firstObject;
    NSString *currentLanguage = [[language componentsSeparatedByString:@"-"] firstObject];
    if (!currentLanguage) {
        currentLanguage = @"en";
    }
    return  currentLanguage;
}

- (NSString*)getDefaultPlist {
    NSString *path = [[NSBundle mainBundle] pathForResource:@"default_config" ofType:@"xml"];
    NSData *data = [NSData dataWithContentsOfFile:path];
    NSString* newStr = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    return newStr;
}
// tuong share
- (void)shareThisApp:(NSString*) text {
    NSString *string = text;
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:@[string] applicationActivities:nil];
    if (UIUserInterfaceIdiomPad == UI_USER_INTERFACE_IDIOM() && [activityViewController respondsToSelector:@selector(popoverPresentationController)] ) {
        activityViewController.popoverPresentationController.sourceView = [[UIApplication sharedApplication].delegate window].rootViewController.view;
        activityViewController.popoverPresentationController.sourceRect = CGRectMake([UIScreen mainScreen].bounds.size.width/2, [UIScreen mainScreen].bounds.size.height/2, 0, 0);
    }
    UIViewController *vc = [UIApplication sharedApplication].delegate.window.rootViewController;
    while (vc.presentedViewController) {
        vc = vc.presentedViewController;
    }
    [activityViewController setCompletionHandler:^(NSString *activityType, BOOL completed)
    {
        UnitySendMessage("InhouseSDKManager", "ShareCallback", completed ? "true" : "false");
    }];
    [vc presentViewController:activityViewController animated:YES completion:nil];
}

- (void)shareThisApp:(NSString*) text withImage:(UIImage*)image {
    NSString *string = text;
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:@[string, image] applicationActivities:nil];
    
    if (UIUserInterfaceIdiomPad == UI_USER_INTERFACE_IDIOM() && [activityViewController respondsToSelector:@selector(popoverPresentationController)] ) {
        activityViewController.popoverPresentationController.sourceView = [[UIApplication sharedApplication].delegate window].rootViewController.view;
        activityViewController.popoverPresentationController.sourceRect = CGRectMake([UIScreen mainScreen].bounds.size.width/2, [UIScreen mainScreen].bounds.size.height/2, 0, 0);
    }
    NSArray * excludeActivities =@[UIActivityTypePostToTwitter,
                                   UIActivityTypePostToWeibo,
                                   UIActivityTypeMail,
                                   UIActivityTypePrint,
                                   UIActivityTypeCopyToPasteboard,
                                   UIActivityTypeAssignToContact,
                                   UIActivityTypeSaveToCameraRoll,
                                   UIActivityTypeAddToReadingList ,
                                   UIActivityTypePostToFlickr,
                                   UIActivityTypePostToVimeo,
                                   UIActivityTypePostToTencentWeibo,
                                   UIActivityTypeAirDrop,
                                   UIActivityTypeOpenInIBooks,
                                   @"com.apple.mobilenotes.SharingExtension",
                                   @"com.apple.reminders.RemindersEditorExtension"];
    UIViewController *vc = [UIApplication sharedApplication].delegate.window.rootViewController;
    while (vc.presentedViewController) {
        vc = vc.presentedViewController;
    }
    [activityViewController setCompletionHandler:^(NSString *activityType, BOOL completed)
     {
         UnitySendMessage("InhouseSDKManager", "ShareCallback", completed ? "true" : "false");
     }];
    [vc presentViewController:activityViewController animated:YES completion:nil];
}

- (void)shareThisApp:(NSString*) text withImageString:(NSString*)imageString {
    NSData* data = [[NSData alloc] initWithBase64EncodedString:imageString options:NSDataBase64DecodingIgnoreUnknownCharacters];
    UIImage* image = [UIImage imageWithData:data];
    [self shareThisApp:text withImage:image];
}
// share popup
- (void)shareThisApp:(NSString*) text path:(NSString*)path {
    NSString *patha = [[NSBundle mainBundle] pathForResource:@"haha" ofType:@"gif"];
    NSString *string = text;
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:@[text,  [NSURL URLWithString:path], [UIImage imageWithContentsOfFile:patha]] applicationActivities:nil];
    NSArray * excludeActivities =@[UIActivityTypePostToTwitter,
                                   UIActivityTypePostToWeibo,
                                   UIActivityTypeMail,
                                   UIActivityTypePrint,
                                   UIActivityTypeCopyToPasteboard,
                                   UIActivityTypeAssignToContact,
                                   UIActivityTypeSaveToCameraRoll,
                                   UIActivityTypeAddToReadingList ,
                                   UIActivityTypePostToFlickr,
                                   UIActivityTypePostToVimeo,
                                   UIActivityTypePostToTencentWeibo,
                                   UIActivityTypeAirDrop,
                                   UIActivityTypeOpenInIBooks,
                                   @"com.apple.mobilenotes.SharingExtension",
                                   @"com.apple.reminders.RemindersEditorExtension"];
    
    if (UIUserInterfaceIdiomPad == UI_USER_INTERFACE_IDIOM() && [activityViewController respondsToSelector:@selector(popoverPresentationController)] ) {
        activityViewController.popoverPresentationController.sourceView = [[UIApplication sharedApplication].delegate window].rootViewController.view;
        activityViewController.popoverPresentationController.sourceRect = CGRectMake([UIScreen mainScreen].bounds.size.width/2, [UIScreen mainScreen].bounds.size.height/2, 0, 0);
    }
    
    UIViewController *vc = [UIApplication sharedApplication].delegate.window.rootViewController;
    while (vc.presentedViewController) {
        vc = vc.presentedViewController;
    }
    [activityViewController setCompletionHandler:^(NSString *activityType, BOOL completed)
     {
         UnitySendMessage("InhouseSDKManager", "ShareCallback", completed ? "true" : "false");
     }];
    [vc presentViewController:activityViewController animated:YES completion:nil];
}

-(void)shareScore:(int)score URL:(NSString*)url text:(NSString*)text
{
    NSString* shareCurrentScore = [NSString stringWithFormat: text, score];
    
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:@[shareCurrentScore, @"\n\n", [NSURL URLWithString:url]] applicationActivities:nil];
    NSArray * excludeActivities =@[UIActivityTypePostToTwitter,
                                   UIActivityTypePostToWeibo,
                                   UIActivityTypeMessage,
                                   UIActivityTypeMail,
                                   UIActivityTypePrint,
                                   UIActivityTypeCopyToPasteboard,
                                   UIActivityTypeAssignToContact,
                                   UIActivityTypeSaveToCameraRoll,
                                   UIActivityTypeAddToReadingList ,
                                   UIActivityTypePostToFlickr,
                                   UIActivityTypePostToVimeo,
                                   UIActivityTypePostToTencentWeibo,
                                   UIActivityTypeAirDrop,
                                   UIActivityTypeOpenInIBooks,
                                   @"com.apple.mobilenotes.SharingExtension"];
    if([[[UIDevice currentDevice] systemVersion] floatValue]>=8)
    {
        CGSize size = [[UIScreen mainScreen] bounds].size;
        activityViewController.popoverPresentationController.sourceRect = CGRectMake(0,0,size.width,size.height);
        activityViewController.popoverPresentationController.sourceView = UnityGetGLViewController().view;
        [activityViewController.popoverPresentationController setPermittedArrowDirections:0];
    }
    [activityViewController setCompletionHandler:^(NSString *activityType, BOOL completed)
     {
         UnitySendMessage("InhouseSDKManager", "ShareCallback", completed ? "true" : "false");
     }];
    [UnityGetGLViewController() presentViewController:activityViewController animated:YES completion:nil];
}

-(void)restorePurchaseInApps:(NSString*)productID{
    NSLog(@"RESTORE PURCHASE IN APP");
    [sIAPMgr restoreTransactionsOnSuccess:^{
        NSLog(@"PURCHASE SUCCESS");
        UnitySendMessage("InhouseSDKManager", "RestorePurchaseInAppsCallback", "ok");
    } failure:^(NSError *error) {
#warning hardcode to test until account is ok
        UnitySendMessage("InhouseSDKManager", "RestorePurchaseInAppsCallback", "failed");
        NSLog(@"HELLO: %@", error);
    }];
}


-(void)buyProduct:(NSString*)productID{
    NSLog(@"BUY PRODUCT: %@", productID);
    
    if (productID == nil)
        return;
    
    [sIAPMgr addPayment:productID success:^(SKPaymentTransaction *transaction){
        UnitySendMessage("InhouseSDKManager", "BuyProductCallback", "ok");
        
    }
                failure:^(SKPaymentTransaction *transaction, NSError *error){
#warning hardcode to test until account is ok
                    UnitySendMessage("InhouseSDKManager", "BuyProductCallback", "failed");
                    NSLog(@"HELLO:  %@", error);
                }];
}

-(void)openURL:(NSString*)URL{
    if (!URL)
        return;
    NSLog(@"OPEN URL: %@", URL);
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:URL]];
}
@end

extern "C"{
    void restorePurchaseInApps(const char* productID){
        [[InhouseSDK sharedInstance] restorePurchaseInApps: [NSString stringWithUTF8String:productID]];
    }
    
    
    void buyProduct(const char* productID){
        [[InhouseSDK sharedInstance] buyProduct: [NSString stringWithUTF8String:productID]];
    }
    void showShareDialog (int score, const char* url, const char* text){
        [[InhouseSDK sharedInstance] shareScore:score URL:[NSString stringWithUTF8String:url] text:[NSString stringWithUTF8String:text]];
    }
    char* MakeStringCopy (const char* string) {
        if (string == NULL) return NULL;
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }
    
    char* CurrentLanguage(){
        return MakeStringCopy([[[InhouseSDK sharedInstance] currentLanguage] UTF8String]);
    }
    
    char* GetDefaultPlist() {
        return MakeStringCopy([[[InhouseSDK sharedInstance] getDefaultPlist] UTF8String]);
    }
    
    char* NLINKFILE(){
        return MakeStringCopy(PLIST_LINK_FILE);
    }
    
    void shareGame (const char* text) {
        [[InhouseSDK sharedInstance] shareThisApp:[NSString stringWithUTF8String:text]];
    }
    
    void shareGameWithImage (const char* text, const char* imageString) {
        [[InhouseSDK sharedInstance] shareThisApp:[NSString stringWithUTF8String:text]
                                  withImageString:[NSString stringWithUTF8String:imageString]];
    }
    
    void shareScoreWithImage (int type, int score, const char* mode, const char* text) {
        NSString* image = [[InhouseSDK sharedInstance] getCaptureImage:type score:score mode:[NSString stringWithUTF8String:mode]];
        [[InhouseSDK sharedInstance] shareThisApp:[NSString stringWithUTF8String:text]
                                  withImageString:image];
    }
    
    void shareGameWithGif(const char* text, const char* path) {
        [[InhouseSDK sharedInstance] shareThisApp:[NSString stringWithUTF8String:text]
                                             path:[NSString stringWithUTF8String:path]];
    }
    
    void showAlert(char* identify, char* title, char* message, char* cancel, char* ok, char* url){
        [[InhouseSDK sharedInstance] showRateAlertWithTitle:[NSString stringWithUTF8String:title]
                                                    message:[NSString stringWithUTF8String:message]
                                                     cancel:[NSString stringWithUTF8String:cancel]
                                                         ok:[NSString stringWithUTF8String:ok]
                                                        url:[NSString stringWithUTF8String:url]
                                                   identify:[NSString stringWithUTF8String:identify]];
    }
    
    void showAlert2(char* identify, char* title, char* message, char* ok1, char* ok2, char* cancel, char* url){
        [[InhouseSDK sharedInstance] showRateAlertWithTitle:[NSString stringWithUTF8String:title]
                                                    message:[NSString stringWithUTF8String:message]
                                                     cancel:[NSString stringWithUTF8String:cancel]
                                                        ok1:[NSString stringWithUTF8String:ok1]
                                                        ok2:[NSString stringWithUTF8String:ok2]
                                                        url:[NSString stringWithUTF8String:url]
                                                   identify:[NSString stringWithUTF8String:identify]];
    }
    
    bool rated(){
        return [[InhouseSDK sharedInstance] rated];
    }
    
    void openURL(char* URL){
        [[InhouseSDK sharedInstance] openURL:[NSString stringWithUTF8String:URL]];
    }
    
    void checkNewVersion() {
        [[InhouseSDK sharedInstance] checkNewVersion];
    }
    
    char* __getImageShare(int score) {
        NSString* result = [[InhouseSDK sharedInstance] getCaptureImage:score];
        return MakeStringCopy([result UTF8String]);
    }
    
    int __getDeviceType() {
        if (IS_IPHONE_4_OR_LESS)
            return 0;
        else if (IS_IPHONE_5)
            return 1;
        else if (IS_IPHONE_6)
            return 2;
        else if (IS_IPHONE_6P)
            return 3;
        else
            return 3;
    }
    
    char* __getGifPath() {
        NSString *path = [[NSBundle mainBundle] pathForResource:@"haha" ofType:@"gif"];
        return MakeStringCopy([path UTF8String]);
    }
}
