using System;
using Ads.Ui;
using JetBrains.Annotations;
using Scripts.RemoteConfig;
using UnityEngine;
using UnityEngine.Advertisements;
using Util;
using Developers = Dev.Developers;
#if STENCIL_ADMOB
using Ads.Admob;
using GoogleMobileAdsMediationTestSuite.Api;
#endif

#if UNITY_ADS
using Ads.UnityAds;
#endif

#if STENCIL_IRONSRC
using Ads.IronSrc;
#endif

namespace Ads
{
    public static class StencilAds
    {
        #if STENCIL_ADMOB
        private static AdConfiguration _appId => AdSettings.Instance.AppConfiguration;
        private static AdConfiguration _banner => AdSettings.Instance.BannerConfiguration;
        private static AdConfiguration _interstitial => AdSettings.Instance.InterstitialConfiguration;
        private static AdConfiguration _rewarded => AdSettings.Instance.RewardConfiguration;
        #endif

        public static VideoAd Interstitial { get; private set; }
        public static VideoAd Rewarded { get; private set; }
        public static IBannerStrategy Banner { get; private set; }
        
        [CanBeNull] 
        public static string AdvertisingId { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnSceneLoad()
        {
            CheckReload();
        }

        private static bool _hasAds;
        private static bool _init;
        public static bool HasInit => _init;

        public static void Init()
        {
            Debug.Log("Attempt to initialize StencilAds");
            if (_init) return;
            _init = true;

#if UNITY_IPHONE && !UNITY_EDITOR
//            Debug.Log("StencilAds disabled webview JIT.");
//            SetEnv.Set("JSC_useJIT", "false");
#endif

#if STENCIL_ADMOB
            Debug.Log("StencilAds (admob) initializing");
            Interstitial = new AdmobInterstitial(_interstitial);
            Interstitial.Init();
            Rewarded = new AdmobRewarded(_rewarded);
            Rewarded.Init();    
            SetBannerAdapter(new AdmobBannerStrategy());
            _hasAds = true;
#elif STENCIL_IRONSRC
            Rewarded = new IronSrcRewarded();
            Interstitial = new IronSrcInterstitial();
            _hasAds = true;
#elif UNITY_ADS
            #if UNITY_IOS
            var appId = AdSettings.Instance.UnityId.Ios;
            #else
            var appId = AdSettings.Instance.UnityId.Android;
            #endif
            Debug.Log($"StencilAds (unity) initializing");
            Advertisement.Initialize(appId, Developers.Enabled);
            Interstitial = UnityVideoAd.Interstitial;
            Interstitial.Init();
            Rewarded = UnityVideoAd.Rewarded;
            Rewarded.Init();   
            SetBannerAdapter(new UnityBannerStrategy());
            _hasAds = true;
#endif
            Debug.Log("StencilAds initialized");

            StencilPremium.OnPremiumPurchased += OnPremium;

            if (_hasAds)
            {
                Application.RequestAdvertisingIdentifierAsync((id, enabled, msg) =>
                {
                    AdvertisingId = id;
                    Debug.Log($"Advertising Id is {id} (enabled={enabled})");
                });
            }
        }

        public static event EventHandler OnBannerChange;
        public static float BannerHeight => Banner?.BannerHeight ?? 0f;
        public static bool BannerNeedsScale = true;

        private static void OnPremium(object sender, EventArgs e)
        {
            if (StencilPremium.HasPremium)
                HideBanner();
            else if (!StencilRemote.IsProd())
                ShowBanner();
        }
        
        public static void ShowBanner()
        {
            if (StencilPremium.HasPremium) return;
            Banner?.BannerShow();
        }

        public static void HideBanner(bool andDestroy = false)
        {
            Banner?.BannerHide(andDestroy);
        }

        public static void SetBannerAdapter([CanBeNull] IBannerStrategy banner)
        {
            if (Banner != null)
                Banner.OnBannerChange -= _OnBannerChange;
            Banner = banner;
            if (banner != null)
                banner.OnBannerChange += _OnBannerChange;
        }

        private static void _OnBannerChange(object sender, EventArgs e)
        {
            OnBannerChange?.Invoke();
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