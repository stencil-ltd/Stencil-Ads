using UnityEngine;
using UnityEngine.UI;

namespace Ratings
{
    [RequireComponent(typeof(Button))]
    public class RateButton : MonoBehaviour
    {
        public bool SkipDialog;
        public bool Review;
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if (SkipDialog)
                    if (Review)
                        RateSettings.Instance.Config.Review();
                    else
                        RateSettings.Instance.Config.Rate();
                else
                    StencilRateController.Instance.ForceShow();
            });
        }
    }
}