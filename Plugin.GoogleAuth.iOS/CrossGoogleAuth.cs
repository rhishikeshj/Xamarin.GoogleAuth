using Plugin.GoogleAuth.Abstractions;
using System;

namespace Plugin.Vibrate
{
	public static class CrossVibrate
	{
		static Lazy<IGoogleAuthenticationService> TTS = new Lazy<IGoogleAuthenticationService>(() => CreateGoogleAuthService(),
																							   System.Threading.LazyThreadSafetyMode.PublicationOnly);
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
			return new GoogleAuthenticationService();
#endif
		}

		internal static Exception NotImplementedInReferenceAssembly()
		{
			return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.");
		}
	}
}