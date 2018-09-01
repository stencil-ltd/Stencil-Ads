using System;
using UnityEngine.Video;

namespace Ads.Promo.Data
{
    [Serializable]
    public class VideoUrls
    {
        public VideoClip video480;

        public VideoClip GetForSize(VideoSize size)
        {
            switch (size)
            {
                case VideoSize.Video480:
                default:
                    return video480;
            }
        }
    }
}