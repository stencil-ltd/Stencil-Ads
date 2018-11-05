using System;

namespace Ads
{
    public interface IBannerArea
    {
        event EventHandler OnBannerChange;
        float BannerHeight { get; }
        void BannerShow();
        void BannerHide();
    }
}