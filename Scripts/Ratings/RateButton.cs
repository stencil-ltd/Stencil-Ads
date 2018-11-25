using UnityEngine;
using UnityEngine.UI;

namespace Ratings
{
    [RequireComponent(typeof(Button))]
    public class RateButton : MonoBehaviour
    {
        public bool SkipDialog;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if (SkipDialog)
                    StencilRateController.Instance.GoToRating();
                else
                    StencilRateController.Instance.ForceShow();
            });
        }
    }
}