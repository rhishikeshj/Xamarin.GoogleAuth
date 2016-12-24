using System;
using System.Threading.Tasks;
using Foundation;
using Plugin.GoogleAuth.Abstractions;
using UIKit;
using GoogleSignIn;
using System.Collections.Generic;

namespace Plugin.GoogleAuth
{
	class GoogleSignInUIDelegate : GIDSignInUIDelegate
	{
		public override void SignInWillDispatch(GIDSignIn signIn, NSError error)
		{
		}

		public override void SignInPresent(GIDSignIn signIn, UIViewController viewController)
		{
			UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(viewController, true, null);
		}

		public override void SignInDismiss(GIDSignIn signIn, UIViewController viewController)
		{
			UIApplication.SharedApplication.KeyWindow.RootViewController.DismissViewController(true, null);
		}

	}

	public class GoogleAuthImpl : GIDSignInDelegate, IGoogleAuthenticationService
	{
		string _clientId;
		string _authToken;
		string _accountName;
		IGoogleAuthenticationCallbacks _callbackDelegate;

		private void verifyInit()
		{
			if (_clientId == null)
			{
				throw new MissingFieldException("Please initialize the Google Authentication service correctly.");
			}
		}

		public GoogleAuthImpl()
		{
			_clientId = null;
			_authToken = _accountName = null;
			_callbackDelegate = null;
		}

		public async Task Connect()
		{
			verifyInit();
			await Task.Run(() =>
			{
				if (IsConnected() == false)
				{
					GIDSignIn.SharedInstance.SignIn();
				}
			});
		}

		public async Task Disconnect()
		{
			verifyInit();
			await Task.Run(() => GIDSignIn.SharedInstance.Disconnect());
		}

		public string GetAccountName()
		{
			verifyInit();
			return _accountName;
		}

		public string GetAuthToken()
		{
			verifyInit();
			return _authToken;
		}

		public bool IsConnected()
		{
			verifyInit();
			return GIDSignIn.SharedInstance.HasAuthInKeychain;
		}

		public async Task SignOut()
		{
			verifyInit();
			await Task.Run(() => GIDSignIn.SharedInstance.SignOut());

		}

		public bool HandleURL(object urlObject, string sourceApplication, object annotation)
		{
			NSUrl url = (NSUrl)urlObject;
			return GIDSignIn.SharedInstance.HandleURL(url, sourceApplication, (NSObject)annotation);
		}

		public void Init(Dictionary<string, object> config)
		{
			if (config["clientId"] == null)
			{
				throw new KeyNotFoundException("Please provide the Android activity via the 'context' key.");
			}
			_clientId = (string)config["clientId"];
			GIDSignIn.SharedInstance.ClientID = _clientId;
			GIDSignIn.SharedInstance.Delegate = this;
			GIDSignIn.SharedInstance.UiDelegate = new GoogleSignInUIDelegate();

			if (IsConnected())
			{
				GIDSignIn.SharedInstance.SignInSilently();
			}
		}

		public void SetAuthenticationCallbacks(IGoogleAuthenticationCallbacks callbacks)
		{
			_callbackDelegate = callbacks;
		}

		public override void DidSignInForUser(GIDSignIn signIn, GIDGoogleUser user, NSError error)
		{
			if (error == null)
			{
				_authToken = user.Authentication.IdToken;
				_accountName = user.Profile.Name;
				if (_callbackDelegate != null)
				{
					_callbackDelegate.OnConnectionSucceeded();
				}
			}
			else
			{
				if (_callbackDelegate != null)
				{
					_callbackDelegate.OnConnectionFailed();
				}
			}
		}

		public override void DidDisconnectWithUser(GIDSignIn signIn, GIDGoogleUser user, NSError error)
		{
			_authToken = _accountName = null;
		}
	}
}
