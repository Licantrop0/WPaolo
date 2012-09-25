using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scudetti.Model;
using SocceramaWin8.ViewModel;
using Windows.Storage;

namespace SocceramaWin8
{
    public static class AppContext
    {
        public const int HintsTreshold = 5;
        public const int LockTreshold = 15;
        public const int BonusTreshold = 50;

        public static bool ToastDisplayed = false;

        public static event EventHandler LoadCompleted;
        static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

        public static IEnumerable<Shield> Shields { get; private set; }
        public static List<LevelViewModel> Levels { get; private set; }

        public static int TotalShieldUnlocked
        {
            get { return Shields.Count(s => s.IsValidated); }
        }

        public static async void LoadShieldsAsync()
        {
            Shields = await ShieldService.Load();
            Levels = Shields.GroupBy(s => s.Level)
                .Select(g=> new LevelViewModel(g))
                .ToList();

            if (LoadCompleted != null)
                LoadCompleted.Invoke(null, EventArgs.Empty);
        }

        private static bool? _soundEnabled;
        public static bool SoundEnabled
        {
            get
            {
                if (!_soundEnabled.HasValue)
                {
                    if (!roamingSettings.Values.ContainsKey("sound_enabled"))
                        roamingSettings.Values.Add("sound_enabled", true);

                    _soundEnabled = (bool)roamingSettings.Values["sound_enabled"];
                }
                return _soundEnabled.Value;
            }
            set
            {
                if (SoundEnabled == value) return;
                roamingSettings.Values["sound_enabled"] = value;
                _soundEnabled = value;
            }
        }

        private static int? _availableHints;
        public static int AvailableHints
        {
            get
            {
                if (!_availableHints.HasValue)
                {
                    if (!roamingSettings.Values.ContainsKey("available_hints"))
                        roamingSettings.Values.Add("available_hints", HintsTreshold);

                    _availableHints = (int)roamingSettings.Values["available_hints"];
                }
                return _availableHints.Value;
            }
            set
            {
                if (AvailableHints == value) return;
                roamingSettings.Values["available_hints"] = value;
                _availableHints = value;
            }
        }

        private static bool? _gameCompleted;
        public static bool GameCompleted
        {
            get
            {
                if (!_gameCompleted.HasValue)
                {
                    if (!roamingSettings.Values.ContainsKey("game_completed"))
                        roamingSettings.Values.Add("game_completed", false);

                    _gameCompleted = (bool)roamingSettings.Values["game_completed"];
                }
                return _gameCompleted.Value;
            }
            set
            {
                if (GameCompleted == value) return;
                roamingSettings.Values["game_completed"] = value;
                _gameCompleted = value;
            }
        }

        public static void ResetShields()
        {
            
        }
    }
}
