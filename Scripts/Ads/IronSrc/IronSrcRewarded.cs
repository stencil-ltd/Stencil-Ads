#if STENCIL_IRONSRC
using Util;

namespace Ads.IronSrc
{
    public class IronSrcRewarded : VideoAd
    {
        public IronSrcRewarded(PlatformValue<string> unitId) : base(unitId)
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
            NotifyError();
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