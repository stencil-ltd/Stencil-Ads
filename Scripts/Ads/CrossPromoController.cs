using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Ads
{
    public class CrossPromoController : MonoBehaviour
    {
        public float LoadTime = 2f;
        public AdSettings Settings; // prevent build-time culling.
        private AsyncOperation _scene;

        private void Awake()
        {
            StencilAds.InitHouse();
            StencilAds.House.OnError += OnError;
            StencilAds.House.OnComplete += OnComplete;
            new GameObject("Main Thread").AddComponent<UnityMainThreadDispatcher>();
        }

        private IEnumerator Start()
        {
            yield return null;
            _scene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            _scene.allowSceneActivation = false;
        
            yield return new WaitForSeconds(LoadTime);
            if (StencilAds.House.IsReady)
            {
                Debug.Log("Found House Ad. Showing.");
                StencilAds.House.Show();
            }
            else
            {
                Debug.Log("No House Ads. Moving On.");
                Continue();
            }
        }

        private void OnDestroy()
        {
            StencilAds.House.OnError -= OnError;
            StencilAds.House.OnComplete -= OnComplete;
        }

        private void Continue()
        {
            _scene.allowSceneActivation = true;
            if (_scene.isDone)
                SceneManager.UnloadSceneAsync("Startup");
            else
                _scene.completed += operation => SceneManager.UnloadSceneAsync("Startup");
        }

        private void OnComplete(object sender, EventArgs e)
        {
            Debug.Log("Loading Game...");
            Continue();
        }

        private void OnError(object sender, EventArgs e)
        {
            Debug.LogWarning("House Ad Error");
            Continue();
        }
    }
}