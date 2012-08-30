using System.Collections.Generic;
using Scudetti.Model;
using System.Linq;
using SocceramaWin8.ViewModel;

namespace SocceramaWin8.Data
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
            return Shields
                .GroupBy(s => s.Level)
                .Select(g => new LevelViewModel(g))
                .ToList();
        }

        private static IEnumerable<Shield> GetShields()
        {
            return new[]
            {
                new Shield(){ Level = 1, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = true },
                new Shield(){ Level = 1, Id="Ajax", Names = new[] { "Ajax" }, Image="ajax.png", IsValidated = false },
                new Shield(){ Level = 1, Id="barcellona", Names = new[] { "Barcellona", "Barcelona" }, Image="barcellona.png", IsValidated = true },
                new Shield(){ Level = 1, Id="celtic", Names = new[] { "Celtic" }, Image="celtic.png", IsValidated = true },
                new Shield(){ Level = 1, Id="chelesa", Names = new[] { "Chelesa" }, Image="chelesa.png", IsValidated = false },
                new Shield(){ Level = 1, Id="fc-porto", Names = new[] { "FC Porto", "F.C. Porto" }, Image="pc-porto.png", IsValidated = true },
                new Shield(){ Level = 2, Id="inter", Names = new[] { "Inter" }, Image="inter.png", IsValidated = false },
                new Shield(){ Level = 2, Id="juventus", Names = new[] { "Juventus" }, Image="juventus.png", IsValidated = true },
                new Shield(){ Level = 2, Id="santos", Names = new[] { "Santos" }, Image="santos.png", IsValidated = false },
                new Shield(){ Level = 2, Id="manchesterunited", Names = new[] { "Manchester United" }, Image="manchesterunited.png", IsValidated = false },
                new Shield(){ Level = 2, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = true },
                new Shield(){ Level = 3, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
                new Shield(){ Level = 3, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
                new Shield(){ Level = 3, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
                new Shield(){ Level = 3, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
            };
        }
    }
}
