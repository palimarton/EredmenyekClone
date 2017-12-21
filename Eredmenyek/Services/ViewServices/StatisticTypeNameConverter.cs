using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Eredmenyek.Services.ViewServices
{
    public class StatisticTypeNameConverter : IValueConverter
    {
        private Dictionary<string, string> _nameDictionary = new Dictionary<string, string>
        {
            { "foulcommit",     "Foul Commited"     },
            { "throwin",        "Throw In"          },
            { "offside",        "Offside"           },
            { "cross",          "Cross"             },
            { "possession",     "Ball Possession"   },
            { "yellow_cards",   "Yellow Card"       },
            { "shotoff",        "Off Target"        },
            { "blocked_shots",  "Blocked"           },
            { "shoton",         "On Target"         },
            { "counter",        "Counter"           },
            { "corner",         "Corner"            },
            { "red_cards",      "Red Card"          },
            { "goal",           "Goal"              },
            { "pass",           "Pass"              },
            { "goalkick",       "Goalkick"          },
            { "treatment",      "Treatment"         },
            { "subst",          "Substitution"      },
            { "assists",        "Assist"            },
            { "freekick",       "Freekick"          },
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var key = value as string;
            return _nameDictionary[key];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
