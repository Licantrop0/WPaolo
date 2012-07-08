using System.Collections.Generic;
using Scudetti.Model;

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
    }
}
