using Ads;
using UnityEngine;

#if STENCIL_ADMOB
using Ads.Admob;
using GoogleMobileAds.Api;
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
            if (_init) return;
            _init = true;
        
#if UNITY_IPHONE && !UNITY_EDITOR
        SetEnv.Set("JSC_useJIT", "false");
#endif
        
#if STENCIL_ADMOB
        Interstitial = new AdmobInterstitial(_interstitial);
        Interstitial.Init();
        Rewarded = new AdmobRewarded(_rewarded);
        Rewarded.Init();
#endif
        }

        public static void CheckReload()
        {
            if (!_init) return;
            Debug.Log("Checking for ad errors...");
            if (Interstitial.HasError)
            {
                Debug.Log("Interstitial error, reload.");
                Interstitial.Load();
            }
            if (Rewarded.HasError)
            {
                Debug.Log("Reward error, reload.");
                Rewarded.Load();
            }
        }

    }
}