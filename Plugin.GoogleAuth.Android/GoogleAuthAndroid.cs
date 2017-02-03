using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Plugin.GoogleAuth.Abstractions;
using System.Collections.Generic;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Android.Gms.Extensions;

namespace Plugin.GoogleAuth
{
	public class GoogleAuthImpl : Java.Lang.Object,
	GoogleApiClient.IOnConnectionFailedListener,
	IGoogleAuthenticationService
	{
		GoogleApiClient _googleApiClient;
		IGoogleAuthenticationCallbacks _callbackDelegate;
		Activity _context;
		string _clientId;
		TaskCompletionSource<GoogleSignInResult> signInResultSource;

		const int RC_SIGN_IN = 9001;
		string _idToken;
		string _accountName;

		private void verifyInit()
		{
			if (_context == null)
			{
				throw new MissingFieldException("Please initialize the Google Authentication service correctly.");
			}
		}

		public void clearOutValues(bool removeConfigs)
		{
			if (removeConfigs)
			{
				_googleApiClient = null;
				_context = null;
				_clientId = null;
				_callbackDelegate = null;
			}
			_idToken = _accountName = null;
		}

		public GoogleAuthImpl()
		{
			clearOutValues(true);
		}

		public void Connect()
		{
			verifyInit();
			Task.Run(async () =>
			{
				try
				{
					var gsoBuilder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
															.RequestIdToken(_clientId)
															.RequestEmail();

					var gso = gsoBuilder.Build();
					_googleApiClient = new GoogleApiClient.Builder(_context)
														  .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
														  .Build();
					_googleApiClient.Connect();

					if (signInResultSource != null && !signInResultSource.Task.IsCompleted)
					{
						signInResultSource.TrySetCanceled();
					}

					signInResultSource = new TaskCompletionSource<GoogleSignInResult>();

					var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(_googleApiClient);
					_context.StartActivityForResult(signInIntent, RC_SIGN_IN);
					var result = await signInResultSource.Task;
					if (result == null || result.Status.IsCanceled || result.Status.IsInterrupted)
					{
						_callbackDelegate.OnConnectionFailed("Connection cancelled");
						return;
					}

					if (!result.IsSuccess)
					{
						_callbackDelegate.OnConnectionFailed(result.Status.StatusMessage);
						return;
					}
					_accountName = result.SignInAccount?.DisplayName;
					_idToken = result.SignInAccount?.IdToken;

					_callbackDelegate.OnConnectionSucceeded();
				}
				catch (Exception ex)
				{
					_callbackDelegate.OnConnectionFailed(ex.Message);
				}

			});
		}

		public void SignOut()
		{
			verifyInit();
			Task.Run(() =>
			{
				if (_googleApiClient != null && _googleApiClient.IsConnected)
				{
					Auth.GoogleSignInApi.SignOut(_googleApiClient);
					clearOutValues(false);
				}
			});
		}

		public void Disconnect()
		{
			verifyInit();
			_idToken = null;
			Task.Run(() =>
			{
				if (_googleApiClient != null && _googleApiClient.IsConnected)
				{
					Auth.GoogleSignInApi.RevokeAccess(_googleApiClient);
					Auth.GoogleSignInApi.SignOut(_googleApiClient);
					clearOutValues(false);
				}
			});
		}

		public string GetIdToken()
		{
			verifyInit();
			return _idToken;
		}

		public string GetAccountName()
		{
			verifyInit();
			return _accountName;
		}

		public bool IsConnected()
		{
			verifyInit();
			return _googleApiClient != null && _googleApiClient.IsConnected;
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
			clearOutValues(false);
		}

		public static object getKeyFromConfig(string key, Dictionary<string, object> config)
		{
			if (config[key] == null)
			{
				throw new KeyNotFoundException("Please provide the " + key + " via config dictionary.");
			}
			return config[key];
		}

		public void Init(Dictionary<string, object> config)
		{
			_clientId = getKeyFromConfig("clientId", config) as string;
			_context = getKeyFromConfig("context", config) as Activity;
		}

		public void SetAuthenticationCallbacks(IGoogleAuthenticationCallbacks callbacks)
		{
			_callbackDelegate = callbacks;
		}

		public bool HandleURL(object urlObject, string sourceApplication, object annotation)
		{
			throw new NotImplementedException();
		}

		public void setSignInResult(GoogleSignInResult result)
		{
			if (signInResultSource != null && !signInResultSource.Task.IsCompleted)
			{
				signInResultSource.TrySetResult(result);
			}
		}

		public void OnActivityResult(int requestCode, Result result, Intent data)
		{
			if (requestCode == RC_SIGN_IN)
			{
				var googleSignInResult = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
				setSignInResult(googleSignInResult);
			}
		}
	}
}
