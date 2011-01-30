using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace PayMe
{
    [DataContract]
    public class Attendance
    {
        public int Id { get { return StartTime.GetHashCode() ^ EndTime.GetHashCode(); } }

        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }
        [DataMember]
        public TimeSpan PauseTimeSpan { get; set; }

        public TimeSpan Duration { get { return EndTime - StartTime - PauseTimeSpan; } }

        [DataMember]
        public Money Income { get; set; }
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string Description { get; set; }

        public Attendance(DateTime startTime, DateTime endTime, TimeSpan pauseTimeSpan, double income, string currencySymbol)
        {
            StartTime = startTime;
            EndTime = endTime;
            PauseTimeSpan = pauseTimeSpan;
            Income = new Money(income, currencySymbol);
        }
    }

    [DataContract]
    public class Money
    {
        public enum CurrencySymbol
        {
            Dollar,
            Euro,
            Pound,
            Yen,
            Franc
        }

        [DataMember]
        public double Value { get; set; }

        [DataMember]
        public CurrencySymbol Symbol { get; set; }

        public Money(double value, string currency)
        {
            Value = value;
            Symbol = GetCurrencyEnum(currency);
        }

        public override string ToString()
        {
            return string.Format("{0:0.00} {1}", Value, GetCurrencyText(Symbol));
        }

        public static string GetCurrencyText(CurrencySymbol cs)
        {
            switch (cs)
            {
                case CurrencySymbol.Dollar:
                    return "$";
                case CurrencySymbol.Euro:
                    return "€";
                case CurrencySymbol.Pound:
                    return "£";
                case CurrencySymbol.Yen:
                    return "¥";
                case CurrencySymbol.Franc:
                    return "Fr";
                default:
                    return "$";
            }
        }

        public static CurrencySymbol GetCurrencyEnum(string cs)
        {
            switch (cs)
            {
                case "$":
                    return CurrencySymbol.Dollar;
                case "€":
                    return CurrencySymbol.Euro;
                case "£":
                    return CurrencySymbol.Pound;
                case "¥":
                    return CurrencySymbol.Yen;
                case "Fr":
                    return CurrencySymbol.Franc;
                default:
                    return CurrencySymbol.Dollar;
            }
        }
    }
}