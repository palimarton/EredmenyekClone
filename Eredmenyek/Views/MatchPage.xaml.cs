using Eredmenyek.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Eredmenyek.Views
{
    public sealed partial class MatchPage : Page
    {
        public MatchPageViewModel ViewModel => (MatchPageViewModel)DataContext;
        public MatchPage()
        {
            this.InitializeComponent();
        }
    }
}
