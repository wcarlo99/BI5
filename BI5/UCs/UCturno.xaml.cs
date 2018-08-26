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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BI5.UCs
{
    /// <summary>
    /// Interaction logic for UCturno.xaml
    /// </summary>
    public partial class UCturno : UserControl
    {
        public UCturno()
        {
            InitializeComponent();
        }

        public void SetTurno(bool t)
        {
            if (t)
            {
                R_YES.Visibility = Visibility.Visible;
                R_NO.Visibility = Visibility.Collapsed;
            }
            else
            {
                R_NO.Visibility = Visibility.Visible;
                R_YES.Visibility = Visibility.Collapsed;
            }
        }

    }
}
