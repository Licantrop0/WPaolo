using System;
using System.Collections.Generic;
using Scudetti.Model;
using SocceramaWin8.Sound;
using Topics.Radical.ComponentModel.Messaging;
using Topics.Radical.Windows.Presentation;
using Topics.Radical.Windows.Presentation.ComponentModel;
using Topics.Radical.Windows.Presentation.Messaging;
using Windows.ApplicationModel.Resources;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;


namespace SocceramaWin8.Presentation
{
    public class LevelsViewModel : AbstractViewModel
    {
        ResourceLoader resources = new ResourceLoader();

        readonly INavigationService _ns;
        readonly IMessageBroker _broker;


        public LevelsViewModel(INavigationService ns, IMessageBroker broker)
        {
            _ns = ns;
            _broker = broker;

            _broker.Subscribe<ApplicationBootCompleted>(this, InvocationModel.Safe, async (sender, msg) =>
            {
                await AppContext.LoadShieldsAsync();
                OnPropertyChanged("StatusText");
                OnPropertyChanged("Levels");
            });

            _broker.Subscribe<ApplicationSuspend>(this, async (sender, msg) =>
            {
                await ShieldService.Save(AppContext.Shields);
            });

            _broker.Subscribe<PropertyChangedMessage<bool>>(this, (sender, msg) =>
            {
                if (msg.PropertyName != "IsValidated") return;
                //Aggiorna lo status text dei completed shields e se il gioco è stato completato
                OnPropertyChanged("StatusText");
                AppContext.GameCompleted = AppContext.Shields.All(s => s.IsValidated);
            });
        }

        private List<LevelViewModel> _levels;
        public List<LevelViewModel> Levels
        {
            get
            {
                if (AppContext.Shields == null)
                {
                    _levels = null;
                    return null;
                }

                if (_levels == null)
                {
                    _levels = AppContext.Shields.GroupBy(s => s.Level)
                        .Select(g => new LevelViewModel(g, _ns, _broker))
                        .OrderBy(l => l.Number)
                        .ToList();
                }
                return _levels;
            }
        }

        public string StatusText
        {
            get
            {
                if (AppContext.Shields == null)
                    return string.Empty;

                return string.Format("{0}: {1}/{2}", resources.GetString("Shields"),
                        AppContext.TotalShieldUnlocked, AppContext.TotalShields);
            }
        }

    }
}
