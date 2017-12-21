using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Common.DataModels.DetailsModels;

namespace Eredmenyek.Services.ViewServices
{
    public class IncidentTypeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GoalTemplate { get; set; }
        public DataTemplate SubstitutionTemplate { get; set; }
        public DataTemplate YellowCardTemplate { get; set; }
        public DataTemplate RedCardTemplate { get; set; }
        public DataTemplate MissedPenaltyTemplate { get; set; }
        public DataTemplate AwayGoalTemplate { get; set; }
        public DataTemplate AwaySubstitutionTemplate { get; set; }
        public DataTemplate AwayYellowCardTemplate { get; set; }
        public DataTemplate AwayRedCardTemplate { get; set; }
        public DataTemplate AwayMissedPenaltyTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var incidentView = item as IncidentViewModel;

            if (incidentView.IsHome)
            {
                if (incidentView.Type.Equals("goal")) return GoalTemplate;
                else if (incidentView.Type.Equals("substitution")) return SubstitutionTemplate;
                else if (incidentView.Type.Equals("yellow_card")) return YellowCardTemplate;
                else if (incidentView.Type.Equals("red_card")) return RedCardTemplate;
                else if (incidentView.Type.Equals("second_yellow_card")) return RedCardTemplate;
                else if (incidentView.Type.Equals("missed_penalty")) return MissedPenaltyTemplate;
            }
            else
            {
                if (incidentView.Type.Equals("goal")) return AwayGoalTemplate;
                else if (incidentView.Type.Equals("substitution")) return AwaySubstitutionTemplate;
                else if (incidentView.Type.Equals("yellow_card")) return AwayYellowCardTemplate;
                else if (incidentView.Type.Equals("red_card")) return AwayRedCardTemplate;
                else if (incidentView.Type.Equals("second_yellow_card")) return AwayRedCardTemplate;
                else if (incidentView.Type.Equals("missed_penalty")) return AwayMissedPenaltyTemplate;

            }
            
            return base.SelectTemplateCore(item, container);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var incidentView = item as IncidentViewModel;

            if (incidentView.IsHome)
            {
                if (incidentView.Type.Equals("goal")) return GoalTemplate;
                else if (incidentView.Type.Equals("substitution")) return SubstitutionTemplate;
                else if (incidentView.Type.Equals("yellow_card")) return YellowCardTemplate;
                else if (incidentView.Type.Equals("red_card")) return RedCardTemplate;
                else if (incidentView.Type.Equals("second_yellow_card")) return RedCardTemplate;
                else if (incidentView.Type.Equals("missed_penalty")) return MissedPenaltyTemplate;
            }
            else
            {
                if (incidentView.Type.Equals("goal")) return AwayGoalTemplate;
                else if (incidentView.Type.Equals("substitution")) return AwaySubstitutionTemplate;
                else if (incidentView.Type.Equals("yellow_card")) return AwayYellowCardTemplate;
                else if (incidentView.Type.Equals("red_card")) return AwayRedCardTemplate;
                else if (incidentView.Type.Equals("second_yellow_card")) return AwayRedCardTemplate;
                else if (incidentView.Type.Equals("missed_penalty")) return AwayMissedPenaltyTemplate;
            }

            return base.SelectTemplateCore(item);
        }
    }
}
