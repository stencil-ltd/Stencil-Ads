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

        [CanBeNull] private static BannerView _banner;
        private static BannerConfiguration _config;
        private static bool _bannerFailed;
        
        private static bool _visible;
        public static bool IsBannerVisible() => _visible;

        public RectTransform Content => Frame.Instance.Contents;
        public RectTransform Scrim => Frame.Instance.Scrim;
        
        public static float BannerHeight
        {
            get
            {
                if (Application.isEditor)
                    return 225f;
                if (!_visible || _banner == null)
                    return 0f;
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

        public override void DidRegister()
        {
            base.DidRegister();
            StencilPremium.OnPremiumPurchased += OnPurchase;
        }

        public override void WillUnregister()
        {
            base.WillUnregister();
            StencilPremium.OnPremiumPurchased -= OnPurchase;
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
                {
                    _banner = new BannerView(_config, AdSize.SmartBanner, AdPosition.Bottom);
                    _banner.LoadAd(new AdRequest.Builder().Build());
                    _banner.OnAdFailedToLoad += (sender, args) => _bannerFailed = true;
                }
            }

            if (_bannerFailed)
            {
                _bannerFailed = false;
                _banner?.LoadAd(new AdRequest.Builder().Build());                
            }
            
            Change();
        }

        private void OnPurchase(object sender, EventArgs e)
        {
            if (StencilPremium.HasPremium)
            {
                _banner?.Destroy();
                _banner = null;
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
    }
#endif
}