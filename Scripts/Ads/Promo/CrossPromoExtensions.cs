using System;
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
            Debug.Log($"Looking up platform url for {Application.platform}...");
            if (Application.isEditor || Application.platform == RuntimePlatform.Android)
                return urls.android;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return urls.iphone;
            return null;
        }
        
        public static string AndroidUrl(string id, string source)
        {
            var campaign = Application.identifier;
            var referrer = $"utm_source={source}&utm_medium=cross-promo&utm_campaign={campaign}";
            referrer = Uri.EscapeDataString(referrer);
            return $"https://play.google.com/store/apps/details?id={id}&referrer={referrer}";
        }

        public static string ItunesUrl(string id, string provider)
        {
            var campaign = Application.identifier;
            return $"https://itunes.apple.com/app/apple-store/id{id}?pt={provider}&ct={campaign}&mt=8";
        }
    }
}