using System;
using UnityEngine;

#if STENCIL_ADMOB
using Ads.Admob;
using GoogleMobileAdsMediationTestSuite.Api;
#endif

namespace Ads
{
    public static class StencilAds
    {
        private static AdConfiguration _appId => AdSettings.Instance.AppConfiguration;
        private static AdConfiguration _banner => AdSettings.Instance.BannerConfiguration;
        private static AdConfiguration _interstitial => AdSettings.Instance.InterstitialConfiguration;
        private static AdConfiguration _rewarded => AdSettings.Instance.RewardConfiguration;

        public static VideoAd Interstitial { get; private set; }
        public static VideoAd Rewarded { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnSceneLoad()
        {
            CheckReload();
        }

        private static bool _init;

        public static void Init()
        {
            Debug.Log("Attempt to initialize StencilAds");
            if (_init) return;
            _init = true;

#if UNITY_IPHONE && !UNITY_EDITOR
            Debug.Log("StencilAds disabled webview JIT.");
            SetEnv.Set("JSC_useJIT", "false");
#endif

#if STENCIL_ADMOB
            Debug.Log("StencilAds (admob) initializing");
            Interstitial = new AdmobInterstitial(_interstitial);
            Interstitial.Init();
            Rewarded = new AdmobRewarded(_rewarded);
            Rewarded.Init();
#endif
            Debug.Log("StencilAds initialized");
        }

        public static VideoAd GetAdByType(AdType type)
        {
            switch (type)
            {
                case AdType.Banner:
                    throw new ArgumentException("Banner is not of type video");
                case AdType.Interstitial:
                    return Interstitial;
                case AdType.Rewarded:
                    return Rewarded;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void CheckReload()
        {
            if (!_init) return;
            Debug.Log("Checking for ad errors...");
            Interstitial?.CheckReload();
            Rewarded?.CheckReload();
        }

#if STENCIL_ADMOB
        public static void ShowTestSuite()
        {
            string id;
            #if UNITY_IOS
            id = AdSettings.Instance.AppId.Ios;
            #else
            id = AdSettings.Instance.AppId.Android;
            #endif
            MediationTestSuite.Show(id);
        }
#else
        public static void ShowTestSuite()
        {
        }
#endif
    }
}