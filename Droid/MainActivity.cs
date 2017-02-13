using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.GoogleAuth.Abstractions;
using System.Collections.Generic;
using Plugin.GoogleAuth;

namespace GoogleAuthSample.Droid
{
	[Activity(Label = "GoogleAuthSample.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IGoogleAuthenticationCallbacks
	{
		IGoogleAuthenticationService _service;

		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			Xamarin.Forms.Forms.Init(this, bundle);
			_service = CrossGoogleAuth.Current;
			Dictionary<string, object> googleSignInConfig = new Dictionary<string, object> {
				{"clientId", ""},
				{"context", this},
			};
			_service.Init(googleSignInConfig);
			_service.SetAuthenticationCallbacks(this);

			LoadApplication(new App());
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			var googleAuthService = _service as GoogleAuthImpl;
			googleAuthService.OnActivityResult(requestCode, resultCode, data);
		}

		public void OnConnectionFailed(string errorMessage)
		{
			Console.WriteLine("Connection failed : " + errorMessage);
		}

		public void OnConnectionSucceeded()
		{
			Console.WriteLine("Signed in as : " + _service.GetAccountName());
		}
	}
}
