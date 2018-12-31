using System;
using PaperPlaneTools;
using Scripts.RemoteConfig;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace Ratings
{
    public class BasicRatingController : Controller<BasicRatingController>
    {
        public float delayShow = 1f;
        public bool checkAtAwake = true;
        
        public override void Register()
        {
            base.Register();
            RateSettings.Instance.BindRemoteConfig();
        }
        
        private void OnEnable()
        {
            if (!StencilRateHelpers.IsEnabled)
            {
                Debug.LogWarning("Rating is not enabled...");
                enabled = false;
                return;
            }
            
            if (checkAtAwake)
                Invoke(nameof(Check), Math.Max(0.1f, delayShow));
        }

        public bool Check()
        {
            if (!RateSettings.Instance.Config.CheckConditions()) return false;
            ForceShow();
            return true;
        }
        
        #if ODIN_INSPECTOR
        [Button]
        #endif
        public void ForceShow()
        {
            StencilRateHelpers.MarkShown();
            #if UNITY_IOS
            StencilRateHelpers.NativeRate();
            #else
            AlertReview();
            #endif
        }
        
        private void AlertReview()
        {
            var config = RateSettings.Instance.Config;
            new Alert($"Enjoying {Application.productName}?", "Would you mind leaving us a review?")
                .SetNegativeButton("Never", StencilRateHelpers.Reject)
                .SetNeutralButton("Maybe Later")
                .SetPositiveButton("OK!", () => { config.GoToRateUrl(); })
                .Show();
        }
    }
}