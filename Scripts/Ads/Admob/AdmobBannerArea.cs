using System;
using Ads.Ui;
using Analytics;
using JetBrains.Annotations;
using Plugins.UI;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Util;
#if STENCIL_ADMOB
using GoogleMobileAds.Api;
#endif

namespace Ads.Admob
{    
    public class AdmobBannerArea : Controller<AdmobBannerArea>
    {   
        public static event EventHandler OnChange;
        
#if STENCIL_ADMOB

        [CanBeNull] private static BannerView _banner;
        private static BannerConfiguration _config;
        private static bool _bannerFailed;
        
        private static bool _visible;

        public RectTransform Content => Frame.Instance.Contents;
        public RectTransform Scrim => Frame.Instance.Scrim;

        private static bool HasBanner => Application.isEditor || _banner != null;
        public static float BannerHeight
        {
            get
            {
                if (!WillDisplayBanner)
                    return 0f;
                if (Application.isEditor)
                    return 150f;
                return _banner?.GetHeightInPixels() ?? 0;
            }
        }

        public static bool IsTop => AdSettings.Instance.BannerAtTop;

        public static bool WillDisplayBanner
            => _visible && HasBanner && !StencilPremium.HasPremium;
        
        public static void ShowBanner()
        {
            Debug.Log("Show Banner");
            _visible = true;
            _banner?.Show();
            Instance?.Change();
        }

        public static void HideBanner()
        {
            Debug.Log("Hide Banner");
            _visible = false;
            _banner?.Hide();
            Instance?.Change();
        }

        private static bool _init;
        private void Start()
        {            
            if (!_init)
            {
                _init = true;
                _config = AdSettings.Instance.BannerConfiguration;
                MobileAds.Initialize(AdSettings.Instance.AppConfiguration);
                MobileAds.SetiOSAppPauseOnBackground(true);
                if (!StencilPremium.HasPremium)
                    CreateBanner();
            }
            else
            {
                SetupBannerCallbacks();
            }

            if (_bannerFailed)
            {
                _bannerFailed = false;
                LoadAd();          
            }
            
            Change();
            StencilPremium.OnPremiumPurchased += OnPurchase;
        }

        private void OnDestroy()
        {
            StencilPremium.OnPremiumPurchased -= OnPurchase;
            if (_banner != null)
            {
                _banner.OnAdLoaded -= OnBannerOnOnAdLoaded;
                _banner.OnAdFailedToLoad -= OnBannerOnOnAdFailedToLoad;
            }
        }

        private void CreateBanner()
        {
            Debug.Log("Create Banner");
            if (_banner != null) Debug.LogError("Duplicate banner detected");
            _banner = new BannerView(_config, AdSize.SmartBanner, IsTop ? AdPosition.Top : AdPosition.Bottom);
            SetupBannerCallbacks();
            LoadAd();
            if (_visible) ShowBanner(); else HideBanner();
        }

        private void SetupBannerCallbacks()
        {
            if (_banner == null) return;
            _banner.OnAdLoaded += OnBannerOnOnAdLoaded;
            _banner.OnAdFailedToLoad += OnBannerOnOnAdFailedToLoad;
        }

        private void OnBannerOnOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.LogError($"Banner AdRequest Failed");
            Tracking.Instance.Track("ad_failed", "type", "banner");
            _bannerFailed = true;
        }

        private void OnBannerOnOnAdLoaded(object sender, EventArgs args)
        {
            if (!WillDisplayBanner) _banner.Hide();
        }

        private static void LoadAd()
        {
            _banner?.LoadAd(AdSettings.Instance.CreateRequest("banner"));
        }

        private void OnPurchase(object sender, EventArgs e)
        {
            if (StencilPremium.HasPremium)
            {
                _banner?.Destroy();
                _banner = null;
                Change();
            }
            else
            {
                CreateBanner();
                Change();
            }
        }

        private void Change()
        {
            var frame = Frame.Instance;
            if (frame && !frame.AutoAdZone)
                Frame.Instance?.SetBannerHeight(BannerHeight, IsTop);
            OnChange?.Invoke();
        }
#else
        public static bool IsTop;
        public static float BannerHeight = 0f;
        public static bool WillDisplayBanner => false;

        public static void SetBannerVisible(bool visible)
        {
            if (visible) ShowBanner(); 
            else HideBanner();
        }
        
        public static void ShowBanner()
        {
            Debug.Log("Show Banner");
        }

        public static void HideBanner()
        {
            Debug.Log("Hide Banner");
        }
#endif
    }
}