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
        public const int LockTreshold = 15;
        public static event RunWorkerCompletedEventHandler LoadCompleted;
        public static List<LevelViewModel> Levels { get; set; }
        public static IEnumerable<Shield> Shields { get; set; }

        public static int TotalShieldUnlocked
        { get { return Shields.Count(s => s.IsValidated); } }

        public static void LoadShieldsAsync()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (sender, e) =>
            {
                Shields = ShieldService.Load();
                Levels = Shields
                    .GroupBy(s => s.Level)
                    .OrderBy(g => g.Key)
                    .Select(g => new LevelViewModel(g)).ToList();
            };
            bw.RunWorkerCompleted += (sender, e) => RaiseLoadCompleted(sender, e);
            bw.RunWorkerAsync();
        }

        private static void RaiseLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (LoadCompleted != null)
                LoadCompleted(sender, e);
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
