using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Ads.Promo
{
    public class NextSceneLoader : MonoBehaviour
    {
        public float LoadTime = 2f;
        public int CustomSceneIndex = -1;
        
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
        }

        private IEnumerator Start()
        {
            yield return null;
            int index = CustomSceneIndex + 1;
            if (index <= 0)
                index = _currentIndex + 1;
            _scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            _scene.allowSceneActivation = false;
        
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
        }
    }
}