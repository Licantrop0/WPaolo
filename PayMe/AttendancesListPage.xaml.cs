using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace PayMe
{
    public partial class AttendancesListPage : PhoneApplicationPage
    {
        public AttendancesListPage()
        {
            InitializeComponent();
            CreateAppBar();
        }

        private void Attendance_Click(object sender, RoutedEventArgs e)
        {
            var a = ((Button)sender).DataContext as Attendance;
            NavigationService.Navigate(new Uri("/AddEditAttendance.xaml?id=" + a.Id, UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            AttendanceListBox.ItemsSource = null;
            AttendanceListBox.ItemsSource = Settings.Attendances;            
        }

        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();

            var ExportToExcelAppBarButton = new ApplicationBarIconButton();
            ExportToExcelAppBarButton.IconUri = new Uri("Toolkit.Content\\appbar.sendmail.png", UriKind.Relative);
            ExportToExcelAppBarButton.Text = AppResources.ExportToMail;
            ExportToExcelAppBarButton.Click += delegate(object sender, EventArgs e)
            {
                var sb = new StringBuilder();
                //Header
                sb.AppendLine(string.Join("\t", new string[]
                {
                    AppResources.StartTime.TrimEnd(":".ToCharArray()),
                    AppResources.EndTime.TrimEnd(":".ToCharArray()),
                    AppResources.CustomerName.TrimEnd(":".ToCharArray()),
                    AppResources.Description.TrimEnd(":".ToCharArray()),
                    AppResources.Income.TrimEnd(":".ToCharArray())
                }));

                foreach (var att in Settings.Attendances)
                {
                    sb.AppendLine(string.Join("\t", new string[]
                    {
                        att.StartTime.ToString(),
                        att.EndTime.ToString(),
                        att.CustomerName,
                        att.Description,
                        att.Income.ToString()
                    }));
                }

                var MailTask = new EmailComposeTask();
                MailTask.Subject = "PayMe - " + AppResources.AttendancesList;
                MailTask.Body = sb.ToString();
                MailTask.Show();
            };

            ApplicationBar.Buttons.Add(ExportToExcelAppBarButton);
        }

    }
}