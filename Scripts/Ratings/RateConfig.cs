using System;
using Binding;
using UnityEngine;

namespace Ratings
{
    [Serializable]
    public class RateConfig
    {
        [Header("App Settings")]
        [RemoteField("rate_app_store_id")]
        public int AppStoreId;
        
        [Header("Timings")]
        [RemoteField("rate_min_sessions")]
        public int MinSessionCount = 5;
        
        [RemoteField("rate_post_install")]
        public float HoursAfterInstall = 24f;
        
        [RemoteField("rate_post_launch")]
        public float HoursAfterLaunch = 0.1f;
        
        [RemoteField("rate_post_cancel")]
        public float HoursAfterPostpone = 72;

        [Header("iOS Settings")]
        [RemoteField("rate_ios_native")]
        public bool IosNativeRating;
        
        public string PlayStoreId => Application.identifier;
        public bool RequireInternetConnection => true;
        public string RateUrl => StencilRateHelpers.GetStoreUrl(""+AppStoreId, PlayStoreId);
    }
}