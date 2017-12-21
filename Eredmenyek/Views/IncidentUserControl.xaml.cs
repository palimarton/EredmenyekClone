using Common.DataModels.DetailsModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Automation.Peers;

namespace Eredmenyek.Views
{
    public sealed partial class IncidentUserControl : UserControl
    {
        public string IncidentType
        {
            get { return (string)GetValue(IncidentTypeProperty); }
            set
            {
                SetValue(IncidentTypeProperty, value);
                
            }
        }
        
        public static readonly DependencyProperty IncidentTypeProperty =
            DependencyProperty.Register("IncidentType", typeof(string), typeof(IncidentUserControl), new PropertyMetadata(0, IncidentType_Changed));

        private static void IncidentType_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = (IncidentUserControl)d;
            uc.IncidentImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{e.NewValue.ToString()}_15x15.png"));
        }

        public bool IsHome
        {
            get { return (bool)GetValue(IsHomeProperty); }
            set
            {
                SetValue(IsHomeProperty, value);
            }
        }

        public static readonly DependencyProperty IsHomeProperty =
            DependencyProperty.Register("IsHome", typeof(bool), typeof(IncidentUserControl), new PropertyMetadata(0, IsHome_Changed));

        private static void IsHome_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = (IncidentUserControl)d;
            if ((bool)e.NewValue)
            {
                //Grid.SetColumn(uc.StackPanel, 0);
                RelativePanel.SetAlignLeftWithPanel(uc.StackPanel, true);
                RelativePanel.SetAlignRightWithPanel(uc.StackPanel, false);
            }
            else
            {
                //Grid.SetColumn(uc.StackPanel, 1);
                RelativePanel.SetAlignLeftWithPanel(uc.StackPanel, false);
                RelativePanel.SetAlignRightWithPanel(uc.StackPanel, true);
            }
        }

        public IncidentUserControl()
        {
            InitializeComponent();
        }

        
    }
}
