using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BI5.DMs;

namespace BI5.PLAYERS
{
    public class ChiamantePlayer : IPlayer
    {

        public ChiamantePlayer() { }

        public int PLAY(TABLE_CLASS t, Giocatore gg)
        {

            int id_carta = 0;

            int punti_chiamante = 0;

            int chiamata = t.partita.chiamata;

            int punti_mancanti = 0;

            #region punti attuali chiamante 

            Dictionary<Giocatore, int> dict = new Dictionary<Giocatore, int>();

            DC_partita p = t.partita;

            foreach (var g in t.giocatori)
            {
                dict.Add(g, 0);
            }

            foreach (var m in p.mani)
            {
                if (m.carte.Count > 0)
                {
                    var rx = m.ValutaMano(p.briscola);
                    dict[rx.vincente] += rx.punti;
                }
            }

            punti_chiamante = dict[p.chiamante];

            #endregion

            punti_mancanti = chiamata - punti_chiamante;

            int mani_mancanti = p.mani.Count - 1;

            Giocatore comp = null;

            #region il compagno e' uscito alla scoperta ?

            foreach (var m in p.mani)
            {
                foreach (var carta in m.carte)
                {
                    if (carta.carta == p.carta_chiamata)
                    {
                        comp = carta.giocatore;
                        break;
                    }
                }
            }

            #endregion

            if (comp != null)
            {
                punti_mancanti = punti_mancanti - dict[comp];
            }

            var briscole_in_mano = (from ci in gg.carte_in_mano
                                    where ci.seme == p.briscola
                                    orderby ci.potere descending
                                    select ci).ToList();

            var carichi_in_mano = (from ci in gg.carte_in_mano
                                    where ci.seme != p.briscola && 
                                    ci.potere > 8
                                    orderby ci.potere descending
                                   select ci).ToList();

            var punti_in_mano = (from ci in gg.carte_in_mano
                                   where ci.seme != p.briscola &&
                                   ci.potere <= 8 && ci.potere >= 6
                                   orderby ci.potere descending
                                 select ci).ToList();

            var lisci_in_mano = (from ci in gg.carte_in_mano
                                 where ci.seme != p.briscola &&
                                 ci.potere < 6 
                                 orderby ci.potere descending
                                 select ci).ToList();

            var briscole_passate = (from mp in p.mani
                                    from cg in mp.carte
                                    where cg.carta.seme == p.briscola
                                    orderby cg.carta.potere descending
                                    select cg.carta).ToList();

            bool carta_chiamata_passata = briscole_passate.Contains(p.carta_chiamata);

            if (punti_mancanti <= 0)
            {
                // TENTATIVO DI CAPPOTTO
            }
            else
            {

                #region se ho in mano tutte le briscole rimaste

                if (briscole_in_mano.Count > 0)
                {

                    if ((briscole_in_mano.Count + briscole_passate.Count == 10) ||
                        ((briscole_in_mano.Count + briscole_passate.Count == 9) && (!carta_chiamata_passata)))
                    {

                        if ((briscole_in_mano.Count >= mani_mancanti) ||
                            ((briscole_in_mano.Count == mani_mancanti - 1) && (!carta_chiamata_passata)))
                        {
                            id_carta = gg.carte_in_mano.IndexOf(briscole_in_mano[0]);
                        }

                    }

                }

                #endregion

                // se sono l'ultimo a giocare
                else if (p.mano_corrente.turno.Count == 0)
                {
                    // se il mio compagno ha giocato
                    if (comp != null)
                    {
                        var rx_mano_attuale = p.mano_corrente.ValutaMano(p.briscola);

                        if (rx_mano_attuale.vincente == comp)
                        {
                            if (carichi_in_mano.Count > 0)
                            {
                                id_carta = gg.carte_in_mano.IndexOf(carichi_in_mano[0]);
                            }
                            else if (punti_in_mano.Count > 0)
                            {
                                id_carta = gg.carte_in_mano.IndexOf(punti_in_mano[0]);
                            }
                            else if (lisci_in_mano.Count > 0)
                            {
                                id_carta = gg.carte_in_mano.IndexOf(lisci_in_mano[0]);
                            }
                            else if (briscole_in_mano.Count > 0)
                            {
                                id_carta = gg.carte_in_mano.IndexOf(briscole_in_mano.Last());
                            }
                        }
                    }
                }

                // momentaneamente cerco di prendere
                else
                {

                    List<Carta> rxx = new List<Carta>();
                    var rx = p.mano_corrente.ValutaMano(p.briscola);

                    Carta c = rx.carta;

                    foreach (var cm in gg.carte_in_mano)
                    {
                        if ((cm.seme == c.seme) && (cm.potere > c.potere))
                        {
                            rxx.Add(cm);
                        }
                    }

                    if (c.seme != p.briscola)
                    {
                        foreach (var cm in gg.carte_in_mano)
                        {
                            if (cm.seme == p.briscola)
                            {
                                rxx.Add(cm);
                            }
                        }
                    }

                    if (rxx.Count > 0)
                    {
                        var carta_minima_no_briscola = (from cg in rxx
                                                        let min = rxx.Min(ccg => ccg.potere)
                                                        where cg.potere == min && cg.seme != p.briscola
                                                        select cg).ToList();

                        if (carta_minima_no_briscola.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(carta_minima_no_briscola[0]);
                        }
                        else
                        {
                            var carta_minima_briscola = (from cg in rxx
                                                         let min = rxx.Min(ccg => ccg.potere)
                                                         where cg.potere == min && cg.seme == p.briscola
                                                         select cg).ToList();

                            if (carta_minima_briscola.Count > 0)
                            {
                                id_carta = gg.carte_in_mano.IndexOf(carta_minima_briscola[0]);
                            }
                        }
                    }
                    else
                    {

                        if (lisci_in_mano.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(lisci_in_mano[0]);
                        }
                        else if (punti_in_mano.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(punti_in_mano[0]);
                        }
                        else if (carichi_in_mano.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(carichi_in_mano[0]);
                        }                       
                        else if (briscole_in_mano.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(briscole_in_mano.Last());
                        }

                    }
                

                }

            }

            return id_carta;

        }

    }
}
