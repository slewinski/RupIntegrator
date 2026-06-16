using Cons2RupModel;
using ConsInterfeces.Rup2ConsImportContentSystemData;
using log4net;
using RupDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace ConsImport
{
    public class ConsImportFromDB
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string ConnectionString = ConfigurationManager.ConnectionStrings["RupIntegrator.Properties.Settings.RupDB"].ConnectionString;


        string filePath;
        public ConsImportFromDB()
        {
#if MOCK
           filePath = ConfigurationManager.AppSettings["MockKartaFilePath"];
#endif

        }

        public ImportContentSystemDataRequest getSinglePosition(DataTable dt)
        {
            return new ImportContentSystemDataRequest();

        }
        public List<ConsImportData> GetDataFromDB(ConsExternalDBConnectionConfig knf, DateTime odDnia, DateTime doDnia)
        {
#if MOCK
            List<ConsImportData> lMock = new List<ConsImportData>();
            lMock.Add(mockKarta(filePath));
            return lMock;
#endif

            DataSet dset = executeSP(knf, odDnia, doDnia);
            if (dset != null)
            {
                return ProceedDataTable(dset);

            }
            else
            {
                log.Info("Nie można pobrać danych z bazy danych przy użyciu " + knf.sp_name + " dla przedziału " + odDnia.ToString() + " do " + doDnia.ToString());
                return null;
            }
        }




        private List<ConsImportData> ProceedDataTable(DataSet ds)
        {
            var result = new List<ConsImportData>();

            if (ds == null || ds.Tables == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return result;

            DataTable mainTable = ds.Tables[0];
            DataTable addressTable = ds.Tables.Count > 1 ? ds.Tables[1] : null;
            DataTable orzeczeniaTable = ds.Tables.Count > 2 ? ds.Tables[2] : null;
            DataTable zdarzeniaTable = ds.Tables.Count > 3 ? ds.Tables[3] : null;
            // testowy zapis orzeczenia do pliku
            /*
            if (orzeczeniaTable != null && orzeczeniaTable.Rows.Count > 0)
            {
                DataRow o = orzeczeniaTable.Rows[0];

                byte[] content = null;
                byte[] raw = null;
                if (o["msword"] != DBNull.Value)
                {
                    if (o["msword"] is byte[])
                    {
                       raw = (byte[])o["msword"];
                       content = Utils.DecompressMsWord(raw);

                    }
                    else
                    {
                        // jeśli msword przychodzi jako base64 string
                        string base64 = Convert.ToString(o["msword"]);
                        content = Convert.FromBase64String(base64);
                    }
                }

                if (content != null && content.Length > 0)
                {
                    string fileName = "orzeczenie_test.doc";
                    string path = System.IO.Path.Combine(@"C:\temp", fileName);

                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                    System.IO.File.WriteAllBytes(path, content);
                }
            }
            */


            Func<DataRow, string, bool> HasColumn = (row, name) =>
                row != null &&
                row.Table != null &&
                row.Table.Columns.Contains(name) &&
                row[name] != DBNull.Value;

            Func<string, string> Clean = value =>
            {
                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                return value
                    .Replace("ł", "l").Replace("Ł", "L")
                    .Replace("ą", "a").Replace("Ą", "A")
                    .Replace("ć", "c").Replace("Ć", "C")
                    .Replace("ę", "e").Replace("Ę", "E")
                    .Replace("ń", "n").Replace("Ń", "N")
                    .Replace("ó", "o").Replace("Ó", "O")
                    .Replace("ś", "s").Replace("Ś", "S")
                    .Replace("ż", "z").Replace("Ż", "Z")
                    .Replace("ź", "z").Replace("Ź", "Z")
                    .Trim();
            };

            Func<DataRow, string, string> S = (row, name) =>
                HasColumn(row, name) ? Clean(Convert.ToString(row[name])) : String.Empty;

            Func<DataRow, string[], string> SAny = (row, names) =>
            {
                foreach (string name in names)
                {
                    string value = S(row, name);
                    if (!String.IsNullOrWhiteSpace(value))
                        return value;
                }

                return String.Empty;
            };

            Func<DataRow, string, int> I = (row, name) =>
            {
                if (!HasColumn(row, name))
                    return 0;

                int value;
                return Int32.TryParse(Convert.ToString(row[name]), out value) ? value : 0;
            };

            Func<DataRow, string, decimal> D = (row, name) =>
            {
                if (!HasColumn(row, name))
                    return 0m;

                string value = Convert.ToString(row[name]).Trim();

                if (String.IsNullOrWhiteSpace(value))
                    return 0m;

                decimal resultd;

                if (Decimal.TryParse(
                    value,
                    System.Globalization.NumberStyles.Any,
                    new System.Globalization.CultureInfo("pl-PL"),
                    out resultd))
                {
                    return resultd;
                }

                if (Decimal.TryParse(
                    value.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out resultd))
                {
                    return resultd;
                }

                return Convert.ToDecimal(row[name]);
            };

            Func<DataRow, string, string> DateS = (row, name) =>
            {
                if (!HasColumn(row, name))
                    return String.Empty;

                if (row[name] is DateTime)
                    return ((DateTime)row[name]).ToString("yyyyMMdd");

                string value = Convert.ToString(row[name]).Trim();

                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                DateTime parsed;

                if (DateTime.TryParse(value, out parsed))
                    return parsed.ToString("yyyyMMdd");

                value = value.Replace("-", "").Replace(".", "").Replace("/", "");

                return value;
            };

            Func<DataRow, string[], string> DateAny = (row, names) =>
            {
                foreach (string name in names)
                {
                    string value = DateS(row, name);
                    if (!String.IsNullOrWhiteSpace(value))
                        return value;
                }

                return String.Empty;
            };

            Func<object, string> ToBase64 = value =>
            {
                if (value == null || value == DBNull.Value)
                    return String.Empty;

                if (value is byte[])
                    return Convert.ToBase64String((byte[])value);

                string text = Convert.ToString(value);

                if (String.IsNullOrWhiteSpace(text))
                    return String.Empty;

                return text.Trim();
            };
            Func<string, string> SafeAttachmentName = name =>
            {
                name = Clean(name);

                if (String.IsNullOrWhiteSpace(name))
                    return "orzeczenie.doc";

                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    name = name.Replace(c.ToString(), String.Empty);

                if (!name.EndsWith(".doc", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                {
                    name += ".doc";
                }

                return name;
            };

            Func<DataRow, string, string> AttachmentContent = (r, columnName) =>
            {
                if (!HasColumn(r, columnName))
                    return String.Empty;

                object value = r[columnName];

                if (value == null || value == DBNull.Value)
                    return String.Empty;

                if (value is byte[])
                {
                    byte[] raw = (byte[])value;

                    try
                    {
                        return ToBase64(Utils.DecompressMsWord(raw));
                    }
                    catch
                    {
                        return Convert.ToBase64String(raw);
                    }
                }

                return Convert.ToString(value);
            };

            Func<DataRow, int, PozycjaPoleKonfigurowalne[]> BuildPolaKonfigurowalne = (r, max) =>
            {
                return Enumerable.Range(1, max)
                    .Select(n =>
                    {
                        string nazwa = S(r, "PozycjaPoleKonfigurowalneNazwa" + n);

                        if (String.IsNullOrWhiteSpace(nazwa))
                            return null;

                        bool isDateField =
                            nazwa == "DATA_ORZ" ||
                            nazwa == "DATA_UPR" ||
                            nazwa == "DATA_SKIEROWANIA" ||
                            nazwa == "DATA_PRZED_KS" ||
                            nazwa == "DATA_PRZED_GR" ||
                            nazwa == "DATA_KOLEJNA" ||
                            nazwa == "DATA_REJESTRACJI_KO" ||
                            nazwa == "DATA_ZALATWIENIA" ||
                            nazwa == "DATA_WYKO"||
                            nazwa == "DATA_WYKONANIA";

                        string wartosc = isDateField
                            ? DateS(r, "PozycjaPoleKonfigurowalneWartosc" + n)
                            : S(r, "PozycjaPoleKonfigurowalneWartosc" + n);

                        if (String.IsNullOrWhiteSpace(wartosc))
                            return null;

                        return new PozycjaPoleKonfigurowalne
                        {
                            Nazwa = nazwa,
                            Wartosc = wartosc
                        };
                    })
                    .Where(x => x != null)
                    .ToArray();
            };

            Func<string, string> CountryCode = value =>
            {
                value = Clean(value);

                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                if (value.Equals("Polska", StringComparison.OrdinalIgnoreCase))
                    return "PL";

                if (value.Equals("Poland", StringComparison.OrdinalIgnoreCase))
                    return "PL";

                if (value.Length == 2)
                    return value.ToUpper();

                return value;
            };

            Func<string, string> RegionCode = value =>
            {
                value = Clean(value).ToLower();

                if (String.IsNullOrWhiteSpace(value))
                    return String.Empty;

                switch (value)
                {
                    case "dolnoslaskie": return "DSL";
                    case "kujawsko-pomorskie": return "K-P";
                    case "lubelskie": return "LBL";
                    case "lubuskie": return "LBS";
                    case "lodzkie": return "LDZ";
                    case "malopolskie": return "MAL";
                    case "mazowieckie": return "MAZ";
                    case "opolskie": return "OPO";
                    case "podkarpackie": return "PDK";
                    case "podlaskie": return "PDL";
                    case "pomorskie": return "POM";
                    case "slaskie": return "SLS";
                    case "swietokrzyskie": return "SWK";
                    case "warminsko-mazurskie": return "W-M";
                    case "wielkopolskie": return "WLK";
                    case "zachodniopomorskie": return "Z-P";
                    default: return value.ToUpper();
                }
            };

            using (RupDBEntities context = new RupDBEntities())
            {
                var groups = mainTable.AsEnumerable()
                    .GroupBy(r => new
                    {
                        IdSprawy = HasColumn(r, "id_sprawy") ? I(r, "id_sprawy") : I(r, "idSprawy"),
                        IdStrony = HasColumn(r, "id_strony") ? I(r, "id_strony") : I(r, "idStrony")
                    });

                foreach (var group in groups)
                {
                    DataRow row = group.First();

                    int idSprawy = group.Key.IdSprawy;
                    int idStrony = group.Key.IdStrony;

                    DataRow orzeczenieRow = null;

                    if (orzeczeniaTable != null)
                    {
                        orzeczenieRow = orzeczeniaTable.AsEnumerable()
                            .Where(o =>
                                (!HasColumn(o, "id_sprawy") || I(o, "id_sprawy") == idSprawy) &&
                                (!HasColumn(o, "id_strony") || I(o, "id_strony") == idStrony))
                            .OrderByDescending(o => HasColumn(o, "d_orzecz") ? Convert.ToDateTime(o["d_orzecz"]) : DateTime.MinValue)
                            .FirstOrDefault();
                    }

                    string zalacznikNazwa = S(row, "ZalacznikNazwa");
                    string zalacznikZawartosc = S(row, "ZalacznikZawartosc");

                    if (orzeczenieRow != null)
                    {
                        string mswordBase64 = ToBase64(Utils.DecompressMsWord((byte[])orzeczenieRow["msword"]));

                        if (!String.IsNullOrWhiteSpace(mswordBase64))
                        {
                            zalacznikZawartosc = mswordBase64;
                            zalacznikNazwa = SafeAttachmentName(S(orzeczenieRow, "nazwa"));
                        }
                    }


                    bool skip = context.ConsKartaTransfer.Any(a =>
                        a.idStronyWydzial == idStrony &&
                        a.idSprawyWydzial == idSprawy &&
                        (
                            (ConsImportStatus)a.status == ConsImportStatus.Done ||
                            (ConsImportStatus)a.status == ConsImportStatus.Pending
                        ));

                    if (skip)
                        continue;

                    var partner = new PozycjaDanePartneraBiznesowego
                    {
                        TypPartnera = SAny(row, new[] { "TypPartnera" }),
                        NumerPartneraNadrzednego = String.Empty,
                        NumerPartneraBiznesowego = String.Empty,
                        TypPartneraHandlowego = SAny(row, new[] { "TypPartneraHandlowego" }),

                        PartnerHandlowyImie = S(row, "PartnerHandlowyImie"),
                        PartnerHandlowyDrugieImie = S(row, "PartnerHandlowyDrugieImie"),
                        PartnerHandlowyNazwisko = S(row, "PartnerHandlowyNazwisko"),
                        PartnerHandlowyNazwiskoRodowe = S(row, "PartnerHandlowyNazwiskoRodowe"),
                        PartnerHandlowyNazwa1 = S(row, "PartnerHandlowyNazwa1"),
                        PartnerHandlowyNazwa2 = S(row, "PartnerHandlowyNazwa2"),
                        PartnerHandlowyNazwa3 = S(row, "PartnerHandlowyNazwa3"),
                        PartnerHandlowyNazwa4 = S(row, "PartnerHandlowyNazwa4"),

                        PartnerHandlowyPesel = S(row, "PartnerHandlowyPesel"),
                        PartnerHandlowyRegon = S(row, "PartnerHandlowyRegon"),
                        PartnerHandlowyNip = S(row, "PartnerHandlowyNip"),

                        PartnerHandlowyPanstwoUrodzenia = "PL",
                        PartnerHandlowyObywatelstwo = "PL",
                        PartnerHandlowyInneObywatelstwa = S(row, "PartnerHandlowyInneObywatelstwa"),
                        PartnerHandlowyStatusZatrudnienia = S(row, "PartnerHandlowyStatusZatrudnienia"),
                        PartnerHandlowyZawod = S(row, "PartnerHandlowyZawod"),
                        PartnerHandlowyWyksztalcenie = S(row, "PartnerHandlowyWyksztalcenie"),
                        PartnerHandlowyEmail = S(row, "PartnerHandlowyEmail"),
                        PartnerHandlowyWykonywanieFunkcji = S(row, "PartnerHandlowyWykonywanieFunkcji"),
                        PartnerHandlowyDataUrodzenia = DateS(row, "PartnerHandlowyDataUrodzenia"),
                        PartnerHandlowyImieOjca = S(row, "PartnerHandlowyImieOjca"),
                        PartnerHandlowyImieMatki = S(row, "PartnerHandlowyImieMatki"),
                        PartnerHandlowyNazwiskoRodoweMatki = S(row, "PartnerHandlowyNazwiskoRodoweMatki"),
                        InformacjaInna = S(row, "InformacjaInna"),
                        PartnerHandlowyPobytZakladKarny = S(row, "PartnerHandlowyPobytZakladKarny"),
                        PartnerHandlowyObronca = S(row, "PartnerHandlowyObronca"),

                        Krs = S(row, "Krs"),
                        TypPartneraZCONS = !String.IsNullOrWhiteSpace(S(row, "TypPartneraZCONS")) ? S(row, "TypPartneraZCONS") : "DL_GLOWNY",
                        NumerRBN = !String.IsNullOrWhiteSpace(S(row, "NumerRBN")) ? S(row, "NumerRBN") : "09",
                        NumerPartneraSystemuZewnetrznego = S(row, "NumerPartneraSystemuZewnetrznego"),
                        NumerNadrzednegoPartneraSystemuZewnetrznego = S(row, "NumerNadrzednegoPartneraSystemuZewnetrznego"),

                        PartnerSprawy = false,
                        PartnerSprawySpecified = true,
                        PartnerKarty = false,
                        PartnerKartySpecified = true,

                        Skorowidz = !String.IsNullOrWhiteSpace(S(row, "Skorowidz")) ? S(row, "Skorowidz") : "Pozostali"
                    };

                    string plec = S(row, "PartnerHandlowyPlec");

                    if (!String.IsNullOrWhiteSpace(plec))
                    {
                        partner.PartnerHandlowyPlec =
                            (PozycjaDanePartneraBiznesowegoPartnerHandlowyPlec)
                            Enum.Parse(typeof(PozycjaDanePartneraBiznesowegoPartnerHandlowyPlec), plec, true);

                        partner.PartnerHandlowyPlecSpecified = true;
                    }

                    if (!String.IsNullOrWhiteSpace(S(row, "PartnerHandlowyZakladPracyNazwa")) ||
                        !String.IsNullOrWhiteSpace(S(row, "PartnerHandlowyZakladPracyNip")))
                    {
                        partner.PartnerHandlowyZakladPracy = new ZakladPracy
                        {
                            Nazwa = S(row, "PartnerHandlowyZakladPracyNazwa"),
                            Nip = S(row, "PartnerHandlowyZakladPracyNip")
                        };
                    }

                    if (addressTable != null)
                    {
                        partner.PartnerHandlowyAdresy = addressTable.AsEnumerable()
                            .Where(a =>
                                (HasColumn(a, "id_strony") && I(a, "id_strony") == idStrony) ||
                                (HasColumn(a, "idStrony") && I(a, "idStrony") == idStrony))
                            .Select(a => new Adres
                            {
                                Rodzaj = S(a, "PartnerHandlowyAdresyRodzaj"),
                                KluczKraju = !String.IsNullOrWhiteSpace(CountryCode(S(a, "PartnerHandlowyAdresyKluczKraju")))
                                    ? CountryCode(S(a, "PartnerHandlowyAdresyKluczKraju"))
                                    : "PL",
                                Miasto = S(a, "PartnerHandlowyAdresyMiasto"),
                                KodPocztowy = S(a, "PartnerHandlowyAdresyKodPocztowy"),
                                Ulica = S(a, "PartnerHandlowyAdresyUlica"),
                                NumerDomu = S(a, "PartnerHandlowyAdresyNumerDomu"),
                                Region = !String.IsNullOrWhiteSpace(RegionCode(SAny(a, new[]
                                {
                                      "PartnerHandlowyAdresyRegion",
                                      "PartnerHandlowyAdresyNumerRegion"
                                })))
                                    ? RegionCode(SAny(a, new[]
                                    {
                                          "PartnerHandlowyAdresyRegion",
                                          "PartnerHandlowyAdresyNumerRegion"
                                    }))
                                    : "DSL"
                            })
                            .ToArray();
                    }

                    string dokumentTyp = S(row, "PartnerHandlowyDokumentTozsamosciTyp");
                    string dokumentNumer = S(row, "PartnerHandlowyDokumentTozsamosciNumer");

                    if (!String.IsNullOrWhiteSpace(dokumentTyp) || !String.IsNullOrWhiteSpace(dokumentNumer))
                    {
                        string dokumentDataWydania = DateAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciDataWydania"
                  });

                        string dokumentDataWaznosciOd = DateAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciDataWaznosciOd",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciOd"
                  });

                        string dokumentDataWaznosciDo = DateAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciDataWaznosciDo",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciDo"
                  });

                        string dokumentKraj = CountryCode(SAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciKraj",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciKraj"
                  }));

                        string dokumentRegion = RegionCode(SAny(row, new[]
                        {
                      "PartnerHandlowyDokumentTozsamosciRegion",
                      "PartnerHandlowyDokumentTozsamosciDataWydaniaDataWaznosciRegion"
                  }));

                        if (String.IsNullOrWhiteSpace(dokumentDataWydania))
                            dokumentDataWydania = "20200101";

                        if (String.IsNullOrWhiteSpace(dokumentDataWaznosciOd))
                            dokumentDataWaznosciOd = dokumentDataWydania;

                        if (String.IsNullOrWhiteSpace(dokumentDataWaznosciDo))
                            dokumentDataWaznosciDo = "20300101";

                        if (String.IsNullOrWhiteSpace(dokumentKraj))
                            dokumentKraj = "PL";

                        if (String.IsNullOrWhiteSpace(dokumentRegion))
                            dokumentRegion = "DSL";

                        partner.PartnerHandlowyDokumentTozsamosci = new[]
                        {
                      new DokumentTozsamosci
                      {
                          Typ = dokumentTyp,
                          Numer = dokumentNumer,
                          Wydal = S(row, "PartnerHandlowyDokumentTozsamosciWydal"),
                          DataWydania = dokumentDataWydania,
                          DataWaznosciOd = dokumentDataWaznosciOd,
                          DataWaznosciDo = dokumentDataWaznosciDo,
                          Kraj = dokumentKraj,
                          Region = dokumentRegion
                      }
                  };
                    }
                    var polaKonfigurowalne = BuildPolaKonfigurowalne(row, 10);

                    var dodatkoweZdarzenia = new List<PozycjaDaneZdarzenia>();

                    if (zdarzeniaTable != null)
                    {
                        string identyfikatorWyroku = S(row, "IdentyfikatorWyrokuZSystemuZewnetrznego");

                        dodatkoweZdarzenia = zdarzeniaTable.AsEnumerable()
                            .Where(z => HasColumn(z, "id_sprawy") && I(z, "id_sprawy") == idSprawy)
                            .Select(z =>
                            {
                                string zalacznikNazwaZdarzenia = S(z, "ZalacznikNazwa");
                                string zalacznikZawartoscZdarzenia = AttachmentContent(z, "ZalacznikZawartosc");

                                return new PozycjaDaneZdarzenia
                                {
                                    DataZdarzenia = DateS(z, "DataZdarzenia"),
                                    DataKsiegowania = DateS(z, "DataZdarzenia"),
                                    IdentyfikatorWyrokuZSystemuZewnetrznego = !String.IsNullOrWhiteSpace(S(z, "IdentyfikatorWyrokuZSystemuZewnetrznego")) ? S(z, "IdentyfikatorWyrokuZSystemuZewnetrznego") : identyfikatorWyroku,
                                    TypZdarzenia = S(z, "TypZdarzenia"),
                                    ZalacznikNazwa = String.IsNullOrWhiteSpace(zalacznikNazwaZdarzenia) ? null : SafeAttachmentName(zalacznikNazwaZdarzenia),
                                    ZalacznikZawartosc = String.IsNullOrWhiteSpace(zalacznikZawartoscZdarzenia) ? null : zalacznikZawartoscZdarzenia,
                                    ListaDaneFinansowe = new PozycjaDaneFinansowe[0],
                                    ListaPlanRatalny = new PozycjaPlanRatalny[0],
                                    ListaParametryRat = new PozycjaParametryRat[0],
                                    ListaPolaKonfigurowalne = BuildPolaKonfigurowalne(z, 14)
                                };
                            })
                            .ToList();
                    }

                    var request = new ImportContentSystemDataRequest
                    {
                        GUID = Guid.NewGuid().ToString(),

                        DaneDziennika = new DaneDziennika
                        {
                            JednostkaGospodarcza = S(row, "DaneSygnaturyAktJednostkaGospodarcza"),
                            StanowiskoFinansowe = S(row, "DaneSygnaturyAktStanowiskoFinansowe"),
                            NumerWydzialuISekcji = S(row, "NumerWydzialuISekcji"),
                            Repertorium = S(row, "Repertorium")
                        },

                        ListaDanePartneraBiznesowego = new[] { partner },

                        DaneKartyDluznika = new DaneKartyDluznika
                        {
                            RodzajKarty = S(row, "DaneKartyDluznikaRodzajKarty"),
                            OznaczenieKontaUmowy = S(row, "OznaczenieKontaUmowy"),
                            NumerKontaUmowy = S(row, "NumerKontaUmowy"),
                            JednostkaGospodarcza = !String.IsNullOrWhiteSpace(S(row, "DaneKartyDluznikaJednostkaGospodarcza"))
                                ? S(row, "DaneKartyDluznikaJednostkaGospodarcza")
                                : S(row, "DaneSygnaturyAktJednostkaGospodarcza"),
                            StanowiskoFinansowe = S(row, "DaneKartyDluznikaStanowiskoFinansowe"),
                            DataKartyZdarzenia = DateS(row, "DataKartyZdarzenia")
                        },

                        DaneSygnaturyAkt = new DaneSygnaturyAkt
                        {
                            PrzedmiotyUmowy = S(row, "PrzedmiotyUmowy"),
                            RodzajPrzedmiotuUmowy = S(row, "RodzajPrzedmiotuUmowy"),
                            JednostkaGospodarcza = S(row, "DaneSygnaturyAktJednostkaGospodarcza"),
                            StanowiskoFinansowe = S(row, "DaneSygnaturyAktStanowiskoFinansowe"),
                            NumerWydzialuISekcji = S(row, "PrzedmiotyUmowyNumerWydzialuISekcji"),
                            Repertorium = S(row, "PrzedmiotyUmowyRepertorium"),
                            KolejnyNumerSprawy = S(row, "PrzedmiotyUmowyKolejnyNumerSprawy"),
                            Rok = S(row, "PrzedmiotyUmowyRok"),
                            RodzajSprawy = S(row, "PrzedmiotyUmowyRodzajSprawy"),
                            PodrodzajSprawy = S(row, "PrzedmiotyUmowyPodrodzajSprawy"),
                            JednostkaGospodarczaSygnaturaArchiwalna = S(row, "JednostkaGospodarczaSygnaturaArchiwalna"),
                            StanowiskoFinansoweSygnaturaArchiwalna = S(row, "StanowiskoFinansoweSygnaturaArchiwalna"),
                            NumerWydzialuISekcjiSygnaturaArchiwalna = S(row, "NumerWydzialuISekcjiSygnaturaArchiwalna"),
                            RepertoriumSygnaturaArchiwalna = S(row, "RepertoriumSygnaturaArchiwalna"),
                            KolejnyNumerSprawySygnaturaArchiwalna = S(row, "KolejnyNumerSprawySygnaturaArchiwalna"),
                            RokSygnaturaArchiwalna = S(row, "RokSygnaturaArchiwalna"),
                            JednostkaGospodarczaWindykacja = S(row, "JednostkaGospodarczaWindykacja"),
                            StanowiskoFinansoweWindykacja = S(row, "StanowiskoFinansoweWindykacja"),
                            KodOkreguKW = S(row, "KodOkreguKW"),
                            KontrolkaSygnaturyKW = S(row, "KontrolkaSygnaturyKW")
                        },

                        ListaDaneZdarzen = new[]
        {
            new PozycjaDaneZdarzenia
            {
                DataZdarzenia = DateS(row, "DataKartyZdarzenia"),
                DataKsiegowania = DateS(row, "DataKartyZdarzenia"),
                IdentyfikatorWyrokuZSystemuZewnetrznego = S(row, "IdentyfikatorWyrokuZSystemuZewnetrznego"),
                TypZdarzenia = S(row, "TypZdarzenia"),
                ZalacznikNazwa = zalacznikNazwa,
                ZalacznikZawartosc = zalacznikZawartosc,
        
                ListaDaneFinansowe = group
                .SelectMany(r =>
                    {
                        var lista = new List<PozycjaDaneFinansowe>();
        
                        Action<string> AddFinanse = suffix =>
                        {
                            decimal kwota = D(r, "PozycjaDaneFinansoweKwota" + suffix);
        
                            if (kwota <= 0)
                                return;
        
                            string data = DateS(r, "PozycjaDaneFinansoweData" + suffix);
                            string typ = S(r, "PozycjaDaneFinansoweTyp" + suffix);
                            string nazwa = S(r, "PozycjaDaneFinansoweNazwa" + suffix);
                            string ilosc = S(r, "PozycjaDaneFinansoweIlosc" + suffix);
                            string numer = S(r, "PozycjaDaneFinansoweNumerDokumentu" + suffix);
                            string pozycja = S(r, "PozycjaDaneFinansowePozycjaDokumentu" + suffix);
                            string operacjaGlowna = S(r, "OperacjaGlowna" + suffix);
                            string operacjaCzesciowa = S(r, "OperacjaCzesciowa" + suffix);
                            decimal kwotaSkladnika = D(r, "PozycjaDaneFinansoweKwotaSkladnika" + suffix);
        
                            if (String.IsNullOrWhiteSpace(data) ||
                                String.IsNullOrWhiteSpace(typ) ||
                                String.IsNullOrWhiteSpace(nazwa) ||
                                String.IsNullOrWhiteSpace(operacjaGlowna) ||
                                String.IsNullOrWhiteSpace(operacjaCzesciowa))
                            {
                                return;
                            }
        
                            lista.Add(new PozycjaDaneFinansowe
                            {
                                Data = data,
                                Typ = typ,
                                Nazwa = nazwa,
                                Ilosc = ilosc,
                                OperacjaGlowna = operacjaGlowna,
                                OperacjaCzesciowa = operacjaCzesciowa,
                                Kwota = kwota,
                                KwotaSkladnika = kwotaSkladnika,
                                PozycjaDokumentu = pozycja.Length > 0 ? pozycja : null,
                                NumerDokumentu = numer.Length > 0 ? numer : null
                            });
                        };
        
                        AddFinanse("Koszty");
                        AddFinanse("FPPSP");
                        AddFinanse("FPPNAW");
                        AddFinanse("PK");
                        AddFinanse("KPNawSP");
                        AddFinanse("Grzywna");
        
                        return lista;
                    })
                    .ToArray(),
        
                            ListaPlanRatalny = new PozycjaPlanRatalny[0],
                            ListaParametryRat = new PozycjaParametryRat[0],
                            ListaPolaKonfigurowalne = polaKonfigurowalne
                        }
                                }
                                .Concat(dodatkoweZdarzenia)
                                .ToArray()
                            };
                            if (String.IsNullOrWhiteSpace(request.DaneSygnaturyAkt.RepertoriumSygnaturaArchiwalna))
                                request.DaneSygnaturyAkt.RepertoriumSygnaturaArchiwalna = null;
        
                            if (String.IsNullOrWhiteSpace(request.DaneSygnaturyAkt.KolejnyNumerSprawySygnaturaArchiwalna))
                                request.DaneSygnaturyAkt.KolejnyNumerSprawySygnaturaArchiwalna = null;
        
                            if (String.IsNullOrWhiteSpace(request.DaneSygnaturyAkt.RokSygnaturaArchiwalna))
                                request.DaneSygnaturyAkt.RokSygnaturaArchiwalna = null;
        
                            // stabilizacja
                            /*
                            foreach (var p in request.ListaDanePartneraBiznesowego ?? new PozycjaDanePartneraBiznesowego[0])
                            {
                                p.PartnerHandlowyPanstwoUrodzenia = "PL";
                                p.PartnerHandlowyObywatelstwo = "PL";
        
                                p.PartnerSprawy = false;
                                p.PartnerSprawySpecified = true;
        
                                p.PartnerKarty = false;
                                p.PartnerKartySpecified = true;
        
                                if (String.IsNullOrWhiteSpace(p.Skorowidz))
                                    p.Skorowidz = "Pozostali";
        
                                if (p.PartnerHandlowyAdresy != null)
                                {
                                    p.PartnerHandlowyAdresy = p.PartnerHandlowyAdresy
                                        .Where(a => a != null && a.Rodzaj == "Koresp.")
                                        .ToArray();
        
                                    if (p.PartnerHandlowyAdresy.Length == 0)
                                    {
                                        p.PartnerHandlowyAdresy = new[]
                                        {
                          new Adres
                          {
                              Rodzaj = "Koresp.",
                              KluczKraju = "PL",
                              Miasto = "Piast",
                              KodPocztowy = "98-332",
                              Ulica = "Poloninska",
                              NumerDomu = "25",
                              Region = "PKR"
                          }
                      };
                                    }
        
                                    foreach (var a in p.PartnerHandlowyAdresy)
                                    {
                                        a.Rodzaj = "Koresp.";
                                        a.KluczKraju = "PL";
        
                                        if (String.IsNullOrWhiteSpace(a.Region))
                                            a.Region = "PKR";
                                    }
                                }
        
                                if (p.PartnerHandlowyDokumentTozsamosci != null)
                                {
                                    foreach (var d in p.PartnerHandlowyDokumentTozsamosci)
                                    {
                                        if (String.IsNullOrWhiteSpace(d.Typ))
                                            d.Typ = "Dowod osobisty";
        
                                        if (String.IsNullOrWhiteSpace(d.Numer))
                                            d.Numer = "TEMP123";
        
                                        if (String.IsNullOrWhiteSpace(d.Wydal))
                                            d.Wydal = "Urzad";
        
                                        d.DataWydania = "20200101";
                                        d.DataWaznosciOd = "20200101";
                                        d.DataWaznosciDo = "20300101";
                                        d.Kraj = "PL";
                                        d.Region = "DSL";
                                    }
                                }
                            }
                             
                            foreach (var z in request.ListaDaneZdarzen ?? new PozycjaDaneZdarzenia[0])
                            {
                                if (String.IsNullOrWhiteSpace(z.DataZdarzenia))
                                    z.DataZdarzenia = "20260101";
        
                                if (String.IsNullOrWhiteSpace(z.DataKsiegowania))
                                    z.DataKsiegowania = z.DataZdarzenia;
        
                                if (z.ListaDaneFinansowe != null)
                                {
                                    foreach (var f in z.ListaDaneFinansowe)
                                    {
                                        if (String.IsNullOrWhiteSpace(f.Data))
                                            f.Data = z.DataZdarzenia;
        
                                        if (String.IsNullOrWhiteSpace(f.Typ))
                                            f.Typ = "WYROK";
        
                                        if (String.IsNullOrWhiteSpace(f.Nazwa))
                                            f.Nazwa = "Wezwanie";
        
                                        if (String.IsNullOrWhiteSpace(f.Ilosc))
                                            f.Ilosc = "1";
        
                                        // Na potrzeby testu walidacji szyny — wartości jak w requestach, które przechodzą.
                                        if (String.IsNullOrWhiteSpace(f.OperacjaGlowna))
                                            f.OperacjaGlowna = "N010";
        
                                        if (String.IsNullOrWhiteSpace(f.OperacjaCzesciowa))
                                            f.OperacjaCzesciowa = "0020";
                                    }
                                }
        
                                if (z.ListaPolaKonfigurowalne != null)
                                {
                                    z.ListaPolaKonfigurowalne = z.ListaPolaKonfigurowalne
                                        .Where(p =>
                                            p != null &&
                                            !String.IsNullOrWhiteSpace(p.Nazwa) &&
                                            !String.IsNullOrWhiteSpace(p.Wartosc))
                                        .ToArray();
                                }
        
                                if (z.ListaPlanRatalny == null)
                                    z.ListaPlanRatalny = new PozycjaPlanRatalny[0];
        
                                if (z.ListaParametryRat == null)
                                    z.ListaParametryRat = new PozycjaParametryRat[0];
                            }
                            */
        
        
        
                            //request.DaneSygnaturyAkt.RepertoriumSygnaturaArchiwalna = null;
                            //request.DaneSygnaturyAkt.KolejnyNumerSprawySygnaturaArchiwalna = null;
                            //request.DaneSygnaturyAkt.RokSygnaturaArchiwalna = null;
                            //request.DaneSygnaturyAkt.JednostkaGospodarczaWindykacja = null;
                            //request.DaneSygnaturyAkt.StanowiskoFinansoweWindykacja = null;
                            //request.DaneSygnaturyAkt.KodOkreguKW = null;
                            //request.DaneSygnaturyAkt.KontrolkaSygnaturyKW = null;
        
                            /*
                            foreach (var p in request.ListaDanePartneraBiznesowego)
                            {
                                p.PartnerHandlowyDrugieImie = null;
                                p.PartnerHandlowyNazwa1 = null;
                                p.PartnerHandlowyNazwa2 = null;
                                p.PartnerHandlowyNazwa3 = null;
                                p.PartnerHandlowyNazwa4 = null;
                                p.PartnerHandlowyRegon = null;
                                p.PartnerHandlowyNip = null;
                                p.PartnerHandlowyInneObywatelstwa = null;
                                p.PartnerHandlowyStatusZatrudnienia = null;
                                p.PartnerHandlowyWyksztalcenie = null;
                                p.PartnerHandlowyWykonywanieFunkcji = null;
                                p.PartnerHandlowyPobytZakladKarny = null;
                                p.PartnerHandlowyObronca = null;
                                p.Krs = null;
                                p.NumerNadrzednegoPartneraSystemuZewnetrznego = null;
                            }
        
                            foreach (var p in request.ListaDanePartneraBiznesowego)
                            {
                                if (p.PartnerHandlowyDokumentTozsamosci != null)
                                {
                                    foreach (var d in p.PartnerHandlowyDokumentTozsamosci)
                                    {
                                        d.Typ = "Dowod osobisty";
                                        d.Wydal = "Urzad Miasta Tychy";
                                    }
                                }
                            }
                            */
                            // koniec stabilizacji
        
        
        
                            result.Add(new ConsImportData
                            {
                                IdSprawy = idSprawy,
                                IdStrony = idStrony,
                                status = ConsImportStatus.Prepared,
                                importContentSystemDataRequest = request
                            });
                        }
                    }

            return result;
        }

        private ConsImportData mockKarta(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Ścieżka do pliku nie może być pusta.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Nie znaleziono pliku XML.", filePath);

            var serializer = new XmlSerializer(typeof(ImportContentSystemDataRequest));

            using (var stream = File.OpenRead(filePath))
            {
                ImportContentSystemDataRequest obj = (ImportContentSystemDataRequest)serializer.Deserialize(stream);
                ConsImportData result = new ConsImportData();
                result.IdSprawy = 99999;
                result.IdStrony = 99999;
                result.status = ConsImportStatus.Prepared;
                result.importContentSystemDataRequest = new ImportContentSystemDataRequest();
                result.importContentSystemDataRequest.DaneDziennika = obj.DaneDziennika;
                result.importContentSystemDataRequest.DaneKartyDluznika = obj.DaneKartyDluznika;
                result.importContentSystemDataRequest.DaneSygnaturyAkt = obj.DaneSygnaturyAkt;
                result.importContentSystemDataRequest.ListaDanePartneraBiznesowego = obj.ListaDanePartneraBiznesowego;
                result.importContentSystemDataRequest.ListaDaneZdarzen = obj.ListaDaneZdarzen;
                return result;
            }


        }



        private DataSet executeSP(ConsExternalDBConnectionConfig knf, DateTime odDnia, DateTime doDnia)
        {

            SqlDataReader rdr = null;

            List<string> l = new List<string>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand storedProcCommand = new SqlCommand(knf.sp_name, con))
                    {
                        storedProcCommand.CommandType = CommandType.StoredProcedure;
                        storedProcCommand.Parameters.AddWithValue("@sourcesrv", (String.IsNullOrEmpty(knf.srvAlias) ? knf.srvName : knf.srvAlias));
                        storedProcCommand.Parameters.AddWithValue("@dbname", knf.DbName);
                        storedProcCommand.Parameters.AddWithValue("@nazwaDok", knf.sp_param);
                        storedProcCommand.Parameters.AddWithValue("@dataOd", odDnia);
                        storedProcCommand.Parameters.AddWithValue("@dataDo", doDnia);
                        storedProcCommand.Parameters.AddWithValue("@tryb", knf.SAPKnsId);

                        storedProcCommand.CommandTimeout = 600;
                        storedProcCommand.Connection = con;
                        SqlDataAdapter da = new SqlDataAdapter();

                        da.SelectCommand = storedProcCommand;
                        da.SelectCommand.CommandType = CommandType.StoredProcedure;
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;

                    }

                }



            }
            catch (Exception ex)
            {
                log.Error("Błąd odczytu danych przy użyciu " + knf.sp_name, ex);
                return null;

            }


        }


    }
}
