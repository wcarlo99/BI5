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

using BI5.DMs;
using BI5.DELEGATES;

namespace BI5.UCs
{
    /// <summary>
    /// Interaction logic for UCcarta.xaml
    /// </summary>
    public partial class UCcarta : UserControl
    {

        public Carta C { get; set; }
        public DMGetMessage GetMessage;

        public UCcarta(Carta c)
        {
            InitializeComponent();

            this.C = c;
            this.DataContext = c;

            var uriSource = new Uri(@"/BI5;component/CARDS/" + c.numerico + c.seme + ".png", UriKind.Relative);
            IMG.Source = new BitmapImage(uriSource);
        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetMessage(this, "PLAY_CARD");
        }
    }
}
