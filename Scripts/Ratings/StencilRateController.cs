using System;
using Scripts.RemoteConfig;
using UI;
using UnityEngine;
using Util;

namespace Ratings
{
    public class StencilRateController : Controller<StencilRateController>
    {
        public RateSettings Settings = new RateSettings();

        [Header("UI")]
        public float DelayShow = 1f;
        public bool CheckAtAwake = true;
        public StencilRater Rater;

        private void Start()
        {
            Rater.OnNever.AddListener(OnNever);
            Rater.OnPositive.AddListener(OnPositive);
            Rater.OnNegative.AddListener(OnNegative);
            Settings.BindRemoteConfig();
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
            if (!StencilRateHelpers.IsEnabled)
            {
                Debug.LogWarning("Rating is not enabled...");
                enabled = false;
                return;
            }
            
            if (CheckAtAwake)
                Invoke(nameof(Check), Math.Max(0.1f, DelayShow));
        }

        public bool Check()
        {
            if (!Settings.CheckConditions()) 
                return false;
            ForceShow();
            return true;
        }

        public void ForceShow()
        {
            StencilRateHelpers.MarkShown();
            Rater.gameObject.SetActive(true);
        }

        public void GoToRating()
        {
            StencilRateHelpers.RecordRating();
            Settings.GoToRateUrl();
            Rater.Dismiss();
        }

        private void OnRemoteConfig(object sender, EventArgs e)
        {
            Settings.BindRemoteConfig();
        }

        private void OnPositive(int arg0)
        {
            StencilRateHelpers.RecordRating();
            Settings.GoToRateUrl();
            Rater.Dismiss();
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