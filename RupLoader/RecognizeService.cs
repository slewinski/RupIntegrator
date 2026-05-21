using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RupLoader
{
    class RecognizeService
    {
        public string recognCode {get; set;}
        public List<string> keys { get; set;}


        public RecognizeService()
        {
            recognCode = "";
            keys = new List <string>();
        }
       


        private void matchPattern(string intext,RL_Schemat pattern)
        {
            string s = pattern.wzorzec;
            
            s = interpreter(s);
            
                Regex r = new Regex(s);
                Match m = r.Match(intext);
                if (m.Success)
                {
                    string myKey;
                    recognCode += pattern.kod;
                    myKey = m.Value;
                    if (!String.IsNullOrEmpty(pattern.detailsPattern))
                    {
                        if (pattern.MatchMode == "M")
                        {
                            Regex rx = new Regex(pattern.detailsPattern);
                            Match mx = rx.Match(myKey);
                            if (mx.Success) myKey = mx.Value;
                        }
                        else
                        {
                            myKey = Regex.Replace(intext, s, pattern.detailsPattern);
                        
                        }
                    }
                    keys.Add(myKey);

                    if (pattern.NextIfYes > 0)
                    {

                        RL_Schemat sch = (from c in RupDatabase.theContext.RL_Schemat where c.Id == pattern.NextIfYes select c).FirstOrDefault();
                        if (sch != null) matchPattern(intext, sch);
                    }
                }
                else
                {
                    if (pattern.NextIfNo > 0)
                    {
                        RL_Schemat sch = (from c in RupDatabase.theContext.RL_Schemat where c.Id == pattern.NextIfNo select c).FirstOrDefault();
                        if (sch != null) matchPattern(intext, sch);

                    }


                }
            }
          

            

        public string odmiana( string nazwisko, string imie)
        {
            string ls_wynik,ls_kon,ls_kon2,ls_kon3,ls_kon4;
            int li_kobieta;

            nazwisko =nazwisko.Trim();
            imie = imie.Trim();
            li_kobieta = 0;
            ls_wynik = nazwisko;
            ls_kon = imie.Substring(imie.Length -1 ,1 ).ToUpper();

            if (ls_kon=="A") li_kobieta=1;    // imię żeńskie
/*
ls_kon4 = upper(right(nazwisko.Substring(nazwisko.,4))
ls_kon3 = upper(right(nazwisko,3))
ls_kon2 = upper(right(nazwisko,2))
ls_kon  = upper(right(nazwisko,1))

if ((ls_kon3="SKA") or (ls_kon3="CKA")) then
   ls_wynik = left(nazwisko,len(nazwisko)-1)+"iej"	// Kowalskiej, Chojeckiej
else
	if (((ls_kon3="SKI") or (ls_kon3="CKI") or (ls_kon2="KI")) and (li_kobieta=0)) then
     ls_wynik=nazwisko+"ego" // Kowalskiego, Gładkiego
    else	
	 if ((ls_kon4="CZEŃ") and (li_kobieta=0)) then  // Styczeń, Toczeń
		ls_wynik = left(nazwisko,len(nazwisko)-2)+"nia"
	 else
    if ((ls_kon4="RZEC") and (li_kobieta=0)) then  // Marzec
		ls_wynik = left(nazwisko,len(nazwisko)-3)+"ca"
	 else
    if ((ls_kon3="IEC")	and (li_kobieta=0)) then  // Kupiec - Kupca  Młyniec - Młyńca
		ls_wynik = left(nazwisko,len(nazwisko)-4)+f_zmiekcz(left(ls_kon4,1))+"ca"
	 else
    if ((ls_kon2="EK")	and (li_kobieta=0)) then
		ls_wynik=left(nazwisko,len(nazwisko)-2)+"ka" // Buzek
	 else
      if ((ls_kon2="EC")	and (li_kobieta=0)) then
	 	ls_wynik=left(nazwisko,len(nazwisko)-2)+"ca" // Michalec
	   else
	     if (((ls_kon2="RY")	or (ls_kon2="NY") or (ls_kon2="SY") or (ls_kon2="ŻY") or &
             (ls_kon2="ZY") or (ls_kon2 ="TY") or (ls_kon2="WY") or &
				 (ls_kon3="SKI") or (ls_kon3="CKI") or (ls_kon2="KI")) and (li_kobieta=0)) then
	 	    ls_wynik=left(nazwisko,len(nazwisko)-1)+"ego" // Batorego
	     else
     if (((ls_kon3="ACH") or (ls_kon3="ICZ") or (ls_kon="K") or &
         (ls_kon="N") or (ls_kon="Ł") or (ls_kon="B") or (ls_kon="C") or &
         (ls_kon="D") or (ls_kon="F") or (ls_kon="G") or (ls_kon="J") or &
         (ls_kon="M") or (ls_kon="P") or (ls_kon="R") or (ls_kon="W") or &			
         (ls_kon="Z") or &			
			(ls_kon="S") or (ls_kon="L") or (ls_kon="T")) and (li_kobieta=0)) then
       ls_wynik = nazwisko+"a"	 // Iwanowicza,Gasika,Kowalczyka,Bogdaniuka,Krokusa
     else
      if ((ls_kon2="KA") or (ls_kon2="GA") or (ls_kon2="KO") or &
          (ls_kon2="IA") or (ls_kon2="JA") or &
		    (ls_kon2="LA")) then
		   ls_wynik = left(nazwisko,len(nazwisko)-1)+"i" // Malagi,Czaji,Formeli
	   else
       if ((ls_kon2="KO") and (li_kobieta=0)) then
		   ls_wynik = left(nazwisko,len(nazwisko)-1)+"i" // Kościuszki
	   else

        if ((ls_kon2="DA") or (ls_kon2="BA") or (ls_kon2="CA") or (ls_kon2="ZA") or &
		      (ls_kon2="RA") or (ls_kon2="WA") or (ls_kon2="TA")) then
		     ls_wynik = left(nazwisko,len(nazwisko)-1)+"y"  // reszta nazwisk męskich
        end if
      end if
		end if
	end if
	end if
	end if
end if

end if
end if
end if
    end if
  end if 
            */
            return ls_wynik;
      
        
    
    
        }

        public string interpreter(string instring)
        {
            
            string repstr = "";
            if (instring.ToUpper().Contains("<REPERTORIUM>"))
            {
                List<string> repLst = (from r in RupDatabase.theContext.SAPRepertorium select r.kod).ToList();
                if (repLst != null)
                {
                    repstr = "(";
                    foreach (string rp in repLst)
                    { 
                        if ( repstr.Length > 1 )   repstr += "|";
                        repstr += rp.Trim().ToUpper(); 
                    
                    }
                    repstr += ")";
                   instring  =  instring.Replace("<REPERTORIUM>", repstr);
                    
                }
            
            }

            return instring;
        
        }
        
       

        public bool ParseTytul(string tytTxt, out int ranking)
        {
            List<RL_Schemat> schema = (from c in RupDatabase.theContext.RL_Schemat where c.priority > 0  orderby c.priority select c).ToList();
            ranking = 0;
            tytTxt  = tytTxt.ToUpper().Replace("\\", "/");
            if (tytTxt.Length < 5 ) return false;
            if (schema != null)
            {
                foreach (RL_Schemat s in schema)
                {
                    matchPattern(tytTxt,s);
                    if (keys.Count > 0)
                    {
                        ranking = s.priority??0;
                        return true;
                    }
                
                }
            
            
            }
            return false;
        
        }
    }
}
