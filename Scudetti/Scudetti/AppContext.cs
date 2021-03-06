﻿using Scudetti.Model;
using Scudetti.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;

namespace Scudetti
{
	public static class AppContext
	{
		public const int HintsTreshold = 5;
		public const int LockTreshold = 15;
		public const int BonusTreshold = 50;

		public static bool ToastDisplayed = false;

		public static event RunWorkerCompletedEventHandler LoadCompleted;
		public static IEnumerable<Shield> Shields { get; private set; }
		public static IList<LevelViewModel> Levels { get; private set; }

		public static int TotalShieldUnlocked
		{
			get { return Shields == null ? 0 : Shields.Count(s => s.IsValidated); }
		}
        public static int TotalShields
        {
            get { return Shields.Count() - 6; }
        }


		public static void LoadShieldsAsync()
		{
			var bw = new BackgroundWorker();
			bw.DoWork += (sender, e) =>
			{
				Shields = ShieldService.Load();
				Levels = GroupLevels(Shields);
			};
			bw.RunWorkerCompleted += RaiseLoadCompleted;
			bw.RunWorkerAsync();
		}

		private static List<LevelViewModel> GroupLevels(IEnumerable<Shield> shields)
		{
			var levels = shields
				.GroupBy(s => s.Level)
				.OrderBy(g => g.Key)
				.Select(g => new LevelViewModel(g)).ToList();

			var b1 = levels[6];
			var b2 = levels[7];

			levels.Insert(4, b2);
			levels.Insert(3, b1);
			levels.RemoveRange(8, 2);

			return levels;
		}

		public static void ResetShields()
		{
			Shields = ShieldService.GetNew();
			Levels = GroupLevels(Shields);
			AvailableHints = HintsTreshold;
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

		private static int? _availableHints;
		public static int AvailableHints
		{
			get
			{
				if (!_availableHints.HasValue)
				{
					if (!IsolatedStorageSettings.ApplicationSettings.Contains("available_hints"))
						IsolatedStorageSettings.ApplicationSettings.Add("available_hints", HintsTreshold);

					_availableHints = (int)IsolatedStorageSettings.ApplicationSettings["available_hints"];
				}
				return _availableHints.Value;
			}
			set
			{
				if (AvailableHints == value) return;
				IsolatedStorageSettings.ApplicationSettings["available_hints"] = value;
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
					if (!IsolatedStorageSettings.ApplicationSettings.Contains("game_completed"))
						IsolatedStorageSettings.ApplicationSettings.Add("game_completed", false);

					_gameCompleted = (bool)IsolatedStorageSettings.ApplicationSettings["game_completed"];
				}
				return _gameCompleted.Value;
			}
			set
			{
				if (GameCompleted == value) return;
				IsolatedStorageSettings.ApplicationSettings["game_completed"] = value;
				_gameCompleted = value;
			}
		}
	}
}
