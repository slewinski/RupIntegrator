using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KnsMigrator
{
    public class MiesPackHelper
    {
        public KnsMigratorEntities Context { get; set; }

        
        public Transfer setTransfer(int idKsiegi,DateTime dOPer, int typOper )
        {
            Konfiguracja konfig = Context.Konfiguracja.FirstOrDefault();
            DateTime dPocz;
            int PackNo = 0;
            return null;
            if (konfig.DataMiesStart.HasValue && konfig.DataMiesStart.Value > new DateTime(2016, 1, 1))
            {
                if (idKsiegi == 0) return null;
                dPocz = new DateTime(konfig.DataMiesStart.Value.Year, konfig.DataMiesStart.Value.Month, 1);
                KnsKsiegi ks = Context.KnsKsiegi.Where(a => a.Id_Ksiegi == idKsiegi).FirstOrDefault();
                if (ks != null)
                    PackNo = ks.czymies ?? 0;
                else
                    PackNo = 0;
                int rok = dOPer.Year;
                int mies = dOPer.Month;
                Transfer t = Context.Transfer.Where(a => a.PackNo == PackNo && a.Rok == rok & a.Miesiac == mies && a.rodzaj == typOper).FirstOrDefault();
                if (t == null) // zakładamy nowy
                {
                    t = new Transfer();
                    t.PackNo = PackNo;
                    t.Miesiac = mies;
                    t.rodzaj = typOper;
                    t.Rok = rok;
                    t.DataOd = dOPer;
                    t.DataDo = dOPer;
                    t.LFaktow = 0;
                    t.Kwota = 0;
                    t.Zaimportowane = 0;
                    t.Bledne = 0;
                    t.DataPOper = DateTime.Now;
                    t.Uwagi = ks.nazwa;
                }
                else
                  {
                    if ( t.DataOd > dOPer)
                            t.DataOd = dOPer;
                    if (t.DataDo < dOPer)
                        t.DataDo = dOPer;
                  }
                  t.DataTransferu = DateTime.Now;
                  t.DataOOper = DateTime.Now;
                
                

                return t;

            }
            else
                return null;
        }


    }
}
