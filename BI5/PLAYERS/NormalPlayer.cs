using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BI5.DMs;

namespace BI5.PLAYERS
{

    public class NormalPlayer
    {

        public NormalPlayer() { }

        public int PLAY(TABLE_CLASS t, Giocatore gg)
        {

            #region INIT

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

            punti_chiamante = p.GetPunti_CHIAMANTE();

            #endregion

            punti_mancanti = chiamata - punti_chiamante;

            int mani_mancanti = p.mani.Count - 1;

            Giocatore compagno = p.GetCompagno();

            if (compagno != null)
            {
                punti_mancanti = punti_mancanti - dict[compagno];
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

            #endregion            

            #region GIOCO IN BASE A POSIZIONE E CARTE A TERRA

            #region GIOCO DA ULTIMO

            if (p.mano_corrente.SonoUltimo())
            {
                var situazione_attuale = p.mano_corrente.ValutaMano(p.briscola);

                #region SO GIA' CHI E' IL COMPAGNO

                if (compagno != null)
                {
                    if ((situazione_attuale.vincente == p.chiamante) || (situazione_attuale.vincente == compagno))
                    {
                        #region POSSO PRENDERE ?

                        var carte = gg.PossoPrendereSuperando();

                        if (carte.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(carte[0]);
                        }
                        else
                        {
                            carte = gg.PossoPrendereConBriscola();

                            if (carte.Count > 0)
                            {
                                int punti_in_gioco = situazione_attuale.punti;
                                int punti_dei_tre = p.GetPunti_TRE();
                                var briscola_maggiore = carte[0];

                                int punti_3_ipotetici = punti_in_gioco + punti_dei_tre + briscola_maggiore.punti;
                                int soglia_vittoria = 120 - p.chiamata;

                                var briscole_esplicite = p.GetBriscoleEspliciteNonPassate_DUE();

                                if ((punti_in_gioco >= 10) || (punti_3_ipotetici > soglia_vittoria) || (briscole_esplicite.Count >= (8 - p.mani.Count)))
                                {
                                    id_carta = gg.carte_in_mano.IndexOf(carte[0]);
                                }
                                else // NON VOGLIO PRENDERE QUINDI CERCO DI DARE MENO PUNTI POSSIBILI
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
                            else // NON POSSO PRENDERE QUINDI CERCO DI DARE MENO PUNTI POSSIBILI
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

                        #endregion
                    }
                    else
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

                #endregion

                #region IL COMPAGNO NON E' ANCORA NOTO

                else
                {
                    if (situazione_attuale.vincente == p.chiamante)
                    {
                        #region POSSO PRENDERE ?

                        var carte = gg.PossoPrendereSuperando();

                        if (carte.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(carte[0]);
                        }
                        else
                        {
                            carte = gg.PossoPrendereConBriscola();

                            if (carte.Count > 0)
                            {
                                int punti_in_gioco = situazione_attuale.punti;
                                int punti_dei_tre = p.GetPunti_TRE();
                                var briscola_maggiore = carte[0];

                                int punti_3_ipotetici = punti_in_gioco + punti_dei_tre + briscola_maggiore.punti;
                                int soglia_vittoria = 120 - p.chiamata;

                                var briscole_esplicite = p.GetBriscoleEspliciteNonPassate_DUE();

                                if ((punti_in_gioco >= 10) || (punti_3_ipotetici > soglia_vittoria) || (briscole_esplicite.Count >= (8 - p.mani.Count)))
                                {
                                    id_carta = gg.carte_in_mano.IndexOf(carte[0]);
                                }
                                else // NON VOGLIO PRENDERE QUINDI CERCO DI DARE MENO PUNTI POSSIBILI
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
                            else // NON POSSO PRENDERE QUINDI CERCO DI DARE MENO PUNTI POSSIBILI
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

                        #endregion
                    }
                    else
                    {
                        #region POSSO PRENDERE ?

                        var carte = gg.PossoPrendereSuperando();

                        if (carte.Count > 0)
                        {
                            id_carta = gg.carte_in_mano.IndexOf(carte[0]);
                        }
                        else
                        {
                            carte = gg.PossoPrendereConBriscola();

                            if (carte.Count > 0)
                            {
                                int punti_in_gioco = situazione_attuale.punti;
                                int punti_dei_tre = p.GetPunti_TRE();
                                var briscola_maggiore = carte[0];

                                int punti_3_ipotetici = punti_in_gioco + punti_dei_tre + briscola_maggiore.punti;
                                int soglia_vittoria = 120 - p.chiamata;

                                var briscole_esplicite = p.GetBriscoleEspliciteNonPassate_DUE();

                                if ((punti_in_gioco >= 10) || (punti_3_ipotetici > soglia_vittoria) || (briscole_esplicite.Count >= (8 - p.mani.Count)))
                                {
                                    id_carta = gg.carte_in_mano.IndexOf(carte[0]);
                                }
                                else // NON VOGLIO PRENDERE QUINDI CERCO DI DARE MENO PUNTI POSSIBILI
                                {
                                    if (punti_in_mano.Count > 0)
                                    {
                                        id_carta = gg.carte_in_mano.IndexOf(punti_in_mano[0]);
                                    }
                                    else if (lisci_in_mano.Count > 0)
                                    {
                                        id_carta = gg.carte_in_mano.IndexOf(lisci_in_mano[0]);
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
                            else // NON POSSO PRENDERE QUINDI CERCO DI DARE MENO PUNTI POSSIBILI
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

                        #endregion
                    }
                }

                #endregion

            }

            #endregion

            #region SONO IL PRIMO

            else if (p.mano_corrente.SonoPrimo())
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

            #endregion

            #region GIOCO IN MEZZO

            else // CODICE PROVVISORIO
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

            #endregion

            #endregion



            return id_carta;

        }

    }

    

}
