using System;
using Plugin.GoogleAuth.Abstractions;
using Plugin.GoogleAuth;

using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.Generic;

namespace GoogleAuthSample
{
	public class App : Application
	{
		public App()
		{
			MainPage = new LoginPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

	}
}
