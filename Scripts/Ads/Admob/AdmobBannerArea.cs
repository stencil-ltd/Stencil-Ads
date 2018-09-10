using System;
using Ads.Ui;
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
        private static AdmobBannerArea Instance;

        [CanBeNull] private static BannerView _banner;
        private static BannerConfiguration _config;
        private static bool _bannerFailed;
        
        private static bool _visible;
        public static bool IsBannerVisible() => _visible;

        public RectTransform Content => Frame.Instance.Contents;
        public RectTransform Scrim => Frame.Instance.Scrim;

        private static bool HasBanner => Application.isEditor || _banner != null;
        public static float BannerHeight
        {
            get
            {
                if (!_visible || !HasBanner || StencilPremium.HasPremium)
                    return 0f;
                if (Application.isEditor)
                    return 225f;
                return _banner.GetHeightInPixels();
            }
        }
        
        public static void ShowBanner()
        {
            if (_visible) return;
            Debug.Log("Show Banner");
            _visible = true;
            if (_banner == null) return;
            _banner.Show();
            Change();
        }

        public static void HideBanner()
        {
            if (!_visible) return;
            Debug.Log("Hide Banner");
            _visible = false;
            if (_banner == null) return;
            _banner.Hide();
            Change();
        }

        private static bool _init;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {            
            if (!_init)
            {
                _init = true;
                _config = AdSettings.Instance.BannerConfiguration;
                MobileAds.Initialize(AdSettings.Instance.AppConfiguration);
                MobileAds.SetiOSAppPauseOnBackground(true);

                if (!StencilPremium.HasPremium)
                {
                    _banner = new BannerView(_config, AdSize.SmartBanner, AdPosition.Bottom);
                    _banner.LoadAd(new AdRequest.Builder().Build());
                    _banner.OnAdFailedToLoad += (sender, args) => _bannerFailed = true;
                    if (Application.isEditor)
                        ShowBanner();
                }
            }

            if (_bannerFailed)
            {
                _bannerFailed = false;
                _banner?.LoadAd(new AdRequest.Builder().Build());                
            }
            
            Change();
            StencilPremium.OnPremiumPurchased += OnPurchase;
        }

        private void OnDestroy()
        {
            StencilPremium.OnPremiumPurchased -= OnPurchase;
            Instance = Instance == this ? null : Instance;
        }

        private void OnPurchase(object sender, EventArgs e)
        {
            if (StencilPremium.HasPremium)
            {
                _banner?.Destroy();
                _banner = null;
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
            Scrim?.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, pixelHeight);
            Content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, pixelHeight, 
                ((RectTransform) Content.parent).rect.height - pixelHeight);  
            Debug.Log($"Setting banner height to {pixelHeight}");
        }
#endif
    }
}