using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Ads.Ui
{
    public class AdButtonOptimist : MonoBehaviour
    {
        [Header("Data")]
        public AdType adType = AdType.Rewarded;
        public float timeout = 5f;
        
        [Header("UI")]
        public Button button;
        public GameObject[] loading = {};
        public GameObject[] ready = {};
        public GameObject[] error = {};
        
        [Header("Events")]
        public AdEvent onResult;
        
        private VideoAd _ad;
        
        private bool _loading;
        private bool _error;

        private void Awake()
        {
            button = button ?? GetComponent<Button>();
            _ad = StencilAds.GetAdByType(adType);
            button.onClick.AddListener(() => StartCoroutine(_ShowAd()));
        }

        private void Start()
        {
            RefreshUi();
        }

        private IEnumerator _ShowAd()
        {
            button.enabled = false;
            _error = false;
            
            var now = DateTime.UtcNow;
            while (!_ad.IsReady && (DateTime.UtcNow - now).TotalSeconds < timeout)
            {
                _loading = true;
                RefreshUi();
                _ad.CheckReload();
                yield return new WaitForSeconds(0.5f);
            }

            _loading = false;
            _error = !_ad.IsReady;
            RefreshUi();

            if (!_error) _ad.ShowOnResult(_OnResult);
            button.enabled = true;
        }

        private void _OnResult(bool obj)
        {
            onResult?.Invoke(obj);
        }
        
        private void OnEnable()
        {
            _ad?.CheckReload();
        }

        private void RefreshUi()
        {
            foreach (var o in loading)
                if (o != null) o.SetActive(_loading);
            foreach (var o in ready)
                if (o != null) o.SetActive(!_loading);
            foreach (var o in error)
                if (o != null) o.SetActive(_error);
        }
    }
}