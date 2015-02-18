using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soccerama.Model;
using Windows.Storage;

namespace Soccerama.Win81
{
    public static class AppContext
    {
        public const int HintsTreshold = 5;
        public const int LockTreshold = 15;
        public const int BonusTreshold = 50;

        public static bool ToastDisplayed = false;

        static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        static ShieldService _shieldService = new ShieldService();

        public static IEnumerable<Shield> Shields = null;

        public static int TotalShieldUnlocked
        {
            get { return Shields == null ? 0 : Shields.Count(s => s.IsValidated); }
        }

        public static int TotalShields
        {
            get { return Shields.Count() - 3; }
        }

        public static bool GameCompleted
        {
            get { return TotalShieldUnlocked == TotalShields; }
        }

        public static async void SaveShields()
        {
            await _shieldService.Save(Shields);
        }

        public static async Task LoadShieldsAsync()
        {
            Shields = await _shieldService.Load();
            //foreach (var validated in ValidatedShields)
            //{
            //    Shields.First(s => s.Id == validated).IsValidated = true;
            //}
        }

        public static async Task ResetShields()
        {
            AvailableHints = HintsTreshold;
            Shields = await _shieldService.GetNew();
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

        public static List<string> _validatedShields;
        public static List<string> ValidatedShields
        {
            get
            {
                if (_validatedShields == null)
                {
                    if (!roamingSettings.Values.ContainsKey("validated_shields"))
                        roamingSettings.Values.Add("validated_shields", new List<string>());

                    _validatedShields = (List<string>)roamingSettings.Values["validated_shields"];
                }
                return _validatedShields;
            }
        }
    }
}
