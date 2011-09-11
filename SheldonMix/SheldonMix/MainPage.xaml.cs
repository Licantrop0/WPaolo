using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Net;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.IO;

namespace SheldonMix
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void AboutAppBarMenu_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.youtube.com/watch?v=SifGskrY_UY") }.Show();
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://twitter.com/#!/BigBang_CBS") }.Show();
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.cbs.com/shows/big_bang_theory") }.Show();
        }

        private void Suggersci_Click(object sender, RoutedEventArgs e)
        {
            new EmailComposeTask()
            {
                Subject = "[SheldonMix] Feedback and request for upgrades",
                To = "wpmobile@hotmail.it",
                Body = SuggerimentoTextBox.Text
            }.Show();
        }

        private void HyperlinkButton_Click_3(object sender, RoutedEventArgs e)
        {
            new WebBrowserTask() { Uri = new Uri("http://www.imdb.com/name/nm1433588/") }.Show();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                new MarketplaceReviewTask().Show();
            }
            catch (InvalidOperationException)
            { /*do nothing */ }
        }

        private void SetRingtone_Click(object sender, RoutedEventArgs e)
        {
            //Ringtone files must be of type MP3 or WMA.
            //Ringtone files must be less than 40 seconds in length.
            //Ringtone files must not have digital rights management (DRM) protection.
            //Ringtone files must be less than 1 MB in size.

            var RingtonePath = "TBBT ringtone path";
            this.SaveFileToIsolatedStorage(RingtonePath);
            var saveRingtoneTask = new SaveRingtoneTask();
            saveRingtoneTask.Completed += (sender1, e1) =>
            {
                if (e1.TaskResult == TaskResult.OK)
                    MessageBox.Show("Ringone Saved.");
                else
                    MessageBox.Show("Save canceled.");
            };

            try
            {
                saveRingtoneTask.Source = new Uri("isostore:/" + RingtonePath);
                saveRingtoneTask.DisplayName = "TBBT Intro Song";
                saveRingtoneTask.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void SaveFileToIsolatedStorage(string fileName)
        {
            var streamResourceInfo = Application.GetResourceStream(new Uri(fileName, UriKind.Relative));
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(fileName))
                {
                    myIsolatedStorage.DeleteFile(fileName);
                }
                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(fileName, FileMode.Create, myIsolatedStorage))
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        var resourceStream = streamResourceInfo.Stream;
                        long length = resourceStream.Length;
                        byte[] buffer = new byte[32];
                        int readCount = 0;
                        using (var reader = new BinaryReader(streamResourceInfo.Stream))
                        {
                            // read file in chunks in order to reduce memory consumption and increase performance
                            while (readCount < length)
                            {
                                int actual = reader.Read(buffer, 0, buffer.Length);
                                readCount += actual;
                                writer.Write(buffer, 0, actual);
                            }
                        }
                    }
                }
            }
        }


    }
}