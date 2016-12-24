using System;
using Plugin.GoogleAuth.Abstractions;

namespace Plugin.GoogleAuth
{
	public static class CrossGoogleAuth
	{
		static Lazy<IGoogleAuthenticationService> TTS = new Lazy<IGoogleAuthenticationService>(() => CreateGoogleAuthService(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

		public static IGoogleAuthenticationService Current
		{
			get
			{
				var ret = TTS.Value;
				if (ret == null)
				{
					throw NotImplementedInReferenceAssembly();
				}
				return ret;
			}
		}

		static IGoogleAuthenticationService CreateGoogleAuthService()
		{
#if PORTABLE
			return null;
#else
			return new GoogleAuthImpl();
#endif
		}

		internal static Exception NotImplementedInReferenceAssembly()
		{
			return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.");
		}
	}
}
