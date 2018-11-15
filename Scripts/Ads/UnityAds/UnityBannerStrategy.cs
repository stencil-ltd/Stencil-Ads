#if UNITY_ADS

using System;
using System.Collections;
using Plugins.UI;
using UnityEngine;
using UnityEngine.Advertisements;
using Util;

namespace Ads.UnityAds
{
    public class UnityBannerStrategy : IBannerStrategy
    {
        public event EventHandler OnBannerChange;
        
        private bool _visible;
        public float BannerHeight => _visible ? AdSettings.Instance.CustomBannerHeight : 0f;

        public UnityBannerStrategy()
        {
            BannerChange();
        }

        public void BannerShow()
        {
            Debug.Log("Show Banner");
            _visible = true;
            Objects.StartCoroutine(TryShow());
        }

        public void BannerHide(bool andDestroy)
        {
            Debug.Log("Hide Banner");
            _visible = false;
            Advertisement.Banner.Hide(andDestroy);
            BannerChange();
        }

        private IEnumerator TryShow()
        {
            while (!Advertisement.IsReady("banner"))
                yield return new WaitForSeconds(0.5f);
            Advertisement.Banner.Show("banner");
            BannerChange();
        }
        
        private void BannerChange()
        {
            OnBannerChange?.Invoke();
        }
    }
}

#endif