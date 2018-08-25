#if STENCIL_ADMOB

using GoogleMobileAds.Api;
using Util;

namespace Ads.Admob
{
    public class AdmobInterstitial : VideoAd
    {
        private InterstitialAd _ad;

        public AdmobInterstitial(AdConfiguration config) : base(config)
        { }

        public override bool SupportsEditor => false;
        public override bool IsReady => _ad?.IsLoaded() ?? false;
        protected override void ShowInternal() => _ad.Show();

        protected override void LoadInternal()
        {
            _ad?.Destroy();
            _ad = new InterstitialAd(UnitId);
            _ad.LoadAd(new AdRequest.Builder().Build());
            _ad.OnAdLoaded += (sender, args) => Objects.Enqueue(NotifyLoad);
            _ad.OnAdFailedToLoad += (sender, args) => Objects.Enqueue(NotifyError);
            _ad.OnAdClosed += (sender, args) => Objects.Enqueue(() =>
            {
                NotifyClose();
                NotifyComplete();
            });
        }
    }
}

#endif