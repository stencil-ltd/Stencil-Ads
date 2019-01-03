#if STENCIL_IRONSRC
using Analytics;
using UnityEngine;
using Util;

namespace Ads.IronSrc
{
    public class IronSrcRewarded : VideoAd
    {
        public IronSrcRewarded()
        {
            StencilIronSrc.Init(AdSettings.Instance.ironSourceId);
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent; 
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent; 
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        }

        public override string MediationName 
            => "IronSrc (Unknown)";
        public override AdType AdType 
            => AdType.Rewarded;
        protected override bool PlatformIsReady 
            => IronSource.Agent.isRewardedVideoAvailable();
        public override bool ShowInPremium 
            => true;
        public override bool SupportsEditor => false;

        protected override void ShowInternal()
        {
            IronSource.Agent.showRewardedVideo();
        }

        protected override void LoadInternal()
        {
            
        }

        private void RewardedVideoAvailabilityChangedEvent(bool obj)
        {
            if (obj) NotifyLoad();
        }

        private void RewardedVideoAdShowFailedEvent(IronSourceError obj)
        {
            NotifyError("error", obj.ToString());
        }

        private void RewardedVideoAdClosedEvent()
        {
            NotifyComplete(false);
        }

        private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj)
        {
            NotifyComplete(true);
        }
    }
}
#endif