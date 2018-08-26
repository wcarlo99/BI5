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
    /// Interaction logic for WNDgiocatori.xaml
    /// </summary>
    public partial class WNDgiocatori : Window
    {
        public WNDgiocatori()
        {

            InitializeComponent();

            if (MainWindow.TABLE.giocatori.Count == 0)
            {
                Giocatore G1 = new Giocatore();
                
                G1.nome = "Carlo1";
                G1.IsComputer = false;
                MainWindow.TABLE.giocatori.Add(G1);
                TB0.Text = G1.nome;

                G1 = new Giocatore();
                
                G1.nome = "Carlo2";
                G1.IsComputer = true;
                MainWindow.TABLE.giocatori.Add(G1);
                TB1.Text = G1.nome;

                G1 = new Giocatore();
                
                G1.nome = "Carlo3";
                G1.IsComputer = true;
                MainWindow.TABLE.giocatori.Add(G1);
                TB2.Text = G1.nome;

                G1 = new Giocatore();
                
                G1.nome = "Carlo4";
                G1.IsComputer = true;
                MainWindow.TABLE.giocatori.Add(G1);
                TB3.Text = G1.nome;

                G1 = new Giocatore();
                
                G1.nome = "Carlo5";
                G1.IsComputer = true;
                MainWindow.TABLE.giocatori.Add(G1);
                TB4.Text = G1.nome;
            }
           
        }

        private void BTN_ok_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.TABLE.giocatori[0].nome = TB0.Text;
            MainWindow.TABLE.giocatori[1].nome = TB1.Text;
            MainWindow.TABLE.giocatori[2].nome = TB2.Text;
            MainWindow.TABLE.giocatori[3].nome = TB3.Text;
            MainWindow.TABLE.giocatori[4].nome = TB4.Text;

            this.DialogResult = true;
        }
    }
}
