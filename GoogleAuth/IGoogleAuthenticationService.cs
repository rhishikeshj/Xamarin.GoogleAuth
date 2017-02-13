using System.Collections.Generic;

namespace Plugin.GoogleAuth.Abstractions
{
	public interface IGoogleAuthenticationService
	{
		void Init(Dictionary<string, object> config);
		void SetAuthenticationCallbacks(IGoogleAuthenticationCallbacks callbacks);
		void SignIn();
		void SignOut();
		string GetIdToken();
		string GetAccountName();
		bool IsConnected();
		void Disconnect();

		bool HandleURL(object urlObject, string sourceApplication, object annotation);
	}

	public interface IGoogleAuthenticationCallbacks
	{
		void OnConnectionSucceeded();
		void OnConnectionFailed(string errorMessage);
	}
}
