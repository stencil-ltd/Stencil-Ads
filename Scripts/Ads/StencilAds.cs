using System;
using Ads.Ui;
using JetBrains.Annotations;
using Scripts.RemoteConfig;
using UnityEngine;
using Util;

namespace Ads
{
    public static class StencilAds
    {
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

        public static void Init([CanBeNull] VideoAd rewarded = null, [CanBeNull] VideoAd interstitial = null)
        {
            Debug.Log("Attempt to initialize StencilAds");
            if (_init) return;
            _init = true;

            Rewarded = rewarded;
            Interstitial = interstitial;
            _hasAds = Rewarded != null || Interstitial != null;
            Debug.Log("StencilAds initialized");

            Interstitial?.Init();
            Rewarded?.Init();
            
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