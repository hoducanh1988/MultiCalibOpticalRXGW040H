using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MultiCalibOpticalRXGW040H.Function;

namespace MultiCalibOpticalRXGW040H
{
    /// <summary>
    /// Interaction logic for waitWindow.xaml
    /// </summary>
    public partial class waitWindow : Window
    {

        int _count = 0;
        DispatcherTimer timer = null;

        public waitWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            globalData.mainWindowINFO.OPACITY = 0.5;

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            _count = 0;
            timer.Start();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) {
            globalData.mainWindowINFO.OPACITY = 1;
            timer.Stop();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            this._count++;
            this.lblwait.Content = string.Format("Please wait...{0}", this._count);
        }
    }
}
