using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Scudetti.Model;
using Scudetti.ViewModel;
using System.IO.IsolatedStorage;

namespace Scudetti
{
    public static class AppContext
    {
        public const int LockTreshold = 5;
        public static event RunWorkerCompletedEventHandler LoadCompleted;
        public static IEnumerable<Shield> Shields { get; set; }
        private static List<LevelViewModel> _levels;

        public static List<LevelViewModel> Levels
        {
            get
            {
                if(_levels == null)
                    LoadShieldsAsync();

                return _levels;
            }
        }

        public static int TotalShieldUnlocked
        { get { return Shields.Count(s => s.IsValidated); } }

        private static void LoadShieldsAsync()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (sender, e) =>
            {
                Shields = ShieldService.Load();
                _levels = Shields
                    .GroupBy(s => s.Level)
                    .OrderBy(g => g.Key)
                    .Select(g => new LevelViewModel(g)).ToList();
            };
            bw.RunWorkerCompleted += (sender, e) => LoadCompleted(sender, e);
            bw.RunWorkerAsync();
        }


        private static bool? _soundEnabled;
        public static bool SoundEnabled
        {
            get
            {
                if (!_soundEnabled.HasValue)
                {
                    if (!IsolatedStorageSettings.ApplicationSettings.Contains("sound_enabled"))
                        IsolatedStorageSettings.ApplicationSettings.Add("sound_enabled", true);

                    _soundEnabled = (bool)IsolatedStorageSettings.ApplicationSettings["sound_enabled"];
                }
                return _soundEnabled.Value;
            }
            set
            {
                if (SoundEnabled == value) return;
                IsolatedStorageSettings.ApplicationSettings["sound_enabled"] = value;
                _soundEnabled = value;
            }
        }
    }
}
