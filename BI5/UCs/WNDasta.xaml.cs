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

using BI5.DMs;

namespace BI5.UCs
{
    /// <summary>
    /// Interaction logic for WNDasta.xaml
    /// </summary>
    public partial class WNDasta : Window
    {
        DC_partita partita;

        public WNDasta(DC_partita p)
        {
            InitializeComponent();

            partita = p;

            CB_chiamante.DisplayMemberPath = "nome";
            CB_chiamante.ItemsSource = MainWindow.TABLE.giocatori;

            List<SEMI> semi = new List<SEMI>();
            semi.Add(SEMI.BASTONI);
            semi.Add(SEMI.COPPE);
            semi.Add(SEMI.ORI);
            semi.Add(SEMI.SPADE);

            CB_seme.ItemsSource = semi;

            List<int> valori = new List<int>();
            for (int i = 1; i < 11; i++)
            {
                valori.Add(i);
            }

            CB_carta.ItemsSource = valori;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            partita.chiamante = (Giocatore)CB_chiamante.SelectedItem;
            partita.primo_nella_mano = partita.chiamante;
            partita.chiamata = int.Parse(TB_chiamata.Text);

            partita.briscola = (SEMI)CB_seme.SelectedItem;
            int valore = (int)CB_carta.SelectedItem;

            partita.carta_chiamata = (from g in MainWindow.TABLE.giocatori
                                      from c in g.carte_in_mano
                                      where c.seme == partita.briscola &&
                                      c.numerico == valore
                                      select c).Single();

            partita.chiamato = (from g in MainWindow.TABLE.giocatori
                                where g.carte_in_mano.Contains(partita.carta_chiamata)
                                select g).Single();

            this.DialogResult = true;
        }
    }
}
