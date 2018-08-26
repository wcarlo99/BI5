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
    /// Interaction logic for UCgiocatore.xaml
    /// </summary>
    public partial class UCgiocatore : UserControl
    {

        public Dictionary<Carta, UCcarta> dict { get; set; }

        public Giocatore G { get; set; }

        public UCgiocatore(Giocatore g)
        {
            InitializeComponent();

            dict = new Dictionary<Carta, UCcarta>();

            G = g;

            LABEL_name.DataContext = g;

            foreach (var c in g.carte_in_mano)
            {
                UCcarta ca = new UCcarta(c);
                ca.GetMessage = g.GiocaCartaUmano;
                LV.Children.Add(ca);
                dict.Add(c, ca);
            }

            if (g.IsComputer)
            {
                LV.IsEnabled = false;
            }
            else
            {
                LV.IsEnabled = true;
            }
        }

        public void SetTurno(bool t)
        {
            Turno.SetTurno(t);
        }        

        public void GiocaCarta(Carta cc)
        {
            UCcarta target = null;

            foreach (var uc in LV.Children)
            {
                if (uc is UCcarta)
                {
                    var ucc = uc as UCcarta;
                    if (ucc.C == cc)
                    {
                        target = ucc;
                        break;
                    }
                }
            }

            LV.Children.Remove(target);
            GRID_cartagiocata.Children.Clear();
            GRID_cartagiocata.Children.Add(target);
        }

    }
}
