﻿#if STENCIL_ADMOB

using GoogleMobileAds.Api;
using Util;

namespace Ads.Admob
{
    public class AdmobRewarded : VideoAd
    {
        private RewardBasedVideoAd _ad => RewardBasedVideoAd.Instance;

        public AdmobRewarded(AdConfiguration config) : base(config)
        { }

        public override AdType AdType => AdType.Rewarded;
        public override bool SupportsEditor => false;
        protected override bool PlatformIsReady => _ad?.IsLoaded() ?? false;
        public override bool ShowInPremium => true;
        protected override void ShowInternal() => _ad.Show();
        
        public override string MediationName => _ad?.MediationAdapterClassName() ?? "Unknown";

        public override void Init()
        {
            base.Init();
            _ad.OnAdLoaded += (sender, args) => Objects.Enqueue(NotifyLoad);
            _ad.OnAdRewarded += (sender, reward) => Objects.Enqueue(() => NotifyComplete(true));
            _ad.OnAdClosed += (sender, reward) => Objects.Enqueue(() => NotifyComplete(false));
            _ad.OnAdFailedToLoad += (sender, args) => Objects.Enqueue(() => NotifyError());
        }

        protected override void LoadInternal()
        {
            _ad.LoadAd(AdSettings.Instance.CreateRequest("rewarded"), UnitId);
        }
    }
}

#endif