using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ads.Ui
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(CanvasGroup))]
    public class AdButton : MonoBehaviour
    {
        public AdType AdType = AdType.Rewarded;
        public AdEvent OnResult;

        private VideoAd _ad;
        private Button _button;
        private CanvasGroup _cg;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _cg = GetComponent<CanvasGroup>();
            _ad = StencilAds.GetAdByType(AdType);
            _button.onClick.AddListener(() =>
            {
                _ad.ShowOnResult(b => OnResult?.Invoke(b));
            });
        }

        private void OnEnable()
        {
            _ad?.CheckReload();
        }

        private void Update()
        {
            _button.enabled = _ad?.IsReady ?? false;
            _cg.alpha = _button.enabled ? 1f : 0.3f;
        }
    }

    [Serializable]
    public class AdEvent : UnityEvent<bool>
    {}
}