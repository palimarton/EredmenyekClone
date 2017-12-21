using Eredmenyek.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Eredmenyek.Views
{
    public sealed partial class TournamentPage : Page
    {
        public TournamentPageViewModel ViewModel => (TournamentPageViewModel)DataContext; 
        public TournamentPage()
        {
            InitializeComponent();
        }
    }
}
