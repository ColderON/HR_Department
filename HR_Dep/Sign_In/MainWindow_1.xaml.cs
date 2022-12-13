using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
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

namespace Sign_In
{    
    /// <summary>
    /// Interaction logic for MainWindow_1.xaml
    /// </summary>
    public partial class MainWindow_1 : Window
    {   
        public MainWindow_1()
        {
            InitializeComponent();
            BIG_Helper.CreateConnection();
            try { BIG_Helper.conn.Open(); }
            catch (DbException ex) { MessageBox.Show(ex.Message); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {         
            Main.Content = new Page_NewEmloyee();
        }

        private void btnNewEmpl_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Page_NewEmloyee();
        }

        private void btnShowEmpl_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new Page_Emloyees();
        }
    }
}
