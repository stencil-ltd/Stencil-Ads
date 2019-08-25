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

        private void Start()
        {
            if (_init) return;
            _init = true;
            IronSource.Agent.shouldTrackNetworkState(AdSettings.Instance.trackNetworkState);
            IronSource.Agent.init(AdSettings.Instance.ironSourceId, REWARDED_VIDEO, INTERSTITIAL);
            if (!StencilRemote.IsDeveloper()) return;
            Debug.Log("IronSrc validating integration...");
            IronSource.Agent.validateIntegration();
            StencilAds.Init(new IronSrcRewarded(), new IronSrcInterstitial());
        }
    }
}
#endif