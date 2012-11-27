using System;
using System.Collections.Generic;
using Scudetti.Model;
using SocceramaWin8.Sound;
using Topics.Radical.ComponentModel.Messaging;
using Topics.Radical.Windows.Presentation;
using Topics.Radical.Windows.Presentation.ComponentModel;
using Topics.Radical.Windows.Presentation.Messaging;
using Windows.ApplicationModel.Resources;

namespace SocceramaWin8.Presentation
{
    public class LevelsViewModel : AbstractViewModel
    {
        ResourceLoader resources = new ResourceLoader();

        readonly INavigationService ns;
        readonly IMessageBroker broker;


        public LevelsViewModel(INavigationService ns, IMessageBroker broker)
        {
            this.ns = ns;
            this.broker = broker;

            this.broker.Subscribe<ApplicationSuspend>(this, async (sender, msg) =>
            {
                await ShieldService.Save(AppContext.Shields);
            });

            this.broker.Subscribe<ApplicationBootCompleted>(this, InvocationModel.Safe, async (sender, msg) =>
            {
                Levels = await AppContext.LoadShieldsAsync();
                OnPropertyChanged("StatusText");
            });



            //MessengerInstance.Register<PropertyChangedMessage<bool>>(this, m =>
            //{
            //    if (m.PropertyName != "IsValidated") return;
            //    //Aggiorna lo status text dei completed shields e se il gioco è stato completato
            //    RaisePropertyChanged("StatusText");
            //    AppContext.GameCompleted = AppContext.Shields.All(s => s.IsValidated);
            //});
        }

        public List<LevelViewModel> Levels
        {
            get { return this.GetPropertyValue<List<LevelViewModel>>(() => Levels); }
            set { this.SetPropertyValue(() => Levels, value); }
        }

        public string StatusText
        {
            get
            {
                return AppContext.Shields == null ? string.Empty :
                    string.Format("{0}: {1}/{2}", resources.GetString("Shields"),
                        AppContext.TotalShieldUnlocked, AppContext.TotalShields);
            }
        }

    }
}
