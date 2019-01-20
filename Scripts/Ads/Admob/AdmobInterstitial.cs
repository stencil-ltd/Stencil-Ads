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

        public override AdType AdType => AdType.Interstitial;
        public override bool SupportsEditor => false;
        protected override bool PlatformIsReady => _ad?.IsLoaded() ?? false;
        public override bool ShowInPremium => false;
        protected override void ShowInternal() => _ad.Show();

        public override string MediationName => _ad?.MediationAdapterClassName() ?? "Unknown";

        protected override void LoadInternal()
        {
            _ad?.Destroy();
            _ad = new InterstitialAd(UnitId);
            _ad.LoadAd(AdSettings.Instance.CreateRequest("interstitial"));
            _ad.OnAdLoaded += (sender, args) => Objects.Enqueue(NotifyLoad);
            _ad.OnAdFailedToLoad += (sender, args) => Objects.Enqueue(() => NotifyError());
            _ad.OnAdClosed += (sender, args) => Objects.Enqueue(() => NotifyComplete(true));
        }
    }
}

#endif