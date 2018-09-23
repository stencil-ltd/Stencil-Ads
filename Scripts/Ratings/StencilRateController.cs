using System;
using UI;
using UnityEngine;

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

        private void OnPositive(int arg0)
        {
            StencilRateHelpers.RecordRating(arg0);
            Settings.GoToRateUrl();
            Rater.Dismiss();
        }

        private void OnNegative(int arg0)
        {
            StencilRateHelpers.RecordRating(arg0);
            Rater.AskForFeedback();
        }

        private void OnNever()
        {
            StencilRateHelpers.Reject();
        }
    }
}