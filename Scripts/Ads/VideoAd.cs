using System;
using System.Collections;
using System.Security.Cryptography;
using Ads.State;
using Ads.Ui;
using Analytics;
using UnityEngine;
using Util;

namespace Ads
{
    public abstract class VideoAd
    {
        public readonly PlatformValue<string> UnitId;

        public event EventHandler OnLoaded;
        public event EventHandler OnError;
        public event EventHandler OnComplete;
        public event EventHandler<bool> OnResult; // single callback for close or rewarded.
        public event EventHandler<VideoAdState> OnState;

        public bool HasError { get; private set; }
        public bool IsLoading { get; private set; }
        public bool IsShowing { get; private set; }

        public bool IsReady
        {
            get 
            {
                if (!SupportsEditor && Application.isEditor)
                    return true;
                return PlatformIsReady;
            }
        }

        public VideoAdState State
        {
            get
            {
                if (IsShowing) return VideoAdState.Showing;
                if (HasError) return VideoAdState.Error;
                if (IsLoading) return VideoAdState.Loading;
                return VideoAdState.None;
            }
        }
        
        public abstract AdType AdType { get; }

        public VideoAd(PlatformValue<string> unitId)
        {
            UnitId = unitId;
        }

        public virtual void Init()
        {
            Load();
        }

        public bool Show()
        {
            if (!IsReady)
            {
                Debug.LogWarning("Video ad not ready. Returning.");
                return false;
            }
            
            if (Application.isEditor && !SupportsEditor)
            {
                Objects.StartCoroutine(FakeShow(true));
                return true;
            }

            if (StencilPremium.HasPremium && !ShowInPremium)
            {
                Objects.StartCoroutine(FakeShow(false));
                return true;
            }

            IsShowing = true;
            NotifyState();
            ShowInternal();
            return true;
        }

        public virtual bool SupportsEditor => true;
        protected abstract bool PlatformIsReady { get; }
        public abstract bool ShowInPremium { get; }

        protected abstract void ShowInternal();
        protected abstract void LoadInternal();

        public void Load()
        {
            HasError = false;
            IsLoading = true;
            NotifyState();
            LoadInternal();
        }
        
        public void CheckReload()
        {
            if (!HasError) return;
            Debug.Log($"{AdType} error, reload.");
            Load();
        }

        protected void NotifyLoad()
        {
            IsLoading = false;
            HasError = false;
            OnLoaded?.Invoke();
            NotifyState();
        }

        protected void NotifyError()
        {
            IsShowing = false;
            IsLoading = false;
            HasError = true;
            Tracking.Instance.Track("ad_failed", "type", AdType);
            OnError?.Invoke();
            NotifyState();
        }

        protected void NotifyComplete(bool success)
        {
            IsShowing = false;
            OnComplete?.Invoke();
            OnResult?.Invoke(null, true);
            NotifyState();
            Load();
        }

        private IEnumerator FakeShow(bool editor)
        {
            var str = editor ? "editor" : "premium";
            Debug.LogWarning($"Ad doesn't support {str}. Completing!");
            IsShowing = true;
            NotifyState();
            if (editor) yield return new WaitForSeconds(3f);
            NotifyComplete(true);
        }

        public void EmergencyReset()
        {
            var type = AdType;
            Tracking.Instance.Track("ad_reset_problem", "type", type);
            Tracking.Report("ad_reset_problem", $"Had to reset {type}");
            NotifyComplete(true);
        }

        private void NotifyState()
        {
            Debug.Log($"{AdType} has entered state {State} (error={HasError}, loading={IsLoading}, showing={IsShowing})");
            OnState?.Invoke(this, State);
        }
    }
}