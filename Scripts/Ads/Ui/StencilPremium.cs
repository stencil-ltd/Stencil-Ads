using System;
using UnityEngine;
using Util;

namespace Ads.Ui
{
    public static class StencilPremium
    {
        public static bool HasPremium
        {
            get { return PlayerPrefsX.GetBool("stencil_premium"); }
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
            OnPremiumPurchased?.Invoke();
        }
    }
}