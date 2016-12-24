using System;
using ObjCRuntime;


namespace GoogleSignIn
{
	[Native]
	public enum GIDSignInErrorCode : int
	{
		Unknown = -1,
		Keychain = -2,
		NoSignInHandlersInstalled = -3,
		HasNoAuthInKeychain = -4,
		Canceled = -5
	}

	[Native]
	public enum GIDSignInButtonStyle : int
	{
		Standard = 0,
		Wide = 1,
		IconOnly = 2
	}

	[Native]
	public enum GIDSignInButtonColorScheme : int
	{
		Dark = 0,
		Light = 1
	}

}
