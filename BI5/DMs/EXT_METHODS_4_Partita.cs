using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI5.DMs
{
    public static class EXT_METHODS_4_Partita
    {

        public static Giocatore GetCompagno(this DC_partita p)
        {
            Giocatore rx = null;

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

            return rx;
        }

        public static List<Carta> GetBriscolePassate(this DC_partita p)
        {
            List<Carta> rx = (from mp in p.mani
                              from cg in mp.carte
                              where cg.carta.seme == p.briscola
                              orderby cg.carta.potere descending
                              select cg.carta).ToList();

            return rx;
        }

        public static List<Carta> GetBriscoleEspliciteNonPassate_DUE(this DC_partita p)
        {
            List<Carta> rx = (from c in p.chiamante.carte_in_mano
                              where c.seme == p.briscola &&
                              c.potere > p.carta_chiamata.potere
                              select c).ToList();

            List<Carta> rx2 = (from mp in p.mani
                               from cg in mp.carte
                               where cg.carta == p.carta_chiamata                               
                               select cg.carta).ToList();

            rx.AddRange(rx2);

            return rx;
        }

        public static List<Carta> GetCarichiPassati(this DC_partita p)
        {
            List<Carta> rx = (from mp in p.mani
                              from cg in mp.carte
                              where cg.carta.potere >= 9
                              orderby cg.carta.potere descending
                              select cg.carta).ToList();

            return rx;
        }

        public static int GetPunti_CHIAMANTE(this DC_partita p)
        {
            int rx = 0;

            var lista_punti = (from mp in p.mani
                               let vincente = mp.ValutaMano(p.briscola).vincente
                               where mp.carte.Count == 5 && vincente == p.chiamante                               
                               let somma = mp.carte.Sum(cc => cc.carta.punti)
                               select somma).ToList();

            if (lista_punti.Count > 0)
            {
                rx = lista_punti.Sum();
            }

            return rx;
        }

        public static int GetPunti_CHIAMATO(this DC_partita p)
        {
            int rx = 0;

            var compagno = p.GetCompagno();

            if (compagno != null)
            {

                var lista_punti = (from mp in p.mani
                                   let vincente = mp.ValutaMano(p.briscola).vincente
                                   where mp.carte.Count == 5 && vincente == compagno
                                   let somma = mp.carte.Sum(cc => cc.carta.punti)
                                   select somma).ToList();

                if (lista_punti.Count > 0)
                {
                    rx = lista_punti.Sum();
                }

            }

            return rx;
        }

        public static int GetPunti_DUE(this DC_partita p)
        {
            var punti_chiamante = p.GetPunti_CHIAMANTE();

            var punti_chiamato = p.GetPunti_CHIAMATO();

            return punti_chiamante + punti_chiamato;
        }

        public static int GetPunti_TRE(this DC_partita p)
        {

            int rx = 0;

            var compagno = p.GetCompagno();

            var lista_punti = (from mp in p.mani
                               let vincente = mp.ValutaMano(p.briscola).vincente
                               where mp.carte.Count == 5 && vincente != p.chiamante && vincente  != compagno
                               let somma = mp.carte.Sum(cc => cc.carta.punti)
                               select somma).ToList();

            if (lista_punti.Count > 0)
            {
                rx = lista_punti.Sum();
            }

            return rx;
            
        }

    }
}
