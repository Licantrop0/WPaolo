using System;
using System.Windows.Navigation;

namespace NascondiChiappe.Helpers
{
    // Only used in WP7
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;
        void Navigate(Uri uri);
        void GoBack();
    }
}