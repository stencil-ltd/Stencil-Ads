using System;
using Binding;
using Scripts.RemoteConfig;
using UnityEngine;
using UnityEngine.Advertisements;
using Util;

#if STENCIL_ADMOB
using GoogleMobileAds.Api;
using Analytics;
#endif

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

            public override string ToString()
            {
                return $"{nameof(Android)}: {Android}, {nameof(Ios)}: {Ios}";
            }

            public static implicit operator string(AdId id)
            {
#if UNITY_IOS
                return id.Ios;
#elif UNITY_ANDROID
                return id.Android;
#else
                return "";
#endif
            }
        }

        public string[] TestIds { get; } =
        {
            "D50D9F51E331521E6AED71AA95834F1D",
            "BE68C06901E98BF1F8C126AD343CD229",
            "2d9c4c646686985dbf30017b0898acfd",
            "0adb23a59f47cfc5c62996ecd26e1f6a12",
            "9E1318CB77013036D10A5DA41CC268A5" // Aaron Pixel 2XL (10/18)
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

        public bool BannerAtTop;
        public bool IgnoreTestIds;
        public bool IgnorePremium;
        public bool ForceNotReady;
        public float CustomBannerHeight;
        public float NudgeBottomSafeZone = 52f;

        [Header("Iron Source")] 
        public AdId ironSourceId;
        
        [RemoteField("ironsrc_track_network_state")]
        public bool trackNetworkState = true;

        [Header("Unity Ads")] 
        public AdId UnityId;

        [Header("AdMob")]
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
            if (!Application.isPlaying) return;
            this.BindRemoteConfig();
#if STENCIL_ADMOB
            AppConfiguration = new AppIdConfiguration(AppId.Android, AppId.Ios);
            BannerConfiguration = new BannerConfiguration(BannerId.Android, BannerId.Ios);
            InterstitialConfiguration = new InterstitialConfiguration(InterstitialId.Android, InterstitialId.Ios);
            RewardConfiguration = new RewardedConfiguration(RewardedId.Android, RewardedId.Ios);
#endif
            
#if UNITY_ADS
          Advertisement.Initialize(UnityId, !StencilRemote.IsProd());
#endif
        }
    }
}