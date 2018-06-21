using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MultiCalibOpticalRXGW040H.Function;

namespace MultiCalibOpticalRXGW040H {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private void setStartupLocation() {
            double scaleX = 1;
            double scaleY = 1;
            //double scaleX = 0.75;
            //double scaleY = 0.8;
            this.Height = SystemParameters.WorkArea.Height * scaleY;
            this.Width = SystemParameters.WorkArea.Width * scaleX;
            this.Top = (SystemParameters.WorkArea.Height * (1 - scaleY)) / 2;
            this.Left = (SystemParameters.WorkArea.Width * (1 - scaleX)) / 2;
        }


        public MainWindow() {
            InitializeComponent();
            setStartupLocation();
            this.DataContext = globalData.mainWindowINFO;

            waitWindow w = null;
            //load bosa report data
            Thread t = new Thread(new ThreadStart(() => {
                //Show waiting form
                App.Current.Dispatcher.Invoke(new Action(() => {
                    w = new waitWindow();
                    w.Show();
                }));

               //load bosa report
                Thread s = new Thread(new ThreadStart(() => {
                    if (globalData.initSetting.BOSAREPORTFILE.Length > 0) {
                        baseFunction.loadBosaReport();
                    }
                }));
                s.IsBackground = true;
                s.Start();

                while (s.IsAlive) Thread.Sleep(100) ;

                App.Current.Dispatcher.Invoke(new Action(() => {
                    w.Close();
                }));

            }));
            t.IsBackground = true;
            t.Start();
        }

      
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            Label l = sender as Label;
            switch (l.Content.ToString()) {
                case "X": {
                        this.Close();
                        break;
                    }
                default: {
                        break;
                    }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            MenuItem mi = sender as MenuItem;
            switch (mi.Header) {
                case "Setting": {
                        this.Opacity = 0.3;
                        settingWindow sw = new settingWindow();
                        sw.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }
                case "Exit": {
                        this.Close();
                        break;
                    }
                case "About": {
                        break;
                    }
                case "View": {
                        this.Opacity = 0.3;
                        viewWindow vw = new viewWindow();
                        vw.ShowDialog();
                        this.Opacity = 1;
                        break;
                    }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string buttonName = b.Name;
            string _index = buttonName.Substring(buttonName.Length - 1, 1);

            //this._resetDisplay(_index); //manual

            this.Opacity = 0.3;
            inputBosaWindow wb = new inputBosaWindow(_index);
            wb.ShowDialog();
            this.Opacity = 1;
        }
    }
}
