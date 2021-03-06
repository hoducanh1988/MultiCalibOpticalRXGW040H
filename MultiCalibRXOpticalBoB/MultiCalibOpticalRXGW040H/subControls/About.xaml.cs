﻿using System;
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

namespace MultiCalibOpticalRXGW040H
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About() {
            InitializeComponent();
            listHist.Add(new history() { ID = "1", VERSION = "1.0.0.0", CONTENT = "- Phát hành lần đầu", DATE = "2018", CHANGETYPE = "Tạo mới", PERSON = "Trần Đức Hòa" });
            listHist.Add(new history() { ID = "2", VERSION = "1.0.0.1", CONTENT = "- Chỉnh sửa ...", DATE = "2018", CHANGETYPE = "Chỉnh sửa", PERSON = "Trần Đức Hòa" });
            listHist.Add(new history() { ID = "3", VERSION = "1.0.0.2", CONTENT = "- Chuyển giao diện phần mềm từ Winform sang WPF (fix lỗi đơ)", DATE = "20/06/2018", CHANGETYPE = "Chỉnh sửa", PERSON = "Hồ Đức Anh" });
            listHist.Add(new history() { ID = "4", VERSION = "1.0.0.3", CONTENT = "- Chỉnh sửa lệnh Save Flash chờ đến khi nhận được dấu #. Nếu quá 5s từ ONT ko có phản hồi sẽ báo FAIL.", DATE = "26/06/2018", CHANGETYPE = "Chỉnh sửa", PERSON = "Hồ Đức Anh" });
            listHist.Add(new history() { ID = "5", VERSION = "1.0.0.4", CONTENT = "- Chỉnh sửa write flash. thiết lập biết Result = true khi check PASS.", DATE = "29/06/2018", CHANGETYPE = "Chỉnh sửa", PERSON = "Hồ Đức Anh" });
            listHist.Add(new history() {
                ID = "6",
                VERSION = "1.0.0.5",
                CONTENT = "- Tích hợp thêm code calib quang RX GW020BoB.",
                DATE = "05/07/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            listHist.Add(new history() {
                ID = "7",
                VERSION = "1.0.0.6",
                CONTENT = "- Tăng thời gian delay đọc PartemAligned từ 600ms -> 900ms",
                DATE = "19/07/2018",
                CHANGETYPE = "Chỉnh sửa",
                PERSON = "Hồ Đức Anh"
            });

            this.GridAbout.ItemsSource = listHist;
        }

        List<history> listHist = new List<history>();

        private class history {
            public string ID { get; set; }
            public string VERSION { get; set; }
            public string CONTENT { get; set; }
            public string DATE { get; set; }
            public string CHANGETYPE { get; set; }
            public string PERSON { get; set; }
        }
    }
}
