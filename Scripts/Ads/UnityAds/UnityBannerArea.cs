using System;
using System.Collections;
using Ads.Ui;
using Plugins.UI;
using Scripts.RemoteConfig;
using UI;
using UnityEngine;
using UnityEngine.Advertisements;
using Util;

namespace Ads.UnityAds
{    
    public class UnityBannerArea : Controller<UnityBannerArea>, IBannerArea
    {   
        public event EventHandler OnBannerChange;
        
        private static bool _visible;
        
        public static bool WillDisplayBanner => _visible && !StencilPremium.HasPremium;
        public float BannerHeight => WillDisplayBanner ? AdSettings.Instance.CustomBannerHeight : 0f;

        public static bool IsTop => false;//AdSettings.Instance.BannerAtTop;
        
        public void BannerShow()
        {
            Debug.Log("Show Banner");
            _visible = true;
            StartCoroutine(TryShow());
        }

        public void BannerHide()
        {
            Debug.Log("Hide Banner");
            _visible = false;
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
                _visible = false;
                Advertisement.Banner.Hide(true);
            }
            else if (!StencilRemote.IsProd())
                BannerShow();

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