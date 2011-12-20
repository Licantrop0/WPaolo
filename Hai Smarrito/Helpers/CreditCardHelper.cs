using System;

namespace HaiSmarrito.Helpers
{
    public static class CreditCardHelper
    {
        public static string GetName(string cardType)
        {
            switch (cardType)
            {
                case "amex":
                    return "American Express";
                case "visa":
                    return "Visa";
                case "mastercard":
                    return "Master Card";
                case "dinersclub":
                    return "Diners Club";
                default:
                    return null;
            }
        }

        public static int GetIndex(string cardType)
        {
            if (cardType == "amex") return 1;
            else if (cardType == "visa") return 2;
            else return 3; //cardType == "mastercard"
        }
    }
}