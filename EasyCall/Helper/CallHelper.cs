using System.Windows;
using Microsoft.Phone.Tasks;
using WPCommon.Helpers;

namespace EasyCall.Helper
{
    public static class CallHelper
    {
        private const int FreeCalls = 6;

        public static void Call(string name, string number)
        {
            if (!CheckTrial()) return;
            new PhoneCallTask
            {
                DisplayName = name,
                PhoneNumber = number
            }.Show();
        }

        public static void SendSms(string number)
        {
            if (!CheckTrial()) return;
            new SmsComposeTask
            {
                To = number
            }.Show();
        }

        private static bool CheckTrial()
        {
            if (!TrialManagement.IsTrialMode)
                return true;

            if (TrialManagement.Counter < FreeCalls)
            {
                MessageBox.Show("You have " + (FreeCalls - TrialManagement.Counter) + " calls left for this demo",
                    "Trial Mode",
                    MessageBoxButton.OK);
                TrialManagement.IncrementCounter();
                return true;
            }

            var result = MessageBox.Show(
                "I'm sorry, you called too many times for this trial, now it's time to pay!",
                "Trial Mode",
                MessageBoxButton.OK);
            if(result == MessageBoxResult.OK)
                TrialManagement.Buy();

            return false;
        }
    }
}