using System;
using Scripts.RemoteConfig;
using UI;
using UnityEngine;
using Util;

namespace Ratings
{
    public class StencilRateController : Controller<StencilRateController>
    {
        [Header("UI")]
        public float DelayShow = 1f;
        public bool CheckAtAwake = true;
        public StencilRater Rater;
        
        [Obsolete("Use Rate Settings + GetConfig() instead.")]
        [Header("Obsolete")]
        public RateConfig Settings = new RateConfig();
        
        public static RateConfig GetConfig() 
            => RateSettings.Instance?.Config ?? Instance?.Settings; 

        private void Start()
        {
            Rater.OnNever.AddListener(OnNever);
            Rater.OnPositive.AddListener(OnPositive);
            Rater.OnNegative.AddListener(OnNegative);
            
            Settings?.BindRemoteConfig();
        }

        public override void Register()
        {
            base.Register();
            StencilRemote.OnRemoteConfig += OnRemoteConfig;
        }

        public override void WillUnregister()
        {
            base.WillUnregister();
            StencilRemote.OnRemoteConfig -= OnRemoteConfig;
        }

        private void OnEnable()
        {
            if (CheckAtAwake)
                Invoke(nameof(Check), Math.Max(0.1f, DelayShow));
        }

        public bool Check()
        {
            if (!GetConfig().CheckConditions()) return false;
            ForceShow();
            return true;
        }

        public void ForceShow()
        {
            StencilRateHelpers.MarkShown();
#if UNITY_IOS
            if (StencilRateHelpers.NativeRate())
                return;
#endif          
            Rater.gameObject.SetActive(true);
        }

        public void GoToRating()
        {
            GetConfig().Rate();
            Rater.Dismiss();
        }

        private void OnRemoteConfig(object sender, EventArgs e)
        {
            Settings?.BindRemoteConfig();
        }

        private void OnPositive(int arg0)
        {
            GoToRating();
        }

        private void OnNegative(int arg0)
        {
            StencilRateHelpers.RecordRating();
            Rater.AskForFeedback();
        }

        private void OnNever()
        {
            StencilRateHelpers.Reject();
        }
    }
}