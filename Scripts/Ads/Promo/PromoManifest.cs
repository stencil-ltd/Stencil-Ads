using System;
using UnityEngine;

namespace Ads.Promo
{
    [Serializable]
    public class PromoManifest
    {
        public Promo[] promos;
    }

    [Serializable]
    public class Promo
    {
        public string id;
        public string name;
        public string genre;
        public bool isNew;
        public bool enabled;
        public VideoUrls videos;
        public DownloadUrls downloads;
    }

    [Serializable]
    public class DownloadUrls
    {
        public string iphone;
        public string android;
        public string landingPage;
    }

    [Serializable]
    public class VideoUrls
    {
        public string video480;

        public string GetForSize(VideoSize size)
        {
            switch (size)
            {
                case VideoSize.Video480:
                default:
                    return video480;
            }
        }
    }

    [Serializable]
    public enum VideoSize
    {
        Video480
    }

    public static class ManifestExtensions
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

        private static string GetBasePlatformUrl(this DownloadUrls urls)
        {
            if (Application.platform == RuntimePlatform.Android)
                return urls.android;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return urls.iphone;

            if (!string.IsNullOrEmpty(urls.landingPage))
                return urls.landingPage;
            
            return urls.iphone;
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