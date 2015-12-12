using Microsoft.Phone.Tasks;
using System.Windows;
using WPCommon.Helpers;

namespace EasyCall.Helper
{
    public static class CallHelper
    {
        private const int FreeCalls = 6;

        private enum SmsCall
        {
            Sms,
            Call
        }

        public static void Call(string name, string number)
        {
            if (!CheckTrial(SmsCall.Call)) return;
            new PhoneCallTask
            {
                DisplayName = name,
                PhoneNumber = number
            }.Show();
        }

        public static void SendSms(string number)
        {
            if (!CheckTrial(SmsCall.Sms)) return;
            new SmsComposeTask
            {
                To = number
            }.Show();
        }

        private static bool CheckTrial(SmsCall smsOrCall)
        {
            if (!TrialManagement.IsTrialMode)
                return true;

            string message1 = string.Empty, message2 = string.Empty;

            switch (smsOrCall)
            {
                case SmsCall.Call:
                    message1 = $"You have {FreeCalls - TrialManagement.Counter} Calls left for this demo";
                    message2 = "I'm sorry, you called too many times for this demo, now it's time to pay!";
                    break;
                case SmsCall.Sms:
                    message1 = $"You have {FreeCalls - TrialManagement.Counter} SMS left for this demo";
                    message2 = "I'm sorry, you sent too many SMS for this demo, now it's time to pay!";
                    break;
            }

            if (TrialManagement.Counter < FreeCalls)
            {
                var result1 = MessageBox.Show(message1, "Demo Mode", MessageBoxButton.OK);
                if (result1 == MessageBoxResult.OK)
                {
                    TrialManagement.IncrementCounter();
                    return true;
                }
            }
            else
            {
                var result2 = MessageBox.Show(message2, "Demo Mode", MessageBoxButton.OK);
                if (result2 == MessageBoxResult.OK)
                {
                    TrialManagement.Buy();
                }
            }
            return false;
        }
    }
}