using System.Collections.Generic;
using Scudetti.Model;
using System.Linq;
using Scudetti.ViewModel;

namespace Scudetti.Data
{
    public static class DesignTimeData
    {
        private static IEnumerable<Shield> _shields;
        public static IEnumerable<Shield> Shields
        {
            get
            {
                if (_shields == null)
                    _shields = GetShields();
                return _shields;
            }
        }

        private static List<LevelViewModel> _levels;
        public static List<LevelViewModel> Levels
        {
            get
            {
                if (_levels == null)
                    _levels = GetLevels();

                return _levels;
            }
        }

        private static List<LevelViewModel> GetLevels()
        {
            return new[]
            {
                new LevelViewModel(1, Shields.Where(s=> s.Level==1)),
                new LevelViewModel(2, Shields.Where(s=> s.Level==2)),
                new LevelViewModel(3, Shields.Where(s=> s.Level==3)),
            }.ToList();
        }

        private static IEnumerable<Shield> GetShields()
        {
            return new[]
            {
                new Shield(){ Level = 1, Id="Milan", Image="milan.png", IsValidated = true },
                new Shield(){ Level = 1, Id="Ajax", Image="ajax.png", IsValidated = false },
                new Shield(){ Level = 1, Id="barcellona", Image="barcellona.png", IsValidated = true },
                new Shield(){ Level = 1, Id="celtic", Image="celtic.png", IsValidated = true },
                new Shield(){ Level = 1, Id="chelesa", Image="chelesa.png", IsValidated = false },
                new Shield(){ Level = 1, Id="fc-porto", Image="pc-porto.png", IsValidated = true },
                new Shield(){ Level = 2, Id="inter", Image="inter.png", IsValidated = false },
                new Shield(){ Level = 2, Id="juventus", Image="juventus.png", IsValidated = true },
                new Shield(){ Level = 2, Id="santos", Image="santos.png", IsValidated = false },
                new Shield(){ Level = 2, Id="manchesterunited", Image="manchesterunited.png", IsValidated = false },
                new Shield(){ Level = 2, Id="Milan", Image="milan.png", IsValidated = true },
                new Shield(){ Level = 3, Id="Milan", Image="milan.png", IsValidated = false },
                new Shield(){ Level = 3, Id="Milan", Image="milan.png", IsValidated = false },
                new Shield(){ Level = 3, Id="Milan", Image="milan.png", IsValidated = false },
                new Shield(){ Level = 3, Id="Milan", Image="milan.png", IsValidated = false },
            };
        }
    }
}
