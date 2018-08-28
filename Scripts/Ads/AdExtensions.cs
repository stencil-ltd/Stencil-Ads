using System;

namespace Ads
{
    public static class AdExtensions
    {
        public static void ShowOnResult(this VideoAd ad, Action<bool> onResult)
        {
            EventHandler<bool> callback = null;
            callback = (sender, b) =>
            {
                ad.OnResult -= callback;
                onResult?.Invoke(b);
            };
            ad.OnResult += callback;
            ad.Show();
        }
    }
}