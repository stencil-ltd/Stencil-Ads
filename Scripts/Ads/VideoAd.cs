using System;
using System.Collections;
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

        public bool HasError { get; private set; }
        public bool IsLoading { get; private set; }

        public VideoAd(PlatformValue<string> unitId)
        {
            UnitId = unitId;
        }

        public virtual void Init()
        {
            Load();
            OnClose += (sender, args) => Objects.StartCoroutine(_OnClose());
            OnError += (sender, args) => Objects.StartCoroutine(HandleError(args));
        }

        public void Refresh()
        {
            if (!IsReady && !IsLoading) Load();
        }

        private IEnumerator _OnClose()
        {
            yield return null;
            Load();
        }

        public void Show()
        {
            if (Application.isEditor && !SupportsEditor)
            {
                Objects.StartCoroutine(FakeShow(true));
                return;
            }

            if (StencilPremium.HasPremium && !ShowInPremium)
            {
                Objects.StartCoroutine(FakeShow(false));
                return;
            }
            
            ShowInternal();
        }

        public virtual bool SupportsEditor => true;
        public abstract bool IsReady { get; }
        public abstract bool ShowInPremium { get; }

        protected abstract void ShowInternal();
        protected abstract void LoadInternal();

        public void Load()
        {
            IsLoading = true;
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
        }

        protected void NotifyError()
        {
            IsLoading = false;
            HasError = true;
            OnError?.Invoke();
        }

        protected void NotifyComplete()
        {
            OnComplete?.Invoke();
            OnResult?.Invoke(null, true);
        }

        protected void NotifyClose()
        {
            OnClose?.Invoke();
            OnResult?.Invoke(null, false);
        }

        private IEnumerator HandleError(EventArgs args)
        {
            Tracking.Instance.Track("ad_failed", "type", GetType().Name);
            yield return null;
        }

        private IEnumerator FakeShow(bool delay)
        {
            Debug.LogWarning("Ad doesn't support editor. Completing!");
            if (delay)
                yield return new WaitForSeconds(0.3f);
            NotifyComplete();
        }

    }
}