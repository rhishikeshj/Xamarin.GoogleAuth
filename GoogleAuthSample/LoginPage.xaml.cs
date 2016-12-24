using System;
using System.Collections.Generic;
using Plugin.GoogleAuth;
using Plugin.GoogleAuth.Abstractions;
using Xamarin.Forms;

namespace GoogleAuthSample
{
	public partial class LoginPage : ContentPage, IGoogleAuthenticationCallbacks
	{
		IGoogleAuthenticationService _service = CrossGoogleAuth.Current;
		public LoginPage()
		{
			InitializeComponent();
			BindingContext = this;
			_service.SetAuthenticationCallbacks(this);
			if (_service.IsConnected() == true)
			{
				LoginButton.IsVisible = false;
				LogoutContainerView.IsVisible = true;
				Content.Text = "Signed in as : " + _service.GetAccountName();
			}
			else {
				Content.Text = "Click the Login button to begin";
			}
		}

		void loginClicked(object sender, EventArgs args)
		{
			_service.Connect();
		}

		void logoutClicked(object sender, EventArgs args)
		{
			_service.SignOut();
			LoginButton.IsVisible = true;
			LogoutContainerView.IsVisible = false;
			Content.Text = "Signed out!";
		}

		void disconnectClicked(object sender, EventArgs args)
		{
			_service.Disconnect();
			LoginButton.IsVisible = true;
			LogoutContainerView.IsVisible = false;
			Content.Text = "Signed out!";
		}

		public void OnConnectionSucceeded()
		{
			LoginButton.IsVisible = false;
			LogoutContainerView.IsVisible = true;
			Content.Text = "Signed in as : " + _service.GetAccountName();
		}

		public void OnConnectionFailed()
		{
			LoginButton.IsVisible = true;
			LogoutContainerView.IsVisible = false;
			Content.Text = "Signed out!";
		}
	}
}
