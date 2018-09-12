using System;
using Analytics;
using GoogleMobileAds.Api;
using UnityEngine;
using Util;

namespace Ads
{
    [CreateAssetMenu(menuName = "Stencil/Ads")]
    public class AdSettings : Singleton<AdSettings>
    {
        [Serializable]
        public struct AdId
        {
            public string Android;
            public string Ios;
        }

        public string[] TestIds { get; } =
        {
            "D50D9F51E331521E6AED71AA95834F1D",
            "BE68C06901E98BF1F8C126AD343CD229"
        };
        
        #if STENCIL_ADMOB
        public AdRequest CreateRequest(string type)
        {
            var builder = new AdRequest.Builder();
            if (!IgnoreTestIds)
            {
                foreach (var str in TestIds)
                    builder.AddTestDevice(str);
            }
            Debug.Log("Create AdRequest");
            Tracking.Instance
                .Track("ad_request", "type", type)
                .Track($"ad_request_{type}");
            return builder.Build();
        }
        #endif

        public bool IgnoreTestIds;
        public bool IgnorePremium;
        
        public AdId AppId;
        public AppIdConfiguration AppConfiguration { get; private set; }        
        
        public AdId BannerId;
        public BannerConfiguration BannerConfiguration { get; private set; }
        
        public AdId InterstitialId;
        public InterstitialConfiguration InterstitialConfiguration { get; private set; }
        
        public AdId RewardedId;
        public RewardedConfiguration RewardConfiguration { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            AppConfiguration = new AppIdConfiguration(AppId.Android, AppId.Ios);
            BannerConfiguration = new BannerConfiguration(BannerId.Android, BannerId.Ios);
            InterstitialConfiguration = new InterstitialConfiguration(InterstitialId.Android, InterstitialId.Ios);
            RewardConfiguration = new RewardedConfiguration(RewardedId.Android, RewardedId.Ios);
        }
    }
}