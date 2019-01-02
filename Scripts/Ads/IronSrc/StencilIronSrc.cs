using Scripts.RemoteConfig;
using UnityEngine;

#if STENCIL_IRONSRC
namespace Ads.IronSrc
{
    public static class StencilIronSrc
    {
        private static bool _init;
        public static void Init(string appKey)
        {
            if (_init) return;
            _init = true;
            IronSource.Agent.init(appKey);
            if (!StencilRemote.IsDeveloper()) return;
            Debug.Log("IronSrc validating integration...");
            IronSource.Agent.validateIntegration();
        }
    }
}
#endif