# Xamarin.GoogleAuth

Cross platform plugin for doing Google Authentication for Xamarin iOS and Android apps

## Why

The short answer is that Google has updated its security restrictions for OAuth flow.
They are not going to allow native web-views to initiate OAuth flows, but rather are encouraging people to use the OS browsers to do so.

## How

Here are the basic steps that you need to follow to add Google Authentication into your Xamarin forms application

1. Setup your Application on the Google developer's console to use OAuth2. Guide [here](https://developers.google.com/identity/protocols/OAuth2). Make sure you select the **Web Application** option.
1. Download this repository and copy over the _GoogleAuth_, _Plugin.GoogleAuth_, _Plugin.GoogleAuth.Android_ and _Plugin.GoogleAuth.iOS_ folders into your application and add them into your solution in VS or Xamarin Studio.
1. Add references to the _Plugin.GoogleAuth_ and _Plugin.GoogleAuth.Abstractions_ projects into your PCL project.
1. Add references to the _Plugin.GoogleAuth.Android_ and _Plugin.GoogleAuth.Abstractions_ projects into your Android project.
1. Add references to the _Plugin.GoogleAuth.iOS_ and _Plugin.GoogleAuth.Abstractions_ projects into your Android project.
1. Inside your platform specific projects initialize the Google Authentication service as shown below.

            :::csharp
			IGoogleAuthenticationService _service;
			_service = CrossGoogleAuth.Current;
			Dictionary<string, object> googleSignInConfig = new Dictionary<string, object> {
				{"clientId", "<client-id>"},
				{"context", this} // Android only
			};
			_service.Init(googleSignInConfig);

1. For further usage details, check out the GoogleAuthSample project in this repository.

## TODOs

1. Convert this into a NuGet package !
