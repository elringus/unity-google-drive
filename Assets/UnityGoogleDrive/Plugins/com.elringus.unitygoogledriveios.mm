#import <Foundation/NSObject.h>
#import "AppAuth.h"

#define SendUnityMessage(m) UnitySendMessage("UnityGoogleDrive_IOSAccessTokenProvider_ResponseHandler", "HandleResponseMessage", m)

extern "C" void UnitySendMessage(const char*, const char*, const char*);
extern "C" UIViewController* UnityGetGLViewController();

@interface UnityGoogleDriveIOS : NSObject

+ (UnityGoogleDriveIOS*)instance;

- (void)performAuth:(NSURL*)authorizationEndpoint
      tokenEndpoint:(NSURL*)tokenEndpoint
           clientId:(NSString*)clientId
   redirectEndpoint:(NSURL*)redirectEndpoint
              scope:(NSString*)scope;

@end

@implementation UnityGoogleDriveIOS

#pragma mark - singleton method
+ (UnityGoogleDriveIOS*)instance
{
    static dispatch_once_t predicate = 0;
    __strong static id sharedObject = nil;
    dispatch_once(&predicate, ^{
        sharedObject = [[self alloc] init];
    });
    return sharedObject;
}

OIDAuthorizationService* service;

- (void)performAuth:(NSURL*)authorizationEndpoint
      tokenEndpoint:(NSURL*)tokenEndpoint
           clientId:(NSString*)clientId
   redirectEndpoint:(NSURL*)redirectEndpoint
              scope:(NSString*)scope {
    
    OIDServiceConfiguration* configuration = [[OIDServiceConfiguration alloc]
                                              initWithAuthorizationEndpoint:authorizationEndpoint
                                              tokenEndpoint:tokenEndpoint];
    
    OIDAuthorizationRequest* request = [[OIDAuthorizationRequest alloc]
                                        initWithConfiguration:configuration
                                        clientId:clientId
                                        scopes:[scope componentsSeparatedByString:@" "]
                                        redirectURL:redirectEndpoint
                                        responseType:OIDResponseTypeCode
                                        additionalParameters:nil];
    
    service = [OIDAuthorizationService
     presentAuthorizationRequest:request
     presentingViewController:UnityGetGLViewController()
     callback:^(OIDAuthorizationResponse*_Nullable authorizationResponse, NSError*_Nullable error) {
         if (authorizationResponse) {
             NSString* response = [NSString stringWithFormat:@"%@ %@ %@",
                                   authorizationResponse.authorizationCode,
                                   authorizationResponse.request.codeVerifier,
                                   authorizationResponse.request.redirectURL.absoluteString];
             SendUnityMessage([response UTF8String]);
         } else {
             NSString* errorStr = [NSString stringWithFormat:@"Error: %@", [error localizedDescription]];
             SendUnityMessage([errorStr UTF8String]);
         }
     }];
}

@end

extern "C" {
    void _UnityGoogleDriveIOS_PerformAuth(const char* authorizationEndpoint, const char* tokenEndpoint, const char* clientId, const char* redirectEndpoint, const char* scope);
}

void _UnityGoogleDriveIOS_PerformAuth(const char* authorizationEndpoint, const char* tokenEndpoint, const char* clientId, const char* redirectEndpoint, const char* scope)
{
    [UnityGoogleDriveIOS.instance
     performAuth:[NSURL URLWithString:[[NSString stringWithUTF8String:authorizationEndpoint]stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding]]
     tokenEndpoint:[NSURL URLWithString:[[NSString stringWithUTF8String:tokenEndpoint]stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding]]
     clientId:[NSString stringWithUTF8String:clientId]
     redirectEndpoint:[NSURL URLWithString:[[NSString stringWithUTF8String:redirectEndpoint]stringByAddingPercentEscapesUsingEncoding:NSUTF8StringEncoding]]
     scope:[NSString stringWithUTF8String:scope]];
}
