using System;
using Scripts.RemoteConfig;
using UnityEngine;
using Util;

namespace Ratings
{
    [CreateAssetMenu(menuName = "Stencil/Ratings")]
    public class RateSettings : Singleton<RateSettings>
    {
        public RateConfig Config = new RateConfig();

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                Config.BindRemoteConfig();
                StencilRemote.OnRemoteConfig += OnRemote;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (Application.isPlaying)
            {
                StencilRemote.OnRemoteConfig -= OnRemote;
            }
        }

        private void OnRemote(object sender, EventArgs e)
        {
            Config.BindRemoteConfig();
        }
    }
}