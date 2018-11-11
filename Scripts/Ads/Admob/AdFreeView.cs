using UnityEngine;

namespace Ads.Admob
{
    public class AdFreeView : MonoBehaviour 
    {
        public static int Count { get; private set; } 

        private void OnEnable()
        {
            Count++;
            StencilAds.HideBanner();
        }

        private void OnDisable()
        {
            if (--Count == 0)
                StencilAds.ShowBanner();
        }
    }
}