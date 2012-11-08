using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Scudetti.Data;
using Scudetti.Localization;
using Scudetti.Sound;

namespace Scudetti.ViewModel
{
	public class LevelsViewModel : ViewModelBase
	{
		public IList<LevelViewModel> Levels
		{
			get
			{
				return IsInDesignMode ? DesignTimeData.Levels : AppContext.Levels;
			}
		}

		public string StatusText
		{
			get
			{
				return AppContext.Shields == null ? string.Empty :
					string.Format("{0}: {1}/{2}", AppResources.Shields,
						AppContext.TotalShieldUnlocked, AppContext.TotalShields);
			}
		}

		public LevelViewModel SelectedLevel
		{
			get { return null; }
			set
			{
				if (value == null) return;
				if (!value.IsUnlocked) return;

				SoundManager.PlayFischietto();
				MessengerInstance.Send(value);
				RaisePropertyChanged("SelectedLevel");
			}
		}

		public LevelsViewModel()
		{
			AppContext.LoadCompleted += (sender, e) =>
			{
				RaisePropertyChanged("Levels");
				RaisePropertyChanged("StatusText");
			};

			MessengerInstance.Register<PropertyChangedMessage<bool>>(this, m =>
			{
				if (m.PropertyName != "IsValidated") return;
				//Aggiorna lo status text dei completed shields e se il gioco è stato completato
				RaisePropertyChanged("StatusText");
				AppContext.GameCompleted = AppContext.Shields.All(s => s.IsValidated);
			});
		}
	}
}
