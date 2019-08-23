using System;
using Analytics;
using Binding;
using PaperPlaneTools;
using Scripts.RemoteConfig;
using UI;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Ratings
{
    public class SimpleRateController : Controller<SimpleRateController>
    {
        public float delayShow = 1f;
        public bool checkAtAwake = true;
        
        [RemoteField("simple_rate_native")]
        public bool nativeRate = true;
        
        [RemoteField("simple_rate_native_ham")]
        public bool goHamWithNativeRate = true;
        
        public override void Register()
        {
            base.Register();
            this.BindRemoteConfig();
            RateSettings.Instance.BindRemoteConfig();
            StencilRateHelpers.CountSession();
        }
        
        private void OnEnable()
        {
            if (checkAtAwake) Invoke(nameof(Check), Math.Max(0.1f, delayShow));
        }

        public bool Check()
        {
            var ready = RateSettings.Instance.Config.CheckConditions();
            if (ready > RateReadiness.MediumFailures) return false;
            if (ready > RateReadiness.MinorFailures && !(goHamWithNativeRate && nativeRate && StencilRateHelpers.HasNativeRate()))
                return false;
            ForceShow();
            return true;
        }
        
        #if ODIN_INSPECTOR
        [Button]
        #endif
        public void ForceShow()
        {
            Tracking.Instance.Track("rate_show");
            StencilRateHelpers.MarkShown();
            #if UNITY_IOS
            if (nativeRate && StencilRateHelpers.NativeRate()) 
                return;
            #endif
            AlertReview();
        }
        
        private void AlertReview()
        {
            new Alert($"Enjoying {Application.productName}?", "Would you mind leaving us a review?")
                .SetNegativeButton("Never", StencilRateHelpers.Reject)
                .SetNeutralButton("Maybe Later")
                .SetPositiveButton("OK!", onOk)
                .Show();
        }

        private void onOk()
        {
            var config = RateSettings.Instance.Config;
            config.GoToRateUrl();
        }
    }
}