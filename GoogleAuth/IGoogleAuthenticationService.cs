using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.GoogleAuth.Abstractions
{
	public interface IGoogleAuthenticationService
	{
		void Init(Dictionary<string, object> config);
		void SetAuthenticationCallbacks(IGoogleAuthenticationCallbacks callbacks);
		Task Connect();
		Task SignOut();
		string GetAuthToken();
		string GetAccountName();
		bool IsConnected();
		Task Disconnect();

		bool HandleURL(object urlObject, string sourceApplication, object annotation);
	}

	public interface IGoogleAuthenticationCallbacks
	{
		void OnConnectionSucceeded();
		void OnConnectionFailed();
	}
}
