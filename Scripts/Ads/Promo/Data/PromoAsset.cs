using System;
using Analytics;
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
        public Videos videos;
        public Sprite outro;

        [Tooltip("Check out https://analytics.itunes.apple.com/#/campaigngenerator")]
        public AppStoreInfo AppStore;

        public bool HasSeen
        {
            get { return PlayerPrefsX.GetBool($"promo-seen-{id}"); }
            set
            {
                PlayerPrefsX.SetBool($"promo-seen-{id}", value);
                if (value)
                    Tracking.Instance
                        .Track($"promo-view-{id}")
                        .Track($"promo-view", "id", id)
                        .SetUserProperty($"promo-seen-{id}", true);
            }
        }

        public DateTime? LastClick
        {
            get { return PlayerPrefsX.GetDateTime($"promo-last-click-{id}"); }
            set
            {
                PlayerPrefsX.SetDateTime($"promo-last-click-{id}", value);
                if (value != null)
                    Tracking.Instance
                        .Track($"promo-click-{id}", "from", Application.identifier)
                        .Track($"promo-click", "id", id, "from", Application.identifier)
                        .SetUserProperty($"promo-clicked-{id}", true);
            }
        }
    }

    [Serializable]
    public class AppStoreInfo
    {
        public uint Id;
    }
}