#if STENCIL_ADMOB
using UnityEngine;

namespace Ads.Admob
{
    public class AdFreeView : MonoBehaviour
    {
        public static int Count { get; private set; } 

        private void OnEnable()
        {
            Count++;
            AdmobBannerArea.HideBanner();
        }

        private void OnDisable()
        {
            if (--Count == 0)
                AdmobBannerArea.ShowBanner();
        }
    }
}
#endif