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

namespace MultiCalibOpticalRXGW040H
{
    /// <summary>
    /// Interaction logic for inputBosaWindow.xaml
    /// </summary>
    public partial class inputBosaWindow : Window
    {

        string dutNumber = "";

        public inputBosaWindow(string _dutnumber)
        {
            InitializeComponent();
            this.dutNumber = string.Format("0{0}", _dutnumber);
            lblTitle.Content = string.Format("NHẬP BOSA SN CỦA ONT #0{0}", _dutnumber);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.txtBosaNumber.Focus();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            this.Close();
        }

        private void txtBosaNumber_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtBosaNumber.Text.Trim().Length > 0) tbMessage.Text = "";
        }

        private void txtBosaNumber_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                string _text = txtBosaNumber.Text.Trim();
                //check thong tin bosa
                if (!baseFunction.bosa_SerialNumber_Is_Correct(_text)) {
                    tbMessage.Text = string.Format("Bosa Serial Number: \"{0}\" không hợp lệ.\nVui lòng nhập lại.", _text);
                    txtBosaNumber.Clear();
                    txtBosaNumber.Focus();
                    return;
                }

                //get thong tin bosa
                switch (this.dutNumber) {
                    case "01": { globalData.testingDataDut1.BOSASERIAL = _text; break; }
                    case "02": { globalData.testingDataDut2.BOSASERIAL = _text; break; }
                    case "03": { globalData.testingDataDut3.BOSASERIAL = _text; break; }
                    case "04": { globalData.testingDataDut4.BOSASERIAL = _text; break; }
                    default: break;
                }

                //close form
                this.Close();
            }
        }
    }
}
