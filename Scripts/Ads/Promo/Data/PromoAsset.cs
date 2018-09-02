using System;
using UnityEngine;
using Util;

namespace Ads.Promo.Data
{
    [Serializable]
    [CreateAssetMenu(menuName = "Stencil/Promo Asset")]
    public class PromoAsset : ScriptableObject
    {
        public string id;
        public string name;
        public CrossPromoGenre genre;
        public VideoUrls videos;

        [Tooltip("Check out https://analytics.itunes.apple.com/#/campaigngenerator")]
        public AppStoreInfo AppStore;
            
        public bool IsNew()
        {
            return !PlayerPrefsX.GetBool($"promo-seen-{id}");
        }

        public void SetSeen()
        {
            PlayerPrefsX.SetBool($"promo-seen{id}", true);
        }
    }

    [Serializable]
    public class AppStoreInfo
    {
        public uint Id;
    }
}