#if UNITY_ADS

using System;
using UnityEngine.Advertisements;
using Util;

namespace Ads.UnityAds
{
    public class UnityVideoAd : VideoAd
    {
        private bool _showInPremium;
        
        public static UnityVideoAd Rewarded => new UnityVideoAd("rewardedVideo", true);
        public static UnityVideoAd Interstitial => new UnityVideoAd("video", false);
        
        public UnityVideoAd(string placement, bool showInPremium) : base(new PlatformValue<string>(placement))
        {
            _showInPremium = showInPremium;
        }

        public override bool IsReady => Advertisement.IsReady(UnitId);
        public override bool ShowInPremium => _showInPremium;
        
        protected override void ShowInternal()
        {
            var options = new ShowOptions();
            options.resultCallback = result =>
            {
                switch (result)
                {
                    case ShowResult.Failed:
                    case ShowResult.Skipped:
                        NotifyClose();
                        break;
                    case ShowResult.Finished:
                        NotifyComplete();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(result), result, null);
                }
            };
            Advertisement.Show(UnitId);
        }

        protected override void LoadInternal()
        {
            // Nothing.
        }
    }
}

#endif