using System;

namespace Ads.Promo.Data
{
    [Serializable]
    public class PromoMetadata
    {
        public int version;
        public DownloadUrls downloads;
        public AppStoreMetadata appStore;
        public Excludes excludes;
    }

    [Serializable]
    public class AppStoreMetadata
    {
        // See https://analytics.itunes.apple.com/#/campaigngenerator
        public string provider;
    }

    [Serializable]
    public class Excludes
    {
        public string[] android = {};
        public string[] iphone = {};
    }
}