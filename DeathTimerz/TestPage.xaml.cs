using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;

namespace DeathTimer
{
    public partial class TestPage : PhoneApplicationPage
    {
        XDocument doc;
        IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public TestPage()
        {
            InitializeComponent();
            BuildApplicationBar();
        }

        void OkAppBarButton_Click(object sender, EventArgs e)
        {
            if (!IsTestFilled())
            {
                MessageBox.Show(AppResources.TestNotCompleted);
                return;
            }

            SaveAnswers();
            StimateDeathAge();
            NavigationService.GoBack();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (var fs = new IsolatedStorageFileStream("Questions.xml", FileMode.Open, FileAccess.Read, isf))
            {
                doc = XDocument.Load(fs);
            }
            BuildTest();
        }

        private void SaveAnswers()
        {
            //resetto le risposte precedentemente salvate
            foreach (var el in doc.Descendants("Answer"))
            {
                el.Attributes("IsChecked").ForEach(a => a.Value = "False");
                el.Attributes("Content").ForEach(a => a.Value = string.Empty);
            }

            //imposto l'attributo IsChecked alle risposte selezionate nell'XML
            (from ctrl in TestStackPanel.Children
             where ctrl is RadioButton
             let rb = (RadioButton)ctrl
             where rb.IsChecked.Value
             select rb.Name).ForEach(ans =>
                doc.Descendants("Answer")
                .Where(el => el.Attribute("Name").Value == ans).First()
                .Attribute("IsChecked").Value = "True");

            //Imposto i valori delle textbox nell'XML
            (from ctrl in TestStackPanel.Children
             where ctrl is TextBox
             let txtb = (TextBox)ctrl
             select txtb).ForEach(ans =>
                doc.Descendants("Answer")
                .Where(el => el.Attribute("Name").Value == ans.Name).First()
                .Attribute("Content").Value = ans.Text);

            using (var fs = new IsolatedStorageFileStream("Questions.xml", FileMode.Create, FileAccess.Write, isf))
            {
                doc.Save(fs);
            }
        }

        private void StimateDeathAge()
        {
            Settings.EstimatedDeathAge =
                 (from q in doc.Descendants("Question")
                  where q.Attribute("Type").Value == "MultipleChoice"
                  from ans in q.Elements("Answer")
                  where ans.Attribute("IsChecked").Value == "True"
                  select TimeSpanFromYears(double.Parse(ans.Attribute("Value").Value, CultureInfo.InvariantCulture))).
                  Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));

            var Weight = (from el in doc.Descendants("Answer")
                          where el.Attribute("Name").Value == "Weight1"
                          select double.Parse(el.Attribute("Content").Value)).First();

            var Height = (from el in doc.Descendants("Answer")
                          where el.Attribute("Name").Value == "Height1"
                          select double.Parse(el.Attribute("Content").Value)).First();

            double bmi;
            if (CultureInfo.CurrentUICulture.Name == "it-IT")
                bmi = Weight / Math.Pow(Height * .01, 2); //kg-cm
            else
                bmi = Weight * 703 / Math.Pow(Height, 2); //lb-in

            if (bmi >= 18 && bmi <= 27)
                Settings.EstimatedDeathAge += TimeSpanFromYears(2);
            else if (bmi > 27 && bmi <= 35)
                Settings.EstimatedDeathAge += TimeSpanFromYears(-2);
            else if (bmi < 18 || bmi > 35)
                Settings.EstimatedDeathAge += TimeSpanFromYears(-4);

            var cigarettes = (from el in doc.Descendants("Answer")
                              where el.Attribute("Name").Value == "Cigarettes1"
                              select double.Parse(el.Attribute("Content").Value)).First();

            if (cigarettes <= 0)
                Settings.EstimatedDeathAge += TimeSpanFromYears(2);
            else if (cigarettes > 0 && cigarettes <= 5)
                Settings.EstimatedDeathAge += TimeSpanFromYears(-1);
            else if (cigarettes > 5)
                Settings.EstimatedDeathAge += TimeSpanFromYears(-4);
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


        TimeSpan TimeSpanFromYears(double year)
        {
            return TimeSpan.FromDays(year * Settings.AverageYear);
        }

        private void BuildTest()
        {
            foreach (var el in doc.Descendants("Question"))
            {
                //Domanda
                TestStackPanel.Children.Add(new TextBlock()
                {
                    Text = AppResources.ResourceManager.GetString(el.Attribute("Name").Value),
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)Application.Current.Resources["RedChillerTest"],
                });

                //Risposta Multipla
                if (el.Attribute("Type").Value == "MultipleChoice")
                {
                    foreach (var answ in el.Elements("Answer"))
                    {
                        TestStackPanel.Children.Add(new RadioButton()
                        {
                            Name = answ.Attribute("Name").Value,
                            Content = AppResources.ResourceManager.GetString(answ.Attribute("Name").Value),
                            GroupName = el.Attribute("Name").Value,
                            Style = (Style)Application.Current.Resources["RedChillerContentControl"],
                            IsChecked = bool.Parse(answ.Attribute("IsChecked").Value)
                        });
                    }
                }
                else //Risposta Aperta
                {
                    InputScope Numbers = new InputScope();
                    Numbers.Names.Add(new InputScopeName() { NameValue = InputScopeNameValue.TelephoneNumber });

                    TestStackPanel.Children.Add(new TextBox()
                    {
                        Name = el.Element("Answer").Attribute("Name").Value,
                        Text = el.Element("Answer").Attribute("Content").Value,
                        //Style = (Style)Application.Current.Resources["RedChiller"],
                        InputScope = Numbers
                    });
                }
            }
        }

        private void BuildApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar
            ApplicationBar = new ApplicationBar();

            var OkAppBarButton = new ApplicationBarIconButton();
            OkAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Check.png", UriKind.Relative);
            OkAppBarButton.Text = AppResources.Ok;
            OkAppBarButton.Click += new EventHandler(OkAppBarButton_Click);

            var CancelAppBarButton = new ApplicationBarIconButton();
            CancelAppBarButton.IconUri = new Uri("Toolkit.Content\\ApplicationBar.Cancel.png", UriKind.Relative);
            CancelAppBarButton.Text = AppResources.Cancel;
            CancelAppBarButton.Click += new EventHandler(CancelAppBarButton_Click);

            ApplicationBar.Buttons.Add(OkAppBarButton);
            ApplicationBar.Buttons.Add(CancelAppBarButton);
        }

        void CancelAppBarButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}