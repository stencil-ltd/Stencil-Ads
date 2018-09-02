using System;

namespace Ads.Promo.Data
{
    [Serializable]
    public class PromoMetadata
    {
        public int version;
        public DownloadUrls downloads;
        public AppStoreMetadata appStore;
    }

    [Serializable]
    public class AppStoreMetadata
    {
        public string provider;
    }
}