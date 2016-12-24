using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Plus;
using Android.Gms.Auth;
using Android.OS;
using Android.Widget;
using Plugin.GoogleAuth.Abstractions;
using System.Collections.Generic;

namespace Plugin.GoogleAuth
{
	public class GoogleAuthImpl : Java.Lang.Object,
	GoogleApiClient.IConnectionCallbacks,
	GoogleApiClient.IOnConnectionFailedListener,
	IGoogleAuthenticationService
	{
		GoogleApiClient _googleApiClient;
		IGoogleAuthenticationCallbacks _callbackDelegate;
		Activity _context;
		const int RC_SIGN_IN = 9001, RC_GET_AUTH_CODE = 9003;
		string _authToken;

		private void verifyInit()
		{
			if (_context == null || _googleApiClient == null)
			{
				throw new MissingFieldException("Please initialize the Google Authentication service correctly.");
			}
		}

		public GoogleAuthImpl()
		{
			_context = null;
			_callbackDelegate = null;
			_authToken = null;
		}

		public async Task Connect()
		{
			verifyInit();
			await Task.Run(() =>
			{
				_googleApiClient.Connect();
			});
		}

		public async Task SignOut()
		{
			verifyInit();
			await Task.Run(() =>
			{
				PlusClass.AccountApi.ClearDefaultAccount(_googleApiClient);
				_googleApiClient.Disconnect();
			});
		}

		public async Task Disconnect()
		{
			verifyInit();
			_authToken = null;
			await Task.Run(async () =>
			{
				PlusClass.AccountApi.ClearDefaultAccount(_googleApiClient);
				await PlusClass.AccountApi.RevokeAccessAndDisconnect(_googleApiClient);
				_googleApiClient.Disconnect();
			});
		}

		public string GetAuthToken()
		{
			verifyInit();
			return _authToken;
		}

		public string GetAccountName()
		{
			verifyInit();
			return PlusClass.AccountApi.GetAccountName(_googleApiClient);
		}

		public bool IsConnected()
		{
			verifyInit();
			return _googleApiClient.IsConnected;
		}

		public void OnConnected(Bundle connectionHint)
		{
			var accountName = PlusClass.AccountApi.GetAccountName(_googleApiClient);
			var scopes = "oauth2:profile";
			ThreadPool.QueueUserWorkItem(
				(o) =>
				{
					try
					{
						_authToken = GoogleAuthUtil.GetToken(_context, accountName, scopes);
						Console.WriteLine("Token is " + _authToken);
					}
					catch (UserRecoverableException e)
					{
						_context.StartActivityForResult(e.Intent, RC_GET_AUTH_CODE);
					}
				}
			);
			_callbackDelegate.OnConnectionSucceeded();
		}

		public void OnConnectionSuspended(int cause)
		{
			_authToken = null;
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			_authToken = null;
			if (result.HasResolution)
			{
				try
				{
					result.StartResolutionForResult(_context, RC_SIGN_IN);
				}
				catch (IntentSender.SendIntentException e)
				{
					_googleApiClient.Connect();
				}
			}
			else {
				ShowErrorDialog(result);
			}
			_callbackDelegate.OnConnectionFailed();
		}

		void ShowErrorDialog(ConnectionResult connectionResult)
		{
			int errorCode = connectionResult.ErrorCode;

			if (GooglePlayServicesUtil.IsUserRecoverableError(errorCode))
			{
				var listener = new DialogInterfaceOnCancelListener();
				listener.OnCancelImpl = (dialog) =>
				{
					Console.WriteLine("Cancelled");
				};
				GooglePlayServicesUtil.GetErrorDialog(errorCode, _context, RC_SIGN_IN, listener).Show();
			}
			else {
				var errorstring = string.Format("Google Play Services Error: {0}", errorCode);
				Toast.MakeText(_context, errorstring, ToastLength.Short).Show();
			}
		}

		public void Init(Dictionary<string, object> config)
		{
			if (config["context"] == null)
			{
				throw new KeyNotFoundException("Please provide the Android activity via the 'context' key.");
			}
			_context = (Activity)config["context"];
			_googleApiClient = new GoogleApiClient.Builder(_context)
												  .AddConnectionCallbacks(this)
												  .AddOnConnectionFailedListener(this)
												  .AddApi(PlusClass.API)
												  .AddScope(new Scope(Scopes.Profile))
												  .Build();

		}

		public void SetAuthenticationCallbacks(IGoogleAuthenticationCallbacks callbacks)
		{
			_callbackDelegate = callbacks;
		}

		public bool HandleURL(object urlObject, string sourceApplication, object annotation)
		{
			throw new NotImplementedException();
		}

		class DialogInterfaceOnCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
		{
			public Action<IDialogInterface> OnCancelImpl { get; set; }

			public void OnCancel(IDialogInterface dialog)
			{
				OnCancelImpl(dialog);
			}
		}
	}
}
