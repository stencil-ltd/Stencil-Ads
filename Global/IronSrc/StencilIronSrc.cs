using Scripts.RemoteConfig;
using UnityEngine;

#if STENCIL_IRONSRC
using static IronSourceAdUnits;

namespace Ads.IronSrc
{
    public static class StencilIronSrc
    {
        private static bool _init;
        public static void Init(string appKey)
        {
            if (_init) return;
            _init = true;
            IronSource.Agent.shouldTrackNetworkState(AdSettings.Instance.trackNetworkState);
            IronSource.Agent.init(appKey, REWARDED_VIDEO, INTERSTITIAL);
            if (!StencilRemote.IsDeveloper()) return;
            Debug.Log("IronSrc validating integration...");
            IronSource.Agent.validateIntegration();
        }
    }
}
#endif