using System;
using System.Runtime.InteropServices;
using Analytics;
using PaperPlaneTools;
using UnityEngine;
using Util;

namespace Ratings
{
    public static class StencilRateHelpers
    {
#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern bool _reviewControllerIsAvailable ();
		[DllImport("__Internal")]
		private static extern void _reviewControllerShow ();
#endif
        
        public static int SessionCount
        {
            get { return PlayerPrefs.GetInt("stencil_rate_sessions"); }
            set
            {
                PlayerPrefs.SetInt("stencil_rate_sessions", value);
                PlayerPrefs.Save();
            }
        }
        
        public static bool IsRejected
        {
            get
            {
                // TODO remove this after migration.
                if (RateBox.Instance.Statistics.DialogIsRejected) return true;
                return PlayerPrefsX.GetBool("stencil_rate_reject");
            }
            set
            {
                PlayerPrefsX.SetBool("stencil_rate_reject", value);
                PlayerPrefs.Save();
            }
        }
        
        public static bool IsRated
        {
            get
            {
                return PlayerPrefsX.GetBool("stencil_rate_rated");
            }
            set
            {
                PlayerPrefsX.SetBool("stencil_rate_rated", value);
                PlayerPrefs.Save();
            }
        }
        
        public static DateTime? LastShow
        {
            get { return PlayerPrefsX.GetDateTime("stencil_rate_last"); }
            set
            {
                PlayerPrefsX.SetDateTime("stencil_rate_last", value);
                PlayerPrefs.Save();
            }
        }

        public static DateTime? FirstCheck
        {
            get { return PlayerPrefsX.GetDateTime("stencil_rate_first_check"); }
            set
            {
                PlayerPrefsX.SetDateTime("stencil_rate_first_check", value);
                PlayerPrefs.Save();
            }
        }
        
        public static string GetStoreUrl(string iTunesAppId, string googlePlayMarketAppBundleId) 
        {
            var url = "";
#if (UNITY_IPHONE || UNITY_EDITOR)
            url = String.Format("https://itunes.apple.com/app/id{0}?action=write-review",  WWW.EscapeURL(iTunesAppId));
#endif
#if UNITY_ANDROID
            url = String.Format("https://play.google.com/store/apps/details?id={0}",  WWW.EscapeURL(googlePlayMarketAppBundleId));
#endif
            return url;
        }

        public static void CountSession()
        {
            SessionCount++;
        }

        public static void RecordRating()
        {
            IsRated = true;
        }

        public static void Reject()
        {
            IsRejected = true;
        }

        public static void Rate(this RateConfig settings)
        {
            Tracking.Instance.Track("rate").SetUserProperty("has_rated", true);
            RecordRating();
            settings.GoToRateUrl();
        }

        public static void Review(this RateConfig settings)
        {
            Tracking.Instance.Track("review").SetUserProperty("has_reviewed", true);
            RecordRating();
            Application.OpenURL(settings.RateUrl);
        }
        
        #if UNITY_IOS
        public static bool NativeRate()
        {
            if (_reviewControllerIsAvailable())
            {
                _reviewControllerShow();
                return true;
            }
            return false;
        }
        #endif

        public static void GoToRateUrl(this RateConfig settings)
        {
#if UNITY_IOS
            if (settings.IosNativeRating && NativeRate())
                return;
#endif
            settings.Review();
        }

        public static void MarkShown()
        {
            LastShow = DateTime.UtcNow;
        }
        
        public static bool CheckConditions(this RateConfig settings)
        {
            var firstCheck = FirstCheck ?? DateTime.UtcNow;
            FirstCheck = firstCheck;
            
            Debug.Log("Check Rate Conditions");
            if (IsRejected)
            {
                Debug.Log("Reject: User rejected rating");
                return false;
            }

            if (IsRated)
            {
                Debug.Log("Reject: User already rated");
                return false;
            }

            if (settings.RequireInternetConnection &&
                Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("Reject: No internet");
                return false;
            }

            var afterLaunch = settings.HoursAfterLaunch;
            if (afterLaunch > 0f)
            {
                var hoursRuntime = Time.time / 60f / 60f;
                if (afterLaunch > hoursRuntime)
                {
                    Debug.Log("Reject: Hours after launch");
                    return false;
                }
            }

            var now = DateTime.UtcNow;
            var last = LastShow;
            if (last != null)
            {
                var postpone = settings.HoursAfterPostpone;
                if (postpone > 0f && now < last.Value.AddHours(postpone))
                {
                    Debug.Log("Reject: Postponed.");
                    return false;
                }
            }

            var afterInstall = settings.HoursAfterInstall;
            if (afterInstall > 0f && now < firstCheck.AddHours(afterInstall))
            {
                Debug.Log("Reject: Hours after install");
                return false;
            }

            var minSessions = settings.MinSessionCount;
            if (minSessions > 0 && SessionCount < minSessions)
            {
                Debug.Log("Reject: Session Count");
                return false;
            } 

            Debug.Log("Accept Rating!");
            return true;
        }
    }
}