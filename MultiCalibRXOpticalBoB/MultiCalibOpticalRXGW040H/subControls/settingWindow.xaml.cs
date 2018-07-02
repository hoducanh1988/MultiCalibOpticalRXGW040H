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
using MultiCalibOpticalRXGW040H.Function;
using Microsoft.Win32;

namespace MultiCalibOpticalRXGW040H {
    /// <summary>
    /// Interaction logic for settingWindow.xaml
    /// </summary>
    public partial class settingWindow : Window {
        public settingWindow() {
            InitializeComponent();

            //Combobox
            this.cbbOntType.ItemsSource = Parameters.ListOntType;
            this.cbbDebug1.ItemsSource = Parameters.ListComport;
            this.cbbDebug2.ItemsSource = Parameters.ListComport;
            this.cbbDebug3.ItemsSource = Parameters.ListComport;
            this.cbbDebug4.ItemsSource = Parameters.ListComport;
            this.cbbBosaType.ItemsSource = Parameters.ListBosaVendor;
            //DataContext
            this.DataContext = globalData.initSetting;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "OK": {
                        Properties.Settings.Default.Save();
                        System.Windows.MessageBox.Show("Success.","Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        //Hien thi cac buoc test khi thay doi san pham test GW040H / GW020BoB
                        globalData.testingDataDut1.ONTTYPE = globalData.initSetting.ONTTYPE;
                        globalData.testingDataDut2.ONTTYPE = globalData.initSetting.ONTTYPE;
                        globalData.testingDataDut3.ONTTYPE = globalData.initSetting.ONTTYPE;
                        globalData.testingDataDut4.ONTTYPE = globalData.initSetting.ONTTYPE;

                        this.Close();
                        baseFunction.loadBosaReport();
                        globalData.mainWindowINFO.WINDOWTITLE = string.Format("PHẦN MỀM CALIBRATION RX QUANG ONT {0}", globalData.initSetting.ONTTYPE);
                        break;
                    }
                case "Default": {
                        if (System.Windows.MessageBox.Show("Bạn muốn thiết lập giá trị cài đặt về mặc định phải ko?\n----------------------------\nChọn 'YES' để thiết lập mặc định.\nChọn 'NO' để hủy.", "SET DEFAULT", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                            globalData.initSetting.setDefault();
                        }
                        break;
                    }
                case "Cancel": {
                        Properties.Settings.Default.Reload();
                        this.Close();
                        break;
                    }
                case "...": {
                        OpenFileDialog dlg = new OpenFileDialog();
                        dlg.Filter = "File bosa report (*.xlsx)|*.xlsx";
                        dlg.Title = "Select a file BOSA Report";
                        if (dlg.ShowDialog() == true) {
                            globalData.initSetting.BOSAREPORTFILE = dlg.FileName;
                        }
                        break;
                    }

            }
        }
    }
}
