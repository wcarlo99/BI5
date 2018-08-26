using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Windows;

using BI5.UCs;
using BI5.PLAYERS;
using System.Threading;

namespace BI5.DMs
{

    public enum SEMI
    {
        BASTONI,
        COPPE,
        ORI,
        SPADE
    }

    public enum VALORI
    {
        ASSO,
        TRE,
        RE,
        CAVALLO,
        DONNA,
        SETTE,
        SEI,
        CINQUE,
        QUATTRO,
        DUE
    }

    public class Carta
    {
        public int Id { get; set; }
        public SEMI seme { get; set; }
        public VALORI valore { get; set; }
        public int potere { get; set; }
        public int numerico { get; set; }
        public string giocatada { get; set; }
        public int punti { get; set; }
    }

    public class TABLE_CLASS
    {
        public List<Giocatore> giocatori { get; set; }
        public List<DC_partita> serie { get; set; }
        public DC_partita partita { get; set; }

        public TABLE_CLASS()
        {
            giocatori = new List<Giocatore>();
            serie = new List<DC_partita>();
        }

        public void NuovaPartita()
        {
            partita = new DC_partita();
            serie.Add(partita);
        }
    }    

    public class DC_partita
    {
        
        

        private Mazzo mazzo { get; set; }
        public List<Mano> mani { get; set; }
        public Giocatore chiamante { get; set; }
        public Giocatore chiamato { get; set; }
        public int chiamata { get; set; }
        public Carta carta_chiamata { get; set; }
        public SEMI briscola { get; set; }
        
        public Giocatore primo_nella_mano { get; set; }
        public Mano mano_corrente { get; set; }

        public Carta carta_giocata_umano { get; set; }

        int id_mano_corrente = -1;

        public DC_partita()
        {            
            mazzo = new Mazzo();
            mani = new List<Mano>();
        }

        public void DistribuisciCarte()
        {

            for (int j = 0; j < 5; j++)
            {
                MainWindow.TABLE.giocatori[j].carte_in_mano.Clear();
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    MainWindow.TABLE.giocatori[j].carte_in_mano.Add(mazzo.Carte.Pop());
                }
            }            
        }

        public void SetTurno()
        {
            foreach (var g in MainWindow.TABLE.giocatori)
            {
                g.UC.SetTurno(false);
            }
            primo_nella_mano.UC.SetTurno(true);

            if (!primo_nella_mano.IsComputer)
                primo_nella_mano.UC.LV.IsEnabled = true;
        }

        public async void GiocaPartita()
        {
            for (int iii = 0; iii < 8; iii++)
            {
                
                #region CREO MANO

                PulisciSchermo();

                Mano m = new Mano();

                int i = MainWindow.TABLE.giocatori.IndexOf(primo_nella_mano);

                for (int j = i; j < MainWindow.TABLE.giocatori.Count; j++)
                {
                    m.turno.Enqueue(MainWindow.TABLE.giocatori[j]);
                }
                for (int j = 0; j < i; j++)
                {
                    m.turno.Enqueue(MainWindow.TABLE.giocatori[j]);
                }

                mani.Add(m);
                mano_corrente = m;

                #endregion

                while (m.turno.Count > 0)
                {

                    var g = mano_corrente.turno.Dequeue();
                    primo_nella_mano = g;
                    SetTurno();

                    Carta c = null;
                    carta_giocata_umano = null;
                    CancellationToken ct = new CancellationToken();

                    #region DEFINIZIONE DELLA CARTA DA GIOCARE

                    if (!g.IsComputer)
                    {
                        await Task.Run(async () =>
                        {
                            while (carta_giocata_umano == null)
                            {
                                await Task.Delay(1000, ct);
                            }
                        });

                        c = carta_giocata_umano;
                        carta_giocata_umano = null;
                    }
                    else
                    {
                        // c = g.GiocaCarta();
                        if (g == chiamante)
                        {
                            ChiamantePlayer cp = new ChiamantePlayer();
                            int carta_id = cp.PLAY(MainWindow.TABLE, g);
                            c = g.carte_in_mano[carta_id];
                        }
                        else
                        {
                            NormalPlayer np = new NormalPlayer();
                            int carta_id = np.PLAY(MainWindow.TABLE, g);
                            c = g.carte_in_mano[carta_id];
                        }
                    }

                    #endregion

                    g.carte_in_mano.Remove(c);

                    CartaGiocata cg = new CartaGiocata();
                    cg.carta = c;
                    cg.giocatore = g;
                    m.carte.Add(cg);

                    #region gioco la carta graficamente

                    UCcarta target = g.UC.dict[c];
                    g.UC.LV.Children.Remove(target);
                    g.UC.GRID_cartagiocata.Children.Clear();
                    g.UC.GRID_cartagiocata.Children.Add(target);

                    #endregion
                                        
                }

                ValutaMano();

            }

            MessageBox.Show("Partita Finita!");
            ValutaPartita();
        }

        



        public void ValutaPartita()
        {

            

            Dictionary<Giocatore, int> dict = new Dictionary<Giocatore, int>();
            foreach (var g in MainWindow.TABLE.giocatori)
            {
                dict.Add(g, 0);
            }

            foreach (var m in mani)
            {
                var rx = m.ValutaMano(briscola);
                dict[rx.vincente] += rx.punti;
            }

            int punti_2 = dict[chiamante];
            MainWindow.LB.Items.Add("PUNTI CHIAMANTE (" + chiamante.nome + ") = " + dict[chiamante]);
            if (chiamato != chiamante)
            {
                punti_2 += dict[chiamato];
                MainWindow.LB.Items.Add("PUNTI CHIAMATO (" + chiamato.nome + ") = " + dict[chiamato]);
            }

            MainWindow.LB.Items.Add("PUNTI COPPIA = " + punti_2);

            dict.Remove(chiamante);
            dict.Remove(chiamato);

            int punti_3 = 0;

            foreach (var kvp in dict)
            {
                punti_3 += kvp.Value;
                MainWindow.LB.Items.Add("PUNTI " + kvp.Key.nome + " = " + kvp.Value);
            }
            
            MainWindow.LB.Items.Add("PUNTI TRE = " + punti_3);

            if (punti_2 >= chiamata)
            {
                MainWindow.LB.Items.Add("VINCONO I DUE");
            }
            else
            {
                MainWindow.LB.Items.Add("VINCONO I TRE");
            }
        }

        public void ValutaMano()
        {

            MessageBox.Show("Mano finita");

            var rx = mano_corrente.ValutaMano(briscola);
            primo_nella_mano = rx.vincente;
            MainWindow.LB.Items.Add("Mano vinta da: " + rx.vincente.nome + " che totalizza: " + rx.punti);
            for (int i = 0; i < mano_corrente.carte.Count; i++)
            {
                MainWindow.LB.Items.Add("G: " + mano_corrente.carte[i].giocatore.nome +
                                        " - C: " + mano_corrente.carte[i].carta.valore + " " + mano_corrente.carte[i].carta.seme);
            }
        }

        public void PulisciSchermo()
        {
            // await Task.Delay(1000);

            for (int i = 0; i < 5; i++)
            {
                MainWindow.TABLE.giocatori[i].UC.GRID_cartagiocata.Children.Clear();
            }
        }

        //public void CreaMano()
        //{
            

        //    PulisciSchermo();

        //    Mano m = new Mano();

        //    int i = MainWindow.TABLE.giocatori.IndexOf(primo_nella_mano);

        //    for (int j=i; j < MainWindow.TABLE.giocatori.Count; j++)
        //    {
        //        m.turno.Enqueue(MainWindow.TABLE.giocatori[j]);
        //    }
        //    for (int j = 0; j < i; j++)
        //    {
        //        m.turno.Enqueue(MainWindow.TABLE.giocatori[j]);
        //    }

        //    mani.Add(m);
        //    mano_corrente = m;
        //}

    }

    public class Mano
    {
        public Queue<Giocatore> turno { get; set; }
        public List<CartaGiocata> carte { get; set; }
        public string stato { get; set; }

        public Mano()
        {
            turno = new Queue<Giocatore>();
            carte = new List<CartaGiocata>();
            stato = "INIZIALIZZATA";
        }

        public bool SonoPrimo()
        {
            return carte.Count == 0;
        }

        public bool SonoUltimo()
        {
            return carte.Count == 4;
        }

        public RisultatoMano ValutaMano(SEMI briscola)
        {
            RisultatoMano rx = new RisultatoMano();

            if (carte.Count > 0)
            {

                rx.punti = (from c in carte
                            select c.carta.punti).Sum();

                var briscola_presente = (from c in carte
                                         where c.carta.seme == briscola
                                         orderby c.carta.potere descending
                                         select c).ToList();

                if (briscola_presente.Count > 0)
                {
                    rx.vincente = briscola_presente[0].giocatore;
                    rx.carta = briscola_presente[0].carta;
                }
                else
                {
                    rx.vincente = (from c in carte
                                   where c.carta.seme == carte[0].carta.seme
                                   orderby c.carta.potere descending
                                   select c.giocatore).ToList()[0];
                    rx.carta = (from c in carte
                                where c.carta.seme == carte[0].carta.seme
                                orderby c.carta.potere descending
                                select c.carta).ToList()[0];
                }

            }

            return rx;
        }
    }

    public class RisultatoMano
    {
        public Giocatore vincente { get; set; }
        public int punti { get; set; }
        public Carta carta { get; set; }

        public RisultatoMano() { }
    }

    public class CartaGiocata
    {
        public Carta carta { get; set; }
        public Giocatore giocatore { get; set; }

        public CartaGiocata() { }
    }

    public class Mazzo
    {

        private List<Carta> Carte_tmp { get; set; }
        public Stack<Carta> Carte { get; set; }

        public Mazzo()
        {
            Carte_tmp = new List<Carta>();

            Init();
            Mischia();
        }

        private void Init()
        {
            List<SEMI> semi = new List<SEMI>();
            semi.Add(SEMI.BASTONI);
            semi.Add(SEMI.COPPE);
            semi.Add(SEMI.ORI);
            semi.Add(SEMI.SPADE);

            foreach (var seme in semi)
            {

                #region CARTE

                Carta c = new Carta();
                c.seme = seme;
                c.valore = VALORI.ASSO;
                c.potere = 10;
                c.numerico = 1;
                c.punti = 11;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.TRE;
                c.potere = 9;
                c.numerico = 3;
                c.punti = 10;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.RE;
                c.potere = 8;
                c.numerico = 10;
                c.punti = 4;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.CAVALLO;
                c.potere = 7;
                c.numerico = 9;
                c.punti = 3;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.DONNA;
                c.potere = 6;
                c.numerico = 8;
                c.punti = 2;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.SETTE;
                c.potere = 5;
                c.numerico = 7;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.SEI;
                c.potere = 4;
                c.numerico = 6;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.CINQUE;
                c.potere = 3;
                c.numerico = 5;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.QUATTRO;
                c.potere = 2;
                c.numerico = 4;
                Carte_tmp.Add(c);

                c = new Carta();
                c.seme = seme;
                c.valore = VALORI.DUE;
                c.potere = 1;
                c.numerico = 2;
                Carte_tmp.Add(c);

                #endregion

            }
        }

        private void Mischia()
        {
            Random rnd = new Random();

            for (int j = 0; j < 10000; j++)
            {
                for (int i = 0; i < 40; i++)
                {
                    int id = rnd.Next(0, 40);

                    Carta tmp = Carte_tmp[id];
                    Carte_tmp[id] = Carte_tmp[i];
                    Carte_tmp[i] = tmp;
                }
            }

            Carte = new Stack<Carta>();
            foreach (var c in Carte_tmp)
            {
                Carte.Push(c);
            }
        }
    }

    public class Giocatore
    {

        public string nome { get; set; }
        public List<Carta> carte_in_mano { get; set; }
        public List<Carta> carte_prese { get; set; }
        public UCs.UCgiocatore UC { get; set; }
        public bool IsComputer { get; set; }
        public Giocatore Compagno { get; set; }

        public Giocatore()
        {
            carte_in_mano = new List<Carta>();
            carte_prese = new List<Carta>();
        }

        public void OrdinaCarte()
        {
            var bastoni = (from c in carte_in_mano
                           where c.seme == SEMI.BASTONI
                           orderby c.potere descending
                           select c).ToList();

            var coppe = (from c in carte_in_mano
                         where c.seme == SEMI.COPPE
                         orderby c.potere descending
                         select c).ToList();

            var ori = (from c in carte_in_mano
                       where c.seme == SEMI.ORI
                       orderby c.potere descending
                       select c).ToList();

            var spade = (from c in carte_in_mano
                         where c.seme == SEMI.SPADE
                         orderby c.potere descending
                         select c).ToList();

            carte_in_mano.Clear();
            carte_in_mano.AddRange(bastoni);
            carte_in_mano.AddRange(coppe);
            carte_in_mano.AddRange(ori);
            carte_in_mano.AddRange(spade);
        }
          

        public void GiocaCartaUmano(object sender, string msg)
        {

            UCcarta c = sender as UCcarta;
            
            //UC.LV.IsEnabled = false;

            //UC.LV.Children.Remove(c);
            //carte_in_mano.Remove(c.C);
            //UC.GRID_cartagiocata.Children.Clear();
            //UC.GRID_cartagiocata.Children.Add(c);

            //CartaGiocata cg = new CartaGiocata();
            //cg.carta = c.C;
            //cg.giocatore = this;

            //MainWindow.TABLE.partita.mano_corrente.carte.Add(cg);
            MainWindow.TABLE.partita.carta_giocata_umano = c.C;
        }

        public List<Carta> PossoPrendereConBriscola()
        {
            List<Carta> rxx = new List<Carta>();

            var p = MainWindow.TABLE.partita;

            if (!p.mano_corrente.SonoPrimo())
            {
                var rx = p.mano_corrente.ValutaMano(p.briscola);
                Carta c = rx.carta;

                foreach (var cm in carte_in_mano)
                {
                    if ((cm.seme == p.briscola) && (cm.potere > c.potere))
                    {
                        rxx.Add(cm);
                    }
                }
            }

            rxx = (from r in rxx
                   orderby r.potere descending
                   select r).ToList();

            return rxx;
        }

        public List<Carta> PossoPrendereSuperando()
        {
            List<Carta> rxx = new List<Carta>();

            var p = MainWindow.TABLE.partita;

            if (!p.mano_corrente.SonoPrimo())
            {
                var rx = p.mano_corrente.ValutaMano(p.briscola);
                Carta c = rx.carta;

                if (c.seme != p.briscola)
                {
                    foreach (var cm in carte_in_mano)
                    {
                        if ((cm.seme == c.seme) && (cm.potere > c.potere))
                        {
                            rxx.Add(cm);
                        }
                    }
                }
            }

            rxx = (from r in rxx
                   orderby r.potere descending
                   select r).ToList();

            return rxx;
        }

        private Giocatore WhoIsCompagno()
        {
            Giocatore rx = null;

            var p = MainWindow.TABLE.partita;

            foreach (var c in carte_in_mano)
            {
                if (c == p.carta_chiamata)
                {
                    rx = this;
                    break;
                }
            }

            if (rx == null)
            {
                foreach (var m in p.mani)
                {
                    foreach (CartaGiocata cg in m.carte)
                    {
                        if ((cg.carta.seme == p.carta_chiamata.seme) &&
                            (cg.carta.numerico == p.carta_chiamata.numerico))
                        {
                            rx = cg.giocatore;
                            break;
                        }
                    }
                }
            }

            return rx;
        }

    }

}
