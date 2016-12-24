using System;
using Foundation;
using GoogleSignIn;
using ObjCRuntime;
using UIKit;

namespace GoogleSignIn
{
	// typedef void (^GIDAuthenticationHandler)(GIDAuthentication *, NSError *);
	delegate void GIDAuthenticationHandler(GIDAuthentication arg0, NSError arg1);

	// typedef void (^GIDAccessTokenHandler)(NSString *, NSError *);
	delegate void GIDAccessTokenHandler(string arg0, NSError arg1);

	// @interface GIDAuthentication : NSObject <NSCoding>
	[BaseType(typeof(NSObject))]
	interface GIDAuthentication : INSCoding
	{
		// @property (readonly, nonatomic) NSString * clientID;
		[Export("clientID")]
		string ClientID { get; }

		// @property (readonly, nonatomic) NSString * accessToken;
		[Export("accessToken")]
		string AccessToken { get; }

		// @property (readonly, nonatomic) NSDate * accessTokenExpirationDate;
		[Export("accessTokenExpirationDate")]
		NSDate AccessTokenExpirationDate { get; }

		// @property (readonly, nonatomic) NSString * refreshToken;
		[Export("refreshToken")]
		string RefreshToken { get; }

		// @property (readonly, nonatomic) NSString * idToken;
		[Export("idToken")]
		string IdToken { get; }

		// @property (readonly, nonatomic) NSDate * idTokenExpirationDate;
		[Export("idTokenExpirationDate")]
		NSDate IdTokenExpirationDate { get; }

		// -(void)getTokensWithHandler:(GIDAuthenticationHandler)handler;
		[Export("getTokensWithHandler:")]
		void GetTokensWithHandler(GIDAuthenticationHandler handler);

		// -(void)refreshTokensWithHandler:(GIDAuthenticationHandler)handler;
		[Export("refreshTokensWithHandler:")]
		void RefreshTokensWithHandler(GIDAuthenticationHandler handler);

		// -(void)getAccessTokenWithHandler:(GIDAccessTokenHandler)handler __attribute__((deprecated("Use |getTokensWithHandler:| instead.")));
		[Export("getAccessTokenWithHandler:")]
		void GetAccessTokenWithHandler(GIDAccessTokenHandler handler);

		// -(void)refreshAccessTokenWithHandler:(GIDAccessTokenHandler)handler __attribute__((deprecated("Use |refreshTokensWithHandler:| instead.")));
		[Export("refreshAccessTokenWithHandler:")]
		void RefreshAccessTokenWithHandler(GIDAccessTokenHandler handler);
	}

	// @interface GIDGoogleUser : NSObject <NSCoding>
	[BaseType(typeof(NSObject))]
	interface GIDGoogleUser : INSCoding
	{
		// @property (readonly, nonatomic) NSString * userID;
		[Export("userID")]
		string UserID { get; }

		// @property (readonly, nonatomic) GIDProfileData * profile;
		[Export("profile")]
		GIDProfileData Profile { get; }

		// @property (readonly, nonatomic) GIDAuthentication * authentication;
		[Export("authentication")]
		GIDAuthentication Authentication { get; }

		// @property (readonly, nonatomic) NSArray * accessibleScopes;
		[Export("accessibleScopes")]
		NSObject[] AccessibleScopes { get; }

		// @property (readonly, nonatomic) NSString * hostedDomain;
		[Export("hostedDomain")]
		string HostedDomain { get; }

		// @property (readonly, nonatomic) NSString * serverAuthCode;
		[Export("serverAuthCode")]
		string ServerAuthCode { get; }
	}

	// @interface GIDProfileData : NSObject <NSCoding>
	[BaseType(typeof(NSObject))]
	interface GIDProfileData : INSCoding
	{
		// @property (readonly, nonatomic) NSString * email;
		[Export("email")]
		string Email { get; }

		// @property (readonly, nonatomic) NSString * name;
		[Export("name")]
		string Name { get; }

		// @property (readonly, nonatomic) NSString * givenName;
		[Export("givenName")]
		string GivenName { get; }

		// @property (readonly, nonatomic) NSString * familyName;
		[Export("familyName")]
		string FamilyName { get; }

		// @property (readonly, nonatomic) BOOL hasImage;
		[Export("hasImage")]
		bool HasImage { get; }

		// -(NSURL *)imageURLWithDimension:(NSUInteger)dimension;
		[Export("imageURLWithDimension:")]
		NSUrl ImageURLWithDimension(nuint dimension);
	}

	[Static]
	partial interface Constants
	{
		// extern NSString *const kGIDSignInErrorDomain;
		[Field("kGIDSignInErrorDomain", "__Internal")]
		NSString kGIDSignInErrorDomain { get; }
	}

	// @protocol GIDSignInDelegate <NSObject>
	[Protocol, Model]
	[BaseType(typeof(NSObject))]
	interface GIDSignInDelegate
	{
		// @required -(void)signIn:(GIDSignIn *)signIn didSignInForUser:(GIDGoogleUser *)user withError:(NSError *)error;
		[Abstract]
		[Export("signIn:didSignInForUser:withError:")]
		void DidSignInForUser(GIDSignIn signIn, GIDGoogleUser user, NSError error);

		// @optional -(void)signIn:(GIDSignIn *)signIn didDisconnectWithUser:(GIDGoogleUser *)user withError:(NSError *)error;
		[Export("signIn:didDisconnectWithUser:withError:")]
		void DidDisconnectWithUser(GIDSignIn signIn, GIDGoogleUser user, NSError error);
	}

	// @protocol GIDSignInUIDelegate <NSObject>
	[Protocol, Model]
	[BaseType(typeof(NSObject))]
	interface GIDSignInUIDelegate
	{
		// @optional -(void)signInWillDispatch:(GIDSignIn *)signIn error:(NSError *)error;
		[Export("signInWillDispatch:error:")]
		void SignInWillDispatch(GIDSignIn signIn, NSError error);

		// @optional -(void)signIn:(GIDSignIn *)signIn presentViewController:(UIViewController *)viewController;
		[Export("signIn:presentViewController:")]
		void SignInPresent(GIDSignIn signIn, UIViewController viewController);

		// @optional -(void)signIn:(GIDSignIn *)signIn dismissViewController:(UIViewController *)viewController;
		[Export("signIn:dismissViewController:")]
		void SignInDismiss(GIDSignIn signIn, UIViewController viewController);
	}


	// @interface GIDSignIn : NSObject
	[BaseType(typeof(NSObject))]
	interface GIDSignIn
	{
		// @property (readonly, nonatomic) GIDGoogleUser * currentUser;
		[Export("currentUser")]
		GIDGoogleUser CurrentUser { get; }

		[Wrap("WeakDelegate")]
		GIDSignInDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<GIDSignInDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Wrap("WeakUiDelegate")]
		GIDSignInUIDelegate UiDelegate { get; set; }

		// @property (nonatomic, weak) id<GIDSignInUIDelegate> uiDelegate;
		[NullAllowed, Export("uiDelegate", ArgumentSemantic.Weak)]
		NSObject WeakUiDelegate { get; set; }

		// @property (copy, nonatomic) NSString * clientID;
		[Export("clientID")]
		string ClientID { get; set; }

		// @property (copy, nonatomic) NSArray * scopes;
		[Export("scopes", ArgumentSemantic.Copy)]
		NSObject[] Scopes { get; set; }

		// @property (assign, nonatomic) BOOL shouldFetchBasicProfile;
		[Export("shouldFetchBasicProfile")]
		bool ShouldFetchBasicProfile { get; set; }

		// @property (copy, nonatomic) NSString * language;
		[Export("language")]
		string Language { get; set; }

		// @property (copy, nonatomic) NSString * loginHint;
		[Export("loginHint")]
		string LoginHint { get; set; }

		// @property (copy, nonatomic) NSString * serverClientID;
		[Export("serverClientID")]
		string ServerClientID { get; set; }

		// @property (copy, nonatomic) NSString * openIDRealm;
		[Export("openIDRealm")]
		string OpenIDRealm { get; set; }

		// @property (copy, nonatomic) NSString * hostedDomain;
		[Export("hostedDomain")]
		string HostedDomain { get; set; }

		// +(GIDSignIn *)sharedInstance;
		[Static]
		[Export("sharedInstance")]
		GIDSignIn SharedInstance { get; }

		// -(BOOL)handleURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;
		[Export("handleURL:sourceApplication:annotation:")]
		bool HandleURL(NSUrl url, string sourceApplication, NSObject annotation);

		// -(BOOL)hasAuthInKeychain;
		[Export("hasAuthInKeychain")]
		bool HasAuthInKeychain { get; }

		// -(void)signInSilently;
		[Export("signInSilently")]
		void SignInSilently();

		// -(void)signIn;
		[Export("signIn")]
		void SignIn();

		// -(void)signOut;
		[Export("signOut")]
		void SignOut();

		// -(void)disconnect;
		[Export("disconnect")]
		void Disconnect();
	}
}