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

namespace MultiCalibOpticalRXGW040H {
    /// <summary>
    /// Interaction logic for viewWindow.xaml
    /// </summary>
    public partial class viewWindow : Window {
        public viewWindow() {
            InitializeComponent();
            this.dgBosaReport.ItemsSource = globalData.listBosaInfo;
            this.lblCount.Content = string.Format("Total Bosa SN: {0}", globalData.listBosaInfo.Count);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "Search": {
                        if (txtBosaSN.Text.Trim().Length == 0) return;
                        this.dgBosaReport.SelectedItems.Clear();
                        bool _flag = false;
                        foreach (var item in globalData.listBosaInfo) {
                            if (item.BosaSN == txtBosaSN.Text) {
                                _flag = true;
                                this.dgBosaReport.Focus();
                                this.dgBosaReport.SelectedItem = item;
                                this.dgBosaReport.ScrollIntoView(item);
                                break;
                            }
                        }

                        if (_flag == false)
                            MessageBox.Show(string.Format("Mã Bosa Serial '{0}' không tồn tại.", txtBosaSN.Text),"SEARCH", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
            }
        }
    }
}
