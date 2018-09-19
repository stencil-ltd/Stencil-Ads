using Lifecycle;
using UnityEngine;

namespace Ratings
{
    [CreateAssetMenu(menuName = "Settings/Ratings")]
    public class RateSettings : Singleton<RateSettings>
    {
        [Header("App Settings")] 
        public int AppStoreId;
        public string PlayStoreId => Application.identifier;
        
        [Header("Timings")]
        public int MinSessionCount = 5;
        public float HoursAfterInstall = 24f;
        public float HoursAfterLaunch = 0.1f;
        public float HoursAfterPostpone = 72;
        
        public bool RequireInternetConnection => true;

        public string RateUrl 
            => StencilRateHelpers.GetStoreUrl(""+AppStoreId, PlayStoreId);
    }
}