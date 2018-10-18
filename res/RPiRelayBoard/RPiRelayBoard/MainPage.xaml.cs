using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RPiRelayBoard
{
    public sealed partial class MainPage : Page
    {
        private RPiRelayBoard board;

        public MainPage()
        {
            InitializeComponent();
            board = new RPiRelayBoard();

            cboRelay.SelectionChanged += UpdateButtonContent;
            btnRelayControl.Click += UpdateButtonContent;
            btnRelayToggle.Click += UpdateButtonContent;
            btnOnAllRelays.Click += UpdateButtonContent;
            btnOffAllRelays.Click += UpdateButtonContent;
        }

        private void Grid_Loading(FrameworkElement sender, object args)
        {
            for (int i = 0; i < RPiRelayBoard.RelayCount; i++)
            {
                cboRelay.Items.Add(i + 1);
            }
        }

        private void btnControlRelay_Click(object sender, RoutedEventArgs e)
        {
            var relay = 0;
            if (cboRelay.SelectedItem != null 
                && !int.TryParse(cboRelay.SelectedItem.ToString(), out relay))
            {
                return;
            }

            switch (btnRelayControl.Content)
            {
                case "On":
                    board.OnRelay(relay);
                    break;

                case "Off":
                    board.OffRelay(relay);
                    break;
            }
        }

        private void btnRelayToggle_Click(object sender, RoutedEventArgs e)
        {
            var relay = 0;
            if (cboRelay.SelectedItem != null 
                &&!int.TryParse(cboRelay.SelectedItem.ToString(), out relay))
            {
                return;
            }

            board.ToggleRelay(relay);
        }

        private void btnOnAllRelays_Click(object sender, RoutedEventArgs e)
        {
            board.OnAllRelays();
        }

        private void btnOffAllRelays_Click(object sender, RoutedEventArgs e)
        {
            board.OffAllRelays();
        }

        private void UpdateButtonContent(object sender, RoutedEventArgs e)
        {
            var relay = 0;
            if (cboRelay.SelectedItem != null
                && !int.TryParse(cboRelay.SelectedItem.ToString(), out relay))
            {
                return;
            }

            if (board.IsRelayOpen(relay))
            {
                btnRelayControl.Content = "Off";
            }
            else
            {
                btnRelayControl.Content = "On";
            }
        }
    }
}
