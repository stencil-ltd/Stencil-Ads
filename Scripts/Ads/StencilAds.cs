using System;
using UnityEngine;

#if STENCIL_ADMOB
using Ads.Admob;
#endif

namespace Ads
{
    public static class StencilAds
    {
        private static AdConfiguration _appId => AdSettings.Instance.AppConfiguration;
        private static AdConfiguration _banner => AdSettings.Instance.BannerConfiguration;
        private static AdConfiguration _interstitial => AdSettings.Instance.InterstitialConfiguration;
        private static AdConfiguration _rewarded => AdSettings.Instance.RewardConfiguration;
        private static AdConfiguration _house => AdSettings.Instance.HouseConfiguration;

        public static VideoAd House { get; private set; }
        public static VideoAd Interstitial { get; private set; }
        public static VideoAd Rewarded { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnSceneLoad()
        {
            CheckReload();   
        }
        
        public static void InitHouse()
        {
#if STENCIL_ADMOB
        House = new AdmobInterstitial(_house);
        House.Init();
#endif
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
                case AdType.House:
                    return House;
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
    }
}