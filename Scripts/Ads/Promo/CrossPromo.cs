using System;
using System.Collections;
using System.Linq;
using System.Text;
using Analytics;
using Binding;
using Dev;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using Util;

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

        [CanBeNull]
        public static string CachedManifest
        {
            get { return PlayerPrefs.GetString("x-promo-manifest", null); }
            set { PlayerPrefs.SetString("x-promo-manifest", value); }
        }

        public static DateTime? CacheDate
        {
            get { return PlayerPrefsX.GetDateTime("x-promo-manifest-date"); }
            set { PlayerPrefsX.SetDateTime("x-promo-manifest-date", value); }
        }

        public string ManifestUrl;
        public VideoSize Size = VideoSize.Video480;

        public Image Tag;
        public RawImage Render;
        public GameObject Content;
        public GameObject New;

        public CrossPromoGenre[] SupportedGenres;

        public UnityEvent OnAdStart;
        public UnityEvent OnAdFinish;
        public UnityEvent OnAdFail;
        
        [Header("Manifest")]
        public int HoursExpiration = 24;
        [TextArea]
        public string DebugManifest; 
        public float DebugLoadTime = 0.5f;
        
        [Bind]
        public VideoPlayer Player { get; private set; }

        private Promo _promo => _manifest.promos[_index];
        private PromoManifest _manifest;
        private int _index = -1;
        private bool _failed;
        private bool _clicked;
        private bool _started;

        public void OnClick()
        {
            _clicked = true;
            Tracking.Instance.Track($"promo_click_{_promo.id}", "from", Application.identifier);
            Tracking.Instance.SetUserProperty($"promo_clicked_{_promo.id}", true);
            var url = _promo.downloads.GetPlatformUrl();
            Debug.Log($"Open {url}");
            Application.OpenURL(url);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // TODO.
        }

        private void Awake()
        {
            this.Bind();
            Player.prepareCompleted += OnPrepare;
            Player.errorReceived += OnError;
            Player.loopPointReached += OnLoop;
            Player.playOnAwake = false;
            Content.SetActive(false);
            new GameObject("Main Thread").AddComponent<UnityMainThreadDispatcher>();
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(LoadManifest());
            if (_failed) yield break;
            if (_manifest == null)
            {
                Fail();
                yield break;
            }

            int? start = null;
            bool repeat; 
            
            _index = LastPromoIndex;
            do
            {
                _index = (_index + 1) % _manifest.promos.Length;
                repeat = _index == start;
                start = start ?? _index;
            } while (!CanUsePromo() && !repeat);

            var genre = GetGenre();
            Tag.gameObject.SetActive(genre != null);
            Tag.sprite = genre?.Tag;
            
            LastPromoIndex = _index;
            var video = _promo.videos.GetForSize(Size);
            Debug.Log($"Moving forward with promo {_promo.id} -> {video}");
            Player.url = video;
            var res = Size.GetResolution();
            Player.targetTexture = new RenderTexture(res.x, res.y, 24);
            Render.texture = Player.targetTexture;
            New.SetActive(_promo.isNew);
            Player.Play();
            PlayerPrefs.Save();
        }

        private bool CanUsePromo()
        {
            if (_promo.id == Application.identifier) return false;
            if (!_promo.enabled) return false;
//            if (GetPromoType() == null) return false;
            return true;
        }

        private CrossPromoGenre GetGenre()
        {
            return SupportedGenres.FirstOrDefault(genre => genre.Id == _promo.genre);
        }

        private IEnumerator LoadManifest()
        {
            if (Developers.Enabled && !string.IsNullOrEmpty(DebugManifest))
            {
                Debug.Log($"Loading debug manifest after {DebugLoadTime}");
                yield return new WaitForSeconds(DebugLoadTime);
                _manifest = Get(DebugManifest);
                yield break;
            }

            // Cache invalidation.
            var cached = CachedManifest;
            if (!string.IsNullOrEmpty(cached))
            {
                var expire = CacheDate + TimeSpan.FromHours(HoursExpiration);
                if (expire < DateTime.UtcNow)
                    cached = null;
            }
            
            if (!string.IsNullOrEmpty(cached))
            {
                Debug.Log($"Found cached manifest.");
                _manifest = Get(cached);
            } else using (var www = new WWW(ManifestUrl))
            {
                Debug.Log($"Loading manifest from {ManifestUrl}");
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    Fail();
                    yield break;
                }
                
                Debug.Log($"Found manifest!");
                var str = Encoding.UTF8.GetString(www.bytes);
                _manifest = Get(str);
                if (_manifest != null)
                {
                    CachedManifest = str;
                    CacheDate = DateTime.UtcNow;   
                }
            }

            if (_manifest == null)
            {
                Debug.LogError("Could not load manifest!");
                Fail();
            }
            else
            {
                Debug.Log($"Loaded manifest with {_manifest.promos.Length} entries.");
            }
        }

        private static PromoManifest Get(string json)
        {
            try
            {
                return JsonUtility.FromJson<PromoManifest>(json);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
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
            OnAdFinish?.Invoke();
        }

        private void Fail()
        {
            if (_failed) return;
            _failed = true;
            OnAdFail?.Invoke();
        }

        private void OnError(VideoPlayer source, string message)
        {
            Fail();
        }
    }
}