namespace Ratings
{
    public enum RateReadiness
    {
        // All good.
        Ready = 0,
        
        // Temporary Setbacks
        MinorFailures = 100,
        FailPostponed,
        
        // Real Setbacks
        MediumFailures = 200,
        FailLaunchTime,
        FailInstallTime,
        FailSessionCount,
        FailNetwork,
        
        // Will Never Show again
        PermanentFailures = 300,
        FailRejected,
        FailRated,
    }
}