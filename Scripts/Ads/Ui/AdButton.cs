using System;
using Currencies;
using JetBrains.Annotations;
//using Currencies;
//using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ads.Ui
{
    public class AdButton : MonoBehaviour
    {
        [Header("Data")]
        public AdType AdType = AdType.Rewarded;
        
        [Header("UI")]
        public Button Button;
        public CanvasGroup CanvasGroup;
        
        [Header("Events")]
        public AdEvent OnResult;

        [CanBeNull] public Price Reward;

        private VideoAd _ad;

        private void Awake()
        {
            if (Button == null)
                Button = GetComponent<Button>();
            if (CanvasGroup == null)
                CanvasGroup = GetComponent<CanvasGroup>();
            
            _ad = StencilAds.GetAdByType(AdType);
            Button.onClick.AddListener(() =>
            {
                _ad.ShowOnResult(_OnResult);
            });
        }

        private void _OnResult(bool obj)
        {
            OnResult?.Invoke(obj);
            if (obj && Reward.Currency != null) 
                Reward.Receive().AndSave();
        }

        private void OnEnable()
        {
            _ad?.CheckReload();
        }

        private void Update()
        {
            Button.enabled = _ad?.IsReady ?? false;
            CanvasGroup.alpha = Button.enabled ? 1f : 0.3f;
        }
    }

    [Serializable]
    public class AdEvent : UnityEvent<bool>
    {}
}