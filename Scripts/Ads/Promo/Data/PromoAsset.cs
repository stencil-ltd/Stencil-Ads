using System;
using UnityEngine;

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
        public DownloadUrls downloads;
    }
}