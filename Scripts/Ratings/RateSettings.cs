using UnityEngine;
using Util;

namespace Ratings
{
    [CreateAssetMenu(menuName = "Stencil/Ratings")]
    public class RateSettings : Singleton<RateSettings>
    {
        public RateConfig Config = new RateConfig();
    }
}