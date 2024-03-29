#if UNITY_ADS

using System;
using UnityEngine;
using UnityEngine.Advertisements;
using Util;

namespace Ads.UnityAds
{
    public class UnityVideoAd : VideoAd
    {
        private bool _showInPremium;
        
        public static UnityVideoAd Rewarded => new UnityVideoAd("rewardedVideo", true, AdType.Rewarded);
        public static UnityVideoAd Interstitial => new UnityVideoAd("video", false, AdType.Interstitial);

        private AdType _type;
        public override AdType AdType => _type;

        public UnityVideoAd(string placement, bool showInPremium, AdType type) : base(new PlatformValue<string>(placement))
        {
            _type = type;
            _showInPremium = showInPremium;
        }

        protected override bool PlatformIsReady => Advertisement.IsReady(UnitId);
        public override bool ShowInPremium => _showInPremium;
        public override bool SupportsEditor => true;

        protected override void ShowInternal()
        {
            var options = new ShowOptions();
            options.resultCallback = result =>
            {
                Debug.Log($"Unity Ads received callback: {result}");
                switch (result)
                {
                    case ShowResult.Failed:
                    case ShowResult.Skipped:
                        NotifyComplete(false);
                        break;
                    case ShowResult.Finished:
                        NotifyComplete(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(result), result, null);
                }
            };
            Advertisement.Show(UnitId, options);
        }

        protected override void LoadInternal()
        {
            // Nothing.
        }
    }
}

#endif