#if STENCIL_IRONSRC
using UI;
using static IronSourceAdUnits;
using Scripts.RemoteConfig;
using UnityEngine;

namespace Ads.IronSrc
{
    public class StencilIronSrc : Controller<StencilIronSrc>
    {
        private static bool _init;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (_init) return;
            _init = true;
            IronSource.Agent.shouldTrackNetworkState(AdSettings.Instance.trackNetworkState);
            IronSource.Agent.init(AdSettings.Instance.ironSourceId, REWARDED_VIDEO, INTERSTITIAL);
            Validate();
            StencilAds.Init(new IronSrcRewarded(), new IronSrcInterstitial());
        }

        private void Validate()
        {
            if (!StencilRemote.IsDeveloper()) return;
            Debug.Log("IronSrc validating integration...");
            IronSource.Agent.validateIntegration();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
#if STENCIL_IRONSRC
            IronSource.Agent.onApplicationPause(pauseStatus);
#endif
        }
    }
}
#endif