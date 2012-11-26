using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scudetti.Model;
using SocceramaWin8.Presentation;
using Windows.Storage;

namespace SocceramaWin8
{
    public static class AppContext
    {
        public const int HintsTreshold = 5;
        public const int LockTreshold = 15;
        public const int BonusTreshold = 50;

        public static bool ToastDisplayed = false;
        public static double ShieldsScrollPosition = 0;

        static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

        public static IEnumerable<Shield> Shields = null;

        public static int TotalShieldUnlocked
        {
            get { return Shields == null ? 0 : Shields.Count(s => s.IsValidated); }
        }
        public static int TotalShields
        {
            get { return Shields.Count() - 6; }
        }

        public static async Task<List<LevelViewModel>> LoadShieldsAsync()
        {
            Shields = await ShieldService.Load();
            return GroupLevels(Shields);
        }

        private static List<LevelViewModel> GroupLevels(IEnumerable<Shield> shields)
        {
            return shields.GroupBy(s => s.Level)
               .Select(g => new LevelViewModel(g))
               .OrderBy(l => l.Number)
               .ToList();
        }

        public static async Task ResetShields()
        {
            AvailableHints = HintsTreshold;
            Shields = await ShieldService.GetNew();
            //Levels = GroupLevels(Shields);
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
    }
}
