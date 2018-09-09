using UnityEngine;
using UnityEngine.UI;

namespace Ads.Ui
{
    [RequireComponent(typeof(Button))]
    public class AdTestButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(StencilAds.ShowTestSuite);
        }
    }
}