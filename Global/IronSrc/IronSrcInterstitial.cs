#if STENCIL_IRONSRC
using Analytics;

namespace Ads.IronSrc
{
    public class IronSrcInterstitial : VideoAd
    {
        public IronSrcInterstitial()
        {
            IronSourceEvents.onInterstitialAdClosedEvent += RewardedVideoAdClosedEvent; 
            IronSourceEvents.onInterstitialAdReadyEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += RewardedVideoAdShowEvent;
            IronSourceEvents.onInterstitialAdClickedEvent += RewardedVideoAdClickEvent;
        }

        public override string MediationName 
            => "IronSrc (Unknown)";
        public override AdType AdType 
            => AdType.Interstitial;
        protected override bool PlatformIsReady 
            => IronSource.Agent.isInterstitialReady();
        public override bool ShowInPremium 
            => false;
        public override bool SupportsEditor => false;
        
        protected override void ShowInternal()
        {
            IronSource.Agent.showInterstitial();
        }

        protected override void LoadInternal()
        {
            IronSource.Agent.loadInterstitial();
        }

        private void RewardedVideoAvailabilityChangedEvent()
        {
            NotifyLoad();
        }

        private void RewardedVideoAdShowFailedEvent(IronSourceError obj)
        {
            NotifyError();
        }

        private void RewardedVideoAdClosedEvent()
        {
            NotifyComplete(false);
        }

        private void RewardedVideoAdShowEvent()
        {
            Tracking.Instance.Track("stencil_ad_impression", "type", "interstitial");
        }

        private void RewardedVideoAdClickEvent()
        {
            Tracking.Instance.Track("stencil_ad_click", "type", "interstitial");
        }
    }
}
#endif