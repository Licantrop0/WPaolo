using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using DeathTimerz.Localization;
using System.Collections.Generic;
using System.Resources;

namespace DeathTimerz
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        XDocument CurrentTest;
        ResourceManager CurrentResources;

        private void TestListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestListbox.SelectedIndex == 0)
            {
                CurrentTest = Settings.Test1;
                CurrentResources = Test1.ResourceManager;
            }
            else
            {
                CurrentTest = Settings.Test2;
                CurrentResources = Test2.ResourceManager;
            }

            PopupBorder.Visibility = Visibility.Collapsed;
            BuildTest();
        }

        private void BuildTest()
        {
            foreach (var el in CurrentTest.Descendants("Question")) //ciclo su ogni domanda
            {
                //Domanda
                TestStackPanel.Children.Add(new TextBlock()
                {
                    Text = CurrentResources.GetString(el.Attribute("Name").Value),
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)Application.Current.Resources["RedChillerTest"],
                });

                //Risposta Multipla
                if (el.Attribute("Type").Value == "MultipleChoice")
                {
                    foreach (var answ in el.Elements("Answer")) //ciclo su ogni risposta
                    {
                        TestStackPanel.Children.Add(new RadioButton()
                        {
                            Name = answ.Attribute("Name").Value,
                            Content = CurrentResources.GetString(answ.Attribute("Name").Value),
                            GroupName = el.Attribute("Name").Value,
                            Style = (Style)Application.Current.Resources["RedChillerContentControl"],
                            IsChecked = bool.Parse(answ.Attribute("IsChecked").Value)
                        });
                    }
                }
                else //Risposta Aperta (textbox)
                {
                    InputScope Numbers = new InputScope();
                    Numbers.Names.Add(new InputScopeName() { NameValue = InputScopeNameValue.TelephoneNumber });
                    var answTextBox = new TextBox()
                    {
                        Name = el.Element("Answer").Attribute("Name").Value,
                        Text = el.Element("Answer").Attribute("Text").Value,
                        //Style = (Style)Application.Current.Resources["RedChiller"],
                        InputScope = Numbers
                    };
                    answTextBox.KeyDown += (sender, e) => { e.Handled = CheckDigits(e); };
                    TestStackPanel.Children.Add(answTextBox);
                }
            }

            TestStackPanel.Children.Add(new TextBlock()
            {
                Text = "[Premi back per salvare e vedere il Risultato]",
                TextWrapping = TextWrapping.Wrap,
                Style = (Style)Application.Current.Resources["RedChillerTest"],
            });

        }

        private void SaveAnswers()
        {
            //resetto le risposte precedentemente salvate
            foreach (var el in CurrentTest.Descendants("Answer"))
            {
                el.Attributes("IsChecked").ForEach(a => a.Value = "False");
                el.Attributes("Text").ForEach(a => a.Value = string.Empty);
            }

            //imposto l'attributo IsChecked alle risposte selezionate nell'XML
            TestStackPanel.Children
                .Where(ctrl => ctrl is RadioButton)
                .Cast<RadioButton>()
                .Where(rb => rb.IsChecked.Value)
                .Select(rb => rb.Name)
                .ForEach(ans =>
                    CurrentTest.Descendants("Answer")
                    .Where(el => el.Attribute("Name").Value == ans).First()
                    .Attribute("IsChecked").Value = "True");

            //Imposto i valori delle textbox nell'XML
            TestStackPanel.Children
                .Where(ctrl => ctrl is TextBox)
                .Cast<TextBox>()
                .ForEach(ans =>
                    CurrentTest.Descendants("Answer")
                    .Where(el => el.Attribute("Name").Value == ans.Name).First()
                    .Attribute("Text").Value = ans.Text);
        }

        private void StimateDeathAge()
        {
            Settings.EstimatedDeathAge = ExtensionMethods.TimeSpanFromYears(Settings.AverageAge);

            Settings.EstimatedDeathAge +=
                 (from q in Settings.Test1.Descendants("Question")
                      .Concat(Settings.Test2.Descendants("Question"))
                  where q.Attribute("Type").Value == "MultipleChoice"
                  from ans in q.Elements("Answer")
                  where ans.Attribute("IsChecked").Value == "True"
                  select ExtensionMethods.TimeSpanFromYears(
                    double.Parse(ans.Attribute("Value").Value, CultureInfo.InvariantCulture))).
                    Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

            #region Test1-Specific evaluation (BMI + Cigarettes)

            double Weight, Height;
            if (double.TryParse(Settings.Test1.Descendants("Answer")
                .First(el => el.Attribute("Name").Value == "Weight1")
                .Attribute("Text").Value, out Weight)
                &&
                double.TryParse(Settings.Test1.Descendants("Answer")
                .First(el => el.Attribute("Name").Value == "Height1")
                .Attribute("Text").Value, out Height))
            {
                double bmi;
                if (CultureInfo.CurrentUICulture.Name == "en-US")
                    bmi = Weight * 703 / Math.Pow(Height, 2); //lb-in
                else
                    bmi = Weight / Math.Pow(Height * .01, 2); //kg-cm

                if (bmi >= 18 && bmi <= 27)
                    Settings.EstimatedDeathAge += ExtensionMethods.TimeSpanFromYears(2);
                else if (bmi > 27 && bmi <= 35)
                    Settings.EstimatedDeathAge += ExtensionMethods.TimeSpanFromYears(-2);
                else if (bmi < 18 || bmi > 35)
                    Settings.EstimatedDeathAge += ExtensionMethods.TimeSpanFromYears(-4);
            }

            double cigarettes;
            if (double.TryParse(Settings.Test1.Descendants("Answer")
                .First(el => el.Attribute("Name").Value == "Cigarettes1")
                .Attribute("Text").Value, out cigarettes))
            {
                //TODO: elaborare meglio sulle sigarette
                if (cigarettes <= 0)
                    Settings.EstimatedDeathAge += ExtensionMethods.TimeSpanFromYears(2);
                else if (cigarettes > 0 && cigarettes <= 5)
                    Settings.EstimatedDeathAge += ExtensionMethods.TimeSpanFromYears(-1);
                else if (cigarettes > 5)
                    Settings.EstimatedDeathAge += ExtensionMethods.TimeSpanFromYears(-4);
            }

            #endregion
        }

        bool IsTestFilled()
        {
            var RbGroups = from ctrl in TestStackPanel.Children
                           where ctrl is RadioButton
                           let rb = (RadioButton)ctrl
                           group rb by rb.GroupName;

            foreach (var RadioGroup in RbGroups)
                if (RadioGroup.All(rb => !rb.IsChecked.Value))
                    return false;

            double temp;
            //ritorna true se non ci sono textbox che falliscono il parsing a double
            return (from ctrl in TestStackPanel.Children
                    where ctrl is TextBox
                    let t = (TextBox)ctrl
                    where !double.TryParse(t.Text, out temp)
                    select t).Count() == 0;
        }

        private static bool CheckDigits(KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.D8 || e.Key == Key.D3) //* o #
                return true;
            if (e.PlatformKeyCode == 188 && CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != ",")
                return true;
            if (e.PlatformKeyCode == 190 && CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator != ".")
                return true;

            return false;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAnswers();
            if (IsTestFilled())
            {
                StimateDeathAge();
            }

        }
    }
}