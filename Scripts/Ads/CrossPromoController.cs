using System;
using System.Collections;
using Ads.Promo;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Ads
{
    public class CrossPromoController : MonoBehaviour
    {
        public NextSceneLoader Loader;
        private AsyncOperation _scene;

        private void Awake()
        {
            StencilAds.InitHouse();
            StencilAds.House.OnError += OnError;
            StencilAds.House.OnComplete += OnComplete;
            StencilAds.House.OnLoaded += OnReady;
            new GameObject("Main Thread").AddComponent<UnityMainThreadDispatcher>();
        }

        private void OnReady(object sender, EventArgs e)
        {
            Loader.Pause();
            StencilAds.House.Show();
        }

        private void OnDestroy()
        {
            StencilAds.House.OnError -= OnError;
            StencilAds.House.OnComplete -= OnComplete;
            StencilAds.House.OnLoaded -= OnReady;
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

        private void Continue()
        {
            Loader.ForceContinue();
        }
    }
}