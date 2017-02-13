using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using Plugin.GoogleAuth.Abstractions;
using Plugin.GoogleAuth;

namespace GoogleAuthSample.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IGoogleAuthenticationCallbacks
	{
		IGoogleAuthenticationService _service;
		public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
		{
			Xamarin.Forms.Forms.Init();
			_service = CrossGoogleAuth.Current;
			Dictionary<string, object> googleSignInConfig = new Dictionary<string, object> {
				{"clientId", ""},
				{"scope", "https://www.googleapis.com/auth/userinfo.profile"}
			};
			_service.Init(googleSignInConfig);
			_service.SetAuthenticationCallbacks(this);

			LoadApplication(new App());
			return base.FinishedLaunching(uiApplication, launchOptions);
		}

		public void OnConnectionFailed(string errorMessage)
		{
			Console.WriteLine("Connection failed : " + errorMessage);
		}

		public void OnConnectionSucceeded()
		{
			Console.WriteLine("Signed in as : " + _service.GetAccountName());
		}

		public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		{
			string sourceApplication = options["UIApplicationOpenURLOptionsSourceApplicationKey"].ToString();
			NSObject annotation = options["UIApplicationOpenURLOptionsAnnotationKey"] ?? new NSObject();
			return _service.HandleURL(url, sourceApplication, annotation);
		}

	}
}