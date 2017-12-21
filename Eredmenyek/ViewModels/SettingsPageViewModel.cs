using Common.Services.SettingsServices;
using System;
using Template10.Mvvm;

namespace Eredmenyek.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class SettingsPartViewModel : ViewModelBase
    {
        SettingsService _settings;

        public SettingsPartViewModel()
        {
            _settings = SettingsService.Instance;
        }

        public bool NotificationsEnabled
        {
            get
            {
                return _settings.NotificationsEnabled;
            }
            set
            {
                _settings.NotificationsEnabled = value;
                base.RaisePropertyChanged();
            }
        }

        public bool FavouriteTeamNotificationsOnly
        {
            get
            {
                return _settings.FavouriteTeamNotificationsOnly;
            }
            set
            {
                _settings.FavouriteTeamNotificationsOnly = value;
                base.RaisePropertyChanged();
            }
        }

        public bool ResultsInNotification
        {
            get
            {
                return _settings.ResultsInNotification;
            }
            set
            {
                _settings.ResultsInNotification = value;
                base.RaisePropertyChanged();
            }
        }

        public bool FixturesInNotification
        {
            get
            {
                return _settings.FixturesInNotification;
            }
            set
            {
                _settings.FixturesInNotification = value;
                base.RaisePropertyChanged();
            }
        }
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => "Márton Páli (marci-95@hotmail.com)";

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }
    }
}

