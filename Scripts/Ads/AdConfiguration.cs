using Util;

namespace Ads
{
    public class AdConfiguration
    {
        public static implicit operator string(AdConfiguration configuration) 
            => configuration.UnitId.Value;

        public static implicit operator PlatformValue<string>(AdConfiguration configuration)
            => configuration.UnitId;

        public readonly PlatformValue<string> UnitId;
        public AdConfiguration(string android, string ios, string debugAndroid, string debugIos)
        {
            UnitId = new PlatformValue<string>()
                .WithAndroid(android, developer: debugAndroid)
                .WithIos(ios, developer: debugIos);
        }
    }

    public class AppIdConfiguration : AdConfiguration
    {
        public AppIdConfiguration(string android, string ios) 
            : base(android, 
                   ios, 
                   "ca-app-pub-3940256099942544~3347511713", 
                   "ca-app-pub-3940256099942544~1458002511")
        {}
    }

    public class BannerConfiguration : AdConfiguration
    {
        public BannerConfiguration(string android, string ios) 
            : base(android, 
                ios, 
                "ca-app-pub-3940256099942544/6300978111", 
                "ca-app-pub-3940256099942544/2934735716")
        {}
    }
    
    public class InterstitialConfiguration : AdConfiguration
    {
        public InterstitialConfiguration(string android, string ios) 
            : base(android, 
                ios, 
                "ca-app-pub-3940256099942544/1033173712", 
                "ca-app-pub-3940256099942544/4411468910")
        {}
    }

    public class RewardedConfiguration : AdConfiguration
    {
        public RewardedConfiguration(string android, string ios) 
            : base(android, 
                ios, 
                "ca-app-pub-3940256099942544/5224354917", 
                "ca-app-pub-3940256099942544/1712485313")
        {}
    }
}