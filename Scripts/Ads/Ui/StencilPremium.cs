using System;
using Scripts.RemoteConfig;
using UnityEngine;
using Util;
using Developers = Dev.Developers;

namespace Ads.Ui
{
    public static class StencilPremium
    {
        public static bool IgnorePremium 
            => StencilRemote.IsDeveloper() && AdSettings.Instance.IgnorePremium;
        
        public static bool HasPremium
        {
            get
            {
                var force = PremiumToggle.ForceEnabled;
                if (force != null) return force.Value;
                if (IgnorePremium) return false;
                return PlayerPrefsX.GetBool("stencil_premium");
            }
            private set
            {
                PlayerPrefsX.SetBool("stencil_premium", value);
                PlayerPrefs.Save();
            }
        }
        public static event EventHandler OnPremiumPurchased;

        public static void Purchase()
        {
            Debug.Log("Premium approved");
            HasPremium = true;
            PremiumToggle.ForceEnabled = null;
            NotifyPurchase();
        }

        public static void NotifyPurchase()
        {
            OnPremiumPurchased?.Invoke();
        }
    }
}