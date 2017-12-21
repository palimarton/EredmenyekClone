using Eredmenyek.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Eredmenyek.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel => (MainPageViewModel)DataContext;
        public MainPage()
        {
            InitializeComponent();
        }
    }
}
