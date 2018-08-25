using System;
using System.Collections;
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
            OnClose += (sender, args) => Load();
            OnError += (sender, args) => Objects.StartCoroutine(HandleError(args));
        }

        public void Refresh()
        {
            if (!IsReady && !IsLoading) Load();
        }

        public void Show()
        {
            if (Application.isEditor && !SupportsEditor)
            {
                Objects.StartCoroutine(FakeShow());
                return;
            }
            ShowInternal();
        }

        public virtual bool SupportsEditor => true;
        public abstract bool IsReady { get; }

        protected abstract void ShowInternal();
        protected abstract void LoadInternal();

        public void Load()
        {
            IsLoading = true;
            LoadInternal();
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

        private IEnumerator FakeShow()
        {
            Debug.LogWarning("Ad doesn't support editor. Completing!");
            yield return new WaitForSeconds(0.3f);
            NotifyComplete();
        }

    }
}