using System.Collections;
using UnityEngine;

namespace Ads.Ui
{
    public class AdLoadingSpinner : MonoBehaviour
    {
        public float DismissableAfter = 2f;
        private bool _dismissable;

        public GameObject DismissableUi;

        private Coroutine _coroutine;
        
        private void OnEnable()
        {
            _coroutine = StartCoroutine(_DismissTimer());
        }

        private void OnDisable()
        {
            _dismissable = false;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            RefreshUi();
        }

        private IEnumerator _DismissTimer()
        {
            _dismissable = false;
            yield return new WaitForSeconds(DismissableAfter);
            _dismissable = true;
        }

        public void Click_Dismiss()
        {
            if (!_dismissable) return;
            _dismissable = false;
            if (StencilAds.Interstitial.IsShowing)
                ResetAd(StencilAds.Interstitial);
            if (StencilAds.Rewarded.IsShowing)
                ResetAd(StencilAds.Rewarded);
        }

        private void ResetAd(VideoAd ad)
        {
            ad.EmergencyReset();
        }

        private void Update()
        {
            RefreshUi();
        }

        private void RefreshUi()
        {
            DismissableUi?.SetActive(_dismissable);
        }
    }
}