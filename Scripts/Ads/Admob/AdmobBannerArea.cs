using System;
using Ads.Ui;
using Analytics;
using JetBrains.Annotations;
using Plugins.UI;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

#if STENCIL_ADMOB
using GoogleMobileAds.Api;
#endif

namespace Ads.Admob
{    
    public class AdmobBannerArea : Controller<AdmobBannerArea>
    {   
        [Serializable]
        public class BannerEvent : UnityEvent
        {}
#if STENCIL_ADMOB
        
        public static BannerEvent OnChange;

        [CanBeNull] private static BannerView _banner;
        private static BannerConfiguration _config;
        private static bool _bannerFailed;
        
        private static bool _visible;

        public bool IsTop;
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
                return _banner.GetHeightInPixels();
            }
        }

        public static bool WillDisplayBanner
            => _visible && HasBanner && !StencilPremium.HasPremium;

        public static void SetBannerVisible(bool visible)
        {
            if (visible) ShowBanner(); 
            else HideBanner();
        }
        
        public static void ShowBanner()
        {
            Debug.Log("Show Banner");
            _visible = true;
            _banner?.Show();
            Change();
        }

        public static void HideBanner()
        {
            Debug.Log("Hide Banner");
            _visible = false;
            _banner?.Hide();
            Change();
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
                if (!StencilPremium.HasPremium) CreateBanner();
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
        }

        private void CreateBanner()
        {
            _banner = new BannerView(_config, AdSize.SmartBanner, IsTop ? AdPosition.Top : AdPosition.Bottom);
            _banner.OnAdLoaded += (sender, args) =>
            {
                if (!WillDisplayBanner)
                    _banner.Hide();
            };
            _banner.OnAdFailedToLoad += (sender, args) =>
            {
                Debug.LogError($"Banner AdRequest Failed");
                Tracking.Instance.Track("ad_failed", "type", "banner");
                _bannerFailed = true;
            };
            LoadAd();
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

        private static void Change()
        {
            Instance?.SetBannerSize(BannerHeight);
            OnChange?.Invoke();
        }
        
        private void SetBannerSize(float pixelHeight)
        {            
            var scaler = Frame.Instance.GetComponentInParent<CanvasScaler>();
            if (scaler == null) return;
            var ratio = scaler.referenceResolution.x / Screen.width;
            pixelHeight *= ratio;
            Scrim?.SetInsetAndSizeFromParentEdge(IsTop ? RectTransform.Edge.Top : RectTransform.Edge.Bottom, 0, pixelHeight);
            Content.SetInsetAndSizeFromParentEdge(IsTop ? RectTransform.Edge.Top : RectTransform.Edge.Bottom, pixelHeight, 
                ((RectTransform) Content.parent).rect.height - pixelHeight);  
            Debug.Log($"Setting banner height to {pixelHeight}");
            Scrim?.gameObject.SetActive(pixelHeight >= 1f);
        }
#endif
    }
}