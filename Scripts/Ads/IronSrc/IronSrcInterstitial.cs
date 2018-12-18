#if STENCIL_IRONSRC
using UnityEngine;
using Util;

namespace Ads.IronSrc
{
    public class IronSrcInterstitial : VideoAd
    {
        public IronSrcInterstitial()
        {
            StencilIronSrc.Init(AdSettings.Instance.ironSourceId);
            IronSourceEvents.onInterstitialAdClosedEvent += RewardedVideoAdClosedEvent; 
            IronSourceEvents.onInterstitialAdReadyEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
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
    }
}
#endif