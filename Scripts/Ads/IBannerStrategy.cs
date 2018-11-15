using System;

namespace Ads
{
    public interface IBannerStrategy
    {
        event EventHandler OnBannerChange;
        float BannerHeight { get; }
        void BannerShow();
        void BannerHide(bool andDestroy);
    }
}