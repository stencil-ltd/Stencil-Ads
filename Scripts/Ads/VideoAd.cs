using System;
using System.Collections;
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
        public event EventHandler OnClose; // these will be the same for some ad types.
        public event EventHandler<bool> OnResult; // single callback for close or rewarded.
        public event EventHandler<VideoAdState> OnState;

        public bool HasError { get; private set; }
        public bool IsLoading { get; private set; }
        public bool IsShowing { get; private set; }

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

        public VideoAd(PlatformValue<string> unitId)
        {
            UnitId = unitId;
        }

        public virtual void Init()
        {
            Load();
            OnClose += (sender, args) => Objects.StartCoroutine(_OnClose());
        }

        public void Refresh()
        {
            if (!IsReady && !IsLoading) Load();
        }

        private IEnumerator _OnClose()
        {
            IsShowing = false;
            yield return null;
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
            OnState?.Invoke(this, State);
            ShowInternal();
            return true;
        }

        public virtual bool SupportsEditor => true;
        public abstract bool IsReady { get; }
        public abstract bool ShowInPremium { get; }

        protected abstract void ShowInternal();
        protected abstract void LoadInternal();

        public void Load()
        {
            IsLoading = true;
            OnState?.Invoke(this, State);
            LoadInternal();
        }
        
        public void CheckReload()
        {
            if (!HasError) return;
            Debug.Log($"{GetType()} error, reload.");
            Load();
        }

        protected void NotifyLoad()
        {
            IsLoading = false;
            HasError = false;
            OnLoaded?.Invoke();
            OnState?.Invoke(this, State);
        }

        protected void NotifyError()
        {
            IsShowing = false;
            IsLoading = false;
            HasError = true;
            Tracking.Instance.Track("ad_failed", "type", GetType().Name);
            OnError?.Invoke();
            OnState?.Invoke(this, State);
        }

        protected void NotifyComplete()
        {
            IsShowing = false;
            OnComplete?.Invoke();
            OnResult?.Invoke(null, true);
            OnState?.Invoke(this, State);
        }

        protected void NotifyClose()
        {
            IsShowing = false;
            OnClose?.Invoke();
            OnResult?.Invoke(null, false);
            OnState?.Invoke(this, State);
        }

        private IEnumerator FakeShow(bool editor)
        {
            var str = editor ? "editor" : "premium";
            Debug.LogWarning($"Ad doesn't support {str}. Completing!");
            IsShowing = true;
            OnState?.Invoke(this, State);
            if (editor) yield return new WaitForSeconds(1f);
            NotifyComplete();
        }

    }
}