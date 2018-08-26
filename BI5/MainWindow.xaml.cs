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
using BI5.UCs;

namespace BI5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static MainWindow MAIN_WINDOW { get; set; }
        public static TABLE_CLASS TABLE { get; set; }

        public static ListBox LB { get; set; }
             

        public MainWindow()
        {
            InitializeComponent();

            MAIN_WINDOW = this;
            TABLE = new TABLE_CLASS();

            LB = LB_messages;
        }

        private void BTN_nuova_partita_Click(object sender, RoutedEventArgs e)
        {
            #region INIZIALIZZAZIONE

            TABLE.NuovaPartita();
            TABLE.partita.DistribuisciCarte();

            LV.Children.Clear();

            for (int i = 0; i < 5; i++)
            {
                var g = TABLE.giocatori[i];
                g.OrdinaCarte();
                g.UC = new UCgiocatore(g);
                g.Compagno = null;
                Grid.SetRow(g.UC, i);
                LV.Children.Add(g.UC);                    
            }

            #endregion            
        }

        private void BTN_asta_Click(object sender, RoutedEventArgs e)
        {
            UCs.WNDasta wnd = new WNDasta(TABLE.partita);
            if (wnd.ShowDialog() == true)
            {
                GRID_current_partita.DataContext = null;
                GRID_current_partita.DataContext = TABLE.partita;

                Carta c = TABLE.partita.carta_chiamata;

                var uriSource = new Uri(@"/BI5;component/CARDS/" + c.numerico + c.seme + ".png", UriKind.Relative);
                IMG_called.Source = new BitmapImage(uriSource);                
            }
        }

        private void BTN_inizia_Click(object sender, RoutedEventArgs e)
        {
            TABLE.partita.GiocaPartita();            
        }

        private void BTN_giocatori_Click(object sender, RoutedEventArgs e)
        {
            WNDgiocatori wnd = new WNDgiocatori();
            wnd.ShowDialog();

        }

        
    }
}
