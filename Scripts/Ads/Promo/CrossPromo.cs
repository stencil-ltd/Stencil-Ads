using System;
using System.Collections;
using Ads.Promo.Data;
using Ads.Ui;
using Analytics;
using Binding;
using Dev;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Util;
using Random = UnityEngine.Random;

namespace Ads.Promo
{
    [RequireComponent(typeof(VideoPlayer))]
    public class CrossPromo : MonoBehaviour
    {
        public static int LastPromoIndex
        {
            get { return PlayerPrefs.GetInt("x-promo-last-index", -1); }
            set { PlayerPrefs.SetInt("x-promo-last-index", value); }
        }

        public static bool HasLaunched
        {
            get { return PlayerPrefsX.GetBool("x-promo-seen"); }
            set { PlayerPrefsX.SetBool("x-promo-seen", value);}
        }
        
        public string MetadataUrl;
        
        public VideoSize Size = VideoSize.Video480;
        public bool SkipCaching;
        public bool SkipOnFirstLaunch = true;

        public Image Tag;
        public RawImage Render;
        public Image Outro;
        public GameObject Content;
        public GameObject New;
        public Button Exit;

        public UnityEvent OnAdStart;
        public UnityEvent OnAdFail;
        public UnityEvent OnAdExit;
       
        [Bind]
        public VideoPlayer Player { get; private set; }

        [Header("Debug Info")]
        public PromoManifest Manifest;
        public PromoAsset Promo;// => _manifest?.Promos[_index];
        
        private PromoMetadata _meta;

        private int _index = -1;
        private bool _failed;
        private bool _clicked;
        private bool _started;
        private bool _destroyed;
        private bool _finished;

        private bool CanContinue(CrossPromo promo)
        {
            if (!HasLaunched && SkipOnFirstLaunch)
            {
                HasLaunched = true;
                return false;
            }
            HasLaunched = true;
            if (StencilPremium.HasPremium) return false;
            return promo != null && !promo._destroyed && !promo._failed;
        }

        public void OnClick()
        {
            StartCoroutine(_OnClick());
        }

        private IEnumerator _OnClick()
        {
            _clicked = true;
            Promo.LastClick = DateTime.UtcNow;
            var url = GetDownloadUrl();
            Debug.Log($"Open {url}");
            Application.OpenURL(url);
            yield return null;
            DoExit();
        }

        private void Awake()
        {
            this.Bind();
            Caching.compressionEnabled = false;
            Player.prepareCompleted += OnPrepare;
            Player.errorReceived += OnError;
            Player.loopPointReached += OnLoop;
            Player.playOnAwake = false;
            Content.SetActive(false);
            Outro.gameObject.SetActive(false);
            Exit.onClick.AddListener(DoExit);
            new GameObject("Main Thread").AddComponent<UnityMainThreadDispatcher>();
        }

        private IEnumerator Start()
        {
            if (!Developers.Enabled || Promo == null)
            {
                yield return StartCoroutine(LoadManifest());
                if (Manifest == null) yield break;
                if (!CanContinue(this)) yield break;
                if (!SelectPromo()) yield break;
            }

            var genre = Promo?.genre;
            Tag.gameObject.SetActive(genre != null);
            Tag.sprite = genre?.Tag;
            
            LastPromoIndex = _index;
            var video = Promo.videos.GetForSize(Size);
            Debug.Log($"Moving forward with promo {Promo.id} -> {video}");
            Player.isLooping = Promo.outro == null;
            Player.clip = video;
            var res = Size.GetResolution();
            Player.targetTexture = new RenderTexture(res.x, res.y, 24);
            Render.texture = Player.targetTexture;
            New.SetActive(!Promo.HasSeen);
            Player.Play();
            Promo.HasSeen = true;
            PlayerPrefs.Save();
        }

        private bool SelectPromo()
        {
            var count = 0;
            bool canUse;
            _index = LastPromoIndex;
            do
            {
                _index = (_index + 1) % Manifest.Promos.Length;
                canUse = CanUsePromo();
                ++count;
            } while (!canUse && count < Manifest.Promos.Length);

            if (!canUse)
                return false;
            Promo = Manifest.Promos[_index];
            return true;
        }

        private void OnDestroy()
        {
            _destroyed = true;
        }

        private void DoExit()
        {
            PlayerPrefs.Save();
            OnAdExit?.Invoke();
        }

        private bool CanUsePromo()
        {
            if (Manifest.Promos[_index].id == Application.identifier) return false;
            if (Manifest.Promos[_index].LastClick != null) return false;
            return true;
        }

        private IEnumerator LoadManifest()
        {
            yield return GetMetadata();
            if (!CanContinue(this))
                yield break;

            var url = _meta.downloads.GetBasePlatformUrl();
            Debug.Log($"Fetching from {url} [useCache = {!SkipCaching}]");
            var request = UnityWebRequestAssetBundle.GetAssetBundle(url, (uint) _meta.version, 0);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError($"Could not load manifest: {request.error}");
                Fail();
                yield break;
            }

            var bundle = DownloadHandlerAssetBundle.GetContent(request);
            Manifest = bundle.LoadAsset<PromoManifest>("Manifest");
        }

        private void OnPrepare(VideoPlayer source)
        {
            Content.SetActive(true);
            if (!_started)
            {
                _started = true;
                OnAdStart?.Invoke();
            }
        }

        private void OnLoop(VideoPlayer source)
        {
            Debug.LogWarning("Finished.");
            _finished = true;
            if (Promo.outro == null)
                return;
            
            Outro.sprite = Promo.outro;
            Outro.gameObject.SetActive(true);
            var c = Outro.color;
            c.a = 0f;
            Outro.color = c;
            LeanTween.alpha(Render.GetComponent<RectTransform>(), 0f, 0.5f);
            LeanTween.alpha(Outro.GetComponent<RectTransform>(), 1f, 0.5f);
        }

        private void Fail()
        {
            if (this == null || _destroyed || _failed) return;
            _failed = true;
            OnAdFail?.Invoke();
        }

        private void OnError(VideoPlayer source, string message)
        {
            Fail();
        }
        
        private IEnumerator GetMetadata()
        {
            var req = UnityWebRequest.Get(MetadataUrl);
            yield return req.SendWebRequest();
            if (!string.IsNullOrEmpty(req.error))
            {
                Fail();
                yield break;
            }

            var json = req.downloadHandler.text;
            Debug.Log($"Received metadata: {json}");
            _meta = JsonUtility.FromJson<PromoMetadata>(json);
            if (Developers.Enabled && SkipCaching)
                _meta.version = Random.Range(0, int.MaxValue);
        }

        private string GetDownloadUrl()
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.Android)
                return CrossPromoExtensions.AndroidUrl(Promo.id, "stencil-ltd");
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return CrossPromoExtensions.ItunesUrl(""+Promo.AppStore.Id, _meta.appStore.provider);
            return null;
        }
    }
}