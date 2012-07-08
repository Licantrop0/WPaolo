using System.Collections.Generic;
using Scudetti.Model;
using System.Linq;

namespace Scudetti
{
    public static class AppContext
    {
        private static IEnumerable<Shield> _shields;
        public static IEnumerable<Shield> Shields
        {
            get
            {
                if (_shields == null)
                    _shields = ShieldService.Load();
                return _shields;
            }
        }

        public static int TotalShieldUnlocked
        { get { return Shields.Count(s => s.IsUnlocked); } }

    }
}
