using UnityEngine;

namespace Ads.Promo.Data
{
    [CreateAssetMenu(menuName = "Stencil/Promo Genre")]
    public class CrossPromoGenre : ScriptableObject
    {
        public string Id;
        public Sprite Tag;
    }
}