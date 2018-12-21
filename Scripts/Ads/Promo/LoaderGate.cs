using State.Active;

namespace Ads.Promo
{
    public class LoaderGate : ActiveGate
    {
        public override void Register(ActiveManager manager)
        {
            base.Register(manager);
            if (NextSceneLoader.Instance != null)
                NextSceneLoader.Instance.OnLoading += _OnLoading;
        }

        public override void Unregister()
        {
            base.Unregister();
            if (NextSceneLoader.Instance != null)
                NextSceneLoader.Instance.OnLoading -= _OnLoading;
        }

        private void _OnLoading(object sender, bool e)
        {
            RequestCheck();
        }

        public override bool? Check()
        {
            var loader = NextSceneLoader.Instance;
            if (loader == null) return null;
            return !loader.IsLoading;
        }
    }
}