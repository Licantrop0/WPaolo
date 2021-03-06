﻿using System;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Model;
using Scudetti.Sound;

namespace Scudetti.ViewModel
{
	public class ShieldViewModel : ViewModelBase
	{
		public string InputShieldName { get; set; }
		public Visibility InputVisibile { get { return CurrentShield.IsValidated ? Visibility.Collapsed : Visibility.Visible; } }
		public string OriginalName { get { return CurrentShield.Names[0]; } }

		private string _hintText;
		public string HintText
		{
			get
			{
				return _hintText ?? (_hintText = string.Format(AppResources.AvailableHints, AppContext.AvailableHints));
			}
			set
			{
				_hintText = value;
				RaisePropertyChanged("HintText");
			}
		}

		private Shield _currentShield;
		public Shield CurrentShield
		{
			get { return _currentShield; }
			set
			{
				if (CurrentShield == value)
					return;
				_currentShield = value;

                HintText = null;
                InputShieldName = string.Empty;

				_currentShield.PropertyChanged += (sender, e) =>
				{
					if (e.PropertyName == "IsValidated")
					{
						MessengerInstance.Send(new PropertyChangedMessage<bool>(
							!_currentShield.IsValidated,
							_currentShield.IsValidated,
							e.PropertyName));
					}
				};
                RaisePropertyChanged("CurrentShield");
			}
		}

		public ShieldViewModel()
		{
			if (IsInDesignMode)
			{
				_currentShield = DesignTimeData.Shields.First();
				_currentShield.IsValidated = false;
				_hintText = string.Format(AppResources.AvailableHints, 5);
                return;
			}

            MessengerInstance.Register<Shield>(this, m => CurrentShield = m);
		}

		public bool Validate()
		{
			if (CurrentShield.IsValidated || string.IsNullOrEmpty(InputShieldName))
			{
				MessengerInstance.Send("goback", "navigation");
			}
			else if (CurrentShield.Names.Any(name => CompareName(name, InputShieldName)))
			{
				SoundManager.PlayValidated();
				CurrentShield.IsValidated = true;
				if (AppContext.TotalShieldUnlocked % AppContext.HintsTreshold == 0)
					AppContext.AvailableHints++;

				MessengerInstance.Send("goback", "navigation");
			}
			else
			{
				SoundManager.PlayBooh();
				MessageBox.Show(AppResources.Wrong);
				return false;
			}

			return true;
		}

		private bool CompareName(string string1, string string2)
		{
			return string.Compare(string1, string2.Trim(), StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public void ShowHint()
		{
			if (AppContext.AvailableHints > 0)
			{
				HintText = CurrentShield.Hint;
				AppContext.AvailableHints--;
			}
			else
			{
				MessageBox.Show(AppResources.NoHintsAvailable);
			}
		}
	}
}

