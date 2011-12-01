using System.Windows;
using Microsoft.Phone.Tasks;

namespace HaiSmarrito
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
            //if (TrialManagement.IsTrialMode)
            //{
            //    if (TrialManagement.Counter >= 5)
            //    {
            //        MessageBox.Show("I'm sorry, you called too many times for this trial, now it's time to pay!", "Trial Mode", MessageBoxButton.OK);
            //        TrialManagement.Buy();
            //        return false;
            //    }

            //    TrialManagement.IncrementCounter();
            //    MessageBox.Show("You have " + (6 - TrialManagement.Counter) + " calls left for this demo", "Trial Mode", MessageBoxButton.OK);
            //}
            return true;
        }
    }
}
