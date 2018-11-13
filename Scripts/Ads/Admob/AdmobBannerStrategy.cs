using System;
using Analytics;
using GoogleMobileAds.Api;
using JetBrains.Annotations;
using UnityEngine;
using Util;

namespace Ads.Admob
{
    public class AdmobBannerStrategy : IBannerStrategy
    {
        public event EventHandler OnBannerChange;

        [CanBeNull] private BannerView _banner;

        private static AdPosition Position 
            => AdSettings.Instance.BannerAtTop ? AdPosition.Top : AdPosition.Bottom;
        
        private bool _visible;
        public float BannerHeight
        {
            get
            {
                if (!_visible)
                    return 0f;
                if (Application.isEditor)
                    return 100f;
                return _banner?.GetHeightInPixels() ?? 0;
            }
        }

        public AdmobBannerStrategy()
        {
            MobileAds.Initialize(AdSettings.Instance.AppConfiguration);
            MobileAds.SetiOSAppPauseOnBackground(true);
            Change();
        }

        public void BannerShow()
        {
            Debug.Log("Show Banner");
            _visible = true;
            if (_banner == null)
                CreateBanner();
            _banner?.Show();
            Change();
        }

        public void BannerHide(bool andDestroy)
        {
            Debug.Log("Hide Banner");
            _visible = false;
            if (andDestroy)
            {
                _banner?.Destroy();
                _banner = null;
            } else _banner?.Hide();
            Change();
        }

        public void Change()
        {
            OnBannerChange?.Invoke();
        }

        private void CreateBanner()
        {
            Debug.Log("Create Banner");
            _banner = new BannerView(AdSettings.Instance.BannerConfiguration, AdSize.SmartBanner, Position);
            SetupBannerCallbacks();
            LoadAd();
        }

        private void LoadAd()
        {
            _banner?.LoadAd(AdSettings.Instance.CreateRequest("banner"));
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
        }

        private void OnBannerOnOnAdLoaded(object sender, EventArgs args)
        {
            if (!_visible) _banner?.Hide();
        }
    }
}