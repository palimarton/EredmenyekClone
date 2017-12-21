using Eredmenyek.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Eredmenyek.Views
{
    public sealed partial class TeamPage : Page
    {
        public TeamPageViewModel ViewModel => (TeamPageViewModel)DataContext;
        public TeamPage()
        {
            InitializeComponent();
        }
    }
}
