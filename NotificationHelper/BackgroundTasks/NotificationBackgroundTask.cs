using Common.DataModels.EventModels;
using Common.DataModels.Infrasturcture;
using Common.DataModels.VisualModels;
using Common.Services.EventServices;
using Common.Services.SettingsServices;
using Common.Services.UserServices;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace NotificationHelper.BackgroundTasks
{
    public sealed class NotificationBackgroundTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        IEventService _eventService;        
        IUserService _userService;
        SettingsService _settingsService;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            _eventService = new EventService();
            _settingsService = SettingsService.Instance;
            _userService = new UserService();
            
            if (_settingsService.NotificationsEnabled)
            {
                var messages = await GetNotificationMessages();
                UpdateNotification(messages);
                UpdateLiveTile(messages);
            }                        

            _deferral.Complete();
        }

        public static async void RegisterBackgroundTask()
        {
            var taskRegistered = BackgroundTaskRegistration.AllTasks.Values
                .Any(a => a.Name.Equals(nameof(NotificationBackgroundTask)));

            if (await BackgroundExecutionManager.RequestAccessAsync() == BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                return;
            }
            if (!taskRegistered)
            {
                var builder = new BackgroundTaskBuilder()
                {
                    Name = $"{nameof(NotificationBackgroundTask)}",
                    TaskEntryPoint = $"{nameof(NotificationHelper)}.{nameof(BackgroundTasks)}.{nameof(NotificationBackgroundTask)}"
                };
                builder.SetTrigger(new TimeTrigger(15, false));
                builder.Register();
            }
        }

        private async Task<List<ResponsiveNotificationText>> GetNotificationMessages()
        {
            var matches = new List<ResponsiveNotificationText>();
            var results = new ResponsiveNotificationText()
            {
                MatchType = "Recent Results: ",
                Matches = new List<string>()
            };
            var fixtures = new ResponsiveNotificationText()
            {
                MatchType = "Starting Soon:",
                Matches = new List<string>()
            };

            var nowMinusFifteenMinutes = DateTime.Now.AddMinutes(-15);
            var nowPlusFifteenMinutes = DateTime.Now.AddMinutes(15);
            string month = DateTime.Now.Month.ToString();
            month = month.Length == 1 ? "0" + month : month;
            string day = DateTime.Now.Day.ToString();
            day = day.Length == 1 ? "0" + day : day;
            var today = $"{DateTime.Now.Year}-{month}-{day}";

            foreach (var tournamentFK in Constants.GetTournamentStageFKs())
            {
                if (_settingsService.ResultsInNotification)
                {
                    var recentResults = await _eventService.GetResultEventsAsync(tournamentFK, "", today, "");
                    var resultsInPreviousMinutes = new List<Event>();
                    if (recentResults != null)
                    {
                        resultsInPreviousMinutes = recentResults.events.Values
                            .Where(w => w.property.Values
                                .Any(a => a.name.Equals("GameEnded")
                                    && DateTime.Parse(a.value) >= nowMinusFifteenMinutes)).ToList();
                    }
                    var filteredResults = new List<Event>();
                    if (_settingsService.FavouriteTeamNotificationsOnly)
                    {
                        var favouriteTeams = (await _userService.GetUser()).FavouriteTeams?.FavouriteTeams.Where(w => w.LeagueId.Equals(tournamentFK)).ToList();
                        if (favouriteTeams.Any())
                        {
                            foreach (var favouriteTeam in favouriteTeams)
                            {
                                var match = resultsInPreviousMinutes
                                    .Where(w => 
                                        w.event_participants.ElementAt(0).Value.participantFK.Equals(favouriteTeam.ParticipantId) ||
                                        w.event_participants.ElementAt(1).Value.participantFK.Equals(favouriteTeam.ParticipantId))
                                    .FirstOrDefault();
                                if (match != null)
                                {
                                    filteredResults.Add(match);
                                }
                            }
                        }                        
                    }
                    else
                    {
                        filteredResults = resultsInPreviousMinutes;
                    }
                    if (filteredResults.Any())
                    {
                        foreach (var match in filteredResults)
                        {
                            var homeTeam = match.event_participants.Values.ElementAt(0);
                            var guestTeam = match.event_participants.Values.ElementAt(1);
                            results.Matches.Add($"{homeTeam.participant.name} - " +
                                                $"{guestTeam.participant.name} " +
                                                $"{homeTeam.result.Values.Where(w => w.result_code.Equals("finalresult")).Select(s => s.value).ElementAt(0)}:" +
                                                $"{guestTeam.result.Values.Where(w => w.result_code.Equals("finalresult")).Select(s => s.value).ElementAt(0)}");
                        }
                    }                    
                }
                
                if (_settingsService.FixturesInNotification)
                {
                    var upcomingFixtures = await _eventService.GetFixtureEventsAsync(tournamentFK, "", today, "");
                    var fixturesInNextMinutes = new List<Event>();
                    if (upcomingFixtures != null)
                    {
                        fixturesInNextMinutes = upcomingFixtures.events.Values
                            .Where(w => w.startdate <= nowPlusFifteenMinutes).ToList();
                    }
                    var filteredFixtures = new List<Event>();
                    if (_settingsService.FavouriteTeamNotificationsOnly)
                    {
                        var favouriteTeams = (await _userService.GetUser()).FavouriteTeams?.FavouriteTeams.Where(w => w.LeagueId.Equals(tournamentFK)).ToList();
                        if (favouriteTeams.Any())
                        {
                            foreach (var favouriteTeam in favouriteTeams)
                            {
                                var match = fixturesInNextMinutes
                                    .Where(w =>
                                        w.event_participants.ElementAt(0).Value.participantFK.Equals(favouriteTeam.ParticipantId) ||
                                        w.event_participants.ElementAt(1).Value.participantFK.Equals(favouriteTeam.ParticipantId))
                                    .FirstOrDefault();
                                if (match != null)
                                {
                                    filteredFixtures.Add(match);
                                }
                            }
                        }
                    }
                    else
                    {
                        filteredFixtures = fixturesInNextMinutes;
                    }
                    if (filteredFixtures.Any())
                    {
                        foreach (var match in filteredFixtures)
                        {
                            var homeTeam = match.event_participants.Values.ElementAt(0);
                            var guestTeam = match.event_participants.Values.ElementAt(1);
                            var minutes = match.startdate.Minute < 10 ? $"0{match.startdate.Minute}" : $"{match.startdate.Minute}";
                            var startingDate = $"{match.startdate.Hour}:{minutes}";
                            fixtures.Matches.Add($"{homeTeam.participant.name} - " +
                                                 $"{guestTeam.participant.name} " +
                                                 $"{startingDate}");
                        }
                    }
                }                
            }

            if (_settingsService.ResultsInNotification)
            {
                matches.Add(results);
            }
            if (_settingsService.FixturesInNotification)
            {
                matches.Add(fixtures);
            }           

            return matches;
        }

        private void UpdateNotification(List<ResponsiveNotificationText> messages)
        {
            var content = BuildNotification(messages);
            if (content != null)
            {
                ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
            }
        }

        private void UpdateLiveTile(List<ResponsiveNotificationText> messages)
        {
            var content = BuildLiveTile(messages);
            if (content != null)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(content.GetXml()));
            }
        }

        private ToastContent BuildNotification(List<ResponsiveNotificationText> messages)
        {
            ToastVisual root = new ToastVisual();
            ToastBindingGeneric binding = new ToastBindingGeneric();
            root.BindingGeneric = binding;

            var results = messages.ElementAt(0);
            var fixtures = messages.ElementAt(1);
            if (!fixtures.Matches.Any() && !results.Matches.Any())
            {
                return null;
            }

            foreach (var notificationGroup in messages)
            {
                if (notificationGroup.Matches.Any())
                {
                    var group = new AdaptiveGroup();
                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintTextStacking = AdaptiveSubgroupTextStacking.Center
                    };
                    subgroup.Children.Add(new AdaptiveText()
                    {
                        Text = notificationGroup.MatchType,
                        HintStyle = AdaptiveTextStyle.Subtitle
                    });
                    foreach (var match in notificationGroup.Matches)
                    {
                        subgroup.Children.Add(new AdaptiveText()
                        {
                            Text = match,
                            HintStyle = AdaptiveTextStyle.BaseSubtle
                        });
                    }
                    group.Children.Add(subgroup);
                    binding.Children.Add(group);
                }
            }
            return new ToastContent()
            {
                Visual = root
            };
        }

        private TileContent BuildLiveTile(List<ResponsiveNotificationText> messages)
        {
            var results = messages.ElementAt(0);
            var fixtures = messages.ElementAt(1);

            if (!results.Matches.Any() && !fixtures.Matches.Any())
            {
                return null;
            }

            TileVisual root = new TileVisual();

            TileBinding wideBinding = new TileBinding();
            TileBindingContentAdaptive wideBindingContent = new TileBindingContentAdaptive();
            wideBinding.Content = wideBindingContent;

            TileBinding largeBinding = new TileBinding();
            TileBindingContentAdaptive largeBindingContent = new TileBindingContentAdaptive();
            largeBinding.Content = largeBindingContent;

            foreach (var notificationGroup in messages)
            {
                if (notificationGroup.Matches.Any())
                {
                    var group = new AdaptiveGroup();
                    var subgroup = new AdaptiveSubgroup()
                    {
                        HintTextStacking = AdaptiveSubgroupTextStacking.Center
                    };
                    subgroup.Children.Add(new AdaptiveText()
                    {
                        Text = notificationGroup.MatchType,
                        HintStyle = AdaptiveTextStyle.Body
                    });
                    foreach (var match in notificationGroup.Matches)
                    {
                        subgroup.Children.Add(new AdaptiveText()
                        {
                            Text = match,
                            HintStyle = AdaptiveTextStyle.Caption
                        });
                    }
                    group.Children.Add(subgroup);
                    wideBindingContent.Children.Add(group);
                    largeBindingContent.Children.Add(group);
                }
            }
            root.TileLarge = largeBinding;
            root.TileWide = wideBinding;

            return new TileContent()
            {
                Visual = root
            };
        }
    }
}
