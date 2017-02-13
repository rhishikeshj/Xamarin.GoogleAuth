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
		string _idToken;
		string _accountName;
		IGoogleAuthenticationCallbacks _callbackDelegate;

		private void verifyInit()
		{
			if (_clientId == null)
			{
				throw new MissingFieldException("Please initialize the Google Authentication service correctly.");
			}
		}

		private void clearOutValues(bool removeConfigs)
		{
			if (removeConfigs)
			{
				_clientId = null;
				_callbackDelegate = null;
			}
			_idToken = _accountName = null;
		}

		public GoogleAuthImpl()
		{
			clearOutValues(true);
		}

		public void SignIn()
		{
			verifyInit();
			Task.Run(() =>
			{
				if (IsConnected() == false)
				{
					GIDSignIn.SharedInstance.SignIn();
				}
			});
		}

		public void SignOut()
		{
			verifyInit();
			Task.Run(() =>
			{
				GIDSignIn.SharedInstance.SignOut();
				clearOutValues(false);
			});
		}

		public void Disconnect()
		{
			verifyInit();
			Task.Run(() =>
			{
				GIDSignIn.SharedInstance.Disconnect();
				clearOutValues(false);
			});
		}

		public string GetAccountName()
		{
			verifyInit();
			return _accountName;
		}

		public string GetIdToken()
		{
			verifyInit();
			return _idToken;
		}

		public bool IsConnected()
		{
			verifyInit();
			return GIDSignIn.SharedInstance.HasAuthInKeychain;
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
				throw new KeyNotFoundException("Please provide the server client id via the 'cliendId' key.");
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
				_idToken = user.Authentication.IdToken;
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
					_callbackDelegate.OnConnectionFailed(error.Description);
				}
			}
		}

		public override void DidDisconnectWithUser(GIDSignIn signIn, GIDGoogleUser user, NSError error)
		{
			clearOutValues(false);
		}
	}
}
