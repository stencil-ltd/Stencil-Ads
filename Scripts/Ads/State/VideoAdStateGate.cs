using System;
using System.Collections;
using System.Linq;
using State.Active;
using Util;

namespace Ads.State
{
    public class VideoAdStateGate : ActiveGate
    {
        public AdType Type = AdType.Interstitial;
        
        public bool Invert;
        public bool AndDestroy;
        public VideoAdState[] States;

        private VideoAd _ad;
        private VideoAdState _state = VideoAdState.None;

        public override void Register(ActiveManager manager)
        {
            base.Register(manager);
            Objects.StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            while (!StencilAds.HasInit)
                yield return null;
            
            switch (Type)
            {
                case AdType.Interstitial:
                    _ad = StencilAds.Interstitial;
                    break;
                case AdType.Rewarded:
                    _ad = StencilAds.Rewarded;
                    break;
            }
            
            if (_ad == null) yield break;
            _ad.OnState += OnState;
        }

        private void OnDestroy()
        {
            if (_ad != null) _ad.OnState -= OnState;
        }

        private void OnState(object sender, VideoAdState e)
        {
            _state = e;
            RequestCheck();
        }

        public override bool? Check()
        {
            try 
            {
                var visible = States.Contains(_state);
                if (Invert) visible = !visible;
                if (AndDestroy && !visible)
                    Destroy(gameObject);
                return visible;
            } catch (Exception) 
            {
                return null;
            }
        }
    }
}