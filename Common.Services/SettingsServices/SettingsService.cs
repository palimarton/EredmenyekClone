namespace Common.Services.SettingsServices
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new SettingsService();
        Template10.Services.SettingsService.ISettingsHelper _helper;

        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool NotificationsEnabled
        {
            get
            {
                return _helper.Read(nameof(NotificationsEnabled), true);
            }
            set
            {
                _helper.Write(nameof(NotificationsEnabled), value);
            }
        }

        public bool FavouriteTeamNotificationsOnly
        {
            get
            {
                return _helper.Read(nameof(FavouriteTeamNotificationsOnly), true);
            }
            set
            {
                _helper.Write(nameof(FavouriteTeamNotificationsOnly), value);
            }
        }

        public bool ResultsInNotification
        {
            get
            {
                return _helper.Read(nameof(ResultsInNotification), true);
            }
            set
            {
                _helper.Write(nameof(ResultsInNotification), value);
            }
        }

        public bool FixturesInNotification
        {
            get
            {
                return _helper.Read(nameof(FixturesInNotification), true);
            }
            set
            {
                _helper.Write(nameof(FixturesInNotification), value);
            }
        }
    }
}

