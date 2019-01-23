using System;
using Scripts.Prefs;

namespace Util
{
    public class Cooldown
    {
        public readonly string key;
        public readonly TimeSpan cooldown;
        
        private DateTime? _lastUse
        {
            get => StencilPrefs.Default.GetDateTime(key);
            set => StencilPrefs.Default.SetDateTime(key, value).Save();
        }

        public Cooldown(string key, TimeSpan? cooldown = null)
        {
            this.key = key;
            this.cooldown = cooldown ?? TimeSpan.FromMinutes(30);
        }

        public void Use()
        {
            _lastUse = DateTime.UtcNow;
        }

        public bool IsReady(out TimeSpan remaining)
        {
            var active = true;
            var last = _lastUse;
            TimeSpan? elapsed;
            if (last != null)
            {
                elapsed = DateTime.UtcNow - last.Value;
                active = elapsed.Value > cooldown;
                remaining = cooldown - elapsed.Value;
            }
            if (active) remaining = TimeSpan.Zero;
            return active;
        }
    }
}