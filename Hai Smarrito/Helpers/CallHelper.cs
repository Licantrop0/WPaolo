using System.Windows;
using Microsoft.Phone.Tasks;
using WPCommon.Helpers;

namespace HaiSmarrito.Helpers
{
    public static class CallHelper
    {
        private static PhoneCallTask CallTask = new PhoneCallTask();

        public static void Call(string name, string number)
        {
            if (!CheckTrial()) return;

            CallTask.DisplayName = name;
            CallTask.PhoneNumber = number;
            CallTask.Show();
        }

        private static bool CheckTrial()
        {
            if (!TrialManagement.IsTrialMode)
                return true;

            if (MessageBox.Show("Questa è la versione Trial, per chiamare devi acquistare l'app",
                "Trial Mode", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                TrialManagement.Buy();

            return false;
        }
    }
}