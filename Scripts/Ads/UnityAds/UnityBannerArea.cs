using System;
using System.Collections;
using Ads.Ui;
using Plugins.UI;
using UI;
using UnityEngine;
using UnityEngine.Advertisements;
using Util;

namespace Ads.UnityAds
{    
    public class UnityBannerArea : Controller<UnityBannerArea>, IBannerArea
    {   
        public event EventHandler OnBannerChange;
        
        private static bool _bannerFailed;
        public float BannerHeight => AdSettings.Instance.CustomBannerHeight;

        public static bool IsTop => false;//AdSettings.Instance.BannerAtTop;
        
        public void BannerShow()
        {
            Debug.Log("Show Banner");
            StartCoroutine(TryShow());
        }

        public void BannerHide()
        {
            Debug.Log("Hide Banner");
            Advertisement.Banner.Hide();
        }

        public override void Register()
        {
            base.Register();
            StencilAds.SetBannerAdapter(this);
        }

        public override void Unregister()
        {
            base.Unregister();
            StencilAds.SetBannerAdapter(null);
        }

        private static bool _hasBanner;
        private void Start()
        {            
            Change();
            StencilPremium.OnPremiumPurchased += OnPurchase;
        }

        private void OnDestroy()
        {
            StencilPremium.OnPremiumPurchased -= OnPurchase;
        }

        private void OnPurchase(object sender, EventArgs e)
        {
            if (StencilPremium.HasPremium)
            {
                _hasBanner = false;
                Advertisement.Banner.Hide(true);
            }
            Change();
        }

        private IEnumerator TryShow()
        {
            while (!Advertisement.IsReady("banner"))
                yield return new WaitForSeconds(0.5f);
            Advertisement.Banner.Show("banner");
            Instance?.Change();
        }
        
        private void Change()
        {
            var frame = Frame.Instance;
            if (frame && !frame.AutoAdZone)
                Frame.Instance?.SetBannerHeight(BannerHeight, IsTop);
            OnBannerChange?.Invoke();
        }
    }
}