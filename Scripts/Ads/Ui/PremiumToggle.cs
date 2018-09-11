using Dev;
using UnityEngine;
using UnityEngine.UI;

namespace Ads.Ui
{
    [RequireComponent(typeof(Button))]
    public class PremiumToggle : MonoBehaviour
    {
        public static bool? ForceEnabled;
//        {
//            get
//            {
//                if (!Developers.Enabled) return null;
//                return PlayerPrefsX.GetBoolNullable("premium_force");
//            }
//            set
//            {
//                PlayerPrefsX.SetBoolNullable("premium_force", value);
//                PlayerPrefs.Save();
//            }
//        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!Developers.Enabled)
                {
                    ForceEnabled = null;
                    return;
                }
                var force = ForceEnabled;
                if (force == null) ForceEnabled = !StencilPremium.HasPremium;
                else ForceEnabled = !force;
                StencilPremium.NotifyPurchase();
            });
        }
    }
}