using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using DeathTimerz.Localization;
using Microsoft.Phone.Controls;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using Microsoft.Phone.Shell;

namespace DeathTimerz
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
            BuildApplicationBar();
        }

        XDocument CurrentTest;
        ResourceManager CurrentResources;

        private void TestListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TestListbox.SelectedIndex == 0)
            {
                CurrentTest = AppContext.Test1;
                CurrentResources = Test1.ResourceManager;
            }
            else
            {
                CurrentTest = AppContext.Test2;
                CurrentResources = Test2.ResourceManager;
            }

            PopupBorder.Visibility = Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
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
                        var content = new TextBlock()
                        {
                            Text = CurrentResources.GetString(answ.Attribute("Name").Value),
                            Style = (Style)Application.Current.Resources["RedChillerCheckBoxContent"],
                        };
                        TestStackPanel.Children.Add(new RadioButton()
                        {
                            Name = answ.Attribute("Name").Value,
                            Content = content,
                            GroupName = el.Attribute("Name").Value,
                            IsChecked = bool.Parse(answ.Attribute("IsChecked").Value),
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
                    answTextBox.KeyDown += new KeyEventHandler(answTextBox_KeyDown);
                    TestStackPanel.Children.Add(answTextBox);
                }
            }
        }


        private void SaveAnswers()
        {
            //resetto le risposte precedentemente salvate
            if (CurrentTest == null) return;

            CurrentTest.Descendants("Question")
            .Where(question => question.Attribute("Type").Value == "MultipleChoice")
            .ForEach(ans => ans.Descendants()
                .ForEach(el => el.Attribute("IsChecked").Value = "False"));

            CurrentTest.Descendants("Question")
            .Where(question => question.Attribute("Type").Value == "Number")
            .ForEach(ans => ans.Element("Answer").Attribute("Text").Value = string.Empty);

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
            AppContext.TimeToDeath = TimeSpan.Zero;

            AppContext.TimeToDeath +=
                 (from q in AppContext.Test1.Descendants("Question")
                      .Concat(AppContext.Test2.Descendants("Question"))
                  where q.Attribute("Type").Value == "MultipleChoice"
                  from ans in q.Elements("Answer")
                  where ans.Attribute("IsChecked").Value == "True"
                  select ExtensionMethods.TimeSpanFromYears(
                    double.Parse(ans.Attribute("Value").Value, CultureInfo.InvariantCulture))).
                        Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

            #region Test1-Specific evaluation (BMI + Cigarettes)

            double Weight, Height;
            if (double.TryParse(AppContext.Test1.Descendants("Answer")
                .First(el => el.Attribute("Name").Value == "Weight1")
                .Attribute("Text").Value, out Weight)
                &&
                double.TryParse(AppContext.Test1.Descendants("Answer")
                .First(el => el.Attribute("Name").Value == "Height1")
                .Attribute("Text").Value, out Height))
            {
                double bmi;
                if (CultureInfo.CurrentUICulture.Name == "en-US")
                    bmi = Weight * 703 / Math.Pow(Height, 2); //lb-in
                else
                    bmi = Weight / Math.Pow(Height * .01, 2); //kg-cm

                if (bmi >= 18 && bmi <= 27)
                    AppContext.TimeToDeath += ExtensionMethods.TimeSpanFromYears(2);
                else if (bmi > 27 && bmi <= 35)
                    AppContext.TimeToDeath += ExtensionMethods.TimeSpanFromYears(-2);
                else if (bmi < 18 || bmi > 35)
                    AppContext.TimeToDeath += ExtensionMethods.TimeSpanFromYears(-4);
            }

            double cigarettesNum, cigaretteYears;
            if (double.TryParse(AppContext.Test1.Descendants("Answer")
                .First(el => el.Attribute("Name").Value == "Cigarettes1")
                .Attribute("Text").Value, out cigarettesNum)
                &&
                double.TryParse(AppContext.Test1.Descendants("Answer")
                .First(el => el.Attribute("Name").Value == "Cigarettes2")
                .Attribute("Text").Value, out cigaretteYears))
            {

                //Ogni sigaretta toglie 11 minuti di vita http://www.rense.com/health3/smoking_h.htm
                var TotalSmokingPeriod =
                    ExtensionMethods.TimeSpanFromYears(cigaretteYears) + //anni che ha fumato finora
                    AppContext.TimeToDeath - (DateTime.Now - AppContext.BirthDay.Value); //anni ancora da vivere

                AppContext.TimeToDeath -= TimeSpan.FromMinutes(TotalSmokingPeriod.Value.TotalDays * cigarettesNum * 11);
            }
            #endregion


            //Mando messaggio al MainViewModel con la property da aggiornare
            Messenger.Default.Send<NotificationMessage>(
                new NotificationMessage("TestUpdated"));
        }

        bool IsTestFilled()
        {
            if (TestStackPanel.Children.Count == 0) return false;

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

        void answTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.PlatformKeyCode)
            {
                case 187:
                case 189:
                case 222://* - #
                    e.Handled = true;
                    break;
                case 188:
                case 190: //, .
                    var t = (TextBox)sender;
                    t.Text += CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
                    t.SelectionStart = t.Text.Length; //riposiziona il cursore in fondo alla textbox
                    e.Handled = true;
                    break;
                case 32: //spazio
                    this.Focus();
                    e.Handled = true;
                    break;
            }
        }


        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            SaveAnswers();
            if (IsTestFilled())
            {
                StimateDeathAge();
            }
        }


        private void BuildApplicationBar()
        {
            var OkAppBarButton = new ApplicationBarIconButton();
            OkAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Check.png", UriKind.Relative);
            OkAppBarButton.Text = AppResources.Ok;
            OkAppBarButton.Click += new EventHandler(OkAppBarButton_Click);
            ApplicationBar.Buttons.Add(OkAppBarButton);
        }

        void OkAppBarButton_Click(object sender, EventArgs e)
        {
            if (FocusManager.GetFocusedElement() is TextBox && !IsTestFilled())
            {
                this.Focus();
                return;
            }

            if (!IsTestFilled())
            {
                MessageBox.Show(AppResources.TestNotCompleted);
                return;
            }

            SaveAnswers();
            StimateDeathAge();
            NavigationService.GoBack();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }
    }
}