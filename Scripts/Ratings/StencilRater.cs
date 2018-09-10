using System;
using Analytics;
using Binding;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ratings
{
    public class StencilRater : MonoBehaviour
    {
        [Serializable]
        public class StarEvent : UnityEvent<int>
        {}

        public int StarsForPositive = 4;
        public int BeginWithStars = 3;
        public RateStar[] Stars;

        [Header("Initial UI")]
        public GameObject Initial;
        public Button Cancel;
        public Button Never;
        
        [Header("Feedback UI")]
        public GameObject Why;
        public Button CancelFeedback;
        public Button SendFeedback;
        
        public StarEvent OnPositive;
        public StarEvent OnNegative;
        public UnityEvent OnNever;
        public UnityEvent OnCancel;

        private bool _rated;

        public void AskForFeedback()
        {
            Initial.SetActive(false);
            Why.SetActive(true);
        }

        private void Awake()
        {
            this.Bind();
            
            Cancel.onClick.AddListener(() => CancelRating(false));
            Never.onClick.AddListener(NeverRating);
            
            CancelFeedback.onClick.AddListener(() => CancelRating(true));
            SendFeedback.onClick.AddListener(Feedback);
        }

        private void OnEnable()
        {
            Debug.Log("Showing Rating");

            _rated = false;
            for (var i = 0; i < Stars.Length; i++)
            {
                var i1 = i;
                var star = Stars[i];
                star.Index = i;
                star.Button.onClick.AddListener(() => Rate(i1+1));
                star.Fill.enabled = i < BeginWithStars;
            }
            
            Initial.SetActive(true);
            Why.SetActive(false);
            
            Tracking.Instance.Track("rating_show")
                .SetUserProperty("rating_shown", true);
        }

        private void Feedback()
        {
            Tracking.Instance.Track("rating_feedback")
                .SetUserProperty("rating_feedback", true);
            var to = "contact@stencil.ltd";
            var subject = $"Feedback for {Application.productName}";
            var url = $"mailto:{to}?subject={Uri.EscapeDataString(subject)}";
            Debug.Log($"Opening Email: {url}");
            Application.OpenURL(url);
            Dismiss();
        }

        private void NeverRating()
        {
            Tracking.Instance.Track("rating_never")
                .SetUserProperty("rating_never", true);
            Dismiss();
            OnNever?.Invoke();
        }

        public void CancelRating(bool feedback)
        {
            var suffix = feedback ? "_feedback" : "";
            Tracking.Instance.Track("rating_cancel"+suffix)
                .SetUserProperty("rating_cancel"+suffix, true);
            Dismiss();
            OnCancel?.Invoke();
        }

        private void Rate(int count)
        {
            if (_rated) return;
            _rated = true;
            var positive = count >= StarsForPositive;
            var posString = positive ? "rating_positive" : "rating_negative"; 
            Tracking.Instance.Track("rating_give", "stars", count)
                .Track($"rating_{count}_stars")
                .Track(posString)
                .SetUserProperty("rating", count)
                .SetUserProperty(posString, true);
            
            for (var i = 0; i < Stars.Length; i++)
                Stars[i].Fill.enabled = i < count;

            if (positive)
                OnPositive?.Invoke(count);
            else
                OnNegative?.Invoke(count);
        }

        public void Dismiss()
        {
            gameObject.SetActive(false);
        }
    }
}