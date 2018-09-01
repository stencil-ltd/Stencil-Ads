using UnityEngine;

namespace Ads.Promo.Data
{
    [CreateAssetMenu(menuName = "Stencil/Promo Manifest")]
    public class PromoManifest : ScriptableObject
    {
        public PromoAsset[] Promos;
    }
}