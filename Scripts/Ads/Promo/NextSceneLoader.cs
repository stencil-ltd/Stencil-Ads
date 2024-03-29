using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ads.Promo
{
    public class NextSceneLoader : Controller<NextSceneLoader>
    {
        public float LoadTime = 2f;
        public int CustomSceneIndex = -1;
        
        public bool IsLoading { get; private set; }
        public event EventHandler<bool> OnLoading;

        private AsyncOperation _scene;
        private int _currentIndex;
        private bool _paused;

        public void Pause()
        {
            _paused = true;
        }

        private void Awake()
        {
            _currentIndex = SceneManager.GetActiveScene().buildIndex;
            IsLoading = true;
            OnLoading?.Invoke(this, IsLoading);
        }

        private IEnumerator Start()
        {
            yield return null;
            int index = CustomSceneIndex + 1;
            if (index <= 0)
                index = _currentIndex + 1;
            _scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
//            _scene.allowSceneActivation = false;
        
            yield return new WaitForSeconds(LoadTime);
            if (!_paused)
                ForceContinue();
        }
        
        public void ForceContinue()
        {
            _scene.allowSceneActivation = true;
            if (_scene.isDone)
                StartCoroutine(GameReady());
            else
                _scene.completed += operation => StartCoroutine(GameReady());
        }
        
        private IEnumerator GameReady()
        {
            yield return null;
            SceneManager.UnloadSceneAsync(_currentIndex);
            IsLoading = false;
            OnLoading?.Invoke(this, IsLoading);
        }
    }
}