using System.Collections.Generic;
using Scudetti.Model;
using System.Linq;
using Scudetti.ViewModel;

namespace Scudetti
{
    public static class AppContext
    {
        public static int TotalShieldUnlocked
        { get { return Shields.Count(s => s.IsValidated); } }

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

        private static List<LevelViewModel> _levels;
        public static List<LevelViewModel> Levels
        {
            get
            {
                if (_levels == null)
                {
                    _levels = AppContext.Shields
                        .GroupBy(s => s.Level)
                        .Select(g => new LevelViewModel(g)).ToList();
                }
                return _levels;
            }
        }
    }
}
