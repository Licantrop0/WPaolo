using System.Collections.Generic;
using Soccerama.Model;
using System.Linq;
namespace Soccerama.Win81.Data
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

        private static IEnumerable<Shield> GetShields()
        {
            return new[]
            {
                new Shield(){ Level = 1, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = true },
                new Shield(){ Level = 1, Id="Ajax", Names = new[] { "Ajax" }, Image="ajax.png", IsValidated = false },
                new Shield(){ Level = 1, Id="barcellona", Names = new[] { "Barcellona", "Barcelona" }, Image="barcellona.png", IsValidated = true },
                new Shield(){ Level = 2, Id="celtic", Names = new[] { "Celtic" }, Image="celtic.png", IsValidated = true },
                new Shield(){ Level = 3, Id="chelesa", Names = new[] { "Chelesa" }, Image="chelesa.png", IsValidated = false },
                new Shield(){ Level = 4, Id="fc-porto", Names = new[] { "FC Porto", "F.C. Porto" }, Image="pc-porto.png", IsValidated = true },
                new Shield(){ Level = 5, Id="inter", Names = new[] { "Inter" }, Image="inter.png", IsValidated = false },
                new Shield(){ Level = 6, Id="juventus", Names = new[] { "Juventus" }, Image="juventus.png", IsValidated = true },
                new Shield(){ Level = 7, Id="santos", Names = new[] { "Santos" }, Image="santos.png", IsValidated = false },
                new Shield(){ Level = 8, Id="manchesterunited", Names = new[] { "Manchester United" }, Image="manchesterunited.png", IsValidated = false },
                new Shield(){ Level = 9, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = true },
                new Shield(){ Level = 10, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
                new Shield(){ Level = 100, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
                new Shield(){ Level = 200, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
                new Shield(){ Level = 300, Id="Milan", Names = new[] { "Milan" }, Image="milan.png", IsValidated = false },
            };
        }
    }
}
