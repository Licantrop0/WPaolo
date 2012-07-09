using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Scudetti.Model;
using Scudetti.ViewModel;

namespace Scudetti
{
    public static class AppContext
    {
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
                    .Select(g => new LevelViewModel(g)).ToList();
            };
            bw.RunWorkerCompleted += (sender, e) => LoadCompleted(sender, e);
            bw.RunWorkerAsync();
        }

    }
}
