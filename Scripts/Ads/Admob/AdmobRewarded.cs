#if STENCIL_ADMOB

using GoogleMobileAds.Api;
using Util;

namespace Ads.Admob
{
    public class AdmobRewarded : VideoAd
    {
        private RewardBasedVideoAd _ad => RewardBasedVideoAd.Instance;

        public AdmobRewarded(AdConfiguration config) : base(config)
        { }

        public override bool SupportsEditor => false;
        public override bool IsReady => _ad?.IsLoaded() ?? false;
        protected override void ShowInternal() => _ad.Show();

        public override void Init()
        {
            base.Init();
            _ad.OnAdLoaded += (sender, args) => Objects.Enqueue(NotifyLoad);
            _ad.OnAdRewarded += (sender, reward) => Objects.Enqueue(NotifyComplete);
            _ad.OnAdClosed += (sender, reward) => Objects.Enqueue(NotifyClose);
            _ad.OnAdFailedToLoad += (sender, args) => Objects.Enqueue(NotifyError);
        }

        protected override void LoadInternal()
        {
            _ad.LoadAd(new AdRequest.Builder().Build(), UnitId);
        }
    }
}

#endif