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
                    RateSettings.Instance.Config.Rate();
                else
                    StencilRateController.Instance.ForceShow();
            });
        }
    }
}