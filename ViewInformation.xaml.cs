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

namespace GuestInfoEntry
{
    /// <summary>
    /// Interaction logic for ViewInformation.xaml
    /// </summary>
    public partial class ViewInformation : Window
    {
        Show s = new Show();
        public ViewInformation()
        {
            InitializeComponent();
        }

        private void backList_Click(object sender, RoutedEventArgs e)
        {
            s.Showdata();
            s.Show();
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
