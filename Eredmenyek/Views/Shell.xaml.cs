using System;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Common.Services.UserServices;

namespace Eredmenyek.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        public delegate void LogInFinished();
        public static event LogInFinished OnLogInFinished;

        public Shell()
        {
            Instance = this;
            InitializeComponent();
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }

        private async void LogInAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await OneDriveBasedUserService.SetUpOneDriveAccess())
                {
                    if (OnLogInFinished != null)
                    {
                        OnLogInFinished.Invoke();
                    }                    
                }
            }
            catch (Microsoft.Graph.ServiceException)
            {

            }          
        }
    }
}

