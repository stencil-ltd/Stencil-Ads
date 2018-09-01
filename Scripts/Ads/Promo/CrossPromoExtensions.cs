using Ads.Promo.Data;
using UnityEngine;

namespace Ads.Promo
{
    public static class CrossPromoExtensions
    {
        public static Vector2Int GetResolution(this VideoSize size)
        {
            switch (size)
            {
                case VideoSize.Video480:
                default:
                    return new Vector2Int(270, 480);
            }
        }

        public static string GetBasePlatformUrl(this DownloadUrls urls)
        {
            if (Application.isEditor || Application.platform == RuntimePlatform.Android)
                return urls.android;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return urls.iphone;
            return null;
        }

        public static string GetPlatformUrl(this DownloadUrls urls)
        {
            var url = GetBasePlatformUrl(urls);
            var source = "Stencil";
            var medium = "CrossPromo";
            var campaign = Application.identifier;
            return $"{url}?utm_source={source}&utm_medium={medium}&utm_campaign={campaign}";
        }
        
    }
}