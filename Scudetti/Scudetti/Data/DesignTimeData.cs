using System.Collections.Generic;
using Scudetti.Model;

namespace Scudetti.Data
{
    public static class DesignTimeData
    {
        public static IEnumerable<Shield> GetShields()
        {
            return new[]
            {
                new Shield(){ Id = 1, Level = 1, Name="Milan", Image="milan.png", IsUnlocked = true },
                new Shield(){ Id = 2, Level = 1, Name="Ajax", Image="ajax.png", IsUnlocked = false },
                new Shield(){ Id = 3, Level = 1, Name="barcellona", Image="barcellona.png", IsUnlocked = true },
                new Shield(){ Id = 4, Level = 1, Name="celtic", Image="celtic.png", IsUnlocked = true },
                new Shield(){ Id = 5, Level = 1, Name="chelesa", Image="chelesa.png", IsUnlocked = false },
                new Shield(){ Id = 6, Level = 1, Name="fc-porto", Image="pc-porto.png", IsUnlocked = true },
                new Shield(){ Id = 7, Level = 2, Name="inter", Image="inter.png", IsUnlocked = false },
                new Shield(){ Id = 8, Level = 2, Name="juventus", Image="juventus.png", IsUnlocked = true },
                new Shield(){ Id = 9, Level = 2, Name="santos", Image="santos.png", IsUnlocked = false },
                new Shield(){ Id = 10, Level = 2, Name="manchesterunited", Image="manchesterunited.png", IsUnlocked = false },
                new Shield(){ Id = 11, Level = 2, Name="Milan", Image="milan.png", IsUnlocked = true },
                new Shield(){ Id = 12, Level = 3, Name="Milan", Image="milan.png", IsUnlocked = false },
                new Shield(){ Id = 13, Level = 3, Name="Milan", Image="milan.png", IsUnlocked = false },
                new Shield(){ Id = 14, Level = 3, Name="Milan", Image="milan.png", IsUnlocked = false },
                new Shield(){ Id = 15, Level = 3, Name="Milan", Image="milan.png", IsUnlocked = false },
            };
        }
    }
}
