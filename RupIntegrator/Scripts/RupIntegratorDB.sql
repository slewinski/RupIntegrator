
GO
/****** Object:  StoredProcedure [dbo].[sp_RatyCR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_RatyCR]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dzien DateTime
	 
AS
BEGIN
  DECLARE 
		   @nextday Datetime,
		   @query varchar(MAX),
		   @dzienString varchar(30),
  		   @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
set @query =  ' select spr.ksiega as Ksiega, r.pier_rata  as pierwsz_rata, r.nast_rata as nst_rata, 0 as  ostatnia_rata, r.grzywna as  grzywna_pr, r.koszty as  koszty_pr, r.dzien_raty as na_jaki_dzien, r.data_raty_1 as dt_pierwszej_raty, ''2000-01-01'' as  dt_ostatniej_raty , '+
			  ' isnull(r.koniec,''2099-01-01'') as  data_odwolania, 0 as  podzial_rat_gk, r.poczatek as data_wyst_post, ''2099-01-01'' as  data_usun_zapisu, nals.id_dluznik as Sprawa_Id ' +
			  '	from (select id_sprawy as id_dluznik,  ' + 
			  ' sum(nal.przypis_grzywny  -  nal.uiszczenia_grzywny - nal.odpisanie_grzywny )   as grzywna, ' +
	          '  sum(nal.przypis_kosztow  -  nal.uiszczenia_kostow - nal.odpisanie_kosztow )	as koszty ' +
			  '	 from ' + @sourcesrv + '.' + @dbname + '.dbo.kns_dz_nal nal  where  isnull(nal.data_r,nal.data_zapisu) < ' + @nextdayString    +
              ' group by id_sprawy ' +
			  '	having sum(nal.przypis_grzywny  -  nal.uiszczenia_grzywny - nal.odpisanie_grzywny ) > 0 or sum(nal.przypis_kosztow  -  nal.uiszczenia_kostow - nal.odpisanie_kosztow ) > 0  ' +
			  '	) nals ' + 
			  '	Inner JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_nal_okres r  ON nals.id_dluznik =  r.id_sprawy '+
			  '	Inner JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_sprawa spr  ON nals.id_dluznik =  spr.id '+
			  '	where  r.poczatek < ' + @nextdayString + ' and isnull ( r.koniec,''2099-12-31'') > ' + @dzienString +
			  '  order by nals.id_dluznik '
print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Raty_HarmonogramRozlicz]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Raty_HarmonogramRozlicz]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		  @query varchar(MAX),
		
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  
  		  	
set @query  =   ' select  dlu.id_r_sprawy as Ksiega, ''Kd '' + rtrim(sl.rodzaj_sprawy) + '' '' + cast(dlu.numer as varchar(5)) + ''/'' + cast(dlu.rok as varchar(4)) as Karta_dl, rh.kwota_raty as Kwota_Raty,  rh.data_raty as Data_Raty, rh.id_raty,  r.id_dluznik as Sprawa_Id, r.data_wyst_post,  ' + 
				' grzywna_pr as grzywna, koszty_pr as koszty  from  ' +    @sourcesrv +'.' +@dbname + '.dbo.Raty r   inner join ' + @sourcesrv +'.' +@dbname + '.dbo.Raty_harmonogram rh    on r.id_raty =  rh.id_raty ' +
				' inner join  ' + @sourcesrv +'.' +@dbname + '.dbo.dluznik  dlu on  dlu.id_dluznik = r.id_dluznik ' +
				' inner join  ' + @sourcesrv +'.' +@dbname + '.dbo.sl_rodzajow_spraw  sl on  dlu.id_r_sprawy = sl.id_r_sprawy ' +
				' where isnull (r.data_usun_zapisu,''2099-12-31'') > ' +  @dataDoString + ' and  r.data_wyst_post <  ' + @nextdayString + '  and  r.data_wyst_post >= '+ @dataOdString  +
				' order by r.id_dluznik,  rh.data_raty desc, rh.id_raty desc '
print @query
EXEC (@query)
				
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Raty_harmonogram]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_Raty_harmonogram]
	-- Add the parameters for the stored procedure here
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dzien DateTime
	 
AS
BEGIN
  DECLARE 
		   @nextday Datetime,
		   @query varchar(MAX),
		   @dzienString varchar(30),
  		   @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
set @query =  ' select  dlu.id_r_sprawy as Ksiega, rh.kwota_raty as Kwota_Raty,  rh.data_raty as Data_Raty, rh.id_raty,  nals.id_dluznik as Sprawa_Id ' +
		      ' from (select id_dluznik, ' +  
			  ' 	sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis )   as grzywna, ' +
			  '	    sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis )	as koszty ' +
              ' 	from ' + @sourcesrv + '.' + @dbname + '.dbo.naleznosci_dziennik nal where  isnull(nal.data_operacji,nal.data_wprow_zapisu) < ' + 
                @nextdayString + ' and isnull ( nal.data_usun_zapisu,''2099-12-31'') > ' + @dzienString + ' group by id_dluznik ' + 
'  having sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis ) > 0 or sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis ) > 0 ) nals ' +  
'		inner join ' + @sourcesrv+ '.' + @dbname + '.dbo.raty r on r.id_dluznik = nals.id_dluznik ' + 
'		Inner JOIN ' + @sourcesrv+ '.' + @dbname + '.dbo.Raty_harmonogram rh  ON nals.id_dluznik =  rh.id_dluznik ' +
 '	    Inner JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.Dluznik dlu  ON nals.id_dluznik =  dlu.id_dluznik '+
'		where isnull (r.data_usun_zapisu,''2099-12-31'') > ' + @dzienString + ' and r.data_wyst_post < ' + @nextdayString + ' and isnull ( r.data_odwolania,''2099-12-31'') > ' + @dzienString +
'       order by nals.id_dluznik,  rh.data_raty desc, rh.id_raty desc '
--print @query
EXEC ( @query )
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Raty]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Raty]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dzien DateTime
	 
AS
BEGIN
  DECLARE 
		   @nextday Datetime,
		   @query varchar(MAX),
		   @dzienString varchar(30),
  		   @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
set @query =  ' select  dlu.id_r_sprawy as Ksiega , r.pierwsz_rata, r.nst_rata, r.ostatnia_rata,r.grzywna_pr, r.koszty_pr, r.na_jaki_dzien, r.dt_pierwszej_raty, r.dt_ostatniej_raty , '+
			  ' r.data_odwolania, r.podzial_rat_gk, r.data_wyst_post, r.data_usun_zapisu, nals.id_dluznik as Sprawa_Id ' +
			  '	from (select id_dluznik,  ' + 
			  ' sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis )   as grzywna, ' +
	          '  sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis )	as koszty ' +
			   '	 from ' + @sourcesrv + '.' + @dbname + '.dbo.naleznosci_dziennik nal  where  isnull(nal.data_operacji,nal.data_wprow_zapisu) < ' + @nextdayString   + ' and isnull ( nal.data_usun_zapisu,''2099-12-31'') > ' + @dzienString +
              ' group by id_dluznik ' +
			  '	having sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis ) > 0 or sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis ) > 0  ' +
			  '	) nals ' + 
			  '	Inner JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.Raty r  ON nals.id_dluznik =  r.id_dluznik '+
			  '	Inner JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.Dluznik dlu  ON nals.id_dluznik =  dlu.id_dluznik '+
			  '	where isnull (r.data_usun_zapisu,''2099-12-31'') > ' +  @dzienString + '  and r.data_wyst_post < ' + @nextdayString + ' and isnull ( r.data_odwolania,''2099-12-31'') > ' + @dzienString +
			  '  order by nals.id_dluznik '
print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_PrzypisyOR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_PrzypisyOR]
	-- Add the parameters for the stored procedure here
	 -- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortDzienString varchar(12),
  		  @SAPjednostka varchar(4)
		   set @dbname = 'OrComNS'
		   
  		 IF  CHARINDEX ( '@@' , @sourcesrv ) > 0  
		BEGIN
		 set @SAPjednostka = Substring(@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) +2,4)
		 set @sourcesrv = left (@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) -1 )
		END 
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dataDo,120)  +''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 



set @query1 =



		   ' SELECT    case when s.PodmiotTypId = 1 then '' '' else ''X'' end 
as "Osoba fizyczna/Osoba prawna", ' +
		   '  case when s.PodmiotTypId = 1 then s.imie else
left(s.nazwisko,40) end as "Imiê/Nazwa 1", ' +
		   '  case when s.PodmiotTypId = 1 then left(s.nazwisko,40) else 
substring(s.nazwisko,41,40) end as	"Nazwisko / Nazwa 2",  ' +
		    '  case when len(isnull(sa.miejsce,'''')) > 0  then left(sa.miejsce,60) else left(sa.miejscowosc,60) end as	 "Ulica",  ' +
		   '  left(sa.dom,10)   as "Nr domu", ' +
		   '  left(sa.lokal,10)   as "Nr mieszkania", ' +
		   '  sa.kod as "Kod pocztowy",  ' +
		   '  left(case when len(sa.miejscowosc) > 0  then sa.miejscowosc  else sa.miejsce  end,40) as "Miejscowoœæ",  ' +
		   '  case when sa.kraj like ''%pols%'' or isnull(rtrim(sa.Kraj),'''') = '''' or rtrim(sa.Kraj)=''rp'' then  ' +
			'		  ''PL'' else sa.Kraj end   as "Klucz kraju",  ' +
			' '''' as IBAN,  ' +
			' case when isnull(spr.KlasyfikacjaRbnId,0) = 1 then ''09'' else 
''08'' end    as "Kwalifikator do RBN" ' +
			' , ''KN'' as "Typ konta umowy",  ' +
			' spr.numer  "Oznaczenie konta umowy", ' +
			' ''99'' as "Relacja konta", ' +
			' spr.OrzeczenieSadAdresatId as   IdSaduOrzek, ' +
			' spr.sygnatura as Sygnatura,  ' +
			' ''    '' as "Jednostka gospodarcza",  ' +
			' spr.SygnWydzial as "Nr wydzia³u i sekcji", ' +
			' spr.SygnRepetytorium as Repertorium,  ' +
			' spr.SygnNumer as "Nr sprawy",  ' +
			' spr.SygnRok as "Rok",   ' +
			' ''     '' as "Rodzaj sprawy", ' +
			' (select top 1 data from  ' +	@sourcesrv + '.' +   @dbname +'.dbo.zapis where zapis.SprawaId = spr.SprawaId and  data < ' +
			@nextdaystring + '  and zapis.przypis > 0 and zapis.NaleznoscTypId = 1  order by  data desc ) as  "Data dokumentu koszty",   ' +
			' (select top 1 data from  ' +	@sourcesrv + '.' +   @dbname +'.dbo.zapis where zapis.SprawaId = spr.SprawaId and  data < ' +
@nextdaystring + '  and zapis.przypis > 0 and zapis.NaleznoscTypId = 0 order by  data desc ) as  "Data dokumentu grzywna", ' +
			 ' dznal.data  as "Data ksiêgowania", ' +
			' ''NS'' as "Rodzaj dokumentu",  ' +
			' ''PLN'' as "Waluta",  ' +
			' ''            '' as "Klucz uzgodnienia", ' +
			' ''    '' as  "Jednostaka gospodarca w³asna" ' +
			' , case when snr.naleznoscRodzajId = 4 then ''s''  ' +
			'	when spr.SygnRepetytorium = ''W'' and spr.PrzypisG > 0 then ''s''  
' +
			'	end as Czysamoistna, ' +
			' ''N010''   as  "Operacja g³ówna", ' +
			' ''    ''   as  "Czêœciowo grzywna", ' +
			'  ''    ''   as  "Czêœciowo koszty", ' +
			' case when dznal.NaleznoscTypId = 0 and dznal.przypis > 0 then dznal.przypis ELSE 0 end as grzywna, ' +
			' case when dznal.NaleznoscTypId = 1 and dznal.przypis > 0 then dznal.przypis else 0  end as koszty, ' +

  		 ' case    when isnull(spr.dataDoreczenia,cast(''1900-01-01'' as 
datetime)) > cast(''2000-01-01'' as datetime)  	then  ' +
		 ' case    when spr.SygnRepetytorium IN (''W'',''K'') then dateadd 
(dd,30, spr.dataDoreczenia) 	 else    dateadd (dd,14, 
spr.dataDoreczenia) ' +
		 ' end    else     spr.DataPrawomocnosci  end   as "Data wymagalnoœci", ' +

		 ' ''''  as "Raty koszty", ' +
		 ' '''' as  "Raty grzywna", ' +
		 ' ''''  as "Egzekucja grzywny", ' +
		 ' ''''  as "Egzekucja koszty",  ' +
		 ' ''''  as "Grzywny odroczone" , ' +
		 ' ''''  as "Koszty odroczone" , ' +
		 ' ''''  as "Kara zastêpcza" , ' +
		 '  spr.SprawaId as Sprawa_id, ' +
			'    spr.JednostkaWydzialId as Ksiega,  ' +
			' (select left(rtrim(sd.nazwa + '' '' +  rtrim(isnull(sd.skrot,'''')+ '' '' ))  + rtrim(isnull(ad.adres,'''')),100) ' +
             ' from  ' + @sourcesrv + '.' + @dbname + '.dbo.adresat sd ' +
             ' inner join  ' + @sourcesrv + '.' + @dbname + '.dbo.Adres ad on ad.AdresId = sd.AdresId ' +
             ' where  spr.OrzeczenieSadAdresatId = sd.AdresatId) as SadKns, ' +
			 --' isnull(j.nazwa,'''')   as SadKns,  ' +
			  '  cast(rtrim(ltrim(isnull(s.pesel,''''))) as varchar(11)) as Pesel ' +
			  ' , s.nip as NIP, ' +
			  ' ''    '' as "Rodzaj przedmiotu umowy", ' +
			   --SCYW Sygnatura – sprawa cywilna SGOS Sygnatura–sprawa gospodarcza SKAR Sygnatura – sprawa karna SPPR Sygnatura–sprawa prawo pracy SROD Sygnatura-sprawa rodzinna SUBE Sygnatura–sprawa ubezpieczenia
			 ' ''001'' as "Iloœæ tomów" ,   cast( '''' as varchar(50)) as  Opis, ROW_NUMBER() over (order by dznal.ZapisId) as pozycja ' +
			 	' from ' + @sourcesrv +'.'  + @dbname +'.dbo.zapis AS dznal ' +
				' left join ' +@sourcesrv + '.' +   @dbname +'.dbo.sprawa AS spr on spr.SprawaId = dznal.SprawaId' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.JednostkaWydzial jw on jw.JednostkaWydzialId = spr.JednostkaWydzialId ' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.Jednostka j on j.JednostkaId = jw.JednostkaId ' +
			    ' left join ' + @sourcesrv + '.' + @dbname + '.dbo.sprawaNaleznoscRodzaj snr on snr.sprawaId = spr.SprawaId and snr.naleznoscRodzajId = 4  ' +
				' cross apply ( select top 1 SprawaDluznikId, DluznikId   from ' + 
@sourcesrv + '.' + @dbname + '.dbo. SprawaDluznik knspskaz where knspskaz.SprawaId = spr.SprawaId order by knspskaz.SprawaDluznikId desc )  knss  ' +
				' INNER JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.Dluznik s  ON s.DluznikId = knss.DluznikId ' +
				' LEFT OUTER JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.DluznikAdres dsa on dsa.DluznikId = s.DluznikId and dsa.domyslny = 1 ' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.adres sa on sa.AdresId = dsa.AdresId ' +
				 ' where spr.JednostkaWydzialId > 0   ' +
				 ' and j.SAP_JednGospId = ' + @SAPjednostka  +
	    '  and ( isnull(dznal.przypis , 0) > 0   and  dznal.data  < ' + 
@nextdaystring + ' and dznal.data >= ' + @dataodstring  +')' +
		' order by spr.SprawaId '


  		  


print (@query1 )

EXEC (@query1 )
end
GO
/****** Object:  StoredProcedure [dbo].[sp_PrzypisyCR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_PrzypisyCR]
	-- Add the parameters for the stored procedure here
	 -- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortDzienString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dataDo,120)  +''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  

set @query1 = 
		   ' SELECT     case s.osoba_lub_firma when 0 then '' '' else ''X'' end as "Osoba fizyczna/Osoba prawna", '+
		   '  case s.osoba_lub_firma when 0  then s.imie else left(s.nazwisko,40) end as "Imiê/Nazwa 1",  '	 +
		    ' case s.osoba_lub_firma when 0  then left(s.nazwisko,40) else substring(s.nazwisko,41,40) end as	"Nazwisko / Nazwa 2", ' +
		    ' sa.ul  as	 "Ulica", ' +
		    ' cast( case when CHARINDEX(''/'',sa.nrdom) > 0 THEN left (sa.nrdom,CHARINDEX(''/'',sa.nrdom) -1)  else  sa.nrdom end as varchar(10) )   as "Nr domu", '+
		    ' case when CHARINDEX(''/'',sa.nrdom) > 0 THEN substring (sa.nrdom,CHARINDEX(''/'',sa.nrdom) + 1,20) else  '''' end   as "Nr mieszkania", '+
		    ' case when len(rtrim(sa.kod)) > 2   then left(sa.kod,2) + ''-'' + substring(sa.kod,3,4) else '''' end as "Kod pocztowy", ' +
		    ' case when len(sa.miejscowosc) > 0  then sa.miejscowosc  else sa.miejsce  end as "Miejscowoœæ", '+
		    ' case when sa.panstwo like ''%pols%'' or isnull(rtrim(sa.panstwo),'''') = '''' or rtrim(sa.panstwo)=''rp'' then '+
			'		  ''PL'' else sa.panstwo end   as "Klucz kraju", ' +
			' '''' as IBAN, ' +
			' case when isnull(s.rbn,0) = 0 then case when s.osoba_lub_firma = 0  then  ''09''  else ''08'' end  ' +
			' else case when s.rbn > 9  then cast (s.rbn as varchar(2 )) else  ''0'' + cast (s.rbn as varchar(1)) end      	end as "Kwalifikator do RBN", ' +
			' ''KN'' as "Typ konta umowy", ' +
			' spr.nr_karty_dl "Oznaczenie konta umowy", '+
			' ''99'' as "Relacja konta", '+
			' spr.id_sad as   IdSaduOrzek, '+
			' spr.sygnatura as Sygnatura, ' +
			' ''    '' as "Jednostka gospodarcza", '+ 
			' ''          '' as "Nr wydzia³u i sekcji", '+
			' ''      '' as Repertorium, ' +
			' ''      '' as "Nr sprawy", ' +
			' 0 as "Rok", ' + 
			' ''     '' as "Rodzaj sprawy", '+
			' dznal.data_r  as "Data dokumentu koszty",   dznal.data_r  as "Data dokumentu grzywna"  , ' +
			' dznal.data_r as "Data ksiêgowania", ' +
			' ''NS'' as "Rodzaj dokumentu", ' +
			' ''PLN'' as "Waluta", ' + 
			' ''            '' as "Klucz uzgodnienia", '+
			' ''    '' as  "Jednostaka gospodarca w³asna", ' +
			' spr.grzywna_sam as Czysamoistna, '+
			' ''N010''   as  "Operacja g³ówna", '+
			' ''    ''   as  "Czêœciowo grzywna", ' +
			'  ''    ''   as  "Czêœciowo koszty", ' +
			'  isnull(dznal.przypis_grzywny , 0) as grzywna, '+
			'  isnull(dznal.przypis_kosztow, 0) as koszty, ' +
 			' case    when isnull(kn.data_dorecz,cast(''1900-01-01'' as datetime)) > cast(''2000-01-01'' as datetime)  	then  '+
			'	case when ksn.typ_ks  =  2  or (select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id ) > 0  then  '+
			'	   dateadd (dd,30, kn.data_dorecz) 	 else    dateadd (dd,14, kn.data_dorecz) 	  end    else     spr.data_u  end   as "Data wymagalnoœci", ' +
			' case when exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_nal_okres  	where ' +
			'  kns_nal_okres.id_sprawy = spr.id and ' +
			' isnull(kns_nal_okres.koniec,''2099-01-01'') > '+ @datadostring + '  and  ' +
			' isnull(kns_nal_okres.poczatek,''1900-01-01'') < ' + @nextdaystring + '  and kns_nal_okres.koszty > 0 ) ' +
			'	and not exists (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id '+
			 '   and knsegz.data_pocz < ' +  @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @datadostring +
			 ' 	)  	then  ''B''   else 	 ''''  end  as "Raty koszty", '+
			 ' case when exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_nal_okres 	where  	kns_nal_okres.id_sprawy = spr.id and '+
			 '					isnull(kns_nal_okres.koniec,''2099-01-01'') > ' + @datadostring  + '  and  ' +
			 '					isnull(kns_nal_okres.poczatek,''1900-01-01'') <  ' + @nextdaystring + ' and kns_nal_okres.grzywna > 0 ) ' +
			 '	 				and not exists (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id ' + 
			 '					and knsegz.data_pocz < ' + @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @datadostring  +
			 '	  			)  	then  	 ''B'' 	 else 	 ''''  	 end  as "Raty grzywna", '+
			 '  case when exists ( select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id '+
			 '					and knsegz.data_pocz < ' + @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @datadostring +
			 '					and kwota > 0 	)  	 then 	 ''C'' 	 else  ''''   end  as "Egzekucja grzywny", '+
			 '	 case when exists ( select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id '  +
			 '					and knsegz.data_pocz < '+  @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @datadostring  +
			 '					and koszty > 0 	) 		 then 		 ''C'' 	 else 	 ''''  	 end  as "Egzekucja koszty", ' +
			 '			 case when	exists ( select null  from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.okres  ' +
			 '					 where okres.id_sprawy = spr.id  and okres.typ_s  = 1 and okres.kwota > 0 and   ' +
			 '					 okres.poczatek < ' + @nextdaystring + ' and okres.koniec > ' + @datadostring + ' )  ' +
			 '		 then  	 ''D'' 	 else 	 '''' end  as "Grzywny odroczone" , ' +
			 '			 case when	exists ( select null  from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.okres  ' +
			 '					 where okres.id_sprawy = spr.id  and okres.typ_s  = 0 and okres.kwota > 0 and   '  +
			'					 okres.poczatek < ' +  @nextdaystring + ' and okres.koniec> ' +  @dataodstring + ' ) ' +  	
			 ' then  ''D'' 	 else  	 ''''  	 end 	 as "Koszty odroczone" , ' +
			 ' case when exists (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_kara where kns_kara.id_sprawy = spr.id and isnull(kns_kara.data_post,kns_kara.data_post_wo) < ' + @nextdaystring + ' ) ' + 
			 '  then      ''I''   else 	 ''''  	 end       as "Kara zastêpcza" , ' +
			 '  spr.id as Sprawa_id, ' +
			 '   spr.ksiega as Ksiega, ' +
			 '   left((select isnull(skor.nazwa,'''') + '' '' + isnull(skor.miejsce,'''') + '' '' + isnull(skor.nazwa2,'''') from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.skor where spr.id_sad = skor.id) ,100) as SadKns, ' +
			 '  cast(rtrim(ltrim(isnull(s.pesel,''''))) as varchar(11)) as Pesel, '+
			 '  cast(rtrim(ltrim(   replace(replace(isnull(s.nip,''''),''-'',''''),'' '' ,'''')  ) ) as varchar(10)) as NIP, '+
			 '  ''    '' as "Rodzaj przedmiotu umowy", ' + --SCYW Sygnatura – sprawa cywilna SGOS Sygnatura–sprawa gospodarcza SKAR Sygnatura – sprawa karna SPPR Sygnatura–sprawa prawo pracy SROD Sygnatura-sprawa rodzinna SUBE Sygnatura–sprawa ubezpieczenia
			  ' ''001'' as "Iloœæ tomów" ,   cast( '''' as varchar(50)) as  Opis, dznal.pos as pozycja ' +
			   ' FROM '	 +
			     @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal AS dznal ' +
			     '    INNER JOIN   ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_sprawa AS spr  on dznal.id_sprawy = spr.id ' +
				'	cross apply ( select top 1 id, id_skazany, flag_wiezien, id_adr   from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_sprawa_skazany knspskaz where knspskaz.id_sprawy = spr.id order by knspskaz.id desc )  knss  ' +
				'	 INNER JOIN 	 ' +  @sourcesrv +'.'  + @dbname +'.dbo.skazani AS s  ON s.id = knss.id_skazany LEFT OUTER JOIN  ' +
                '    '  +   @sourcesrv +'.'  + @dbname +'.dbo.skaz_adres sa on knss.id_adr = sa.id  inner join ' +  @sourcesrv +'.'  + @dbname +'.dbo.ksiegi_sady kss on spr.ksiega  = kss.id inner join ' +
                '    '  +   @sourcesrv +'.'  + @dbname +'.dbo.ksiegi_nazwy  ksn on kss.id_nazwy = ksn.id inner join '  +   @sourcesrv +'.'  + @dbname +'.dbo.kns_nal kn on spr.id  = kn.id_sprawy  ' +
        ' where isnull(spr.czyus,0) = 0 and spr.ksiega > 0 and  ' +
	    '  ( isnull(dznal.przypis_grzywny , 0) > 0  or  isnull( dznal.przypis_kosztow, 0) > 0 ) and  dznal.data_r  < ' + @nextdaystring + ' and dznal.data_r >= ' + @dataodstring  +
		' order by spr.id ' 


  		  	



EXEC (@query1 )
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Przypisy]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Przypisy]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12),
  	      @shortDzienString  varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dataDo,120)  +''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  
  		  	
set @query1 = 'select  '+
	         '  dluos.fizpraw as "Osoba fizyczna/Osoba prawna" , ' +
			 '  dluos.imie as "Imiê/Nazwa 1", ' +
			 '	dluos.nazwisko as  "Nazwisko / Nazwa 2", ' +
			'	cast(ltrim(rtrim(dluos.Pesel)) as varchar(11))  as Pesel, '+ 
			'	case when len(rtrim(isnull(dloa.ulica,''''))) > 0 then rtrim(isnull(dloa.ulica,'''')) else isnull(dloa.miejscowosc,'''') end   as  "Ulica" , 	    left(isnull(dloa.nr_domu,''''),10) as "Nr domu" ,	    left(isnull(dloa.nr_lokalu,''''),10) as  "Nr mieszkania",	    left(isnull(dloa.kod_pocztowy,''''),10) AS "Kod pocztowy", ' +
	        '  case when len(rtrim(isnull(dloa.ulica,''''))) > 0 then isnull(dloa.miejscowosc,'''') else  isnull(dloa.poczta,'''') end as "Miejscowoœæ" , '+ 
	        '    case when kr.nazwa like ''%pols%'' or isnull(rtrim(kr.nazwa),'''') = '''' or rtrim(kr.nazwa)=''rp'' then   ''PL'' else kr.nazwa end   as "Klucz kraju", '+
	        ' '''' as IBAN, '+
			' case when  dluos.kod_typu_osoby = 1 and  (kr.nazwa like ''%pols%'' or isnull(rtrim(kr.nazwa),'''') = '''' or rtrim(kr.nazwa)=''rp'')   then ''09'' 		else  case when datalength(rtrim(isnull(sltp.rodzaj_podmiotu,''''))) > 0  '+
			' then 	rtrim(isnull(sltp.rodzaj_podmiotu,''''))   else   '''' 	  end      end  as "Kwalifikator do RBN", '+
	        ' ''KN'' as "Typ konta umowy", ' +
	        ' ''Kd '' + rtrim(slop.rodzaj_sprawy) + '' '' + cast(dlu.numer as varchar(6)) + ''/'' + substring(cast(dlu.rok  as varchar(4)),3,2) + case when len(rtrim(admw.wydzial))> 0  then ''/'' + admw.wydzial  else '''' end as "Oznaczenie konta umowy" , '+
 		    ' ''99'' as "Relacja konta", ' +
		    '	dlss.id_sad_obcy as IdSaduOrzek, ' +
			' rtrim(isnull(dlss.syg_wydzial,'''')) + '' '' + rtrim(isnull(dlss.syg_symbol,'''')) + '' '' + cast (rtrim(dlss.syg_nr_kolejny)  as varchar(7)) + ''/'' + substring(cast (dlss.syg_rok + 1000 as varchar(4)),3,2) as Sygnatura, '+
		    ' ''    '' as "Jednostka gospodarcza", '+
			' rtrim(dlss.syg_wydzial) as "Nr wydzia³u i sekcji", '+
			' rtrim(dlss.syg_symbol) as Repertorium, ' +
			' cast (rtrim(dlss.syg_nr_kolejny)  as varchar(7)) as "Nr sprawy", '+
			'  case   when dlss.syg_rok < 50 then   dlss.syg_rok + 2000 	when dlss.syg_rok > 50 and dlss.syg_rok < 100  then    dlss.syg_rok + 1900		   	end     as "Rok", '+
			' ''     '' as "Rodzaj sprawy" , '  +
			'  isnull(nals.data_operacji,nals.data_wprow_zapisu)  as   "Data dokumentu koszty", '+
			'  isnull(nals.data_operacji,nals.data_wprow_zapisu)  as   "Data dokumentu grzywna", ' +
			' isnull(nals.data_operacji,nals.data_wprow_zapisu)   as    "Data ksiêgowania", '+
			' ''NS'' as "Rodzaj dokumentu", '+
			' ''PLN'' as "Waluta", '+
			' ''            '' as "Klucz uzgodnienia", '+
			' ''    '' as  "Jednostaka gospodarca w³asna", '+
			' case when ltrim(rtrim(slop.rodzaj_sprawy)) = ''G'' ' +
			' then ''s'' else '''' end as Czysamoistna, 	''N010''   as  "Operacja g³ówna", '+
			' ''    ''   as  "Czêœciowo grzywna", ' +
			' ''    ''   as  "Czêœciowo koszty", '+
			' nals.grzywna as grzywna,  '+
			' nals.koszty as koszty, '+
			' isnull(isnull(dlstspr.data_1_raty,dlss.data_upraw),dlss.data_uprawomocnienia_g) as "Data wymagalnoœci" , ' +
			'  case when exists ( select null  from ' + @sourcesrv +'.' +@dbname + '.dbo.raty r  ' +
			'				where   r.id_dluznik = dlu.id_dluznik and      isnull ( r.data_usun_zapisu,''2099-12-31'') > GetDate() and ' +
			'							isnull(r.data_wyst_post, r.platna_od ) < ' +  @nextdayString + ' and '+
			'						   isnull(r.data_odwolania,''2099-01-01'') > '+ @dataDoString + ' and  r.koszty_pr > 0 ) '+
			'			and not exists ( select null  from '  + @sourcesrv + '.' + @dbname + '.dbo.egzekucje e ' +
			'			                 where  e.id_dluznik = dlu.id_dluznik and ' +   
			'						            isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and  ' +
			'						            e.data_egzekucji < ' + @nextdayString + ' and  '+ 
			'						            e.koszty_do_zaplaty > 0 )	 ' +
			'  then  ''B'' 	 else 	 '''' 	 end  as "Raty koszty", ' +
			'  case when exists ( select null  from ' + @sourcesrv +'.' + @dbname + '.dbo.raty r '+
			'					where   r.id_dluznik = dlu.id_dluznik and '+
			'					        isnull ( r.data_usun_zapisu,''2099-12-31'') > GetDate() and ' +
			'							isnull(r.data_wyst_post, r.platna_od ) < ' + @nextdayString +  '  and ' + 
			'						   isnull(r.data_odwolania,''2099-01-01'') >' +  @dataDoString  + ' and  r.grzywna_pr > 0 ) ' +
			'			and not exists ( select null  from ' + @sourcesrv + '.' + @dbname + '.dbo.egzekucje e  ' +
			'			                 where  e.id_dluznik = dlu.id_dluznik and     ' + 
			'						            isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and '+
			'						            e.data_egzekucji <  ' + @nextdayString  + ' and  '+ 
			'						            e.do_zaplaty - e.koszty_do_zaplaty > 0 ) ' +
			'	then 	 ''B'' 	 else 	 ''''  	 end  as "Raty grzywna", ' +
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.egzekucje e join  ' +  @sourcesrv +'.'  + @dbname +'.dbo.dluznik_stan_sprawy dlussprawy on dlussprawy.id_dluznik =  e.id_dluznik  '+
			'			                    where  e.id_dluznik = dlu.id_dluznik and    ' +
			'						            isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and '+
			'						            e.data_egzekucji <  ' + @nextdayString  + ' and  ' +
			'						            e.koszty_do_zaplaty > 0 and ' +
			'						            isnull(dlussprawy.data_bez,''2099-12-31'') > ' + @dataDoString  + ' ) ' + 
			' then 		 ''C''   else 	 ''''   end  as "Egzekucja grzywny", '+
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.egzekucje e join  ' +  @sourcesrv +'.'  + @dbname +'.dbo.dluznik_stan_sprawy dlussprawy on dlussprawy.id_dluznik =  e.id_dluznik  ' +
			'			                    where  e.id_dluznik = dlu.id_dluznik and   ' +  
			'									isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and ' +
			'						            e.data_egzekucji <  ' + @nextdayString  + ' and ' +
			'						             e.do_zaplaty - e.koszty_do_zaplaty > 0 and ' +
			'						            isnull(dlussprawy.data_bez,''2099-12-31'') > ' + @dataDoString + ' ) ' +
			' then 	 ''C''  else  ''''  end  as "Egzekucja koszty" , ' +
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.odroczenia_postanowienia op  '  +
			'			                    where  op.id_dluznik = dlu.id_dluznik and ' +   
			'									isnull ( op.data_usun_zapisu,''2099-12-31'') > GetDate() and '+
			'						            op.dt_post_grzywny  <  ' + @nextdayString  + ' and ' + 
			'						            op.GRZYWNA > 0 and ' + 
			'						            op.do_kiedy_grzywna <  ' + @nextdayString  + ' and ' + 
			'						            isnull(op.dt_odwolania_g,''2099-12-31'') >' + @dataDoString  + ' ) ' +
	 		' then  ''D''  else  '''' end  as "Grzywny odroczone" , ' + 
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.odroczenia_postanowienia op '+    
			'			                    where  op.id_dluznik = dlu.id_dluznik and ' +
			'			                       isnull ( op.data_usun_zapisu,''2099-12-31'') > GetDate() and  ' +   
			'						            op.dt_post_koszty  <  ' + @nextdayString  + ' and ' + 
			'						            op.koszty > 0 and ' + 
			'						            op.do_kiedy_koszty <  ' + @nextdayString  + ' and ' + 
			'						            isnull(op.dt_odwolania_k,''2099-12-31'') >' + @dataDoString  + ' ) ' + 
	 		' then 	 ''D'' 	 else 	 '''' end  as "Koszty odroczone" , '+
			'  case when exists   (select null from  '+ @sourcesrv +'.'  + @dbname + + '.dbo.wog  wogg where wogg.id_dluznik = dlu.id_dluznik and ' +
			'											  isnull ( wogg.data_usun_zapisu,''2099-12-31'') > GetDate() and ' + 
			'											  wogg.data_postanowienia < ' + @nextdayString  + ' and ' +
			'											  isnull(wogg.data_odwolania,''2099-12-31'')> ' + @dataDoString  + ' ) ' + 
			'						or exists  (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.areszty ar inner join ' +  @sourcesrv +'.'  + @dbname +'.dbo.areszty_postanowienia arpo on ar.id_areszt_p = arpo.id_areszt_p  ' +
			'									where arpo.id_dluznik = dlu.id_dluznik and ' +
			'								isnull ( arpo.data_post,ar.data_wprow_zapisu) <' + @nextdayString  + ' and ' + 
			'					        	isnull ( ar.data_usun_zapisu,''2099-12-31'') > GetDate() )'	+								  
			' then  ''I''   else  '''' 	 end     as "Kara zastêpcza" ,  '

	set @query2 = '   nals.id_dluznik as Sprawa_id,  ' + 
			'	dlu.id_r_sprawy as Ksiega, ' + 
			' left( isnull(slas.nazwa,'''') + '' '' + isnull(slas.miejscowosc,'''') + '' '' + isnull(slas.nazwa1,'''')  ,100) as SadKns, ' + 
			'  rtrim(ltrim(   replace(replace(isnull(dd.nr_identyf_podatkowe,''''),''-'',''''),'' '' ,'''')  ) ) as NIP, '+
			'  ''    '' as "Rodzaj przedmiotu umowy" , ' + 
			'  ''001'' as "Iloœæ tomów" , ' +  
			'  cast( '''' as varchar(50)) as  Opis, nals.pozycja as pozycja  ' + 
			'	from (select id_dluznik,  data_operacji, data_wprow_zapisu , ' +
			'	     nal.grzywna_przypis    as grzywna, ' +
			'	     nal.oplatakoszty_przypis  	as koszty, ' + 
            '	     nal.nr_poz  	as pozycja ' + 
            '   from ' +  @sourcesrv +'.'  + @dbname +'.dbo.naleznosci_dziennik nal where ( nal.grzywna_przypis > 0 or nal.oplatakoszty_przypis >  0 )  and  isnull(nal.data_operacji,nal.data_wprow_zapisu) >= ' + @dataOdString   + ' and   isnull(nal.data_operacji,nal.data_wprow_zapisu) < ' + @nextdayString  + ' and isnull ( nal.data_usun_zapisu,''2099-12-31'') > GetDate() ) nals '+ 
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.Dluznik dlu  ON  dlu.id_dluznik=  nals.id_dluznik ' + 
			' cross apply ( select  top 1 ' +
			' dlos.id_dluznik as id_dluznik, ' + 
			'   dlos.id_osoba  as id_osoba,  ' + 
			'	dlos.id_typ_uczestnictwa as id_typ_uczestnictwa, ' + 
			'	dlos.kod_typu_osoby as kod_typu_osoby  , ' +  
			'	case  (dlos.kod_typu_osoby)  when 1   then  '' ''     else ''X''   end   as  fizpraw, ' +
	       	'    case  (dlos.kod_typu_osoby)    when 1   then left(dlos.imie,40)    else  substring(dlos.nazwisko_nazwa,1,40)    end   as  imie, ' +
	    	'    case  (dlos.kod_typu_osoby)    when 1   then left(dlos.nazwisko,40)    else  substring(dlos.nazwisko_nazwa,41,40)     end   as  nazwisko, ' +
			'   cast(rtrim(ltrim(isnull(dlos.pesel,''''))) as varchar(11)) as Pesel ' + 
			'	from  ' + +  @sourcesrv +'.'  + @dbname + '.dbo.DLUZNIK_OSOBY dlos ' +
			'	where dlos.id_dluznik  = dlu.id_dluznik  order by dlos.id_osoba asc)    dluos ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_UCZESTNICTWA sltu ON sltu.kod=dluos.id_typ_uczestnictwa ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_OSOB slto ON slto.kod=dluos.kod_typu_osoby ' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_OSOBY_ADRESY dloa ON dloa.id_osoba=  dluos.id_osoba and dloa.czy_adres_glowny=1 ' +
			' Left join ' +  @sourcesrv +'.'  + @dbname +'.dbo.Dluznik_osoby_dane_dod dd ON dd.id_osoba = dluos.id_osoba ' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_KRAJE kr on kr.kod = dloa.kod_kraju ' +
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_RODZAJOW_SPRAW slop ON dlu.id_r_sprawy =  slop.id_r_sprawy ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_RODZAJOW_SPRAW stow ON slop.typ_sp=  stow.typ_sp ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_DZIENNIKOW sdzn ON stow.id_dziennika=  sdzn.id_dziennika ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.adm_wydzialy admw ON  slop.k_wydzial=admw.kod ' + 
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_PODMIOTY sltp ON sltp.kod=dd.id_podmiot' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_SPRAWA_SADOWA dlss ON dlss.id_dluznik  = dlu.id_dluznik ' + 
			' LEFT OUTER Join ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_ADR_SADOW slas ON dlss.id_sad_obcy = slas.kod ' +
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_STAN_SPRAWY dlstspr on dlstspr.id_dluznik = dlu.id_dluznik ' +
			'	order by dlu.id_dluznik '
			-- print @query1 + ' ' + @query2
EXEC (@query1 + ' ' + @query2)
end
GO
/****** Object:  StoredProcedure [dbo].[sp_OdpisyOR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_OdpisyOR]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12),
  		    @SAPjednostka varchar(4)
		   set @dbname = 'OrComNS'
		   
  		 IF  CHARINDEX ( '@@' , @sourcesrv ) > 0  
		BEGIN
		 set @SAPjednostka = Substring(@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) +2,4)
		 set @sourcesrv = left (@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) -1 )
		END 
		
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  
  		  	


set @query1 = 'select  spr.SprawaId  as Sprawa_Id, spr.JednostkaWydzialId as Ksiega ,  ' + 
			  ' nal.ZapisId  as Naleznosc_id ,	  ROW_NUMBER() over (order by nal.ZapisId) as Pozycja, 	  year(nal.data) as Rok,  nal.data as DataDokumentu,	' +
			  '	nal.opis as nr_dowodu,	 case nal.WplataRodzajId  ' +
									 	' when 6 then ''5a''  ' +
									 	' when 8 then ''5c''  ' +
										' else  ''5d'' end  as  ns1, ' +
			' nal.data as DataKsiegowania,  case when nal.NaleznoscTypId = 0  then  isnull(nal.odpis,0) else 0 end   as grzywna_odpis, '   +
			'  case when nal.NaleznoscTypId = 1 then  isnull(nal.odpis,0) else 0 end   as  koszty_odpis ,  
			   case nal.WplataRodzajId when 7 then ''PSU'' when 11 then ''przedawnienie'' else '''' end as zrodlo , ' +
			'	isnull(nal.opis,'''')  +  '' Poz.dz.nal: '' +  ' +
			' cast ( isnull(nal.pozycja,0) as varchar(9))  as opis '+  
			'	from   ' + @sourcesrv + '.' + @dbname + '.dbo.zapis nal LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.sprawa spr  ON  nal.SprawaId = spr.SprawaId ' +  
						' left join ' + @sourcesrv + '.' + @dbname + '.dbo.JednostkaWydzial jw on jw.JednostkaWydzialId = spr.JednostkaWydzialId ' +
						' left join ' + @sourcesrv + '.' + @dbname + '.dbo.Jednostka j on j.JednostkaId = jw.JednostkaId ' +
			'	where j.SAP_JednGospId = ' + @SAPjednostka + ' and (isnull(nal.odpis , 0) > 0) ' +	 
			'	and nal.data >= '  +  @dataOdString + ' and nal.data< ' + @nextdayString + ' order by spr.SprawaId '
 


EXEC (@query1 )
end
GO
/****** Object:  StoredProcedure [dbo].[sp_OdpisyCR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_OdpisyCR]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  
  		  	


set @query1 = 'select  spr.id  as Sprawa_Id, spr.ksiega as Ksiega ,  ' + 
			  ' nal.id  as Naleznosc_id ,	  nal.pos as Pozycja, 	  nal.rok as Rok,  nal.data_r as DataDokumentu,	' +
			  '	nal.dow as nr_dowodu,	 case nal.zrodlo  ' +
									 	' when ''z'' then case (select top 1 rtrim(isnull(flag_wiezien,'''')) from ' + @sourcesrv + '.' + @dbname + '.dbo.kns_sprawa_skazany kss where kss.id_sprawy = spr.id order by kss.id asc) ' +
									    '  when ''w'' then ''5b''   else ''5a'' 	  end ' +
									 	'  when ''u'' then ''5c''  ' +
										' else  ''5d'' end  as  ns1, ' +
			' nal.data_zapisu as DataKsiegowania,  nal.odpisanie_grzywny  as grzywna_odpis, '   +
			'  nal.odpisanie_kosztow  as koszty_odpis ,  case nal.zrodlo when ''t'' then ''PSU'' when ''c'' then ''przedawnienie'' else '''' end as zrodlo , ' +
			'	isnull(nal.dow,'''')  +  '' Poz.dz.nal: '' +  ' +
			' cast ( nal.pos as varchar(9)) + ''/'' + cast ( nal.rok as varchar(4)) as opis, nal.pos as pozycja '+  
			'	from   ' + @sourcesrv + '.' + @dbname + '.dbo.kns_dz_nal nal LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.kns_sprawa spr  ON  spr.id =  nal.id_sprawy  ' +  
			'	where nal.ksiega > 0 	and  (nal.odpisanie_grzywny <> 0 or nal.odpisanie_kosztow <> 0 ) ' +	 
			'	and nal.data_r >= '  +  @dataOdString + ' and nal.data_r< ' + @nextdayString + ' order by spr.id '
 


EXEC (@query1 )
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Odpisy]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Odpisy]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  
  		  	
set @query1 = ' select  dlu.id_dluznik as Sprawa_Id, dlu.id_r_sprawy as Ksiega ,  nal.id_naleznosci as Naleznosc_id ,	  nal.nr_poz as Pozycja, 	  nal.rok_naleznosci as Rok, '+
              ' nal.data_operacji as DataDokumentu,	nal.nr_dowodu,	' +
              ' nal.ns1,  nal.data_operacji as DataKsiegowania,  nal.grzywna_odpis       as grzywna_odpis,   nal.oplatakoszty_odpis  as koszty_odpis , ' +
              ' case nzrd.id_zrodla when 7 then ''PSU'' when 13 then ''przedawnienie'' else '''' end as zrodlo , '+
              ' isnull(nwpl.nrdokwplaty,'''') + '' Poz.dz.nal: '' + cast ( nal.nr_poz as varchar(9)) + ''/'' + cast ( nal.rok_naleznosci as varchar(4)) as opis,  nal.nr_poz  	as pozycja  '+
              ' from   ' + @sourcesrv + '.' + @dbname + '.dbo.naleznosci_dziennik nal LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.Dluznik dlu  ON  dlu.id_dluznik=  nal.id_dluznik ' +
              ' LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.NALEZNOSCI_WPLATY  nwpl ON nwpl.id_naleznosci = nal.id_NALEZNOSCI LEFT OUTER JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.S_NALEZNOSCI_ZRODLA nzrd ON nwpl.id_zrodlo = nzrd.id_zrodla '+
               ' where isnull(nal.data_usun_zapisu,''2099-01-01'') > GetDate()	and  (nal.grzywna_odpis <> 0 or nal.oplatakoszty_odpis <> 0 )	' +
               ' and nal.data_operacji >= ' + @dataOdString + ' and nal.data_operacji< ' + @nextdaystring +  ' order by dlu.id_dluznik '

print @query1 

EXEC (@query1 + ' ' + @query2)

end
GO
/****** Object:  StoredProcedure [dbo].[sp_KsiegiOR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_KsiegiOR]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50)
	  
AS
BEGIN
  DECLARE 
		  
		  
	@query varchar(MAX),
	@jego  varchar(4)
   
   IF  CHARINDEX ( '@@' , @sourcesrv ) > 0  
		BEGIN
		 set @jego = Substring(@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) +2,4)
		 set @sourcesrv = left (@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) -1 )
		END
    		   
   	
	set @dbname = 'OrComNS'
		   
    set @sourcesrv  = '"' + @sourcesrv + '"'
  		  
  		  
set @query = 'select ks.JednostkaWydzialId as Id, rtrim(ks.symbol) + '' '' + ks.nazwa as Nazwa, '''' as Wydzial ' +
			 ' from '  +   @sourcesrv + '.' + @dbname + '.dbo.jednostkawydzial ks  inner join ' + 
              @sourcesrv + '.' + @dbname + '.dbo.jednostka jg on jg.JednostkaId = ks.JednostkaId ' +
              ' where jg.SAP_JednGospId = ' + @jego 
              
  
print @query
Exec (@query)			

   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_KsiegiCR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_KsiegiCR]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50)
	  
AS
BEGIN
  DECLARE 
		  
		  
   @query varchar(MAX)
		   
    set @sourcesrv  = '"' + @sourcesrv + '"'
  		  
  		  
set @query = 'select ks.Id, rtrim(ks.skrot) + ''  '' + kn.nazwa as Nazwa, sk.nazwa2 as Wydzial     from ' + 
              @sourcesrv + '.' + @dbname + '.dbo.ksiegi_sady ks  inner join ' + 
              @sourcesrv + '.' + @dbname + '.dbo.ksiegi_nazwy kn on kn.id = ks.id_nazwy inner join ' + 
              @sourcesrv + '.' + @dbname + '.dbo.skor sk on sk.id = ks.id_sadu  where sys  = 0 ' 
  
print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Ksiegi]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Ksiegi]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50)
	  
AS
BEGIN
  DECLARE 
		  
		  
   @query varchar(MAX)
		   
    set @sourcesrv  = '"' + @sourcesrv + '"'
  		  
  		  
set @query = '  select distinct   ltrim(admw.wydzial + '' '' + slop.rodzaj_sprawy +'' ('' + slop.nazwa + '')'' )   as Nazwa, admw.wydzial as Wydzial ,slop.id_r_sprawy as Id  ' +
             '  from  ' + @sourcesrv + '.' + @dbname + '.dbo.SL_RODZAJOW_SPRAW slop ' +
		     '  LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.adm_wydzialy admw ON  slop.k_wydzial=admw.kod ' +
             '  LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.S_TYPY_RODZAJOW_SPRAW stow ON slop.typ_sp=  stow.typ_sp ' +
             '  where exists (select null from ' + @sourcesrv + '.' + @dbname + '.dbo.dluznik dlu where dlu.id_r_sprawy = slop.id_r_sprawy ) ' + 
              ' order by 2 ' 


 
print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_KomornicyCR]    Script Date: 08/13/2015 17:01:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_KomornicyCR]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		   @nextday Datetime,
		   @query varchar(MAX),
		   @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		    

		   
   
  		  
  		  
set @query = ' select s.id as Id, max(rtrim(isnull(s.nazwa,'''')+ '' '' + isnull(s.nazwa2,''''))) as nazwa,   max(isnull('''' + s.miejsce,'''')) as miasto,  max(isnull('''' + s.ulica,'''')) as ulica , count(*) as ile ' +
             ' from  ' + @sourcesrv + '.' + @dbname + '.dbo.skor s inner join  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_egzekucja egz on egz.id_egz = s.id inner join '     + @sourcesrv + '.' + @dbname + '.dbo.kns_sprawa spr on spr.id_egzekucji = egz.id ' +
             ' where s.typ = 10 and  ( select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r < ' + @nextdaystring + ' ) > 0 ' +
             ' or (select isnull(sum (kns_dz_nal.przypis_kosztow), 0) - isnull(sum (kns_dz_nal.uiszczenia_kostow), 0) - isnull(sum(kns_dz_nal.odpisanie_kosztow), 0) from  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r  < ' + @nextdaystring + ' )> 0 '  +
              ' group by s.id  order by ile desc '

 
print @query
Exec (@query)			   
end
GO
/****** Object:  Table [dbo].[User]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](100) NOT NULL,
	[Pssword] [varchar](100) NOT NULL,
	[role] [int] NOT NULL,
	[LastPwdChngDate] [datetime] NULL,
	[suspend] [bit] NULL,
	[ChangePwd] [bit] NULL,
	[FirstName] [varchar](100) NULL,
	[LastName] [varchar](100) NULL,
	[deleted] [bit] NOT NULL,
	[CreationDate] [datetime] NULL,
	[DeleteDate] [datetime] NULL,
	[PwdPeriodChange] [int] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TypTransferu]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TypTransferu](
	[kod] [int] NOT NULL,
	[opis] [varchar](50) NULL,
 CONSTRAINT [PK_TypTransferu] PRIMARY KEY CLUSTERED 
(
	[kod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (1, N'Salda')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (2, N'Przypisy')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (3, N'Odpisy')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (4, N'Wp³aty')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (5, N'Raty/Egzekucje/Odroczenia')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (6, N'Uiszczenia Grz.Odp.')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (7, N'Zwrot 3/4')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (8, N'Przypis op³at')
INSERT [dbo].[TypTransferu] ([kod], [opis]) VALUES (9, N'Terminy Wymagalnoœci')
/****** Object:  Table [dbo].[Transfer]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transfer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DataTransferu] [datetime] NOT NULL,
	[rodzaj] [int] NULL,
	[LFaktow] [int] NULL,
	[DataOd] [datetime] NULL,
	[DataDo] [datetime] NULL,
	[Uwagi] [varchar](255) NULL,
	[status] [int] NOT NULL,
	[SAPKluczUzgodnienia] [varchar](12) NULL,
 CONSTRAINT [PK_Transfer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Sprawa]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Sprawa](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sygnatura] [varchar](100) NULL,
	[Karta] [varchar](30) NULL,
	[SAPSadId] [varchar](4) NULL,
	[SAPWydzia³] [varchar](20) NULL,
	[SAPRepertorium] [varchar](10) NULL,
	[Numer] [int] NULL,
	[Rok] [int] NULL,
	[SAPRodzajPrzedmiotuUmowy] [varchar](4) NULL,
	[SAPKontoUmowy] [varchar](20) NULL,
	[DataWyroku] [datetime] NULL,
	[DataPrawomocn] [datetime] NULL,
	[DataWymagalnosci] [datetime] NULL,
	[KnsSprawa_id] [int] NULL,
	[KNSSadOrzek_id] [int] NULL,
	[KnsSad] [varchar](100) NULL,
	[KnsWydzial] [varchar](100) NULL,
	[KnsWydz_Id] [int] NULL,
	[wyklucz] [int] NULL,
	[KnsKsiega] [int] NULL,
	[SAPRodzajSprawy] [varchar](5) NULL,
	[SAPTomyAkt] [varchar](3) NULL,
	[KdNumer] [int] NULL,
	[KdRok] [int] NULL,
	[SAPPrzedmiotUmowy] [varchar](20) NULL,
	[grzSamoistna] [varchar](1) NULL,
	[SAPTypKontaUmowy] [varchar](2) NULL,
	[SAPRelacjaKontaUmowy] [varchar](2) NULL,
 CONSTRAINT [PK_Sprawa] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_Zwrot34CR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Zwrot34CR]

                -- Add the parameters for the stored procedure here

                -- Add the parameters for the stored procedure here


@sourcesrv varchar(50),
@dbname varchar(50),
@dataOd DateTime,
@dataDo DateTime
         


AS

BEGIN

declare 
/*
@sourcesrv varchar(50),
@dbname varchar(50),
@dataOd DateTime,
@dataDo DateTime,
*/  

	@nextday Datetime,
	@query1 varchar(MAX),
	@query2 varchar(MAX),
	@query3 varchar(MAX),
	@dataOdString varchar(12),
	@dataDoString varchar(30),
	@nextdayString varchar(12),
	@shortDzienString varchar(12)

set @nextday  = DateAdd(d,1,@dataDo)
set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
set @shortDzienString = '''' + convert ( varchar(10),@dataDo,120)  +''''

--set @sourcesrv  = 'Fin1'
--set @dbname = 'wcyw_j'


set @query1 =                                          
' set QUOTED_IDENTIFIER on ' +
' select ' +
'  ro.ident  ' +
' , case when ds.fizpraw = 0 then  '' ''  else ''X'' end as fizpraw ' +
' , rtrim(case when ds.fizpraw = 0 then ds.imie else left(ds.nazwisko,40) end) as Imie_Nazwisko ' +
' , rtrim(case when ds.fizpraw = 0 then left(ds.nazwisko,40) else substring(ds.nazwisko,41,40) end) as Nazwisko_Nazwa2 ' +
' , rtrim(ds.pesel) as pesel ' +
' , rtrim(ds.nip) as nip ' +
' , rtrim(st.nr_konta) as nr_konta ' +
' , rtrim(ad.ulica) as ulica ' +
' , rtrim(cast( case when CHARINDEX(''/'',ad.numer) > 0 THEN left (ad.numer,CHARINDEX(''/'',ad.numer) -1)  else  ad.numer end as varchar(10) ))   as nr_domu ' +
' , case when CHARINDEX(''/'',ad.numer) > 0 THEN substring (ad.numer,CHARINDEX(''/'',ad.numer) + 1,20) else '' ''  end   as Nr_mieszkania ' +
' , case when len(rtrim(ad.kod)) > 2   then left(ad.kod,2) + ''-'' + substring(ad.kod,3,3) else '' '' end as kod ' +
' , rtrim(case when len(ad.miejscowosc) > 0  then ad.miejscowosc  else ad.poczta  end) as Miejscowosc ' +
' , rtrim(case when ad.kraj like ''%pols%'' or isnull(rtrim(ad.kraj),'''') =  '''' or rtrim(ad.kraj)=''rp'' then ''PL'' else ad.kraj end) as kraj ' +
' , rtrim(ko.oznaczenie) as oznaczenie' +
' , rtrim(re.symbol) as ''Repertorium''  ' +
' , sp.numer ' +
' , sp.rok ' +
' , sp.d_prawomoc ' +
' , ro.kwota ' +
' , sp.d_zakreslenia ' +
' , sp.d_wplywu' +
' , rz.nazwa' +
' , rtrim(ko.oznaczenie)  +rtrim(re.symbol) +'' ''+rtrim(cast(sp.numer as varchar)) + ''/'' + right(cast(sp.rok as varchar(4)),2) as sygnatura ' +
' into dbo.##doch_zapis ' + 
' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.roszczenie ro ' +
' left join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.sprawa sp on sp.ident = ro.id_sprawy ' +
' left join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.strona st on st.id_sprawy = sp.ident and st.czyus = 0 ' +
' left join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.status sta on sta.ident = st.id_statusu and sta.czyus = 0 and sta.typ_roli = 2 ' +
' left join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.dane_strony ds on ds.ident = st.id_danych and ds.czyus = 0 ' +
' left join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.adres ad on ad.id_strony = st.ident and ad.czybiezacy = 1 and ad.czyus = 0 ' +
' cross join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.konfig ko ' +
' left join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.repertorium re on re.numer = sp.repertorium and re.rodzaj = 0 ' +
' left join "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.rodzaj_zalatwienia rz on rz.ident = sp.id_rodzaju ' +
' where sp.czyus = 0 ' +
' and ro.kwota > 0 ' +
' and ro.czyus = 0 ' +
' and sta.typ_roli = 2 ' +
 ' and ro.typ_kwoty = 1 ' +
'set QUOTED_IDENTIFIER off ' 

--select * from dbo.##dep_zapis
print @query1
exec (@query1)


select 

za.fizpraw as 'Osoba fizyczna/Osoba prawna'
, za.Imie_Nazwisko as 'Imiê/Nazwa 1'
, za.Nazwisko_Nazwa2 as 'Nazwisko / Nazwa 2'
, za.ulica as Ulica
, za.nr_domu as 'Nr domu'
, za.nr_mieszkania as 'Nr mieszkania'
, za.kod as 'Kod pocztowy'
, za.miejscowosc as 'Miejscowoœæ'
, za.kraj as 'Klucz kraju'
, case when len(za.nr_konta) > 10 then  'PL'+za.nr_konta else '' end as 'IBAN'
, '09' as 'Kwalifikator do RBN'
, 'DO' as 'Typ konta umowy'
, za.sygnatura +'/DN'  as 'Oznaczenie konta umowy'
, '99' as 'Relacja konta'
, null as 'IdSaduOrzek'
, rtrim(za.oznaczenie) + ' ' + rtrim(za.repertorium) + ' ' +cast(za.numer as varchar) +'/'+ substring(cast(za.rok as varchar),3,2)  as 'Sygnatura'
, '' as 'Jednostka gospodarcza'
, za.oznaczenie as 'Nr wydzia³u i sekcji'
, za.repertorium
, za.numer as 'Nr sprawy'
, za.Rok
, 'SCYW' as 'Rodzaj sprawy'
, za.d_prawomoc as 'Data dokumentu koszty'
, za.d_prawomoc as 'Data dokumentu grzywna'
, za.d_prawomoc as 'Data ksiêgowania'
, 'DN' as 'Rodzaj dokumentu'
, 'PLN' as 'Waluta'
, '' as 'Klucz uzgodnienia'
, '' as 'Jednostka gospodarcza w³asna'
, null as 'Czysamoistna'
, 'P020' as 'Operacja g³ówna'
, '0040' as 'Czêœciowo grzywna'
, '' as 'Czêœciowo koszty'
, -Round(za.kwota * 0.75,0) as grzywna
, -Round(za.kwota *0,75,0) as koszty
, za.d_prawomoc as 'Data wymagalnoœci'
, '' as 'Raty koszty'
, '' as 'Raty grzywna'
, '' as 'Egzekucja grzywny'
, '' as 'Egzekucja koszty'
, '' as 'Grzywny odroczone'
, '' as 'Koszty odroczone'
, '' as 'Kara zastêpcza'
, za.ident as 'Sprawa_id'
, 1 as 'Ksiega'
, null as 'SadKns'
, isnull(za.pesel,'') as PESEL
, '' as 'NIP'
, '' as 'Rodzaj przedmiotu umowy'
, '001' as 'Iloœæ tomów'
, '' as opis
, za.ident as pozycja
--select *
from dbo.##doch_zapis za
where za.d_prawomoc between @dataOd and @dataDo
and za.kwota > 0

--select * from ##doch_zapis
drop table ##doch_zapis

/*
exec sp_PrzypisyCR_doch 'fin1', 'wcyw_lancut', '2015-01-01', '2015-03-31'
*/
 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UgoOR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UgoOR]
	-- Add the parameters for the stored procedure here
	 -- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortDzienString varchar(12),
  		  @SAPjednostka varchar(4)
		   set @dbname = 'OrComNS'
		   
  		 IF  CHARINDEX ( '@@' , @sourcesrv ) > 0  
		BEGIN
		 set @SAPjednostka = Substring(@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) +2,4)
		 set @sourcesrv = left (@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) -1 )
		END 
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dataDo,120)  +''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 



set @query1 =



		   ' SELECT     case when s.PodmiotTypId = 1 then '' '' else ''X'' end 
as "Osoba fizyczna/Osoba prawna", ' +
		   '  case when s.PodmiotTypId = 1 then s.imie else
left(s.nazwisko,40) end as "Imiê/Nazwa 1", ' +
		   '  case when s.PodmiotTypId = 1 then left(s.nazwisko,40) else 
substring(s.nazwisko,41,40) end as	"Nazwisko / Nazwa 2",  ' +
		    '  case when len(isnull(sa.miejsce,'''')) > 0  then left(sa.miejsce,60) else left(sa.miejscowosc,60) end as	 "Ulica",  ' +
		   '  sa.dom   as "Nr domu", ' +
		   '  sa.lokal   as "Nr mieszkania", ' +
		   '  sa.kod as "Kod pocztowy",  ' +
		   '  case when len(sa.miejscowosc) > 0  then sa.miejscowosc  else sa.miejsce  end as "Miejscowoœæ",  ' +
		   '  case when sa.kraj like ''%pols%'' or isnull(rtrim(sa.Kraj),'''') = '''' or rtrim(sa.Kraj)=''rp'' then  ' +
			'		  ''PL'' else sa.Kraj end   as "Klucz kraju",  ' +
			' '''' as IBAN,  ' +
			' case when isnull(spr.KlasyfikacjaRbnId,0) = 1 then ''09'' else 
''08'' end    as "Kwalifikator do RBN" ' +
			' , ''KN'' as "Typ konta umowy",  ' +
			' spr.numer  "Oznaczenie konta umowy", ' +
			' ''99'' as "Relacja konta", ' +
			' spr.OrzeczenieSadAdresatId as   IdSaduOrzek, ' +
			' spr.sygnatura as Sygnatura,  ' +
			' ''    '' as "Jednostka gospodarcza",  ' +
			' spr.SygnWydzial as "Nr wydzia³u i sekcji", ' +
			' spr.SygnRepetytorium as Repertorium,  ' +
			' spr.SygnNumer as "Nr sprawy",  ' +
			' spr.SygnRok as "Rok",   ' +
			' ''     '' as "Rodzaj sprawy", ' +
			' (select top 1 data from  ' +	@sourcesrv + '.' +   @dbname +'.dbo.zapis where zapis.SprawaId = spr.SprawaId and  data < ' +
			@nextdaystring + '  and zapis.przypis > 0 and zapis.NaleznoscTypId = 1  order by  data desc ) as  "Data dokumentu koszty",   ' +
			' (select top 1 data from  ' +	@sourcesrv + '.' +   @dbname +'.dbo.zapis where zapis.SprawaId = spr.SprawaId and  data < ' +
			@nextdaystring + '  and zapis.uiszczenie > 0 and zapis.WplataRodzajId = 5 order by  data desc ) as  "Data dokumentu grzywna", ' +
			' dznal.data as  "Data ksiêgowania", ' +
			' ''NS'' as "Rodzaj dokumentu",  ' +
			' ''PLN'' as "Waluta",  ' +
			' ''            '' as "Klucz uzgodnienia", ' +
			' ''    '' as  "Jednostaka gospodarca w³asna" ' +
			' , case when snr.naleznoscRodzajId = 4 then ''s''  ' +
			'	when spr.SygnRepetytorium = ''W'' and spr.PrzypisG > 0 then ''s''  
' +
			'	end as Czysamoistna, ' +
			' ''N010''   as  "Operacja g³ówna", ' +
			' ''    ''   as  "Czêœciowo grzywna", ' +
			'  ''    ''   as  "Czêœciowo koszty", ' +
			' uiszczenie as grzywna, ' +
			' 0 as koszty, ' +

  		 ' case    when isnull(spr.dataDoreczenia,cast(''1900-01-01'' as 
datetime)) > cast(''2000-01-01'' as datetime)  	then  ' +
		 ' case    when spr.SygnRepetytorium IN (''W'',''K'') then dateadd 
(dd,30, spr.dataDoreczenia) 	 else    dateadd (dd,14, 
spr.dataDoreczenia) ' +
		 ' end    else     spr.DataPrawomocnosci  end   as "Data wymagalnoœci", ' +

		 ' ''''  as "Raty koszty", ' +
		 ' '''' as  "Raty grzywna", ' +
		 ' ''''  as "Egzekucja grzywny", ' +
		 ' ''''  as "Egzekucja koszty",  ' +
		 ' ''''  as "Grzywny odroczone" , ' +
		 ' ''''  as "Koszty odroczone" , ' +
		 ' ''I''  as "Kara zastêpcza" , ' +
		 '  spr.SprawaId as Sprawa_id, ' +
			'    spr.JednostkaWydzialId as Ksiega,  ' +
			 ' isnull(j.nazwa,'''')   as SadKns,  ' +
			  '  cast(rtrim(ltrim(isnull(s.pesel,''''))) as varchar(11)) as Pesel ' +
			  ' , s.nip as NIP, ' +
			  ' ''    '' as "Rodzaj przedmiotu umowy", ' +
			   --SCYW Sygnatura – sprawa cywilna SGOS Sygnatura–sprawa gospodarcza SKAR Sygnatura – sprawa karna SPPR Sygnatura–sprawa prawo pracy SROD Sygnatura-sprawa rodzinna SUBE Sygnatura–sprawa ubezpieczenia
			 ' ''001'' as "Iloœæ tomów" ,   cast( '''' as varchar(50)) as  Opis, ROW_NUMBER() over (order by dznal.ZapisId) as pozycja ' +
			 	' from ' + @sourcesrv +'.'  + @dbname +'.dbo.zapis AS dznal ' +
				' left join ' +@sourcesrv + '.' +   @dbname +'.dbo.sprawa AS spr on spr.SprawaId = dznal.SprawaId' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.JednostkaWydzial jw on jw.JednostkaWydzialId = spr.JednostkaWydzialId ' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.Jednostka j on j.JednostkaId = jw.JednostkaId ' +
			    ' left join ' + @sourcesrv + '.' + @dbname + '.dbo.sprawaNaleznoscRodzaj snr on snr.sprawaId = spr.SprawaId and snr.naleznoscRodzajId = 4  ' +
				' cross apply ( select top 1 SprawaDluznikId, DluznikId   from ' + 
@sourcesrv + '.' + @dbname + '.dbo. SprawaDluznik knspskaz where knspskaz.SprawaId = spr.SprawaId order by knspskaz.SprawaDluznikId desc )  knss  ' +
				' INNER JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.Dluznik s  ON s.DluznikId = knss.DluznikId ' +
				' LEFT OUTER JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.DluznikAdres dsa on dsa.DluznikId = s.DluznikId and dsa.domyslny = 1 ' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.adres sa on sa.AdresId = dsa.AdresId ' +
				 ' where spr.JednostkaWydzialId > 0   ' +
				 ' and j.SAP_JednGospId = ' + @SAPjednostka  +
	    '  and ( isnull(dznal.uiszczenie , 0) > 0   and dznal.WplataRodzajId = 5  and   dznal.data  < ' + 
@nextdaystring + ' and dznal.data >= ' + @dataodstring  +')' +
		' order by spr.SprawaId '


  		  

print (@query1)


EXEC (@query1 )
end

--exec sp_Przypisy 'Fin1', 'kns_orcom', '2014-12-01', '2014-12-31'
GO
/****** Object:  StoredProcedure [dbo].[sp_UgoCR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UgoCR]
	-- Add the parameters for the stored procedure here
	 -- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortDzienString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dataDo,120)  +''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  

set @query1 = 
		   ' SELECT     case s.osoba_lub_firma when 0 then '' '' else ''X'' end as "Osoba fizyczna/Osoba prawna", '+
		   '  case s.osoba_lub_firma when 0  then s.imie else left(s.nazwisko,40) end as "Imiê/Nazwa 1",  '	 +
		    ' case s.osoba_lub_firma when 0  then left(s.nazwisko,40) else substring(s.nazwisko,41,40) end as	"Nazwisko / Nazwa 2", ' +
		    ' sa.ul  as	 "Ulica", ' +
		    ' cast( case when CHARINDEX(''/'',sa.nrdom) > 0 THEN left (sa.nrdom,CHARINDEX(''/'',sa.nrdom) -1)  else  sa.nrdom end as varchar(10) )   as "Nr domu", '+
		    ' case when CHARINDEX(''/'',sa.nrdom) > 0 THEN substring (sa.nrdom,CHARINDEX(''/'',sa.nrdom) + 1,20) else  '''' end   as "Nr mieszkania", '+
		    ' case when len(rtrim(sa.kod)) > 2   then left(sa.kod,2) + ''-'' + substring(sa.kod,3,4) else '''' end as "Kod pocztowy", ' +
		    ' case when len(sa.miejscowosc) > 0  then sa.miejscowosc  else sa.miejsce  end as "Miejscowoœæ", '+
		    ' case when sa.panstwo like ''%pols%'' or isnull(rtrim(sa.panstwo),'''') = '''' or rtrim(sa.panstwo)=''rp'' then '+
			'		  ''PL'' else sa.panstwo end   as "Klucz kraju", ' +
			' '''' as IBAN, ' +
			' case when isnull(s.rbn,0) = 0 then case when s.osoba_lub_firma = 0  then  ''09''  else ''08'' end  ' +
			' else case when s.rbn > 9  then cast (s.rbn as varchar(2 )) else  ''0'' + cast (s.rbn as varchar(1)) end      	end as "Kwalifikator do RBN", ' +
			' ''KN'' as "Typ konta umowy", ' +
			' spr.nr_karty_dl "Oznaczenie konta umowy", '+
			' ''99'' as "Relacja konta", '+
			' spr.id_sad as   IdSaduOrzek, '+
			' spr.sygnatura as Sygnatura, ' +
			' ''    '' as "Jednostka gospodarcza", '+ 
			' ''          '' as "Nr wydzia³u i sekcji", '+
			' ''      '' as Repertorium, ' +
			' ''      '' as "Nr sprawy", ' +
			' 0 as "Rok", ' + 
			' ''     '' as "Rodzaj sprawy", '+
			' dznal.data_r  as "Data dokumentu koszty",   dznal.data_r  as "Data dokumentu grzywna"  , ' +
			' dznal.data_r as "Data ksiêgowania", ' +
			' ''NS'' as "Rodzaj dokumentu", ' +
			' ''PLN'' as "Waluta", ' + 
			' ''            '' as "Klucz uzgodnienia", '+
			' ''    '' as  "Jednostaka gospodarca w³asna", ' +
			' spr.grzywna_sam as Czysamoistna, '+
			' ''N010''   as  "Operacja g³ówna", '+
			' ''    ''   as  "Czêœciowo grzywna", ' +
			'  ''    ''   as  "Czêœciowo koszty", ' +
			'  isnull(dznal.grzywna_areszt , 0) as grzywna, '+
			'  0 as koszty, ' +
			' case    when isnull(kn.data_dorecz,cast(''1900-01-01'' as datetime)) > cast(''2000-01-01'' as datetime)  	then  '+
			'	case when ksn.typ_ks  =  2  or (select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id ) > 0  then  '+
			'	   dateadd (dd,30, kn.data_dorecz) 	 else    dateadd (dd,14, kn.data_dorecz) 	  end    else     spr.data_u  end   as "Data wymagalnoœci", ' +
 		      ' ''''  as "Raty koszty", ' +
		 ' '''' as  "Raty grzywna", ' +
		 ' ''''  as "Egzekucja grzywny", ' +
		 ' ''''  as "Egzekucja koszty",  ' +
		 ' ''''  as "Grzywny odroczone" , ' +
		 ' ''''  as "Koszty odroczone" , ' +
		   ' ''I''     as "Kara zastêpcza" , ' +
			 '  spr.id as Sprawa_id, ' +
			 '   spr.ksiega as Ksiega, ' +
			 '   left((select isnull(skor.nazwa,'''') + '' '' + isnull(skor.miejsce,'''') + '' '' + isnull(skor.nazwa2,'''') from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.skor where spr.id_sad = skor.id) ,100) as SadKns, ' +
			 '  cast(rtrim(ltrim(isnull(s.pesel,''''))) as varchar(11)) as Pesel, '+
			 '  cast(rtrim(ltrim(   replace(replace(isnull(s.nip,''''),''-'',''''),'' '' ,'''')  ) ) as varchar(10)) as NIP, '+
			 '  ''    '' as "Rodzaj przedmiotu umowy", ' + --SCYW Sygnatura – sprawa cywilna SGOS Sygnatura–sprawa gospodarcza SKAR Sygnatura – sprawa karna SPPR Sygnatura–sprawa prawo pracy SROD Sygnatura-sprawa rodzinna SUBE Sygnatura–sprawa ubezpieczenia
			  ' ''001'' as "Iloœæ tomów" ,   cast( '''' as varchar(50)) as  Opis, dznal.pos as pozycja ' +
			   ' FROM '	 +
			     @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal AS dznal ' +
			     '    INNER JOIN   ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_sprawa AS spr  on dznal.id_sprawy = spr.id ' +
				'	cross apply ( select top 1 id, id_skazany, flag_wiezien, id_adr   from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_sprawa_skazany knspskaz where knspskaz.id_sprawy = spr.id order by knspskaz.id desc )  knss  ' +
				'	 INNER JOIN 	 ' +  @sourcesrv +'.'  + @dbname +'.dbo.skazani AS s  ON s.id = knss.id_skazany LEFT OUTER JOIN  ' +
                '    '  +   @sourcesrv +'.'  + @dbname +'.dbo.skaz_adres sa on knss.id_adr = sa.id  inner join ' +  @sourcesrv +'.'  + @dbname +'.dbo.ksiegi_sady kss on spr.ksiega  = kss.id inner join ' +
                '    '  +   @sourcesrv +'.'  + @dbname +'.dbo.ksiegi_nazwy  ksn on kss.id_nazwy = ksn.id inner join '  +   @sourcesrv +'.'  + @dbname +'.dbo.kns_nal kn on spr.id  = kn.id_sprawy  ' +
        ' where isnull(spr.czyus,0) = 0 and spr.ksiega > 0 and  ' +
	    '  ( isnull(dznal.grzywna_areszt , 0) > 0   ) and  dznal.data_r  < ' + @nextdaystring + ' and dznal.data_r >= ' + @dataodstring  +
		' order by spr.id ' 


  		  	



EXEC (@query1 )
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Ugo]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Ugo]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataOd DateTime,
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dataOdString varchar(12),
  		  @dataDoString varchar(30),
  		  @nextdayString varchar(12),
  	      @shortDzienString  varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @dataDoString =  '''' + convert ( varchar(20),@dataDo,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @dataOdString =  '''' + convert ( varchar(10),@dataOd,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dataDo,120)  +''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  
  		  	
set @query1 = 'select  '+
	         '  dluos.fizpraw as "Osoba fizyczna/Osoba prawna" , ' +
			 '  dluos.imie as "Imiê/Nazwa 1", ' +
			 '	dluos.nazwisko as  "Nazwisko / Nazwa 2", ' +
			'	cast(ltrim(rtrim(dluos.Pesel)) as varchar(11))  as Pesel, '+ 
			'	case when len(rtrim(isnull(dloa.ulica,''''))) > 0 then rtrim(isnull(dloa.ulica,'''')) else isnull(dloa.miejscowosc,'''') end   as  "Ulica" , 	    isnull(dloa.nr_domu,'''') as "Nr domu" ,	    isnull(dloa.nr_lokalu,'''') as  "Nr mieszkania",	    isnull(dloa.kod_pocztowy,'''') AS "Kod pocztowy", ' +
	        '  case when len(rtrim(isnull(dloa.ulica,''''))) > 0 then isnull(dloa.miejscowosc,'''') else  isnull(dloa.poczta,'''') end as "Miejscowoœæ" , '+ 
	        '    case when kr.nazwa like ''%pols%'' or isnull(rtrim(kr.nazwa),'''') = '''' or rtrim(kr.nazwa)=''rp'' then   ''PL'' else kr.nazwa end   as "Klucz kraju", '+
	        ' '''' as IBAN, '+
			' case when  dluos.kod_typu_osoby = 1 and  (kr.nazwa like ''%pols%'' or isnull(rtrim(kr.nazwa),'''') = '''' or rtrim(kr.nazwa)=''rp'')   then ''09'' 		else  case when datalength(rtrim(isnull(sltp.rodzaj_podmiotu,''''))) > 0  '+
			' then 	rtrim(isnull(sltp.rodzaj_podmiotu,''''))   else   '''' 	  end      end  as "Kwalifikator do RBN", '+
	        ' ''KN'' as "Typ konta umowy", ' +
	        ' ''Kd '' + rtrim(slop.rodzaj_sprawy) + '' '' + cast(dlu.numer as varchar(6)) + ''/'' + substring(cast(dlu.rok  as varchar(4)),3,2) + case when len(rtrim(admw.wydzial))> 0  then ''/'' + admw.wydzial  else '''' end as "Oznaczenie konta umowy" , '+
 		    ' ''99'' as "Relacja konta", ' +
		    '	dlss.id_sad_obcy as IdSaduOrzek, ' +
			' rtrim(dlss.syg_wydzial) + '' '' + rtrim(dlss.syg_symbol) + '' '' + cast (rtrim(dlss.syg_nr_kolejny)  as varchar(7)) + ''/'' + substring(cast (dlss.syg_rok + 1000 as varchar(4)),3,2) as Sygnatura, '+
		    ' ''    '' as "Jednostka gospodarcza", '+
			' rtrim(dlss.syg_wydzial) as "Nr wydzia³u i sekcji", '+
			' rtrim(dlss.syg_symbol) as Repertorium, ' +
			' cast (rtrim(dlss.syg_nr_kolejny)  as varchar(7)) as "Nr sprawy", '+
			'  case   when dlss.syg_rok < 50 then   dlss.syg_rok + 2000 	when dlss.syg_rok > 50 and dlss.syg_rok < 100  then    dlss.syg_rok + 1900		   	end     as "Rok", '+
			' ''     '' as "Rodzaj sprawy" , '  +
			'  isnull(nals.data_operacji,nals.data_wprow_zapisu)  as   "Data dokumentu koszty", '+
			'  isnull(nals.data_operacji,nals.data_wprow_zapisu)  as   "Data dokumentu grzywna", ' +
			' isnull(nals.data_operacji,nals.data_wprow_zapisu)   as    "Data ksiêgowania", '+
			' ''NS'' as "Rodzaj dokumentu", '+
			' ''PLN'' as "Waluta", '+
			' ''            '' as "Klucz uzgodnienia", '+
			' ''    '' as  "Jednostaka gospodarca w³asna", '+
			' case when ltrim(rtrim(slop.rodzaj_sprawy)) = ''G'' ' +
			' then ''s'' else '''' end as Czysamoistna, 	''N010''   as  "Operacja g³ówna", '+
			' ''    ''   as  "Czêœciowo grzywna", ' +
			' ''    ''   as  "Czêœciowo koszty", '+
			' nals.grzywna as grzywna,  '+
			' nals.koszty as koszty, '+
			' isnull(isnull(dlstspr.data_1_raty,dlss.data_upraw),dlss.data_uprawomocnienia_g) as "Data wymagalnoœci" , ' +
			'   ''I''      as "Kara zastêpcza" ,  '

	set @query2 = '   nals.id_dluznik as Sprawa_id,  ' + 
			'	dlu.id_r_sprawy as Ksiega, ' + 
			' left( isnull(slas.nazwa,'''') + '' '' + isnull(slas.miejscowosc,'''') + '' '' + isnull(slas.nazwa1,'''')  ,100) as SadKns, ' + 
			'  rtrim(ltrim(   replace(replace(isnull(dd.nr_identyf_podatkowe,''''),''-'',''''),'' '' ,'''')  ) ) as NIP, '+
			'  ''    '' as "Rodzaj przedmiotu umowy" , ' + 
			'  ''001'' as "Iloœæ tomów" , ' +  
			'  cast( '''' as varchar(50)) as  Opis, nals.pozycja as pozycja  ' + 
			'	from (select id_dluznik,  data_operacji, data_wprow_zapisu , ' +
			'	     nal.uiszcz_grzywny_odpis    as grzywna, ' +
			'	     0  	as koszty, ' + 
            '	     nal.nr_poz  	as pozycja ' + 
            '   from ' +  @sourcesrv +'.'  + @dbname +'.dbo.naleznosci_dziennik nal where ( nal.uiszcz_grzywny_odpis > 0 )  and  isnull(nal.data_operacji,nal.data_wprow_zapisu) >= ' + @dataOdString   + ' and   isnull(nal.data_operacji,nal.data_wprow_zapisu) < ' + @nextdayString  + ' and isnull ( nal.data_usun_zapisu,''2099-12-31'') > GetDate() ) nals '+ 
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.Dluznik dlu  ON  dlu.id_dluznik=  nals.id_dluznik ' + 
			' cross apply ( select  top 1 ' +
			' dlos.id_dluznik as id_dluznik, ' + 
			'   dlos.id_osoba  as id_osoba,  ' + 
			'	dlos.id_typ_uczestnictwa as id_typ_uczestnictwa, ' + 
			'	dlos.kod_typu_osoby as kod_typu_osoby  , ' +  
			'	case  (dlos.kod_typu_osoby)  when 1   then  '' ''     else ''X''   end   as  fizpraw, ' +
	       	'    case  (dlos.kod_typu_osoby)    when 1   then left(dlos.imie,40)    else  substring(dlos.nazwisko_nazwa,1,40)    end   as  imie, ' +
	    	'    case  (dlos.kod_typu_osoby)    when 1   then left(dlos.nazwisko,40)    else  substring(dlos.nazwisko_nazwa,41,40)     end   as  nazwisko, ' +
			'   cast(rtrim(ltrim(isnull(dlos.pesel,''''))) as varchar(11)) as Pesel ' + 
			'	from  ' + +  @sourcesrv +'.'  + @dbname + '.dbo.DLUZNIK_OSOBY dlos ' +
			'	where dlos.id_dluznik  = dlu.id_dluznik  order by dlos.id_osoba asc)    dluos ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_UCZESTNICTWA sltu ON sltu.kod=dluos.id_typ_uczestnictwa ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_OSOB slto ON slto.kod=dluos.kod_typu_osoby ' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_OSOBY_ADRESY dloa ON dloa.id_osoba=  dluos.id_osoba and dloa.czy_adres_glowny=1 ' +
			' Left join ' +  @sourcesrv +'.'  + @dbname +'.dbo.Dluznik_osoby_dane_dod dd ON dd.id_osoba = dluos.id_osoba ' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_KRAJE kr on kr.kod = dloa.kod_kraju ' +
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_RODZAJOW_SPRAW slop ON dlu.id_r_sprawy =  slop.id_r_sprawy ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_RODZAJOW_SPRAW stow ON slop.typ_sp=  stow.typ_sp ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_DZIENNIKOW sdzn ON stow.id_dziennika=  sdzn.id_dziennika ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.adm_wydzialy admw ON  slop.k_wydzial=admw.kod ' + 
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_PODMIOTY sltp ON sltp.kod=dd.id_podmiot' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_SPRAWA_SADOWA dlss ON dlss.id_dluznik  = dlu.id_dluznik ' + 
			' LEFT OUTER Join ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_ADR_SADOW slas ON dlss.id_sad_obcy = slas.kod ' +
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_STAN_SPRAWY dlstspr on dlstspr.id_dluznik = dlu.id_dluznik ' +
			'	order by dlu.id_dluznik '
			-- print @query1 + ' ' + @query2
EXEC (@query1 + ' ' + @query2)
end
GO
/****** Object:  StoredProcedure [dbo].[sp_TestOR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_TestOR]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50)
	 
	 
AS
BEGIN
Declare 
 @query as varchar(Max)
 
 set @dbname = 'OrComNS'
 set @sourcesrv  = '"' + @sourcesrv + '"' 
   
set @query =  ' select top 1 zap.ZapisId  from ' + @sourcesrv + '.' + @dbname + '.dbo.Zapis zap ' 

print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_TestCR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TestCR]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50)
	 
	 
AS
BEGIN
Declare 

 @query as varchar(Max)
 
 set @sourcesrv  = '"' + @sourcesrv + '"' 
   
set @query =  ' select top 1 nal.id  from ' + @sourcesrv + '.' + @dbname + '.dbo.kns_dz_nal nal ' 

print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Test]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  PROCEDURE [dbo].[sp_Test]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50)
	 
	 
AS
BEGIN
Declare 

 @query as varchar(Max)
 
 set @sourcesrv  = '"' + @sourcesrv + '"' 
   
set @query =  ' select top 1 nal.grzywna_przypis  from ' + @sourcesrv + '.' + @dbname + '.dbo.naleznosci_dziennik nal ' 

print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_DziennikNaleznosciOR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_DziennikNaleznosciOR]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dzien DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @SAPjednostka varchar(4),
		   
		  --@query text,
  		  @dzienString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortdzienString varchar(12)
   
		IF  CHARINDEX ( '@@' , @sourcesrv ) > 0  
		BEGIN
		 set @SAPjednostka = Substring(@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) +2,4)
		 set @sourcesrv = left (@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) -1 )
		END
  		  
  		  set @dbname = 'OrComNS'
  		  
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  + ''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  set @shortDzienString = '''' + convert ( varchar(10),@dzien,120)  + ''''
  		  


set @query1 = 
		   
		   

		   ' SELECT     case when s.PodmiotTypId = 1 then '' '' else ''X'' end as "Osoba fizyczna/Osoba prawna", ' +
		   '  case when s.PodmiotTypId = 1 then s.imie else left(s.nazwisko,40) end as "Imiê/Nazwa 1", ' + 	 
		   '  case when s.PodmiotTypId = 1 then left(s.nazwisko,40) else substring(s.nazwisko,41,40) end as	"Nazwisko / Nazwa 2",  ' +
		   '  case when len(isnull(sa.miejsce,'''')) > 0  then left(sa.miejsce,60) else left(sa.miejscowosc,60) end as	 "Ulica",  ' +
		   '  sa.dom   as "Nr domu", ' +
		   '  sa.lokal   as "Nr mieszkania", ' +
		   '  sa.kod as "Kod pocztowy",  ' +
		   '  case when len(sa.miejscowosc) > 0  then sa.miejscowosc  else sa.miejsce  end as "Miejscowoœæ",  ' +
		   '  case when sa.kraj like ''%pols%'' or isnull(rtrim(sa.Kraj),'''') = '''' or rtrim(sa.Kraj)=''rp'' then  ' +
			'		  ''PL'' else sa.Kraj end   as "Klucz kraju",  ' +
			' '''' as IBAN,  ' +
			' case when isnull(spr.KlasyfikacjaRbnId,0) = 1 then ''09'' else ''08'' end    as "Kwalifikator do RBN" ' +
			' , ''KN'' as "Typ konta umowy",  ' +
			' spr.numer "Oznaczenie konta umowy", ' +
			' ''99'' as "Relacja konta", ' +
			' spr.OrzeczenieSadAdresatId as   IdSaduOrzek, ' +
			' spr.sygnatura as Sygnatura,  ' +
			' ''    '' as "Jednostka gospodarcza",  ' +
			' spr.SygnWydzial as "Nr wydzia³u i sekcji", ' +
			' spr.SygnRepetytorium as Repertorium,  ' +
			' spr.SygnNumer as "Nr sprawy",  ' +
			' spr.SygnRok as "Rok",   ' +
			' ''     '' as "Rodzaj sprawy", ' +
			' (select top 1 data from  ' +	@sourcesrv + '.' +   @dbname +'.dbo.zapis where zapis.SprawaId = spr.SprawaId and  data < ' +  @nextdaystring + '  and zapis.przypis > 0 and zapis.NaleznoscTypId = 1 order by  data desc ) as  "Data dokumentu koszty",   ' +
			' (select top 1 data from  ' +	@sourcesrv + '.' +   @dbname +'.dbo.zapis where zapis.SprawaId = spr.SprawaId and  data < ' +  @nextdaystring + '  and zapis.przypis > 0 and zapis.NaleznoscTypId = 0 order by  data desc ) as  "Data dokumentu grzywna", ' +  
			  @shortDzienString +  ' as "Data ksiêgowania", ' + 
			--''31-12-2014'' as "Data ksiêgowania",  
			' ''NS'' as "Rodzaj dokumentu",  ' +
			' ''PLN'' as "Waluta",  ' + 
			' ''            '' as "Klucz uzgodnienia", ' +
			' ''    '' as  "Jednostaka gospodarca w³asna" ' +
			' , case when snr.naleznoscRodzajId = 4 then ''s''  ' +
			'	when spr.SygnRepetytorium = ''W'' and spr.PrzypisG > 0 then ''s''  ' +
			'	end as Czysamoistna, ' +
			' ''N010''   as  "Operacja g³ówna", ' +
			' ''    ''   as  "Czêœciowo grzywna", ' + 
			'  ''    ''   as  "Czêœciowo koszty", ' + 
			' (select isnull(sum(zapis.przypis ), 0) - isnull(sum(zapis.uiszczenie), 0) - isnull(sum(zapis.odpis), 0) from  ' +  @sourcesrv +'.'+    @dbname +'.dbo.zapis where NaleznoscTypId = 0 and zapis.SprawaId = spr.SprawaId and data <  ' +  @nextdaystring  + ' ) as grzywna, ' +
			' (select isnull(sum(zapis.przypis ), 0) - isnull(sum(zapis.uiszczenie), 0) - isnull(sum(zapis.odpis), 0) from  ' +  @sourcesrv +'.'+    @dbname +'.dbo.zapis where NaleznoscTypId = 1 and zapis.SprawaId = spr.SprawaId and data <  ' +  @nextdaystring  + ' ) as koszty,  ' +
 			
 		 ' case    when isnull(spr.dataDoreczenia,cast(''1900-01-01'' as datetime)) > cast(''2000-01-01'' as datetime)  	then  ' +
		 ' case    when spr.SygnRepetytorium IN (''W'',''K'') then dateadd (dd,30, spr.dataDoreczenia) 	 else    dateadd (dd,14, spr.dataDoreczenia) ' +	  
		 ' end    else     spr.DataPrawomocnosci  end   as "Data wymagalnoœci", ' + 

		 ' ''''  as "Raty koszty", ' +
		 ' '''' as  "Raty grzywna", ' +
		 ' ''''  as "Egzekucja grzywny", ' +
		 ' ''''  as "Egzekucja koszty",  ' +
		 ' ''''  as "Grzywny odroczone" , ' + 
		 ' ''''  as "Koszty odroczone" , ' + 
		 ' ''''  as "Kara zastêpcza" , ' + 
		 '  spr.SprawaId as Sprawa_id, ' + 
			'    spr.JednostkaWydzialId as Ksiega,  ' +
			 ' isnull(j.nazwa,'''')   as SadKns,  ' +
			  '  cast(rtrim(ltrim(isnull(s.pesel,''''))) as varchar(11)) as Pesel ' +
			  ' , s.nip as NIP, ' +
			  ' ''    '' as "Rodzaj przedmiotu umowy", ' +
			   --SCYW Sygnatura – sprawa cywilna SGOS Sygnatura–sprawa gospodarcza SKAR Sygnatura – sprawa karna SPPR Sygnatura–sprawa prawo pracy SROD Sygnatura-sprawa rodzinna SUBE Sygnatura–sprawa ubezpieczenia 
			  ' ''001'' as "Iloœæ tomów" ,   cast( '''' as varchar(50)) as  Opis  ' +
			   ---select *
				' FROM ' +	@sourcesrv + '.' +   @dbname +'.dbo.sprawa AS spr ' +  
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.JednostkaWydzial jw on jw.JednostkaWydzialId = spr.JednostkaWydzialId ' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.Jednostka j on j.JednostkaId = jw.JednostkaId ' +
			    ' left join ' + @sourcesrv + '.' + @dbname + '.dbo.sprawaNaleznoscRodzaj snr on snr.sprawaId = spr.SprawaId and snr.naleznoscRodzajId = 4  ' +
				' cross apply ( select top 1 SprawaDluznikId, DluznikId   from ' + @sourcesrv + '.' + @dbname + '.dbo. SprawaDluznik knspskaz where knspskaz.SprawaId = spr.SprawaId order by knspskaz.SprawaDluznikId desc )  knss  ' + 
				' INNER JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.Dluznik s  ON s.DluznikId = knss.DluznikId ' +
				' LEFT OUTER JOIN  ' + @sourcesrv + '.' + @dbname + '.dbo.DluznikAdres dsa on dsa.DluznikId = s.DluznikId and dsa.domyslny = 1 ' +
				' left join ' + @sourcesrv + '.' + @dbname + '.dbo.adres sa on sa.AdresId = dsa.AdresId ' +
				 ' where spr.JednostkaWydzialId > 0   ' +
				 ' and j.SAP_JednGospId = ' + @SAPjednostka +'   ' +
	    ' and ((select isnull(sum(zapis.przypis), 0) - isnull(sum(zapis.uiszczenie), 0) - isnull(sum(zapis.odpis), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.zapis where NaleznoscTypId = 0 and zapis.SprawaId = spr.SprawaId and data < ' + @nextdaystring + ' ) > 0  or  ' +
		'	  (select isnull(sum (zapis.przypis), 0) - isnull(sum (zapis.uiszczenie), 0) - isnull(sum(zapis.odpis), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.zapis where NaleznoscTypId = 1 and zapis.SprawaId = spr.SprawaId and data  < ' + @nextdaystring + ' ) > 0  )  ' +
		' order by spr.SprawaId ' 



  		  	


print  @query1

EXEC (@query1 )
end

--

--exec sp_DziennikNaleznosci 'Fin1', 'kns_orcom', '2014-12-31'
GO
/****** Object:  StoredProcedure [dbo].[sp_DziennikNaleznosciCR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DziennikNaleznosciCR]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dzien DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   
		   
		  --@query text,
  		  @dzienString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortdzienString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  set @shortDzienString = '''' + convert ( varchar(10),@dzien,120)  +''''
  		  


set @query1 = 
		   ' SELECT     case s.osoba_lub_firma when 0 then '' '' else ''X'' end as "Osoba fizyczna/Osoba prawna", '+
		   '  case s.osoba_lub_firma when 0  then s.imie else left(s.nazwisko,40) end as "Imiê/Nazwa 1",  '	 +
		    ' case s.osoba_lub_firma when 0  then left(s.nazwisko,40) else substring(s.nazwisko,41,40) end as	"Nazwisko / Nazwa 2", ' +
		    ' sa.ul  as	 "Ulica", ' +
		    ' cast( case when CHARINDEX(''/'',sa.nrdom) > 0 THEN left (sa.nrdom,CHARINDEX(''/'',sa.nrdom) -1)  else  sa.nrdom end as varchar(10) )   as "Nr domu", '+
		    ' case when CHARINDEX(''/'',sa.nrdom) > 0 THEN substring (sa.nrdom,CHARINDEX(''/'',sa.nrdom) + 1,20) else  '''' end   as "Nr mieszkania", '+
		    ' case when len(rtrim(sa.kod)) > 2   then left(sa.kod,2) + ''-'' + substring(sa.kod,3,4) else '''' end as "Kod pocztowy", ' +
		    ' case when len(sa.miejscowosc) > 0  then sa.miejscowosc  else sa.miejsce  end as "Miejscowoœæ", '+
		    ' case when sa.panstwo like ''%pols%'' or isnull(rtrim(sa.panstwo),'''') = '''' or rtrim(sa.panstwo)=''rp'' then '+
			'		  ''PL'' else sa.panstwo end   as "Klucz kraju", ' +
			' '''' as IBAN, ' +
			' case when isnull(s.rbn,0) = 0 then case when s.osoba_lub_firma = 0  then  ''09''  else ''08'' end  ' +
			' else case when s.rbn > 9  then cast (s.rbn as varchar(2 )) else  ''0'' + cast (s.rbn as varchar(1)) end      	end as "Kwalifikator do RBN", ' +
			' ''KN'' as "Typ konta umowy", ' +
			' spr.nr_karty_dl "Oznaczenie konta umowy", '+
			' ''99'' as "Relacja konta", '+
			' spr.id_sad as   IdSaduOrzek, '+
			' spr.sygnatura as Sygnatura, ' +
			' ''    '' as "Jednostka gospodarcza", '+ 
			' ''          '' as "Nr wydzia³u i sekcji", '+
			' ''      '' as Repertorium, ' +
			' ''      '' as "Nr sprawy", ' +
			' 0 as "Rok", ' + 
			' ''     '' as "Rodzaj sprawy", '+
			' (select top 1 data_r from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and  data_r < ' + @nextdaystring + ' and kns_dz_nal.przypis_kosztow > 0  order by  data_r desc ) as  "Data dokumentu koszty", ' + 
			' (select top 1 data_r from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and  data_r < ' + @nextdaystring + ' and kns_dz_nal.przypis_grzywny > 0  order by  data_r desc ) as  "Data dokumentu grzywna", ' + 
			  @shortDzienString +  ' as "Data ksiêgowania", ' +
			' ''NS'' as "Rodzaj dokumentu", ' +
			' ''PLN'' as "Waluta", ' + 
			' ''            '' as "Klucz uzgodnienia", '+
			' ''    '' as  "Jednostaka gospodarca w³asna", ' +
			' spr.grzywna_sam as Czysamoistna, '+
			' ''N010''   as  "Operacja g³ówna", '+
			' ''    ''   as  "Czêœciowo grzywna", ' +
			'  ''    ''   as  "Czêœciowo koszty", ' +
			' (select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r < ' +   @nextdaystring + ' ) as grzywna, '+
			' (select isnull(sum (kns_dz_nal.przypis_kosztow), 0) - isnull(sum (kns_dz_nal.uiszczenia_kostow), 0) - isnull(sum(kns_dz_nal.odpisanie_kosztow), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r  < ' + @nextdaystring + ' ) as koszty, ' +
 			' case    when isnull(kn.data_dorecz,cast(''1900-01-01'' as datetime)) > cast(''2000-01-01'' as datetime)  	then  '+
			'	case when ksn.typ_ks  =  2  or (select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id ) > 0  then  '+
			'	   dateadd (dd,30, kn.data_dorecz) 	 else    dateadd (dd,14, kn.data_dorecz) 	  end    else     spr.data_u  end   as "Data wymagalnoœci", ' +
			' case when exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_nal_okres  	where ' +
			'  kns_nal_okres.id_sprawy = spr.id and ' +
			' isnull(kns_nal_okres.koniec,''2099-01-01'') > '+ @dzienstring + '  and  ' +
			' isnull(kns_nal_okres.poczatek,''1900-01-01'') < ' + @nextdaystring + '  and kns_nal_okres.koszty > 0 ) ' +
			'	and not exists (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id '+
			 '   and knsegz.data_pocz < ' +  @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @dzienstring +
			 ' 	)  	then  ''B''   else 	 ''''  end  as "Raty koszty", '+
			 ' case when exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_nal_okres 	where  	kns_nal_okres.id_sprawy = spr.id and '+
			 '					isnull(kns_nal_okres.koniec,''2099-01-01'') > ' + @dzienstring  + '  and  ' +
			 '					isnull(kns_nal_okres.poczatek,''1900-01-01'') <  ' + @nextdaystring + ' and kns_nal_okres.grzywna > 0 ) ' +
			 '	 				and not exists (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id ' + 
			 '					and knsegz.data_pocz < ' + @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @dzienstring  +
			 '	  			)  	then  	 ''B'' 	 else 	 ''''  	 end  as "Raty grzywna", '+
			 '  case when exists ( select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id '+
			 '					and knsegz.data_pocz < ' + @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @dzienstring +
			 '					and kwota > 0 	)  	 then 	 ''C'' 	 else  ''''   end  as "Egzekucja grzywny", '+
			 '	 case when exists ( select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_egzekucja knsegz where knsegz.id_sprawy =  spr.id '  +
			 '					and knsegz.data_pocz < '+  @nextdaystring + ' and isnull(knsegz.data_kon,''2099-01-01'') > ' + @dzienstring  +
			 '					and koszty > 0 	) 		 then 		 ''C'' 	 else 	 ''''  	 end  as "Egzekucja koszty", ' +
			 '			 case when	exists ( select null  from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.okres  ' +
			 '					 where okres.id_sprawy = spr.id  and okres.typ_s  = 1 and okres.kwota > 0 and   ' +
			 '					 okres.poczatek < ' + @nextdaystring + ' and okres.koniec > ' + @dzienstring + ' )  ' +
			 '		 then  	 ''D'' 	 else 	 '''' end  as "Grzywny odroczone" , ' +
			 '			 case when	exists ( select null  from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.okres  ' +
			 '					 where okres.id_sprawy = spr.id  and okres.typ_s  = 0 and okres.kwota > 0 and   '  +
			'					 okres.poczatek < ' +  @nextdaystring + ' and okres.koniec> ' +  @dzienstring + ' ) ' +  	
			 ' then  ''D'' 	 else  	 ''''  	 end 	 as "Koszty odroczone" , ' +
			 ' case when exists (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_kara where kns_kara.id_sprawy = spr.id and isnull(kns_kara.data_post,kns_kara.data_post_wo) < ' + @nextdaystring + ' ) ' + 
			 '  then      ''I''   else 	 ''''  	 end       as "Kara zastêpcza" , ' +
			 '  spr.id as Sprawa_id, ' +
			 '   spr.ksiega as Ksiega, ' +
			 '   left((select isnull(skor.nazwa,'''') + '' '' + isnull(skor.miejsce,'''') + '' '' + isnull(skor.nazwa2,'''') from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.skor where spr.id_sad = skor.id) ,100) as SadKns, ' +
			 '  cast(rtrim(ltrim(isnull(s.pesel,''''))) as varchar(11)) as Pesel, '+
			 '  cast(rtrim(ltrim(   replace(replace(isnull(s.nip,''''),''-'',''''),'' '' ,'''')  ) ) as varchar(10)) as NIP, '+
			 '  ''    '' as "Rodzaj przedmiotu umowy", ' + --SCYW Sygnatura – sprawa cywilna SGOS Sygnatura–sprawa gospodarcza SKAR Sygnatura – sprawa karna SPPR Sygnatura–sprawa prawo pracy SROD Sygnatura-sprawa rodzinna SUBE Sygnatura–sprawa ubezpieczenia
			  ' ''001'' as "Iloœæ tomów" ,   cast( '''' as varchar(50)) as  Opis ' +
			   ' FROM '	 +			  
			    '    ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_sprawa AS spr  ' +
				'	cross apply ( select top 1 id, id_skazany, flag_wiezien, id_adr   from  ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_sprawa_skazany knspskaz where knspskaz.id_sprawy = spr.id order by knspskaz.id desc )  knss  ' +
				'	 INNER JOIN 	 ' +  @sourcesrv +'.'  + @dbname +'.dbo.skazani AS s  ON s.id = knss.id_skazany LEFT OUTER JOIN  ' +
                '    '  +   @sourcesrv +'.'  + @dbname +'.dbo.skaz_adres sa on knss.id_adr = sa.id  inner join ' +  @sourcesrv +'.'  + @dbname +'.dbo.ksiegi_sady kss on spr.ksiega  = kss.id inner join ' +
                '    '  +   @sourcesrv +'.'  + @dbname +'.dbo.ksiegi_nazwy  ksn on kss.id_nazwy = ksn.id inner join '  +   @sourcesrv +'.'  + @dbname +'.dbo.kns_nal kn on spr.id  = kn.id_sprawy  ' +
        ' where isnull(spr.czyus,0) = 0 and spr.ksiega > 0 and  ( isnull(spr.stan,'''') <> ''z''  or  ( isnull(spr.stan,'''') = ''z'' and spr.data_kon >= '+ @nextdaystring +  ' ) ) and  ' +
	    '  ((select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r < ' + @nextdaystring + ' ) > 0  or  ' +
		'	  (select isnull(sum (kns_dz_nal.przypis_kosztow), 0) - isnull(sum (kns_dz_nal.uiszczenia_kostow), 0) - isnull(sum(kns_dz_nal.odpisanie_kosztow), 0) from ' +  @sourcesrv +'.'  + @dbname +'.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r  < ' + @nextdaystring + ' ) > 0  )  ' +
		' order by spr.id ' 


  		  	



EXEC (@query1 )
end
GO
/****** Object:  StoredProcedure [dbo].[sp_DziennikNaleznosci]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DziennikNaleznosci]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dzien DateTime
	 
AS
BEGIN
  DECLARE 
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		  --@query text,
  		  @dzienString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortdzienString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		  set @shortDzienString = '''' + convert ( varchar(10),@dzien,120)  +''''
  		  
  		  	
set @query1 = 'select  '+
	         '  dluos.fizpraw as "Osoba fizyczna/Osoba prawna" , ' +
			 '  dluos.imie as "Imiê/Nazwa 1", ' +
			 '	dluos.nazwisko as  "Nazwisko / Nazwa 2", ' +
			 '	cast(ltrim(rtrim(dluos.Pesel)) as varchar(11))  as Pesel, '+ 
			'	case when len(rtrim(isnull(dloa.ulica,''''))) > 0 then rtrim(isnull(dloa.ulica,'''')) else isnull(dloa.miejscowosc,'''') end   as  "Ulica" , 	    isnull(dloa.nr_domu,'''') as "Nr domu" ,	    isnull(dloa.nr_lokalu,'''') as  "Nr mieszkania",	    isnull(dloa.kod_pocztowy,'''') AS "Kod pocztowy", ' +
	        '   case when len(rtrim(isnull(dloa.ulica,''''))) > 0 then isnull(dloa.miejscowosc,'''') else  isnull(dloa.poczta,'''') end as "Miejscowoœæ", '+ 
	        '    case when kr.nazwa like ''%pols%'' or isnull(rtrim(kr.nazwa),'''') = '''' or rtrim(kr.nazwa)=''rp'' then   ''PL'' else kr.nazwa end   as "Klucz kraju", '+
	        ' '''' as IBAN, '+
			' case when  dluos.kod_typu_osoby = 1 and  (kr.nazwa like ''%pols%'' or isnull(rtrim(kr.nazwa),'''') = '''' or rtrim(kr.nazwa)=''rp'')   then ''09'' 		else  case when datalength(rtrim(isnull(sltp.rodzaj_podmiotu,''''))) > 0  '+
			' then 	rtrim(isnull(sltp.rodzaj_podmiotu,''''))   else   ''??'' 	  end      end  as "Kwalifikator do RBN", '+
	        ' ''KN'' as "Typ konta umowy", ' +
	        ' ''Kd '' + rtrim(slop.rodzaj_sprawy) + '' '' + cast(dlu.numer as varchar(6)) + ''/'' + substring(cast(dlu.rok  as varchar(4)),3,2) +  case when len(rtrim(admw.wydzial))> 0  then ''/'' + admw.wydzial  else '''' end as "Oznaczenie konta umowy" , '+
 		    ' ''99'' as "Relacja konta", ' +
		    '	dlss.id_sad_obcy as IdSaduOrzek, ' +
			' rtrim(isnull(dlss.syg_wydzial,'''')) + '' '' + rtrim(isnull(dlss.syg_symbol,'''')) + '' '' + cast (rtrim(dlss.syg_nr_kolejny)  as varchar(7)) + ''/'' + substring(cast (dlss.syg_rok + 1000 as varchar(4)),3,2) as Sygnatura, '+
		    ' ''    '' as "Jednostka gospodarcza", '+
			' rtrim(dlss.syg_wydzial) as "Nr wydzia³u i sekcji", '+
			' rtrim(dlss.syg_symbol) as Repertorium, ' +
			' cast (rtrim(dlss.syg_nr_kolejny)  as varchar(7)) as "Nr sprawy", '+
			'  case   when dlss.syg_rok < 50 then   dlss.syg_rok + 2000 	when dlss.syg_rok > 50 and dlss.syg_rok < 100  then    dlss.syg_rok + 1900		   	end     as "Rok", '+
			' ''     '' as "Rodzaj sprawy" , '  +
			' (select top 1 isnull(dnn.data_operacji,dnn.data_wprow_zapisu) from ' + @sourcesrv + '.' + @dbname + '.dbo.NALEZNOSCI_DZIENNIK dnn where dnn.id_dluznik = nals.id_dluznik and oplatakoszty_przypis > 0 and isnull(dnn.data_operacji,dnn.data_wprow_zapisu) < ' + @nextdayString +  ' and isnull ( dnn.data_usun_zapisu,''2099-12-31'') > GetDate()   order by 1 desc  ) as   "Data dokumentu koszty", '+
			' (select top 1 isnull(dnn.data_operacji,dnn.data_wprow_zapisu) from  '  + @sourcesrv  + '.' + @dbname + '.dbo.NALEZNOSCI_DZIENNIK dnn where dnn.id_dluznik = nals.id_dluznik and grzywna_przypis > 0 and isnull(dnn.data_operacji,dnn.data_wprow_zapisu) < ' + @nextdayString + '  and isnull ( dnn.data_usun_zapisu,''2099-12-31'') > GetDate()  order by 1 desc  ) as   "Data dokumentu grzywna", ' +
			' cast ( ' + @shortDzienString +  '  as datetime) as "Data ksiêgowania", '+
			' ''NS'' as "Rodzaj dokumentu", '+
			' ''PLN'' as "Waluta", '+
			' ''            '' as "Klucz uzgodnienia", '+
			' ''    '' as  "Jednostaka gospodarca w³asna", '+
			' case when ltrim(rtrim(slop.rodzaj_sprawy)) = ''G'' ' +
			' then ''s'' else '''' end as Czysamoistna, 	''N010''   as  "Operacja g³ówna", '+
			' ''    ''   as  "Czêœciowo grzywna", ' +
			' ''    ''   as  "Czêœciowo koszty", '+
			' nals.grzywna as grzywna,  '+
			' nals.koszty as koszty, '+
			' isnull(isnull(dlstspr.data_1_raty,dlss.data_upraw),dlss.data_uprawomocnienia_g) as "Data wymagalnoœci" , ' +
			'  case when exists ( select null  from ' + @sourcesrv +'.' +@dbname + '.dbo.raty r  ' +
			'				where   r.id_dluznik = dlu.id_dluznik and      isnull ( r.data_usun_zapisu,''2099-12-31'') > GetDate()  and ' +
			'							isnull(r.data_wyst_post, r.platna_od ) < ' +  @nextdayString + ' and '+
			'						   isnull(r.data_odwolania,''2099-01-01'') > '+ @dzienString + ' and  r.koszty_pr > 0 ) '+
			'			and not exists ( select null  from '  + @sourcesrv + '.' + @dbname + '.dbo.egzekucje e ' +
			'			                 where  e.id_dluznik = dlu.id_dluznik and ' +   
			'						            isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and  ' +
			'						            e.data_egzekucji < ' + @nextdayString + ' and  '+ 
			'						            e.koszty_do_zaplaty > 0 )	 ' +
			'  then  ''B'' 	 else 	 '''' 	 end  as "Raty koszty", ' +
			'  case when exists ( select null  from ' + @sourcesrv +'.' + @dbname + '.dbo.raty r '+
			'					where   r.id_dluznik = dlu.id_dluznik and '+
			'					        isnull ( r.data_usun_zapisu,''2099-12-31'') > GetDate() and ' +
			'							isnull(r.data_wyst_post, r.platna_od ) < ' + @nextdayString +  '  and ' + 
			'						   isnull(r.data_odwolania,''2099-01-01'') >' +  @dzienString  + ' and  r.grzywna_pr > 0 ) ' +
			'			and not exists ( select null  from ' + @sourcesrv + '.' + @dbname + '.dbo.egzekucje e  ' +
			'			                 where  e.id_dluznik = dlu.id_dluznik and     ' + 
			'						            isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and '+
			'						            e.data_egzekucji <  ' + @nextdayString  + ' and  '+ 
			'						            e.do_zaplaty - e.koszty_do_zaplaty > 0 ) ' +
			'	then 	 ''B'' 	 else 	 ''''  	 end  as "Raty grzywna", ' +
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.egzekucje e join  ' +  @sourcesrv +'.'  + @dbname +'.dbo.dluznik_stan_sprawy dlussprawy on dlussprawy.id_dluznik =  e.id_dluznik  '+
			'			                    where  e.id_dluznik = dlu.id_dluznik and    ' +
			'						            isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and '+
			'						            e.data_egzekucji <  ' + @nextdayString  + ' and  ' +
			'						            e.koszty_do_zaplaty > 0 and ' +
			'						            isnull(dlussprawy.data_bez,''2099-12-31'') > ' + @dzienString  + ' ) ' + 
			' then 		 ''C''   else 	 ''''   end  as "Egzekucja grzywny", '+
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.egzekucje e join  ' +  @sourcesrv +'.'  + @dbname +'.dbo.dluznik_stan_sprawy dlussprawy on dlussprawy.id_dluznik =  e.id_dluznik  ' +
			'			                    where  e.id_dluznik = dlu.id_dluznik and   ' +  
			'									isnull ( e.data_usun_zapisu,''2099-12-31'') > GetDate() and ' +
			'						            e.data_egzekucji <  ' + @nextdayString  + ' and ' +
			'						             e.do_zaplaty - e.koszty_do_zaplaty > 0 and ' +
			'						            isnull(dlussprawy.data_bez,''2099-12-31'') > ' + @dzienString + ' ) ' +
			' then 	 ''C''  else  ''''  end  as "Egzekucja koszty" , ' +
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.odroczenia_postanowienia op  '  +
			'			                    where  op.id_dluznik = dlu.id_dluznik and ' +   
			'									isnull ( op.data_usun_zapisu,''2099-12-31'') > GetDate() and '+
			'						            op.dt_post_grzywny  <  ' + @nextdayString  + ' and ' + 
			'						            op.GRZYWNA > 0 and ' + 
			'						            op.do_kiedy_grzywna <  ' + @nextdayString  + ' and ' + 
			'						            isnull(op.dt_odwolania_g,''2099-12-31'') >' + @dzienString  + ' ) ' +
	 		' then  ''D''  else  '''' end  as "Grzywny odroczone" , ' + 
			'  case when  exists ( select null  from ' +  @sourcesrv +'.'  + @dbname +'.dbo.odroczenia_postanowienia op '+    
			'			                    where  op.id_dluznik = dlu.id_dluznik and ' +
			'			                       isnull ( op.data_usun_zapisu,''2099-12-31'') > GetDate() and  ' +   
			'						            op.dt_post_koszty  <  ' + @nextdayString  + ' and ' + 
			'						            op.koszty > 0 and ' + 
			'						            op.do_kiedy_koszty <  ' + @nextdayString  + ' and ' + 
			'						            isnull(op.dt_odwolania_k,''2099-12-31'') >' + @dzienString  + ' ) ' + 
	 		' then 	 ''D'' 	 else 	 '''' end  as "Koszty odroczone" , '+
			'  case when exists   (select null from  '+ @sourcesrv +'.'  + @dbname + + '.dbo.wog  wogg where wogg.id_dluznik = dlu.id_dluznik and ' +
			'											  isnull ( wogg.data_usun_zapisu,''2099-12-31'') > GetDate() and ' + 
			'											  wogg.data_postanowienia < ' + @nextdayString  + ' and ' +
			'											  isnull(wogg.data_odwolania,''2099-12-31'')> ' + @dzienString  + ' ) ' + 
			'						or exists  (select null from ' +  @sourcesrv +'.'  + @dbname +'.dbo.areszty_postanowienia arpo  ' +
			'									where arpo.id_dluznik = dlu.id_dluznik and ' +
			'					        	isnull ( arpo.data_usun_zapisu,''2099-12-31'') > GetDate() )'	+								  
			' then  ''I''   else  '''' 	 end     as "Kara zastêpcza" ,  '

	set @query2 = '   nals.id_dluznik as Sprawa_id,  ' + 
			'	dlu.id_r_sprawy as Ksiega, ' + 
			' left( isnull(slas.nazwa,'''') + '' '' + isnull(slas.miejscowosc,'''') + '' '' + isnull(slas.nazwa1,'''')  ,100) as SadKns, ' + 
			'  rtrim(ltrim(   replace(replace(isnull(dd.nr_identyf_podatkowe,''''),''-'',''''),'' '' ,'''')  ) ) as NIP, '+
			'  ''    '' as "Rodzaj przedmiotu umowy" , ' + 
			'  ''001'' as "Iloœæ tomów" , ' +  
			'  cast( '''' as varchar(50)) as  Opis ' + 
			'	from (select id_dluznik, ' +
			'	sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis )   as grzywna, ' +
			'	sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis )	as koszty ' + 
            '   from ' +  @sourcesrv +'.'  + @dbname +'.dbo.naleznosci_dziennik nal where  isnull(nal.data_operacji,nal.data_wprow_zapisu) < ' + @nextdayString  + ' and isnull ( nal.data_usun_zapisu,''2099-12-31'') >  GetDate() '   +
			'	group by id_dluznik ' +
			'	having sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis ) > 0 or sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis ) > 0  ) nals '+ 
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.Dluznik dlu  ON  dlu.id_dluznik=  nals.id_dluznik ' + 
			' cross apply ( select  top 1 ' +
			' dlos.id_dluznik as id_dluznik, ' + 
			'   dlos.id_osoba  as id_osoba,  ' + 
			'	dlos.id_typ_uczestnictwa as id_typ_uczestnictwa, ' + 
			'	dlos.kod_typu_osoby as kod_typu_osoby  , ' +  
			'	case  (dlos.kod_typu_osoby)  when 1   then  '' ''     else ''X''   end   as  fizpraw, ' +
	       	'    case  (dlos.kod_typu_osoby)    when 1   then left(dlos.imie,40)    else  substring(dlos.nazwisko_nazwa,1,40)    end   as  imie, ' +
	    	'    case  (dlos.kod_typu_osoby)    when 1   then left(dlos.nazwisko,40)    else  substring(dlos.nazwisko_nazwa,41,40)     end   as  nazwisko, ' +
			'   cast(rtrim(ltrim(isnull(dlos.pesel,''''))) as varchar(11)) as Pesel ' + 
			'	from  ' + +  @sourcesrv +'.'  + @dbname + '.dbo.DLUZNIK_OSOBY dlos ' +
			'	where dlos.id_dluznik  = dlu.id_dluznik  order by dlos.id_osoba asc)    dluos ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_UCZESTNICTWA sltu ON sltu.kod=dluos.id_typ_uczestnictwa ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_OSOB slto ON slto.kod=dluos.kod_typu_osoby ' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_OSOBY_ADRESY dloa ON dloa.id_osoba=  dluos.id_osoba and dloa.czy_adres_glowny=1 ' +
			' Left join ' +  @sourcesrv +'.'  + @dbname +'.dbo.Dluznik_osoby_dane_dod dd ON dd.id_osoba = dluos.id_osoba ' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_KRAJE kr on kr.kod = dloa.kod_kraju ' +
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_RODZAJOW_SPRAW slop ON dlu.id_r_sprawy =  slop.id_r_sprawy ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_RODZAJOW_SPRAW stow ON slop.typ_sp=  stow.typ_sp ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.S_DZIENNIKOW sdzn ON stow.id_dziennika=  sdzn.id_dziennika ' +
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.adm_wydzialy admw ON  slop.k_wydzial=admw.kod ' + 
			' LEFT JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_PODMIOTY sltp ON sltp.kod=dd.id_podmiot' + 
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_SPRAWA_SADOWA dlss ON dlss.id_dluznik  = dlu.id_dluznik ' + 
			' LEFT OUTER Join ' +  @sourcesrv +'.'  + @dbname +'.dbo.SL_ADR_SADOW slas ON dlss.id_sad_obcy = slas.kod ' +
			' LEFT OUTER JOIN ' +  @sourcesrv +'.'  + @dbname +'.dbo.DLUZNIK_STAN_SPRAWY dlstspr on dlstspr.id_dluznik = dlu.id_dluznik ' +
			'	order by dlu.id_dluznik '
		
print  @query1
print  @query2
EXEC (@query1 + ' ' + @query2)
end
GO
/****** Object:  Table [dbo].[SAPWydzial]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPWydzial](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[kodSad] [varchar](4) NULL,
	[numer] [varchar](10) NULL,
	[nazwa] [varchar](100) NULL,
	[sadWydzial] [varchar](15) NULL,
	[rodzajSprawy] [varchar](4) NULL,
	[numerWydz] [int] NULL,
 CONSTRAINT [PK_SAPWydzial] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[SAPWydzial] ON
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1, N'2002', N'I', N'I WYDZIA£ CYWILNY', N'2002@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2, N'2002', N'II', N'II WYDZIA£ KARNY', N'2002@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (3, N'2002', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2002@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (4, N'2002', N'IV', N'IV WYDZIA£ WIZYTACJI I DOSKONALENIA KADR', N'2002@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (5, N'2002', N'VI', N'VI WYDZIA£ CYWILNY', N'2002@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (6, N'2003', N'I', N'I WYDZIA£ CYWILNY', N'2003@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (7, N'2003', N'II', N'II WYDZIA£ KARNY', N'2003@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (8, N'2003', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2003@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (9, N'2003', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2003@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (10, N'2003', N'V', N'V WYDZIA£ CYWILNY', N'2003@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (11, N'2004', N'I', N'I WYDZIA£ CYWILNY', N'2004@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (12, N'2004', N'II', N'II WYDZIA£ KARNY', N'2004@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (13, N'2004', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2004@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (14, N'2004', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2004@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (15, N'2004', N'V', N'V WYDZIA£ CYWILNY', N'2004@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (16, N'2005', N'I', N'I WYDZIA£ CYWILNY', N'2005@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (17, N'2005', N'II', N'II WYDZIA£ KARNY', N'2005@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (18, N'2005', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2005@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (19, N'2005', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2005@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (20, N'2006', N'I', N'I WYDZIA£ CYWILNY', N'2006@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (21, N'2006', N'II', N'II WYDZIA£ KARNY', N'2006@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (22, N'2006', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2006@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (23, N'2006', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2006@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (24, N'2007', N'I', N'I WYDZIA£ CYWILNY', N'2007@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (25, N'2007', N'II', N'II WYDZIA£ KARNY', N'2007@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (26, N'2007', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2007@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (27, N'2007', N'IV', N'IV WYDZIA£ WIZYTACJI I DOSKONALENIA KADR', N'2007@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (28, N'2008', N'I', N'I WYDZIA£ CYWILNY', N'2008@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (29, N'2008', N'II', N'II WYDZIA£ KARNY', N'2008@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (30, N'2008', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2008@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (31, N'2008', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2008@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (32, N'2009', N'I', N'I WYDZIA£ CYWILNY', N'2009@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (33, N'2009', N'II', N'II WYDZIA£ KARNY', N'2009@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (34, N'2009', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2009@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (35, N'2009', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2009@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (36, N'2010', N'I', N'I WYDZIA£ CYWILNY', N'2010@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (37, N'2010', N'II', N'II WYDZIA£ KARNY', N'2010@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (38, N'2010', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2010@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (39, N'2010', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2010@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (40, N'2011', N'I', N'I WYDZIA£ CYWILNY', N'2011@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (41, N'2011', N'II', N'II WYDZIA£ KARNY', N'2011@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (42, N'2011', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2011@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (43, N'2011', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2011@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (44, N'2012', N'I', N'I WYDZIA£ CYWILNY', N'2012@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (45, N'2012', N'II', N'II WYDZIA£ KARNY', N'2012@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (46, N'2012', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'2012@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (47, N'2012', N'IV', N'IV WYDZIA£ WIZYTACJI', N'2012@IV', NULL, 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (48, N'3001', N'I', N'I WYDZIA£ CYWILNY', N'3001@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (49, N'3001', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3001@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (50, N'3001', N'III', N'III WYDZIA£ KARNY', N'3001@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (51, N'3001', N'IV', N'IV WYDZ PENITENCJARNY I NADZORU WYK ORZ', N'3001@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (52, N'3001', N'VI', N'VI WYDZIA£ KARNY ODWO£AWCZY', N'3001@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (53, N'3001', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3001@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (54, N'3002', N'I', N'I WYDZIA£ CYWILNY', N'3002@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (55, N'3002', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3002@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (56, N'3002', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'3002@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (57, N'3002', N'III', N'III WYDZIA£ KARNY', N'3002@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (58, N'3002', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3002@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (59, N'3002', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3002@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (60, N'3002', N'VII', N'VII WYDZIA£ WIZYTACYJNY', N'3002@VII', NULL, 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (61, N'3003', N'I', N'I WYDZIA£ CYWILNY', N'3003@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (62, N'3003', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3003@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (63, N'3003', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'3003@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (64, N'3003', N'III', N'III WYDZIA£ KARNY', N'3003@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (65, N'3003', N'VII', N'VII WYDZIA£ KARNY ODWO£AWCZY', N'3003@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (66, N'3003', N'IV', N'IV WYDZ PENITENCJARNY I NADZORU WYK ORZ', N'3003@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (67, N'3003', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3003@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (68, N'3003', N'VIII', N'VIII WYDZIA£ WIZYTACYJNY', N'3003@VIII', NULL, 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (69, N'3004', N'I', N'I WYDZIA£ CYWILNY', N'3004@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (70, N'3004', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3004@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (71, N'3004', N'III', N'III WYDZIA£ KARNY', N'3004@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (72, N'3004', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3004@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (73, N'3004', N'V', N'V WYDZIA£ PENITENCJARNY', N'3004@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (74, N'3004', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3004@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (75, N'3005', N'I', N'I WYDZIA£ CYWILNY', N'3005@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (76, N'3005', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3005@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (77, N'3005', N'X', N'X WYDZIA£ GOSPODARCZY', N'3005@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (78, N'3005', N'III', N'III WYDZIA£ KARNY', N'3005@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (79, N'3005', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3005@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (80, N'3005', N'V', N'V WYDZ PENITENCJARNY I NADZORU WYKON ORZ', N'3005@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (81, N'3005', N'IX', N'IX WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3005@IX', N'SUBE', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (82, N'3005', N'VII', N'VII WYDZIA£ PRACY', N'3005@VII', N'SPPR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (83, N'3005', N'VIII', N'VIII WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3005@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (84, N'3005', N'XIII', N'XIII WYDZIA£ CYWILNY RODZINNY', N'3005@XIII', N'SROD', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (85, N'3005', N'VI', N'VI WYDZIA£ WIZYTACYJNY', N'3005@VI', NULL, 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (86, N'3006', N'I', N'I WYDZIA£ CYWILNY', N'3006@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (87, N'3006', N'II', N'II WYDZIA£ CYWILNY', N'3006@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (88, N'3006', N'III', N'III WYDZIA£ KARNY', N'3006@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (89, N'3006', N'IV', N'IV WYDZ PENIT I NADZORU WYKON ORZ', N'3006@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (90, N'3006', N'V', N'V WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZNYCH', N'3006@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (91, N'3006', N'VII', N'VII WYDZIA£ GOSPODARCZY', N'3006@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (92, N'3006', N'VIII', N'VIII WYDZIA£ KARNY ODWO£AWCZY', N'3006@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (93, N'3007', N'I', N'I WYDZIA£ CYWILNY', N'3007@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (94, N'3007', N'II', N'II WYDZIA£ KARNY', N'3007@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (95, N'3007', N'III', N'III WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZ', N'3007@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (96, N'3008', N'I', N'I WYDZIA£ CYWILNY', N'3008@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (97, N'3008', N'II', N'II WYDZIA£ KARNY', N'3008@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (98, N'3008', N'III', N'III WYDZ PENITENCJARNY I NADZ WYKON ORZ', N'3008@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (99, N'3008', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£', N'3008@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (100, N'3008', N'V', N'V WYDZIA£ GOSPODARCZY', N'3008@V', N'SGOS', 5)
GO
print 'Processed 100 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (101, N'3008', N'VI', N'VI WYDZIA£ CYWILNY RODZINNY', N'3008@VI', N'SROD', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (102, N'3008', N'VII', N'VII WYDZIA£ KARNY ODWO£AWCZY', N'3008@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (103, N'3008', N'VIII', N'VIII WYDZIA£ WIZYTACYJNY', N'3008@VIII', NULL, 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (104, N'3008', N'IX', N'IX WYDZIA£ CYWILNY ODWO£AWCZY', N'3008@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (105, N'3009', N'I', N'I WYDZIA£ CYWILNY', N'3009@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (106, N'3009', N'II', N'II WYDZIA£ KARNY', N'3009@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (107, N'3009', N'III', N'III WYDZIA£ PRACY I UBEZPECZEÑ SPO£', N'3009@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (108, N'3010', N'I', N'I WYDZIA£ CYWILNY', N'3010@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (109, N'3010', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3010@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (110, N'3010', N'III', N'III WYDZIA£ KARNY', N'3010@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (111, N'3010', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3010@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (112, N'3010', N'V', N'V WYDZ PENITENCJARNY I NADZORU WYKON ORZ', N'3010@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (113, N'3010', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3010@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (114, N'3010', N'VIII', N'VIII WYDZIA£ GOSPODARCZY', N'3010@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (115, N'3010', N'IX', N'IX WYDZIA£ WIZYTACYJNY', N'3010@IX', NULL, 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (116, N'3010', N'X', N'X WYDZIA£ CYWILNY - RODZINNY', N'3010@X', N'SROD', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (117, N'3011', N'I', N'I WYDZIA£ CYWILNY', N'3011@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (118, N'3011', N'II', N'II WYDZIA£ KARNY', N'3011@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (119, N'3011', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3011@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (120, N'3011', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3011@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (121, N'3011', N'V', N'V WYDZIA£ CYWILNY - RODZINNY', N'3011@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (122, N'3011', N'VI', N'VI WYDZIA£ KARNY ODWO£AWCZY', N'3011@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (123, N'3012', N'I', N'I WYDZIA£ CYWILNY', N'3012@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (124, N'3012', N'II', N'II WYDZIA£ CYWILNY - RODZINNY', N'3012@II', N'SROD', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (125, N'3012', N'III', N'III WYDZIA£ CYWILNY ODWO£AWCZY', N'3012@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (126, N'3012', N'IV', N'IV WYDZIA£ KARNY', N'3012@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (127, N'3012', N'V', N'V WYDZIA£ KARNY ODWO£AWCZY', N'3012@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (128, N'3012', N'VI', N'VI WYDZIA£ PENITENCJARNY', N'3012@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (129, N'3012', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3012@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (130, N'3012', N'VIII', N'VIII WYDZPRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3012@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (131, N'3012', N'IX', N'IX WYDZIA£ GOSPODARCZY', N'3012@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (132, N'3012', N'X', N'X WYDZIA£ WIZYTACYJNY', N'3012@X', NULL, 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (133, N'3012', N'XI', N'XI WYDZIA£ WYKONAWCZY', N'3012@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (134, N'3012', N'XII', N'XII WYDZIA£ GOSPODARCZY-ODWO£AWCZY', N'3012@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (135, N'3012', N'XIII', N'XIII WYDZIA£ KARNY ODWO£AWCZY', N'3012@XIII', N'SKAR', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (136, N'3012', N'XIV', N'XIV WYDZIA£ KARNY', N'3012@XIV', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (137, N'3012', N'XV', N'XV WYDZIA£ CYWILNY', N'3012@XV', N'SCYW', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (138, N'3012', N'XVI', N'XVI WYDZIA£ CYWILNY ODWO£AWCZY', N'3012@XVI', N'SCYW', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (139, N'3013', N'I', N'I WYDZIA£ CYWILNY', N'3013@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (140, N'3013', N'I.1', N'SEKCJA RODZINNA', N'3013@I.1', N'SROD', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (141, N'3013', N'II', N'II WYDZIA£ KARNY', N'3013@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (142, N'3013', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3013@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (143, N'3013', N'IV', N'IV WYDZIA£ CYWILNY ODWO£AWCZY', N'3013@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (144, N'3013', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3013@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (145, N'3013', N'VI', N'VI WYDZIA£ KARNY ODWO£AWCZY', N'3013@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (146, N'3014', N'I', N'I WYDZIA£ CYWILNY', N'3014@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (147, N'3014', N'II', N'II WYDZIA£ KARNY', N'3014@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (148, N'3014', N'III', N'III WYDZ PENITENCJARNY', N'3014@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (149, N'3014', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3014@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (150, N'3014', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'3014@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (151, N'3014', N'VII', N'VII WYDZIA£ WIZYTACYJNY', N'3014@VII', NULL, 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (152, N'3014', N'VIII', N'VIII WYDZIA£ CYWILNY ODWO£AWCZY', N'3014@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (153, N'3014', N'IX', N'IX WYDZIA£ KARNY ODWO£AWCZY', N'3014@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (154, N'3015', N'I', N'I WYDZIA£ CYWILNY', N'3015@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (155, N'3015', N'II', N'II WYDZIA£ KARNY', N'3015@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (156, N'3015', N'III', N'III WYDZ PENITENCJARNY', N'3015@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (157, N'3015', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3015@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (158, N'3015', N'V', N'V WYDZIA£ CYWILNY - RODZINNY', N'3015@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (159, N'3016', N'I', N'I WYDZIA£ CYWILNY', N'3016@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (160, N'3016', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3016@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (161, N'3016', N'III', N'III WYDZIA£ KARNY', N'3016@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (162, N'3016', N'IV', N'IV WYDZIA£ PENITENCJARNY', N'3016@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (163, N'3016', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3016@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (164, N'3016', N'VII', N'VII WYDZIA£ KARNY ODWO£AWCZY', N'3016@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (165, N'3017', N'I', N'I WYDZIA£ CYWILNY', N'3017@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (166, N'3017', N'I.1', N'SEKCJA DS.ROZP.SPRAW O ROZWÓD I SEP.', N'3017@I.1', N'SROD', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (167, N'3017', N'II', N'II WYDZIA£ KARNY', N'3017@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (168, N'3017', N'II.1', N'SEKCJA WYKONAWCZA II WYDZIA£ KARNY', N'3017@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (169, N'3017', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3017@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (170, N'3017', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3017@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (171, N'3017', N'IV.1', N'SEKCJA DS. PRAWA PRACY I UBEZ.SPO£.', N'3017@IV.1', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (172, N'3017', N'V', N'V WYDZIA£ GOSPODARCZY', N'3017@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (173, N'3017', N'VI', N'VI WYDZIA£ CYWILNY ODWO£AWCZY', N'3017@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (174, N'3017', N'VII', N'VII WYDZIA£ KARNY ODWO£AWCZY', N'3017@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (175, N'3018', N'I', N'I WYDZIA£ CYWILNY', N'3018@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (176, N'3018', N'III', N'III WYDZIA£ CYWILNY ODWO£AWCZY', N'3018@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (177, N'3018', N'IV', N'IV WYDZIA£ KARNY', N'3018@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (178, N'3018', N'IV.1', N'SEKCJA WYKONAWCZA IV KARNEGO', N'3018@IV.1', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (179, N'3018', N'VI', N'VI WYDZIA£ KARNY ODWO£AWCZY', N'3018@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (180, N'3018', N'VII', N'VII WYDZIA£ PENITENCJARNY', N'3018@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (181, N'3018', N'VIII', N'VIII WYDZ PRACY I UBEZPIECZEÑ SPO£.', N'3018@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (182, N'3018', N'X', N'X WYDZIA£ GOSPODARCZY', N'3018@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (183, N'3018', N'XI', N'XI WYDZIA£ WIZYTACYJNY', N'3018@XI', NULL, 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (184, N'3018', N'XII', N'XII WYDZIA£ CYWILNY', N'3018@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (185, N'3018', N'II', N'OZ RYBNIK II WYDZIA£ CYWILNY', N'3018@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (186, N'3018', N'V', N'OZ RYBNIK V WYDZIA£ KARNY', N'3018@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (187, N'3018', N'IX', N'OZ RYBNIK IX WYDZ PRACY I UBEZP. SPO£.', N'3018@IX', N'SUBE', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (188, N'3019', N'I', N'I WYDZIA£ CYWILNY', N'3019@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (189, N'3019', N'II', N'II WYDZIA£ CYWILNY', N'3019@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (190, N'3019', N'III', N'III WYDZIA£ CYWILNY ODWO£AWCZY', N'3019@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (191, N'3019', N'IV', N'IV WYDZIA£ CYWILNY ODWO£AWCZY', N'3019@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (192, N'3019', N'V', N'V WYDZIA£ KARNY', N'3019@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (193, N'3019', N'VI', N'VI WYDZIA£ KARNY ODWO£AWCZY', N'3019@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (194, N'3019', N'VII', N'VII WYDZIA£ KARNY ODWO£AWCZY', N'3019@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (195, N'3019', N'VIII', N'VIII WYDZIA£ PENITENCJARNY', N'3019@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (196, N'3019', N'IX', N'IX WYDZIA£ PRACY', N'3019@IX', N'SPPR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (197, N'3019', N'IX.1', N'SEKCJA I INSTANCYJNA IX PRACY', N'3019@IX.1', N'SPPR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (198, N'3019', N'X', N'X WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3019@X', N'SUBE', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (199, N'3019', N'X.1', N'SEKCJA II INSTANCYJNA X U S', N'3019@X.1', NULL, 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (200, N'3019', N'XI', N'XI WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3019@XI', N'SUBE', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (201, N'3019', N'XIII', N'XIII WYDZIA£ GOSPODARCZY', N'3019@XIII', N'SGOS', 13)
GO
print 'Processed 200 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (202, N'3019', N'XIV', N'XIV WYDZIA£ GOSPODARCZY', N'3019@XIV', N'SGOS', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (203, N'3019', N'XV', N'XV WYDZIA£ WIZYTACYJNY', N'3019@XV', NULL, 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (204, N'3019', N'XVI', N'XVI WYDZIA£ KARNY', N'3019@XVI', N'SKAR', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (205, N'3019', N'XVII', N'XVII WYDZIA£ CYWILNY -RODZINNY', N'3019@XVII', N'SROD', 17)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (206, N'3019', N'XVIII', N'XVIII WYDZIA£ CYWILNY - RODZINNY', N'3019@XVIII', N'SROD', 18)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (207, N'3019', N'XIX', N'XIX WYDZIA£ GOSPODARCZY-ODWO£AWCZY', N'3019@XIX', N'SGOS', 19)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (208, N'3019', N'XXI', N'XXI WYDZIA£ KARNY', N'3019@XXI', N'SKAR', 21)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (209, N'3019', N'XXII', N'XXII WYDZIA£ WYKONAWCZY', N'3019@XXII', N'SKAR', 22)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (210, N'3019', N'XXIII', N'XXIII WYDZIA£ KARNY ODWO£AWCZY', N'3019@XXIII', N'SKAR', 23)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (211, N'3020', N'I', N'I WYDZIA£ CYWILNY', N'3020@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (212, N'3020', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3020@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (213, N'3020', N'III', N'III WYDZIA£ KARNY', N'3020@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (214, N'3020', N'IV', N'IV WYDZ PENITENCJARNY I NADZORU WYK ORZ', N'3020@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (215, N'3020', N'V', N'V WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZNYCH', N'3020@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (216, N'3020', N'VI', N'VI WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZNYC', N'3020@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (217, N'3020', N'VII', N'VII WYDZIA£ GOSPODARCZY', N'3020@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (218, N'3020', N'VIII', N'VIII WYDZIA£ WIZYTACYJNY', N'3020@VIII', NULL, 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (219, N'3020', N'IX', N'IX WYDZIA£ KARNY ODWO£AWCZY', N'3020@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (220, N'3021', N'I', N'I WYDZIA£ CYWILNY', N'3021@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (221, N'3021', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3021@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (222, N'3021', N'III', N'III WYDZIA£ KARNY', N'3021@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (223, N'3021', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3021@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (224, N'3021', N'V', N'V WYDZ PENITENCJARNY I NADZORU WYKON ORZ', N'3021@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (225, N'3021', N'VI', N'VI WYDZIA£ KARNY', N'3021@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (226, N'3021', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3021@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (227, N'3021', N'VIII', N'VIII WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3021@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (228, N'3021', N'IX', N'IX WYDZIA£ GOSPODARCZY', N'3021@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (229, N'3021', N'X', N'X WYDZIA£ WIZYTACYJNY', N'3021@X', NULL, 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (230, N'3021', N'XI', N'XI WYDZIA£ CYWILNY RODZINNY', N'3021@XI', N'SROD', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (231, N'3021', N'XII', N'XII WYDZIA£ GOSPODARCZY ODWO£AWCZY', N'3021@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (232, N'3022', N'I', N'I WYDZIA£ CYWILNY', N'3022@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (233, N'3022', N'II', N'II WYDZIA£ KARNY', N'3022@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (234, N'3022', N'III', N'III WYDZIA£ CYWILNY ODWO£AWCZY', N'3022@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (235, N'3022', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3022@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (236, N'3023', N'I', N'I WYDZIA£ CYWILNY', N'3023@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (237, N'3023', N'II', N'II WYDZIA£ KARNY', N'3023@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (238, N'3023', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3023@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (239, N'3023', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3023@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (240, N'3024', N'I', N'I WYDZIA£ CYWILNY', N'3024@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (241, N'3024', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3024@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (242, N'3024', N'III', N'III WYDZIA£ CYWILNY RODZINNY', N'3024@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (243, N'3024', N'IV', N'IV WYDZIA£ KARNY', N'3024@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (244, N'3024', N'V', N'V WYDZIA£ KARNY ODWO£AWCZY', N'3024@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (245, N'3024', N'VI', N'VI WYDZ PENITEN. I NADZORU WYKON ORZ', N'3024@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (246, N'3024', N'VII', N'VII WYDZIA£ PRACY I UBEZPECZEÑ SPO£.', N'3024@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (247, N'3024', N'VIII', N'VIII WYDZIA£ PRACY I UBEZPECZEÑ SPO£.', N'3024@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (248, N'3024', N'IX', N'IX WYDZIA£ GOSPODARCZY', N'3024@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (249, N'3024', N'X', N'X WYDZIA£ WIZYTACYJNY', N'3024@X', NULL, 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (250, N'3024', N'XI', N'XI WYDZIA£ KARNY ODWO£AWCZY', N'3024@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (251, N'3025', N'I', N'I WYDZIA£ CYWILNY', N'3025@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (252, N'3025', N'II', N'II WYDZIA£ KARNY', N'3025@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (253, N'3025', N'III', N'III WYDZ PENITENCJARNY I NADZORU WYK ORZ', N'3025@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (254, N'3025', N'IV', N'IV WYDZIA£ CYWILNY ODWO£AWCZY', N'3025@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (255, N'3025', N'V', N'V WYDZIA£ KARNY ODWO£AWCZY', N'3025@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (256, N'3025', N'VI', N'VI WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZNYC', N'3025@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (257, N'3025', N'VII', N'VII WYDZIA£ WIZYTACYJNY', N'3025@VII', NULL, 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (258, N'3026', N'I', N'I WYDZIA£ CYWILNY', N'3026@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (259, N'3026', N'II', N'II WYDZIA£ KARNY', N'3026@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (260, N'3026', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3026@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (261, N'3026', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£.', N'3026@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (262, N'3026', N'V', N'V WYDZIA£ CYWILNY ODWO£AWCZY', N'3026@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (263, N'3027', N'I', N'I WYDZIA£ CYWILNY', N'3027@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (264, N'3027', N'II', N'II WYDZIA£ KARNY', N'3027@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (265, N'3027', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3027@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (266, N'3027', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZNY', N'3027@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (267, N'3028', N'I', N'I WYDZIA£ CYWILNY', N'3028@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (268, N'3028', N'II', N'II WYDZIA£ CYWILNY', N'3028@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (269, N'3028', N'III', N'III WYDZIA£ KARNY', N'3028@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (270, N'3028', N'IV', N'IV WYDZIA£ KARNY', N'3028@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (271, N'3028', N'V', N'V WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZNYCH', N'3028@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (272, N'3029', N'I', N'I WYDZIA£ CYWILNY', N'3029@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (273, N'3029', N'II', N'II WYDZ CYWILNY', N'3029@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (274, N'3029', N'III', N'III WYDZIA£ CYWILNY ODWO£AWCZY', N'3029@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (275, N'3029', N'IV', N'IV WYDZIA£ KARNY', N'3029@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (276, N'3029', N'V', N'V WYDZIA£ KARNY ODWO£AWCZY', N'3029@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (277, N'3029', N'VI', N'VI WYDZ PENITEN I NADZORU WYKON ORZ', N'3029@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (278, N'3029', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3029@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (279, N'3029', N'VIII', N'VIII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYC', N'3029@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (280, N'3029', N'X', N'X WYDZIA£ GOSPODARCZY', N'3029@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (281, N'3029', N'XI', N'XI WYDZIA£ WIZYTACYJNY', N'3029@XI', NULL, 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (282, N'3029', N'XII', N'XII WYDZIA£ CYWILNY RODZINNY', N'3029@XII', N'SROD', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (283, N'3029', N'XIII', N'XIII WYDZIA£ GOSPODARCZY ODWO£AWCZY', N'3029@XIII', N'SGOS', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (284, N'3029', N'XVIII', N'XVIII WYDZIA£ KARNY', N'3029@XVIII', N'SKAR', 18)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (285, N'3030', N'I', N'I WYDZIA£ CYWILNY', N'3030@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (286, N'3030', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3030@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (287, N'3030', N'III', N'III WYDZIA£ KARNY', N'3030@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (288, N'3030', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3030@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (289, N'3030', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3030@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (290, N'3031', N'I', N'I WYDZIA£ CYWILNY', N'3031@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (291, N'3031', N'II', N'II WYDZIA£ KARNY', N'3031@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (292, N'3031', N'III', N'III WYDZ PENITENCJARNY I NADZORU WYK ORZ', N'3031@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (293, N'3031', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3031@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (294, N'3032', N'I', N'I WYDZIA£ CYWILNY', N'3032@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (295, N'3032', N'II', N'II WYDZIA£ KARNY', N'3032@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (296, N'3032', N'III', N'III WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3032@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (297, N'3033', N'I', N'I WYDZIA£ CYWILNY', N'3033@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (298, N'3033', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3033@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (299, N'3033', N'III', N'III WYDZIA£ KARNY', N'3033@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (300, N'3033', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3033@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (301, N'3033', N'V', N'V WYDZ PENITENCJARNY I NADZORU WYKON ORZ', N'3033@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (302, N'3033', N'VI', N'VI WYDZIA£ PRACY', N'3033@VI', N'SPPR', 6)
GO
print 'Processed 300 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (303, N'3033', N'VII', N'VII WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3033@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (304, N'3033', N'VIII', N'VIII WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3033@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (305, N'3033', N'IX', N'IX WYDZIA£ GOSPODARCZY', N'3033@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (306, N'3033', N'X', N'X WYDZIA£ GOSPODARCZY ODWO£AWCZY', N'3033@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (307, N'3033', N'XI', N'XI WYDZIA£ WIZYTACYJNY', N'3033@XI', NULL, 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (308, N'3033', N'XII', N'XII WYDZIA£ CYWILNY', N'3033@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (309, N'3033', N'XIII', N'OZ LESZNO XIII WYDZIA£ CYWILNY', N'3033@XIII', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (310, N'3033', N'XIV', N'OZ PI£A XIV WYDZIA£ CYWILNY', N'3033@XIV', N'SCYW', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (311, N'3033', N'XV', N'XV WYDZIA£ CYWILNY ODWO£AWCZY', N'3033@XV', N'SCYW', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (312, N'3033', N'XVI', N'XVI WYDZIA£ KARNY', N'3033@XVI', N'SKAR', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (313, N'3033', N'XVII', N'XVII WYDZIA£ KARNY ODWO£AWCZY', N'3033@XVII', N'SKAR', 17)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (314, N'3034', N'I', N'I WYDZIA£ CYWILNY', N'3034@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (315, N'3034', N'II', N'II WYDZIA£ KARNY', N'3034@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (316, N'3034', N'III', N'III WYDZIA£ PENITENCJARNY', N'3034@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (317, N'3034', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3034@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (318, N'3034', N'VI', N'VI WYDZIA£ CYWILNY ODWO£AWCZY', N'3034@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (319, N'3034', N'VII', N'VII WYDZIA£ KARNY ODWO£AWCZY', N'3034@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (320, N'3034', N'VIII', N'VIII WYDZIA£ WIZYTACYJNY', N'3034@VIII', NULL, 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (321, N'3035', N'I', N'I WYDZIA£ CYWILNY', N'3035@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (322, N'3035', N'II', N'II WYDZIA£ KARNY', N'3035@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (323, N'3035', N'III', N'III WYDZ PENITENCJARNY I NADZORU WYK ORZ', N'3035@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (324, N'3035', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£', N'3035@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (325, N'3036', N'I', N'I WYDZIA£ CYWILNY', N'3036@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (326, N'3036', N'II', N'II WYDZIA£ KARNY', N'3036@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (327, N'3036', N'III', N'III WYDZIA£ PRACY I UBEZPECZEÑ SPO£', N'3036@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (328, N'3036', N'IV', N'IV WYDZ PENITENCJARNY I NADZ WYKON ORZ', N'3036@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (329, N'3037', N'I', N'I WYDZIA£ CYWILNY', N'3037@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (330, N'3037', N'II', N'II WYDZIA£ KARNY', N'3037@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (331, N'3037', N'III', N'III WYDZ PENITENCJARNY I NADZ WYKON ORZ', N'3037@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (332, N'3037', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£', N'3037@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (333, N'3037', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'3037@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (334, N'3037', N'VII', N'VII WYDZIA£ WIZYTACYJNY', N'3037@VII', NULL, 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (335, N'3038', N'I', N'I WYDZIA£ CYWILNY', N'3038@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (336, N'3038', N'II', N'II WYDZIA£ KARNY', N'3038@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (337, N'3038', N'III', N'III WYDZIA£ PRACY I UBEZPECZEÑ SPO£', N'3038@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (338, N'3038', N'V', N'V WYDZ PENITENCJARNY I NADZORU WYKON ORZ', N'3038@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (339, N'3039', N'I', N'I WYDZIA£ CYWILNY', N'3039@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (340, N'3039', N'II', N'II WYDZIA£ KARNY', N'3039@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (341, N'3039', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3039@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (342, N'3039', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3039@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (343, N'3039', N'V', N'V WYDZIA£ CYWILNY ODWO£AWCZY', N'3039@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (344, N'3039', N'VI', N'VI WYDZIA£ PRACY I UBEZPECZEÑ SPO£.', N'3039@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (345, N'3040', N'I', N'I WYDZIA£ CYWILNY', N'3040@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (346, N'3040', N'II', N'II WYDZIA£ KARNY', N'3040@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (347, N'3040', N'III', N'III WYDZ PENITEN. I NADZORU WYKON ORZ', N'3040@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (348, N'3040', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£.', N'3040@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (349, N'3040', N'V', N'V WYDZIA£ KARNY ODWO£AWCZY', N'3040@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (350, N'3040', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'3040@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (351, N'3040', N'VII', N'VII WYDZIA£ CYWILNY ODWO£AWCZY', N'3040@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (352, N'3040', N'VIII', N'VIII WYDZIA£ WIZYTACYJNY', N'3040@VIII', NULL, 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (353, N'3041', N'I', N'I WYDZIA£ CYWILNY', N'3041@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (354, N'3041', N'II', N'II WYDZIA£ CYWILNY ODWO£AWCZY', N'3041@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (355, N'3041', N'III', N'III WYDZIA£ KARNY', N'3041@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (356, N'3041', N'IV', N'IV WYDZIA£ KARNY ODWO£AWCZY', N'3041@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (357, N'3041', N'V', N'V WYDZ PENITENCJARNY I NADZORU WYKON ORZ', N'3041@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (358, N'3041', N'VI', N'VI WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZ.', N'3041@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (359, N'3041', N'VII', N'VII WYDZIA£ PRACY I UBEZPECZEÑ SPO£ECZ.', N'3041@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (360, N'3041', N'VIII', N'VIII WYDZIA£ GOSPODARCZY', N'3041@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (361, N'3041', N'IX', N'IX WYDZIA£ WIZYTACYJNY', N'3041@IX', NULL, 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (362, N'3041', N'X', N'X WYDZIA£ CYWILNY RODZINNY', N'3041@X', N'SROD', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (363, N'3042', N'I', N'I WYDZIA£ CYWILNY', N'3042@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (364, N'3042', N'II', N'II WYDZIA£ KARNY', N'3042@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (365, N'3042', N'III', N'III WYDZIA£ PRACY I UBEZPECZEÑ SPO£', N'3042@III', N'SUBE', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (366, N'3043', N'I', N'I WYDZIA£ CYWILNY', N'3043@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (367, N'3043', N'II', N'II WYDZIA£ KARNY', N'3043@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (368, N'3043', N'III', N'III WYDZ PENITENCJARNY I NADZORU WYK ORZ', N'3043@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (369, N'3043', N'IV', N'IV WYDZIA£ CYWILNY ODWO£AWCZY', N'3043@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (370, N'3043', N'V', N'V WYDZIA£ KARNY ODWO£AWCZY', N'3043@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (371, N'3043', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3043@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (372, N'3044', N'I', N'I WYDZIA£ CYWILNY', N'3044@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (373, N'3044', N'II', N'II WYDZIA£ CYWILNY', N'3044@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (374, N'3044', N'III', N'III WYDZIA£ CYWILNY', N'3044@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (375, N'3044', N'IV', N'IV WYDZIA£ CYWILNY', N'3044@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (376, N'3044', N'V', N'V WYDZIA£ CYWILNY ODWO£AWCZY', N'3044@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (377, N'3044', N'VI', N'VI WYDZIA£ CYWILNY RODZINNY ODWO£AWCZY', N'3044@VI', N'SROD', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (378, N'3044', N'VII', N'VII WYDZIA£ CYWILNY REJESTROWY', N'3044@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (379, N'3044', N'VIII', N'VIII WYDZIA£ KARNY', N'3044@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (380, N'3044', N'IX', N'IX WYDZIA£ KARNY ODWO£AWCZY', N'3044@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (381, N'3044', N'X', N'X WYDZIA£ KARNY ODWO£AWCZY', N'3044@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (382, N'3044', N'XI', N'XI WYDZ PENITEN. I NADZORU WYKON ORZ', N'3044@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (383, N'3044', N'XII', N'XII WYDZIA£ KARNY', N'3044@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (384, N'3044', N'XIII', N'XIII WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3044@XIII', N'SUBE', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (385, N'3044', N'XIV', N'XIV WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'3044@XIV', N'SUBE', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (386, N'3044', N'XV', N'XV WYDZIA£ WYKONYWANIA ORZECZEÑ', N'3044@XV', N'SKAR', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (387, N'3044', N'XVI', N'XVI WYDZIA£ GOSPODARCZY', N'3044@XVI', N'SGOS', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (388, N'3044', N'XVII', N'XVII WYDZ S¥D OCHR. KONK. I KONSUMENTÓW', N'3044@XVII', N'SCYW', 17)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (389, N'3044', N'XVIII', N'XVIII WYDZIA£ KARNY', N'3044@XVIII', N'SKAR', 18)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (390, N'3044', N'XIX', N'XIX WYDZIA£ WIZYTACYJNY', N'3044@XIX', NULL, 19)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (391, N'3044', N'XX', N'XX WYDZIA£ GOSPODARCZY', N'3044@XX', N'SGOS', 20)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (392, N'3044', N'XXI', N'XXI WYDZIA£ PRACY', N'3044@XXI', N'SPPR', 21)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (393, N'3044', N'XXII', N'XXII WYDZ. S¥D WSPÓLN. ZNAK. TOWAR.', N'3044@XXII', N'SCYW', 22)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (394, N'3044', N'XXIII', N'XXIII WYDZIA£ GOSPODARCZY ODWO£AWCZY', N'3044@XXIII', N'SGOS', 23)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (395, N'3044', N'XXIV', N'XXIV WYDZIA£ CYWILNY', N'3044@XXIV', N'SCYW', 24)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (396, N'3044', N'XXV', N'XXV WYDZIA£ CYWILNY', N'3044@XXV', N'SCYW', 25)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (397, N'3044', N'XXVI', N'XXVI WYDZIA£ GOSPODARCZY', N'3044@XXVI', N'SGOS', 26)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (398, N'3045', N'I', N'I WYDZIA£ CYWILNY', N'3045@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (399, N'3045', N'II', N'II WYDZIA£ CYWILNY', N'3045@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (400, N'3045', N'III', N'III WYDZIA£ CYWILNY', N'3045@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (401, N'3045', N'IV', N'IV WYDZIA£ CYWILNY ODWO£AWCZY', N'3045@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (402, N'3045', N'V', N'V WYDZIA£ KARNY', N'3045@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (403, N'3045', N'VI', N'VI WYDZIA£ KARNY ODWO£AWCZY', N'3045@VI', N'SKAR', 6)
GO
print 'Processed 400 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (404, N'3045', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'3045@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (405, N'3045', N'VIII', N'VIII WYDZ PENIT. I NADZORU WYKON ORZ', N'3045@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (406, N'3045', N'IX', N'IX WYDZIA£ WIZYTACYJNY', N'3045@IX', NULL, 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (407, N'3045', N'X', N'X WYDZIA£ GOSPODARCZY', N'3045@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (408, N'3045', N'XI', N'XI WYDZIA£ KSI¥G WIECZYSTYCH', N'3045@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (409, N'4001', N'I', N'I WYDZIA£ CYWILNY', N'4001@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (410, N'4001', N'V', N'V WYDZIA£ GOSPODARCZY', N'4001@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (411, N'4001', N'II', N'II WYDZIA£ KARNY', N'4001@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (412, N'4001', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4001@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (413, N'4001', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4001@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (414, N'4001', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4001@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (415, N'4001', N'II.1', N'SEKCJA DS. WYKROCZEÑ', N'4001@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (416, N'4001', N'VII', N'OZ VII WYDZIA£ CYWILNY', N'4001@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (417, N'4001', N'VIII', N'OZ VIII WYDZIA£ KARNY', N'4001@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (418, N'4001', N'X', N'OZ X WYDZIA£ KSI¥G WIECZYSTYCH', N'4001@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (419, N'4001', N'IX', N'OZ IX WYDZIA£ RODZINNY I NIELETNICH', N'4001@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (420, N'4002', N'VIII', N'VIII WYDZIA£ KARNY', N'4002@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (421, N'4002', N'I', N'I WYDZIA£ CYWILNY', N'4002@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (422, N'4002', N'VII', N'VII WYDZIA£ CYWILNY', N'4002@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (423, N'4002', N'V', N'V WYDZIA£ GOSPODARCZY', N'4002@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (424, N'4002', N'II', N'II WYDZIA£ KARNY', N'4002@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (425, N'4002', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4002@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (426, N'4002', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4002@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (427, N'4002', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4002@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (428, N'4003', N'I', N'I WYDZIA£ CYWILNY', N'4003@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (429, N'4003', N'II', N'II WYDZIA£ KARNY', N'4003@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (430, N'4003', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4003@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (431, N'4003', N'IV', N'IV WYDZIA£ PRACY', N'4003@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (432, N'4003', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4003@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (433, N'4004', N'I', N'I WYDZIA£ CYWILNY', N'4004@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (434, N'4004', N'II', N'II WYDZIA£ KARNY', N'4004@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (435, N'4004', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4004@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (436, N'4004', N'IV', N'IV WYDZIA£ PRACY', N'4004@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (437, N'4004', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4004@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (438, N'4005', N'I', N'I WYDZIA£ CYWILNY', N'4005@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (439, N'4005', N'II', N'II WYDZIA£ KARNY', N'4005@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (440, N'4005', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4005@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (441, N'4005', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4005@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (442, N'4005', N'V', N'OZ V WYDZIA£ CYWILNY', N'4005@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (443, N'4005', N'VI', N'OZ VI WYDZIA£ KARNY', N'4005@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (444, N'4005', N'VII', N'OZ VII WYDZIA£ RODZINNY I NIELETNICH', N'4005@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (445, N'4005', N'VIII', N'OZ VIII ZAMIEJSCOWY WYDZIA£ KSI¥G WIECZ', N'4005@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (446, N'4006', N'I', N'I WYDZIA£ CYWILNY', N'4006@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (447, N'4006', N'II', N'II WYDZIA£ KARNY', N'4006@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (448, N'4006', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4006@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (449, N'4006', N'IV', N'IV WYDZIA£ PRACY', N'4006@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (450, N'4006', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4006@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (451, N'4007', N'I', N'I WYDZIA£ CYWILNY', N'4007@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (452, N'4007', N'V', N'V WYDZIA£ GOSPODARCZY', N'4007@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (453, N'4007', N'VIII', N'VIII WYDZIA£ GOSPODARCZY KRS', N'4007@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (454, N'4007', N'II', N'II WYDZIA£ KARNY', N'4007@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (455, N'4007', N'VII', N'VII WYDZIA£ KARNY', N'4007@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (456, N'4007', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4007@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (457, N'4007', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4007@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (458, N'4007', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4007@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (459, N'4008', N'I', N'I WYDZIA£ CYWILNY', N'4008@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (460, N'4008', N'II', N'II WYDZIA£ KARNY', N'4008@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (461, N'4008', N'VI', N'VI WYDZIA£ KARNY', N'4008@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (462, N'4008', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4008@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (463, N'4008', N'VII', N'VII ZAMIEJSCOWY WYDZ KSI¥G WIECZYSTYCH', N'4008@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (464, N'4008', N'IV', N'IV WYDZIA£ PRACY', N'4008@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (465, N'4008', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4008@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (466, N'4009', N'I', N'I WYDZIA£ CYWILNY', N'4009@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (467, N'4009', N'II', N'II WYDZIA£ KARNY', N'4009@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (468, N'4009', N'VI', N'VI WYDZIA£ KARNY', N'4009@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (469, N'4009', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4009@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (470, N'4009', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4009@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (471, N'4009', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4009@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (472, N'4010', N'I', N'I WYDZIA£ CYWILNY', N'4010@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (473, N'4010', N'VIII', N'VIII WYDZIA£ CYWILNY', N'4010@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (474, N'4010', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4010@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (475, N'4010', N'II', N'II WYDZIA£ KARNY', N'4010@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (476, N'4010', N'III', N'III WYDZIA£ KARNY', N'4010@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (477, N'4010', N'VII', N'VII WYDZIA£ KSI¥G WIECZYSTYCH', N'4010@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (478, N'4010', N'V', N'V WYDZIA£ PRACY', N'4010@V', N'SPPR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (479, N'4010', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4010@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (480, N'4011', N'I', N'I WYDZIA£ CYWILNY', N'4011@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (481, N'4011', N'XI', N'XI WYDZIA£ CYWILNY', N'4011@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (482, N'4011', N'XIV', N'XIV WYDZIA£ CYWILNY', N'4011@XIV', N'SCYW', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (483, N'4011', N'IV', N'IV WYDZIA£ GOSPODARCZY', N'4011@IV', N'SGOS', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (484, N'4011', N'V', N'V WYDZIA£ GOSPODARCZY', N'4011@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (485, N'4011', N'VIII', N'VIII WYDZ GOSPODARCZY DS. UPAD£OŒCIOWYCH', N'4011@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (486, N'4011', N'XV', N'XV WYDZIA£ GOSPODARCZY', N'4011@XV', N'SGOS', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (487, N'4011', N'IX', N'IX WYDZIA£ GOSPODARCZY KRS', N'4011@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (488, N'4011', N'VI', N'VI WYDZIA£ GOSPODARCZY KRS', N'4011@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (489, N'4011', N'VII', N'VII WYDZIA£ GOSPOD. REJESTRU ZASTAWÓW', N'4011@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (490, N'4011', N'II', N'II WYDZIA£ KARNY', N'4011@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (491, N'4011', N'X', N'X WYDZIA£ KARNY', N'4011@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (492, N'4011', N'XII', N'XII WYDZIA£ KARNY', N'4011@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (493, N'4011', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4011@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (494, N'4012', N'I', N'I WYDZIA£ CYWILNY', N'4012@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (495, N'4012', N'VI', N'VI WYDZIA£ CYWILNY', N'4012@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (496, N'4012', N'II', N'II WYDZIA£ KARNY', N'4012@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (497, N'4012', N'V', N'V WYDZIA£ KARNY', N'4012@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (498, N'4012', N'VII', N'VII WYDZIA£ KARNY', N'4012@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (499, N'4012', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4012@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (500, N'4012', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4012@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (501, N'4013', N'I', N'I WYDZIA£ CYWILNY', N'4013@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (502, N'4013', N'IX', N'IX WYDZIA£ CYWILNY', N'4013@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (503, N'4013', N'VIII', N'VIII WYDZIA£ CYWILNY', N'4013@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (504, N'4013', N'II', N'II WYDZIA£ KARNY', N'4013@II', N'SKAR', 2)
GO
print 'Processed 500 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (505, N'4013', N'V', N'V WYDZIA£ KARNY', N'4013@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (506, N'4013', N'VI', N'VI WYDZIA£ KARNY', N'4013@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (507, N'4013', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4013@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (508, N'4013', N'X', N'X WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4013@X', N'SUBE', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (509, N'4013', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4013@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (510, N'4014', N'I', N'I WYDZIA£ CYWILNY', N'4014@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (511, N'4014', N'II', N'II WYDZIA£ KARNY', N'4014@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (512, N'4014', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4014@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (513, N'4014', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4014@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (514, N'4016', N'I', N'I WYDZIA£ CYWILNY', N'4016@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (515, N'4016', N'II', N'II WYDZIA£ KARNY', N'4016@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (516, N'4016', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4016@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (517, N'4016', N'IV', N'IV WDZIA£ PRACY', N'4016@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (518, N'4016', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4016@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (519, N'4017', N'I', N'I WYDZIA£ CYWILNY', N'4017@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (520, N'4017', N'V', N'V WYDZIA£ GOSPODARCZY', N'4017@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (521, N'4017', N'II', N'II WYDZIA£ KARNY', N'4017@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (522, N'4017', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4017@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (523, N'4017', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4017@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (524, N'4017', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4017@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (525, N'4019', N'I', N'I WYDZIA£ CYWILNY', N'4019@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (526, N'4019', N'II', N'II WYDZIA£ KARNY', N'4019@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (527, N'4019', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4019@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (528, N'4019', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4019@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (529, N'4019', N'VI', N'OZ VI WYDZIA£ CYWILNY', N'4019@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (530, N'4019', N'VII', N'OZ VII WYDZIA£ KARNY', N'4019@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (531, N'4019', N'VIII', N'OZV III WYDZIA£ RODZINNY I NIELETNICH', N'4019@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (532, N'4019', N'IX', N'IX ZAMIEJSCOWY WYDZIA£ KSI¥G WIECZYSTYCH', N'4019@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (533, N'4021', N'I', N'I WYDZIA£ CYWILNY', N'4021@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (534, N'4021', N'II', N'II WYDZIA£ KARNY', N'4021@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (535, N'4021', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4021@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (536, N'4021', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4021@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (537, N'4021', N'VI', N'OZ VI ZAMIEJSCOWY WYDZIA£ CYWILNY', N'4021@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (538, N'4021', N'VII', N'OZ VII ZAMIEJSCOWY WYDZIA£ KARNY', N'4021@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (539, N'4021', N'IX', N'OZ IX ZAMIEJSCOWY WYDZIA£ KSI¥G WIECZYST', N'4021@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (540, N'4021', N'VIII', N'OZ VIII ZAMIEJSCOWY WYDZIA£ RODZ I NIELE', N'4021@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (541, N'4022', N'I', N'I WYDZIA£ CYWILNY', N'4022@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (542, N'4022', N'II', N'II WYDZIA£ KARNY', N'4022@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (543, N'4022', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4022@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (544, N'4022', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4022@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (545, N'4022', N'V', N'OZ V WYDZIA£ CYWILNY', N'4022@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (546, N'4022', N'VI', N'OZ VI WYDZIA£ KARNY', N'4022@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (547, N'4022', N'VIII', N'OZ VIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4022@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (548, N'4022', N'VII', N'OZ VII WYDZIA£ RODZINNY I NIELETNICH', N'4022@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (549, N'4024', N'I', N'I WYDZIA£ CYWILNY', N'4024@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (550, N'4024', N'II', N'II WYDZIA£ KARNY', N'4024@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (551, N'4024', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4024@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (552, N'4024', N'IV', N'IV WYDZIA£ PRACY', N'4024@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (553, N'4024', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4024@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (554, N'4026', N'I', N'I WYDZIA£ CYWILNY', N'4026@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (555, N'4026', N'II', N'II WYDZIA£ KARNY', N'4026@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (556, N'4026', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4026@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (557, N'4026', N'IV', N'IV WYDZIA£ PRACY', N'4026@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (558, N'4026', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4026@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (559, N'4026', N'VI', N'OZ VI WYDZIA£ CYWILNY', N'4026@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (560, N'4026', N'VII', N'OZ VII WYDZIA£ KARNY', N'4026@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (561, N'4026', N'VIII', N'OZ VIII WYDZIA£ RODZINNY I NIELETNICH', N'4026@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (562, N'4026', N'IX', N'OZ IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4026@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (563, N'4027', N'I', N'I WYDZIA£ CYWILNY', N'4027@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (564, N'4027', N'II', N'II WYDZIA£ KARNY', N'4027@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (565, N'4027', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4027@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (566, N'4027', N'IV', N'IV WYDZIA£ PRACY', N'4027@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (567, N'4027', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4027@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (568, N'4029', N'I', N'I WYDZIA£ CYWILNY', N'4029@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (569, N'4029', N'II', N'II WYDZIA£ KARNY', N'4029@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (570, N'4029', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4029@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (571, N'4029', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4029@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (572, N'4029', N'VI', N'OZ VI WYDZIA£ CYWILNY', N'4029@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (573, N'4029', N'VII', N'OZ VII WYDZIA£ KARNY', N'4029@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (574, N'4029', N'IX', N'OZ IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4029@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (575, N'4029', N'VIII', N'OZ VIII WYDZIA£ RODZINNY I NIELETNICH', N'4029@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (576, N'4030', N'I', N'I WYDZIA£ CYWILNY', N'4030@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (577, N'4030', N'II', N'II WYDZIA£ KARNY', N'4030@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (578, N'4030', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4030@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (579, N'4030', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4030@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (580, N'4033', N'I', N'I WYDZIA£ CYWILNY', N'4033@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (581, N'4033', N'II', N'II WYDZIA£ KARNY', N'4033@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (582, N'4033', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4033@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (583, N'4033', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4033@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (584, N'4033', N'VI', N'VI WYDZIA£ CYWILNY', N'4033@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (585, N'4033', N'VII', N'VII WYDZIA£ KARNY', N'4033@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (586, N'4033', N'IX', N'IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4033@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (587, N'4033', N'VIII', N'VIII WYDZIA£ RODZINNY I NIELETNICH', N'4033@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (588, N'4034', N'I', N'I WYDZIA£ CYWILNY', N'4034@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (589, N'4034', N'II', N'II WYDZIA£ KARNY', N'4034@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (590, N'4034', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4034@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (591, N'4034', N'V', N'V WYDZ KSI¥G WIECZ', N'4034@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (592, N'4035', N'I', N'I WYDZIA£ CYWILNY', N'4035@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (593, N'4035', N'II', N'II WYDZIA£ KARNY', N'4035@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (594, N'4035', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4035@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (595, N'4035', N'IV', N'IV WYDZIA£ PRACY', N'4035@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (596, N'4035', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4035@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (597, N'4036', N'I', N'I WYDZIA£ CYWILNY', N'4036@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (598, N'4036', N'II', N'II WYDZIA£ CYWILNY', N'4036@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (599, N'4036', N'II.1', N'SEKCJA EGZEKUCYJNA', N'4036@II.1', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (600, N'4036', N'III', N'III WYDZIA£ KARNY', N'4036@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (601, N'4036', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4036@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (602, N'4036', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4036@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (603, N'4036', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4036@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (604, N'4036', N'VII', N'VII WYDZIA£ KARNY', N'4036@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (605, N'4036', N'VIII', N'VIII WYDZIA£ GOSPODARCZY', N'4036@VIII', N'SGOS', 8)
GO
print 'Processed 600 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (606, N'4036', N'IX', N'IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4036@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (607, N'4036', N'X', N'X WYDZIA£ GOSPODARCZY I REJESTRU ZASTAW', N'4036@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (608, N'4036', N'XI', N'XI WYDZIA£ CYWILNY', N'4036@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (609, N'4036', N'XII', N'XII WYDZIA£ GOSPODARCZY KRS', N'4036@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (610, N'4036', N'XIII', N'XIII WYDZIA£ KARNY', N'4036@XIII', N'SKAR', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (611, N'4036', N'XV', N'XV WYDZIA£ KARNY', N'4036@XV', N'SKAR', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (612, N'4037', N'I', N'I WYDZIA£ CYWILNY', N'4037@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (613, N'4037', N'II', N'II WYDZIA£ KARNY', N'4037@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (614, N'4037', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4037@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (615, N'4037', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4037@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (616, N'4037', N'VII', N'OZ HAJNÓWKA VII WYDZIA£ KARNY', N'4037@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (617, N'4037', N'VIII', N'OZ SIEMIATYCZE VIII WYDZIA£ KARNY', N'4037@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (618, N'4037', N'IX', N'OZ HAJNÓWKA IX WYDZ KSI¥G WIECZYSTYCH', N'4037@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (619, N'4037', N'X', N'OZ SIEMIATYCZE X WYDZ KSI¥G WIECZYSTYCH', N'4037@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (620, N'4038', N'I', N'I WYDZIA£ CYWILNY', N'4038@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (621, N'4038', N'II', N'II WYDZIA£ KARNY', N'4038@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (622, N'4038', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4038@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (623, N'4038', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4038@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (624, N'4039', N'I', N'I WYDZIA£ CYWILNY', N'4039@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (625, N'4039', N'II', N'II WYDZIA£ KARNY', N'4039@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (626, N'4039', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4039@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (627, N'4039', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4039@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (628, N'4039', N'V', N'V WYDZIA£ GOSPODARCZY', N'4039@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (629, N'4039', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4039@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (630, N'4039', N'VII', N'OZ KOLNO VII WYDZIA£ KARNY', N'4039@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (631, N'4039', N'VIII', N'OZ GRAJEWO VIII WYDZIA£ CYWILNY', N'4039@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (632, N'4039', N'IX', N'OZ GRAJEWO IX WYDZIA£ KARNY', N'4039@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (633, N'4039', N'X', N'OZ GRAJEWO X WYDZ. RODZ. I NIELETNICH', N'4039@X', N'SROD', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (634, N'4039', N'XI', N'OZ GRAJEWO XI WYDZ KSI¥G WIECZYSTYCH', N'4039@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (635, N'4040', N'I', N'I WYDZIA£ CYWILNY', N'4040@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (636, N'4040', N'II', N'II WYDZIA£ KARNY', N'4040@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (637, N'4040', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4040@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (638, N'4040', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4040@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (639, N'4040', N'VI', N'OZ WYSOKIE MAZ. VI WYDZIA£ CYWILNY', N'4040@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (640, N'4040', N'VII', N'OZ WYSOKIE MAZ. VII WYDZIA£ KARNY', N'4040@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (641, N'4040', N'VIII', N'OZ WYSOKIE MAZ. VIII WYDZ. RODZ.I NIEL', N'4040@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (642, N'4040', N'IX', N'OZ WYSOKIE MAZ. IX WYDZ KSI¥G WIECZ.', N'4040@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (643, N'4041', N'I', N'I WYDZIA£ CYWILNY', N'4041@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (644, N'4041', N'II', N'II WYDZIA£ KARNY', N'4041@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (645, N'4041', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4041@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (646, N'4041', N'IV', N'IV WYDZIA£ PRACY', N'4041@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (647, N'4041', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4041@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (648, N'4041', N'VI', N'OZ LIDZBARK WARMIÑSKI VI WYDZIA£ CYWILNY', N'4041@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (649, N'4041', N'VII', N'OZ LIDZBARK WARMIÑSKI VII WYDZIA£ KARNY', N'4041@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (650, N'4041', N'VIII', N'OZ LIDZBARK WARM VIII WYDZ.RODZ.I NIELET', N'4041@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (651, N'4041', N'IX', N'OZ LIDZBARK WARMIÑSKI IX WYDZ KS WIECZ.', N'4041@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (652, N'4042', N'I', N'I WYDZIA£ CYWILNY', N'4042@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (653, N'4042', N'II', N'II WYDZIA£ KARNY', N'4042@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (654, N'4042', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4042@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (655, N'4042', N'IV', N'IV WYDZIA£ PRACY', N'4042@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (656, N'4042', N'V', N'OZ WÊGORZEWO V WYDZIA£ KARNY', N'4042@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (657, N'4042', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4042@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (658, N'4042', N'VII', N'OZ WÊGORZEWO VII WYDZ KSI¥G WIECZYSTYCH', N'4042@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (659, N'4043', N'I', N'I WYDZIA£ CYWILNY', N'4043@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (660, N'4043', N'II', N'II WYDZIA£ KARNY', N'4043@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (661, N'4043', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4043@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (662, N'4043', N'IV', N'IV WYDZIA£ PRACY', N'4043@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (663, N'4043', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4043@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (664, N'4044', N'I', N'I WYDZIA£ CYWILNY', N'4044@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (665, N'4044', N'II', N'II WYDZIA£ KARNY', N'4044@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (666, N'4044', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4044@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (667, N'4044', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4044@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (668, N'4044', N'V', N'OZ BISKUPIEC V WYDZIA£ CYWILNY', N'4044@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (669, N'4044', N'VI', N'OZ BISKUPIEC VI WYDZIA£ KARNY', N'4044@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (670, N'4044', N'VII', N'OZ BISKUPIEC VII WYDZ. RODZ. I NIELET.', N'4044@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (671, N'4044', N'VIII', N'OZ BISKUPIEC VIII WYDZIA£ PRACY', N'4044@VIII', N'SPPR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (672, N'4044', N'IX', N'OZ BISKUPIEC IX WYDZ KSI¥G WIECZYSTYCH', N'4044@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (673, N'4045', N'I', N'I WYDZIA£ CYWILNY', N'4045@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (674, N'4045', N'II', N'II WYDZIA£ KARNY', N'4045@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (675, N'4045', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4045@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (676, N'4045', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4045@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (677, N'4045', N'V', N'V WYDZIA£ GOSPODARCZY', N'4045@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (678, N'4045', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4045@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (679, N'4045', N'VII', N'VII WYDZIA£ KARNY', N'4045@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (680, N'4045', N'VIII', N'VIII WYDZIA£ GOSPODARCZY KRS', N'4045@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (681, N'4045', N'IX', N'IX WYDZIA£ KARNY', N'4045@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (682, N'4045', N'X', N'X WYDZIA£ CYWILNY', N'4045@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (683, N'4045', N'XI', N'OZ NIDZICA XI WYDZIA£ CYWILNY', N'4045@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (684, N'4045', N'XII', N'OZ NIDZICA XII WYDZIA£ KARNY', N'4045@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (685, N'4045', N'XIII', N'OZ NIDZICA XIII WYDZ. RODZ. I NIELETNICH', N'4045@XIII', N'SROD', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (686, N'4045', N'XIV', N'OZ NIDZICA XIV WYDZ KSI¥G WIECZYSTYCH', N'4045@XIV', N'SCYW', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (687, N'4046', N'I', N'I WYDZIA£ CYWILNY', N'4046@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (688, N'4046', N'II', N'II WYDZIA£ KARNY', N'4046@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (689, N'4046', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4046@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (690, N'4046', N'IV', N'IV WYDZIA£ PRACY', N'4046@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (691, N'4046', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4046@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (692, N'4046', N'VI', N'OZ PISZ VI WYDZIA£ CYWILNY', N'4046@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (693, N'4046', N'VII', N'OZ PISZ VII WYDZIA£ KARNY', N'4046@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (694, N'4046', N'VIII', N'OZ PISZ VIII WYDZ. RODZ. I NIELETNICH', N'4046@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (695, N'4046', N'IX', N'OZ PISZ IX WYDZ KSI¥G WIECZYSTYCH', N'4046@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (696, N'4047', N'I', N'I WYDZIA£ CYWILNY', N'4047@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (697, N'4047', N'II', N'II WYDZIA£ KARNY', N'4047@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (698, N'4047', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4047@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (699, N'4047', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4047@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (700, N'4047', N'V', N'V WYDZIA£ GOSPODARCZY', N'4047@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (701, N'4047', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4047@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (702, N'4048', N'I', N'I WYDZIA£ CYWILNY', N'4048@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (703, N'4048', N'II', N'II WYDZIA£ KARNY', N'4048@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (704, N'4048', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4048@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (705, N'4048', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4048@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (706, N'4049', N'I', N'I WYDZIA£ CYWILNY', N'4049@I', N'SCYW', 1)
GO
print 'Processed 700 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (707, N'4049', N'II', N'II WYDZIA£ KARNY', N'4049@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (708, N'4049', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4049@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (709, N'4049', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4049@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (710, N'4050', N'I', N'I WYDZIA£ CYWILNY', N'4050@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (711, N'4050', N'II', N'II WYDZIA£ KARNY', N'4050@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (712, N'4050', N'II.1', N'SEKCJA WYKONAWCZA II WYDZIA£ KARNY', N'4050@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (713, N'4050', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4050@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (714, N'4050', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4050@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (715, N'4050', N'V', N'OZ PU£TUSK V WYDZIA£ CYWILNY', N'4050@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (716, N'4050', N'VI', N'OZ PU£TUSK VI WYDZIA£ KARNY', N'4050@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (717, N'4050', N'VII', N'OZ PU£TUSK VII WYDZ. RODZ. I NIELETNICH', N'4050@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (718, N'4050', N'VIII', N'OZ PU£TUSK VIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4050@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (719, N'4051', N'I', N'I WYDZIA£ CYWILNY', N'4051@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (720, N'4051', N'II', N'II WYDZIA£ KARNY', N'4051@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (721, N'4051', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4051@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (722, N'4051', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4051@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (723, N'4051', N'V', N'OZ SEJNY V WYDZIA£ CYWILNY', N'4051@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (724, N'4051', N'VI', N'OZ SEJNY VI WYDZIA£ KARNY', N'4051@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (725, N'4051', N'VII', N'OZ SEJNY VII WYDZ. RODZ. I NIELETNICH', N'4051@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (726, N'4051', N'VIII', N'OZ SEJNY VIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4051@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (727, N'4052', N'I', N'I WYDZIA£ CYWILNY', N'4052@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (728, N'4052', N'II', N'II WYDZIA£ KARNY', N'4052@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (729, N'4052', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4052@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (730, N'4052', N'IV', N'IV WYDZIA£ PRACY', N'4052@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (731, N'4052', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4052@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (732, N'4053', N'I', N'I WYDZIA£ CYWILNY', N'4053@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (733, N'4053', N'II', N'II WYDZIA£ KARNY', N'4053@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (734, N'4053', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4053@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (735, N'4053', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4053@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (736, N'4054', N'I', N'I WYDZIA£ CYWILNY', N'4054@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (737, N'4054', N'II', N'II WYDZIA£ KARNY', N'4054@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (738, N'4054', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4054@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (739, N'4054', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4054@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (740, N'4054', N'V', N'V WYDZIA£ GOSPODARCZY', N'4054@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (741, N'4054', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4054@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (742, N'4054', N'VII', N'VII WYDZIA£ KARNY', N'4054@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (743, N'4055', N'I', N'I WYDZIA£ CYWILNY', N'4055@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (744, N'4055', N'II', N'II WYDZIA£ CYWILNY', N'4055@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (745, N'4055', N'III', N'III WYDZIA£ KARNY', N'4055@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (746, N'4055', N'IV', N'IV WYDZIA£ KARNY', N'4055@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (747, N'4055', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4055@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (748, N'4055', N'VI', N'VI WYDZIA£ RODZINNY I NIELETNICH', N'4055@VI', N'SROD', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (749, N'4055', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4055@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (750, N'4055', N'VIII', N'VIII WYDZIA£ GOSPODARCZY', N'4055@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (751, N'4055', N'X', N'X WYDZIA£ KSI¥G WIECZYSTYCH', N'4055@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (752, N'4055', N'XI', N'XI WYDZIA£ KARNY', N'4055@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (753, N'4055', N'XII', N'XII WYDZIA£ CYWILNY', N'4055@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (754, N'4055', N'XIII', N'XIII WYDZIA£ GOSPODARCZY KRS', N'4055@XIII', N'SGOS', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (755, N'4055', N'XIV', N'XIV WYDZIA£ KARNY', N'4055@XIV', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (756, N'4055', N'XV', N'XV WYDZIA£ GOSPODARCZY', N'4055@XV', N'SGOS', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (757, N'4055', N'XVI', N'XVI WYDZIA£ KARNY', N'4055@XVI', N'SKAR', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (758, N'4056', N'I', N'I WYDZIA£ CYWILNY', N'4056@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (759, N'4056', N'II', N'II WYDZIA£ KARNY', N'4056@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (760, N'4056', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4056@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (761, N'4056', N'IV', N'IV WYDZIA£ PRACY', N'4056@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (762, N'4056', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4056@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (763, N'4056', N'VI', N'VI WYDZIA£ KARNY', N'4056@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (764, N'4056', N'VII', N'OZ MOGILNO VII WYDZIA£ CYWILNY', N'4056@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (765, N'4056', N'VIII', N'OZ MOGILNO VIII WYDZIA£ KARNY', N'4056@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (766, N'4056', N'IX', N'OZ MOGILNO IX WYDZIA£ RODZ. I NIELETNICH', N'4056@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (767, N'4056', N'X', N'OZ MOGILNO X WYDZIA£ KSI¥G WIECZYSTYCH', N'4056@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (768, N'4057', N'I', N'I WYDZIA£ CYWILNY', N'4057@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (769, N'4057', N'II', N'II WYDZIA£ KARNY', N'4057@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (770, N'4057', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4057@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (771, N'4057', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4057@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (772, N'4057', N'V', N'OZ NAK£O NAD NOTECI¥ V WYDZIA£ CYWILNY', N'4057@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (773, N'4057', N'VI', N'OZ NAK£O NAD NOTECI¥ VI WYDZIA£ KARNY', N'4057@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (774, N'4057', N'VII', N'OZ NAK£O VII WYDZ. RODZ. I NIELETNICH', N'4057@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (775, N'4057', N'VIII', N'OZ NAK£O VIII WYDZ. KSI¥G WIECZYSTYCH', N'4057@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (776, N'4057', N'IX', N'OZ ¯NIN IX WYDZIA£ CYWILNY', N'4057@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (777, N'4057', N'X', N'OZ ¯NIN X WYDZIA£ KARNY', N'4057@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (778, N'4057', N'XI', N'OZ ¯NIN XI WYDZIA£ RODZ. I NIELETNICH', N'4057@XI', N'SROD', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (779, N'4057', N'XII', N'OZ ZNIN XII WYDZIA£ KSI¥G WIECZYSTYCH', N'4057@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (780, N'4058', N'I', N'I WYDZIA£ CYWILNY', N'4058@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (781, N'4058', N'II', N'II WYDZIA£ KARNY', N'4058@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (782, N'4058', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4058@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (783, N'4058', N'IV', N'IV WYDZIA£ S¥DU PRACY', N'4058@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (784, N'4058', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4058@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (785, N'4058', N'VI', N'OZ TUCHOLA VI WYDZIA£ CYWILNY', N'4058@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (786, N'4058', N'VII', N'OZ TUCHOLA VII WYDZIA£ KARNY', N'4058@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (787, N'4058', N'VIII', N'OZ TUCHOLA VIII WYDZ. RODZ. I NIELETNICH', N'4058@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (788, N'4058', N'IX', N'OZ TUCHOLA IX WYDZ. KSI¥G WIECZYSTYCH', N'4058@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (789, N'4058', N'X', N'OZ SÊPÓLNO KRAJEÑSKIE X WYDZ.KSI¥G WIECZ', N'4058@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (790, N'4059', N'I', N'I WYDZIA£ CYWILNY', N'4059@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (791, N'4059', N'II', N'II WYDZIA£ KARNY', N'4059@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (792, N'4059', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4059@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (793, N'4059', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4059@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (794, N'4060', N'I', N'I WYDZIA£ CYWILNY', N'4060@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (795, N'4060', N'II', N'II WYDZIA£ KARNY', N'4060@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (796, N'4060', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4060@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (797, N'4060', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4060@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (798, N'4060', N'V', N'V WYDZIA£ GOSPODARCZY', N'4060@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (799, N'4060', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4060@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (800, N'4060', N'VII', N'VII WYDZIA£ WYKONAWANIA ORZECZEÑ S¥D.', N'4060@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (801, N'4060', N'VIII', N'VIII WYDZIA£ KARNY', N'4060@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (802, N'4060', N'IX', N'IX WYDZIA£ CYWILNY', N'4060@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (803, N'4061', N'I', N'I WYDZIA£ CYWILNY', N'4061@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (804, N'4061', N'II', N'II WYDZIA£ KARNY', N'4061@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (805, N'4061', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4061@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (806, N'4061', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4061@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (807, N'4061', N'VI', N'OZ NOWE MIASTO LUBAWSKIE VI WYDZ.CYWILNY', N'4061@VI', N'SCYW', 6)
GO
print 'Processed 800 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (808, N'4061', N'VII', N'OZ NOWE MIASTO LUBAWSKIE VII WYDZ.KARNY', N'4061@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (809, N'4061', N'VIII', N'OZ VIII WYDZ.RODZ. I NIELET.', N'4061@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (810, N'4061', N'IX', N'OZ IX WYDZ. KSI¥G WIECZ.', N'4061@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (811, N'4062', N'I', N'I WYDZIA£ CYWILNY', N'4062@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (812, N'4062', N'II', N'II WYDZIA£ KARNY', N'4062@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (813, N'4062', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4062@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (814, N'4062', N'IV', N'IV WYDZIA£ PRACY', N'4062@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (815, N'4062', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4062@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (816, N'4062', N'VI', N'OZ MOR¥G VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4062@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (817, N'4062', N'VII', N'OZ MOR¥G VII WYDZIA£ KARNY', N'4062@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (818, N'4063', N'I', N'I WYDZIA£ CYWILNY', N'4063@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (819, N'4063', N'II', N'II WYDZIA£ KARNY', N'4063@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (820, N'4063', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4063@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (821, N'4063', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4063@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (822, N'4063', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4063@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (823, N'4063', N'XI', N'XI WYDZIA£ WYKONAWCZY', N'4063@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (824, N'4063', N'VIII', N'VIII WYDZIA£ KARNY', N'4063@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (825, N'4063', N'IX', N'IX WYDZIA£ CYWILNY', N'4063@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (826, N'4063', N'X', N'X WYDZIA£ KARNY', N'4063@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (827, N'4063', N'XII', N'XII WYDZIA£ CYWILNY', N'4063@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (828, N'4063', N'XII.1', N'SEKCJA EGZEKUCYJNA PRZY XII WYDZ.CYWILNY', N'4063@XII.1', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (829, N'4064', N'I', N'I WYDZIA£ CYWILNY', N'4064@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (830, N'4064', N'II', N'II WYDZIA£ KARNY', N'4064@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (831, N'4064', N'III', N'III WYDZIA£ KSI¥G WIECZYSTYCH', N'4064@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (832, N'4064', N'IV', N'IV WYDZIA£ GOSPODARCZY', N'4064@IV', N'SGOS', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (833, N'4064', N'V', N'V WYDZIA£ GOSPODARCZY', N'4064@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (834, N'4064', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4064@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (835, N'4064', N'VII', N'VII WYDZIA£ GOSPODARCZY KRS', N'4064@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (836, N'4064', N'VIII', N'VIII WYDZIA£ GOSPODARCZY KRS', N'4064@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (837, N'4064', N'IX', N'IX WYDZIA£ GOSPODARCZY REJESTR ZASTAWÓW', N'4064@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (838, N'4064', N'XI', N'XI WYDZIA£ KARNY', N'4064@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (839, N'4064', N'XII', N'XII WYDZIA£ WYKONAWCZY', N'4064@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (840, N'4064', N'XIII', N'XIII WYDZIA£ CYWILNY', N'4064@XIII', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (841, N'4064', N'XIII.1', N'SEKCJA EGZEKUCYJNA PRZY XIII WYDZ.CYWILNY', N'4064@XIII.1', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (842, N'4064', N'I.1', N'SEKCJA DS. UPROSZCZ. PRZY I WYDZ.CYWILNYM', N'4064@I.1', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (843, N'4065', N'I', N'I WYDZIA£ CYWILNY', N'4065@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (844, N'4065', N'I.1', N'SEKCJA DS. UPROSZCZONYCH', N'4065@I.1', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (845, N'4065', N'II', N'II WYDZIA£ KARNY', N'4065@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (846, N'4065', N'II.1', N'SEKCJA DS. WYKROCZENIOWYCH', N'4065@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (847, N'4065', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4065@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (848, N'4065', N'IV', N'IV WYDZIA£ PRACY', N'4065@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (849, N'4065', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4065@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (850, N'4065', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4065@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (851, N'4065', N'VII', N'VII WYDZIA£ CYWILNY', N'4065@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (852, N'4065', N'IX', N'IX WYDZIA£ KARNY', N'4065@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (853, N'4065', N'X', N'X WYDZIA£ WYKONAWCZY', N'4065@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (854, N'4066', N'I', N'I WYDZIA£ CYWILNY', N'4066@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (855, N'4066', N'II', N'II WYDZIA£ KARNY', N'4066@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (856, N'4066', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4066@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (857, N'4066', N'IV', N'IV WYDZIA£ PRACY', N'4066@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (858, N'4066', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4066@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (859, N'4066', N'VI', N'OZ KOŒCIERZYNA VI WYDZIA£ CYWILNY', N'4066@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (860, N'4066', N'VII', N'OZ KOŒCIERZYNA VII WYDZIA£ KARNY', N'4066@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (861, N'4066', N'VIII', N'OZ KOŒCIERZYNA VIII WYDZ.RODZ. I NIELET.', N'4066@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (862, N'4066', N'IX', N'OZ KOŒCIERZYNA IX WYDZ. KSI¥G WIECZ.', N'4066@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (863, N'4067', N'I', N'I WYDZIA£ CYWILNY', N'4067@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (864, N'4067', N'II', N'II WYDZIA£ KARNY', N'4067@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (865, N'4067', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4067@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (866, N'4067', N'IV', N'IV WYDZIA£ PRACY', N'4067@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (867, N'4067', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4067@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (868, N'4067', N'VIII', N'OZ SZTUM VIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4067@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (869, N'4068', N'I', N'I WYDZIA£ CYWILNY', N'4068@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (870, N'4068', N'II', N'II WYDZIA£ KARNY', N'4068@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (871, N'4068', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4068@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (872, N'4068', N'IV', N'IV WYDZIA£ PRACY', N'4068@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (873, N'4068', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4068@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (874, N'4068', N'VII', N'VII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4068@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (875, N'4068', N'IX', N'OZ NOWY DWÓR GDAÑSKI IX WYDZ.KS.WIECZ.', N'4068@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (876, N'4069', N'I', N'I WYDZIA£ CYWILNY', N'4069@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (877, N'4069', N'II', N'II WYDZIA£ KARNY', N'4069@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (878, N'4069', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4069@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (879, N'4069', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4069@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (880, N'4070', N'I', N'I WYDZIA£ CYWILNY', N'4070@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (881, N'4070', N'II', N'II WYDZIA£ KARNY', N'4070@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (882, N'4070', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4070@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (883, N'4070', N'IV', N'IV WYDZIA£ PRACY', N'4070@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (884, N'4070', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4070@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (885, N'4071', N'I', N'I WYDZIA£ CYWILNY', N'4071@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (886, N'4071', N'II', N'II WYDZIA£ KARNY', N'4071@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (887, N'4071', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4071@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (888, N'4071', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4071@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (889, N'4071', N'VI', N'VI WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4071@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (890, N'4072', N'I', N'I WYDZIA£ CYWILNY', N'4072@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (891, N'4072', N'II', N'II WYDZIA£ KARNY', N'4072@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (892, N'4072', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4072@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (893, N'4072', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4072@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (894, N'4072', N'V', N'OZ PUCK V WYDZIA£ KSI¥G WIECZYSTYCH', N'4072@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (895, N'4072', N'IX', N'OZ PUCK IX WYDZIA£ KARNY', N'4072@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (896, N'4073', N'I', N'I WYDZIA£ CYWILNY', N'4073@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (897, N'4073', N'II', N'II WYDZIA£ KARNY', N'4073@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (898, N'4073', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4073@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (899, N'4073', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4073@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (900, N'4074', N'I', N'I WYDZIA£ CYWILNY', N'4074@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (901, N'4074', N'II', N'II WYDZIA£ KARNY', N'4074@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (902, N'4074', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4074@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (903, N'4074', N'IV', N'IV WYDZIA£ PRACY', N'4074@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (904, N'4074', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4074@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (905, N'4075', N'I', N'I WYDZIA£ CYWILNY', N'4075@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (906, N'4075', N'II', N'II WYDZIA£ KARNY', N'4075@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (907, N'4075', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4075@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (908, N'4075', N'IV', N'IV WYDZIA£ PRACY', N'4075@IV', N'SPPR', 4)
GO
print 'Processed 900 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (909, N'4075', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4075@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (910, N'4075', N'VI', N'OZ BYTÓW VI WYDZIA£ CYWILNY', N'4075@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (911, N'4075', N'VII', N'OZ BYTÓW VII WYDZIA£ KARNY', N'4075@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (912, N'4075', N'VIII', N'OZ BYTÓW VIII WYDZ.RODZINNY I NIELETNICH', N'4075@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (913, N'4075', N'IX', N'OZ BYTÓW IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4075@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (914, N'4076', N'I', N'I WYDZIA£ CYWILNY', N'4076@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (915, N'4076', N'II', N'II WYDZIA£ KARNY', N'4076@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (916, N'4076', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4076@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (917, N'4076', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4076@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (918, N'4076', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4076@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (919, N'4076', N'VII', N'VII WYDZIA£ KSI¥G WIECZYSTYCH', N'4076@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (920, N'4076', N'IX', N'IX WYDZIA£ CYWILNY', N'4076@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (921, N'4076', N'XIV', N'XIV WYDZIA£ KARNY', N'4076@XIV', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (922, N'4076', N'XIV.1', N'SEKCJA DS. WYKROCZENIOWYCH', N'4076@XIV.1', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (923, N'4076', N'XV', N'OZ MIASTKO XV WYDZIA£ CYWILNY', N'4076@XV', N'SCYW', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (924, N'4076', N'XVI', N'OZ MIASTKO XVI WYDZIA£ KARNY', N'4076@XVI', N'SKAR', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (925, N'4076', N'XVII', N'OZ MIASTKO XVII WYDZIA£ RODZINNY I NIEL', N'4076@XVII', N'SROD', 17)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (926, N'4076', N'XVIII', N'OZ MIASTKO XVIII WYDZIA£ KSI¥G WIECZ', N'4076@XVIII', N'SCYW', 18)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (927, N'4077', N'I', N'I WYDZIA£ CYWILNY', N'4077@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (928, N'4077', N'II', N'II WYDZIA£ KARNY', N'4077@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (929, N'4077', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4077@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (930, N'4077', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4077@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (931, N'4077', N'VI', N'OZ GOLUB-DOBRZYÑ VI WYDZIA£ CYWILNY', N'4077@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (932, N'4077', N'VII', N'OZ GOLUB-BOBRZYÑ VII WYDZIA£ KARNY', N'4077@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (933, N'4077', N'VIII', N'OZ GOLUB-DOBRZYÑ VIII WYDZ. RODZ. I NIEL', N'4077@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (934, N'4077', N'IX', N'OZ GOLUB-DOBRZYÑ IX WYDZIA£ KSI¥G WIECZ', N'4077@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (935, N'4078', N'I', N'I WYDZIA£ CYWILNY', N'4078@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (936, N'4078', N'II', N'II WYDZIA£ KARNY', N'4078@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (937, N'4078', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4078@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (938, N'4078', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4078@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (939, N'4078', N'VI', N'OZ W¥BRZENO VI WYDZIA£ CYWILNY', N'4078@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (940, N'4078', N'VII', N'OZ W¥BRZENO VII WYDZIA£ KARNY', N'4078@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (941, N'4078', N'VIII', N'OZ W¥BRZENO VIII WYDZ. RODZ. I NIELET', N'4078@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (942, N'4078', N'IX', N'OZ W¥BRZENO IX WYDZIA£ KSI¥G WIECZ', N'4078@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (943, N'4079', N'I', N'I WYDZIA£ CYWILNY', N'4079@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (944, N'4079', N'II', N'II WYDZIA£ KARNY', N'4079@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (945, N'4079', N'III', N'III WYDZIA£ RODZINNY', N'4079@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (946, N'4079', N'IV', N'IV WYDZIA£ PRACY', N'4079@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (947, N'4079', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4079@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (948, N'4079', N'VII', N'VII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4079@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (949, N'4080', N'I', N'I WYDZIA£ CYWILNY', N'4080@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (950, N'4080', N'II', N'II WYDZIA£ KARNY', N'4080@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (951, N'4080', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4080@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (952, N'4080', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4080@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (953, N'4080', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4080@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (954, N'4080', N'VII', N'VII WYDZIA£ GOSPODARCZY KRS', N'4080@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (955, N'4080', N'VIII', N'VIII WYDZIA£ KARNY', N'4080@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (956, N'4080', N'IX', N'IX WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4080@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (957, N'4080', N'X', N'X WYDZIA£ CYWILNY', N'4080@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (958, N'4080', N'XI', N'XI WYDZIA£ CYWILNY', N'4080@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (959, N'4080', N'XII', N'XII WYDZIA£ KARNY', N'4080@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (960, N'4080', N'V', N'V WYDZIA£ GOSPODARCZY', N'4080@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (961, N'4081', N'I', N'I WYDZIA£ CYWILNY', N'4081@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (962, N'4081', N'II', N'II WYDZIA£ KARNY', N'4081@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (963, N'4081', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4081@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (964, N'4081', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4081@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (965, N'4081', N'V', N'OZ RADZIEJOWICE V WYDZIA£ CYWILNY', N'4081@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (966, N'4081', N'VI', N'OZ RADZIEJOWICE VI WYDZIA£ KARNY', N'4081@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (967, N'4081', N'VII', N'OZ RADZIEJOWICE VII WYDZ.RODZ.I NIEL', N'4081@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (968, N'4081', N'VIII', N'OZ RADZIEJOWICE VIII WYDZ.KSI¥G WIECZ.', N'4081@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (969, N'4082', N'I', N'I WYDZIA£ CYWILNY', N'4082@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (970, N'4082', N'II', N'II WYDZIA£ KARNY', N'4082@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (971, N'4082', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4082@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (972, N'4082', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4082@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (973, N'4082', N'VI', N'OZ RYPIN VI WYDZIA£ CYWILNY', N'4082@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (974, N'4082', N'VII', N'OZ RYPIN VII WYDZIA£ KARNY', N'4082@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (975, N'4082', N'VIII', N'OZ RYPIN VIII WYDZIA£ RODZINNY I NIELET', N'4082@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (976, N'4082', N'IX', N'OZ RYPIN IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4082@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (977, N'4083', N'I', N'I WYDZIA£ CYWILNY', N'4083@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (978, N'4083', N'II', N'II WYDZIA£ KARNY', N'4083@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (979, N'4083', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4083@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (980, N'4083', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4083@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (981, N'4083', N'V', N'V WYDZIA£ GOSPODARCZY', N'4083@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (982, N'4083', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4083@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (983, N'4084', N'I', N'I WYDZIA£ CYWILNY', N'4084@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (984, N'4084', N'II', N'II WYDZIA£ CYWILNY', N'4084@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (985, N'4084', N'III', N'III WYDZIA£ KARNY', N'4084@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (986, N'4084', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4084@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (987, N'4084', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4084@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (988, N'4084', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4084@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (989, N'4084', N'VII', N'VII WYDZIA£ KSI¥G WIECZYSTYCH', N'4084@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (990, N'4084', N'VIII', N'VIII WYDZIA£ KRAJOWEGO REJESTRU S¥DOWEGO', N'4084@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (991, N'4084', N'IX', N'IX WYDZIA£ KARNY', N'4084@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (992, N'4084', N'X', N'X WYDZIA£ CYWILNY', N'4084@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (993, N'4085', N'I', N'I WYDZIA£ CYWILNY', N'4085@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (994, N'4085', N'II', N'II WYDZIA£ KARNY', N'4085@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (995, N'4085', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4085@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (996, N'4085', N'IV', N'IV WYDZIA£ PRACY', N'4085@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (997, N'4085', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4085@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (998, N'4086', N'I.P', N'I WYDZIA£ CYWILNY PROCESOWY', N'4086@I.P', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (999, N'4086', N'I.N', N'I WYDZIA£ CYWILNY NIEPROCESOWY', N'4086@I.N', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1000, N'4086', N'II', N'II WYDZIA£ KARNY', N'4086@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1001, N'4086', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4086@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1002, N'4086', N'IV', N'IV WYDZIA£ PRACY', N'4086@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1003, N'4086', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4086@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1004, N'4087', N'I', N'I WYDZIA£ CYWILNY', N'4087@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1005, N'4087', N'II', N'II WYDZIA£ CYWILNY', N'4087@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1006, N'4087', N'III', N'III WYDZIA£ KARNY', N'4087@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1007, N'4087', N'IV', N'IV WYDZIA£ KARNY', N'4087@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1008, N'4087', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4087@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1009, N'4087', N'VI', N'VI WYDZIA£ RODZINNY I NIELETNICH', N'4087@VI', N'SROD', 6)
GO
print 'Processed 1000 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1010, N'4087', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4087@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1011, N'4087', N'VIII', N'VIII WYDZIA£ GOSPODARCZY', N'4087@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1012, N'4087', N'IX', N'IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4087@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1013, N'4087', N'X', N'OZ K£OBUCK X WYDZIA£ KSI¥G WIECZYSTYCH', N'4087@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1014, N'4087', N'XI', N'XI WYDZIA£ KARNY', N'4087@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1015, N'4087', N'XII', N'XII WYDZIA£ CYWILNY', N'4087@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1016, N'4087', N'XIII', N'OZ K£OBUCK XIII WYDZIA£ KARNY', N'4087@XIII', N'SKAR', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1017, N'4087', N'XIV', N'XIV WYDZIA£ KARNY WYKONAWCZY', N'4087@XIV', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1018, N'4087', N'XV', N'XV WYDZIA£ CYWILNY WYKONAWCZY', N'4087@XV', N'SKAR', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1019, N'4087', N'XVI', N'XVI WYDZIA£ KARNY', N'4087@XVI', N'SKAR', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1020, N'4087', N'XVII', N'XVII WYDZIA£ GOSPODARCZY KRS', N'4087@XVII', N'SGOS', 17)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1021, N'4088', N'I', N'I WYDZIA£ CYWILNY', N'4088@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1022, N'4088', N'II', N'II WYDZIA£ KARNY', N'4088@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1023, N'4088', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4088@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1024, N'4088', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4088@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1025, N'4089', N'I', N'I WYDZIA£ CYWILNY', N'4089@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1026, N'4089', N'II', N'II WYDZIA£ KARNY', N'4089@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1027, N'4089', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4089@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1028, N'4089', N'IV', N'IV WYDZIA£ S¥DU PRACY', N'4089@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1029, N'4089', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4089@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1030, N'4089', N'VI', N'VI WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4089@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1031, N'4090', N'I', N'I WYDZIA£ CYWILNY', N'4090@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1032, N'4090', N'II', N'II WYDZIA£ KARNY', N'4090@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1033, N'4090', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4090@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1034, N'4090', N'IV', N'IV WYDZIA£ PRACY', N'4090@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1035, N'4090', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4090@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1036, N'4090', N'VII', N'VII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4090@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1037, N'4091', N'I', N'I WYDZIA£ CYWILNY', N'4091@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1038, N'4091', N'II', N'II WYDZIA£ CYWILNY', N'4091@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1039, N'4091', N'III', N'III WYDZIA£ KARNY', N'4091@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1040, N'4091', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4091@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1041, N'4091', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4091@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1042, N'4091', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4091@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1043, N'4091', N'VII', N'VII WYDZIA£ GOSPODARCZY', N'4091@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1044, N'4091', N'VIII', N'VIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4091@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1045, N'4091', N'IX', N'IX WYDZIA£ KARNY', N'4091@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1046, N'4091', N'X', N'X WYDZIA£ GOSPODARCZY KRS', N'4091@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1047, N'4091', N'XII', N'XII WYDZIA£ GOSP. DS.UPAD£.I NAPRAW.', N'4091@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1048, N'4091', N'XIII', N'XIII WYDZIA£ WYKON. ORZECZEÑ KARNYCH', N'4091@XIII', N'SKAR', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1049, N'4092', N'I', N'I WYDZIA£ CYWILNY', N'4092@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1050, N'4092', N'II', N'II WYDZIA£ KARNY', N'4092@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1051, N'4092', N'II.1', N'SEKCJA WYKONAWCZA II WYDZIA£ KARNY', N'4092@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1052, N'4092', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4092@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1053, N'4092', N'IV', N'IV WYDZIA£ PRACY', N'4092@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1054, N'4092', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4092@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1055, N'4093', N'I', N'I WYDZIA£ CYWILNY', N'4093@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1056, N'4093', N'II', N'II WYDZIA£ KARNY', N'4093@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1057, N'4093', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4093@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1058, N'4093', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4093@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1059, N'4094', N'I', N'I WYDZIA£ CYWILNY', N'4094@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1060, N'4094', N'II', N'II WYDZIA£ KARNY', N'4094@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1061, N'4094', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4094@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1062, N'4094', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4094@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1063, N'4094', N'VI', N'VI WYDZIA£ KARNY', N'4094@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1064, N'4095', N'I', N'I WYDZIA£ CYWILNY', N'4095@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1065, N'4095', N'II', N'II WYDZIA£ CYWILNY', N'4095@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1066, N'4095', N'III', N'III WYDZIA£ KARNY', N'4095@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1067, N'4095', N'III.1', N'SEKCJA WYKONAWCZA III WYDZIA£ KARNY', N'4095@III.1', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1068, N'4095', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4095@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1069, N'4095', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4095@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1070, N'4095', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4095@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1071, N'4095', N'VII', N'VII WYDZIA£ KSI¥G WIECZYSTYCH', N'4095@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1072, N'4095', N'IX', N'IX WYDZIA£ KARNY', N'4095@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1073, N'4096', N'I', N'I WYDZIA£ CYWILNY', N'4096@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1074, N'4096', N'II', N'II WYDZIA£ KARNY', N'4096@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1075, N'4096', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4096@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1076, N'4096', N'IV', N'IV WYDZIA£ PRACY', N'4096@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1077, N'4096', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4096@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1078, N'4096', N'VI', N'OZ PIEKARY ŒL¥SKIE VI WYDZIA£ KARNY', N'4096@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1079, N'4097', N'I', N'I WYDZIA£ CYWILNY', N'4097@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1080, N'4097', N'II', N'II WYDZIA£ KARNY', N'4097@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1081, N'4097', N'III', N'III WYDZIA£ RODZINNY', N'4097@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1082, N'4097', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4097@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1083, N'4097', N'VI', N'VI WYDZIA£ KARNY', N'4097@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1084, N'4098', N'I', N'I WYDZIA£ CYWILNY', N'4098@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1085, N'4098', N'II', N'II WYDZIA£ KARNY', N'4098@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1086, N'4098', N'III', N'III WYDZIA£ RODZINNY', N'4098@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1087, N'4098', N'IV', N'IV WYDZIA£ PRACY', N'4098@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1088, N'4098', N'V', N'V WYDZIA£ WYKONYWANIA ORZECZEÑ KARNYCH', N'4098@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1089, N'4098', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4098@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1090, N'4098', N'VII', N'VII WYDZIA£ KARNY', N'4098@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1091, N'4098', N'VIII', N'VIII WYDZIA£ CYWILNY', N'4098@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1092, N'4099', N'I', N'I WYDZIA£ CYWILNY', N'4099@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1093, N'4099', N'II', N'II WYDZIA£ KARNY', N'4099@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1094, N'4099', N'II.1', N'SEKCJA WYKONAWCZA II KARNEGO', N'4099@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1095, N'4099', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4099@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1096, N'4099', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4099@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1097, N'4100', N'I', N'I WYDZIA£ CYWILNY', N'4100@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1098, N'4100', N'II', N'II WYDZIA£ KARNY', N'4100@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1099, N'4100', N'III', N'III WYDZIA£ RODZINNY', N'4100@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1100, N'4100', N'IV', N'IV WYDZIA£ PRACY', N'4100@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1101, N'4100', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4100@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1102, N'4100', N'VII', N'VII WYDZIA£ KARNY', N'4100@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1103, N'4100', N'VIII', N'VIII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4100@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1104, N'4101', N'I', N'I WYDZIA£ CYWILNY', N'4101@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1105, N'4101', N'I.1', N'SEKCJA EGZEK. PRZY I WYDZIALE CYWILNYM', N'4101@I.1', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1106, N'4101', N'I.2', N'SEKCJA DS.UPROSZCZ.W I WYDZIALE CYWILNYM', N'4101@I.2', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1107, N'4101', N'II', N'II WYDZIA£ KARNY', N'4101@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1108, N'4101', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4101@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1109, N'4101', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4101@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1110, N'4101', N'V', N'V WYDZIA£ PRACY', N'4101@V', N'SPPR', 5)
GO
print 'Processed 1100 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1111, N'4101', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4101@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1112, N'4101', N'VII', N'VII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4101@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1113, N'4101', N'VIII', N'VIII WYDZIA£ KARNY', N'4101@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1114, N'4101', N'IX', N'IX WYDZIA£ RODZINNY I NIELETNICH', N'4101@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1115, N'4102', N'I', N'I WYDZIA£ CYWILNY', N'4102@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1116, N'4102', N'I.1', N'SEKCJA DS.UPROSZCZ.W I WYDZIALE CYWILNYM', N'4102@I.1', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1117, N'4102', N'II', N'II WYDZIA£ KARNY', N'4102@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1118, N'4102', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4102@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1119, N'4102', N'IV', N'IV WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4102@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1120, N'4102', N'V', N'V WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4102@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1121, N'4102', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4102@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1122, N'4102', N'VII', N'VII WYDZIA£ KARNY', N'4102@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1123, N'4103', N'I', N'I WYDZIA£ CYWILNY', N'4103@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1124, N'4103', N'II', N'II WYDZIA£ KARNY', N'4103@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1125, N'4103', N'III', N'III WYDZIA£ RODZINNY', N'4103@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1126, N'4103', N'IV', N'IV WYDZIA£ PRACY', N'4103@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1127, N'4103', N'V', N'V WYDZIA£ GOSPODARCZY', N'4103@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1128, N'4103', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4103@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1129, N'4103', N'VII', N'VII WYDZIA£ KARNY', N'4103@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1130, N'4103', N'VIII', N'VIII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4103@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1131, N'4104', N'I', N'I WYDZIA£ CYWILNY', N'4104@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1132, N'4104', N'II', N'II WYDZIA£ KARNY', N'4104@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1133, N'4104', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4104@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1134, N'4104', N'IV', N'IV WYDZIA£ PRACY', N'4104@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1135, N'4104', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4104@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1136, N'4105', N'I', N'I WYDZIA£ CYWILNY', N'4105@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1137, N'4105', N'I.1', N'SEKCJA EGZEK. PRZY I WYDZIALE CYWILNYM', N'4105@I.1', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1138, N'4105', N'I.2', N'SEKCJA DS.UPROSZCZ.W I WYDZIALE CYWILNYM', N'4105@I.2', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1139, N'4105', N'II', N'II WYDZIA£ CYWILNY', N'4105@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1140, N'4105', N'III', N'III WYDZIA£ KARNY', N'4105@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1141, N'4105', N'IV', N'IV WYDZIA£ KARNY', N'4105@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1142, N'4105', N'V', N'V WYDZIA£ KARNY', N'4105@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1143, N'4105', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4105@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1144, N'4105', N'VII', N'VII WYDZIA£ GOSPODARCZY', N'4105@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1145, N'4105', N'VIII', N'VIII WYDZIA£ GOSPODARCZY KRS', N'4105@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1146, N'4105', N'IX', N'IX WYDZIA£ GOSPODARCZY RZ', N'4105@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1147, N'4105', N'X', N'X WYDZIA£ GOSPODARCZY', N'4105@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1148, N'4105', N'XI', N'XI WYDZIA£ KSI¥G WIECZYSTYCH', N'4105@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1149, N'4105', N'XII', N'XII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4105@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1150, N'4106', N'I', N'I WYDZIA£ CYWILNY', N'4106@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1151, N'4106', N'I.1', N'SEKCJA EGZEK. PRZY I WYDZIALE CYWILNYM', N'4106@I.1', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1152, N'4106', N'I.2', N'SEKCJA DS.UPROSZCZ W I WYDZIALE CYWILNYM', N'4106@I.2', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1153, N'4106', N'II', N'II WYDZIA£ CYWILNY', N'4106@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1154, N'4106', N'III', N'III WYDZIA£ KARNY', N'4106@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1155, N'4106', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4106@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1156, N'4106', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4106@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1157, N'4106', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4106@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1158, N'4106', N'VIII', N'VIII WYDZIA£ KARNY', N'4106@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1159, N'4106', N'X', N'X WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4106@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1160, N'4107', N'I', N'I WYDZIA£ CYWILNY', N'4107@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1161, N'4107', N'II', N'II WYDZIA£ KARNY', N'4107@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1162, N'4107', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4107@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1163, N'4107', N'IV', N'IV WYDZIA£ PRACY', N'4107@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1164, N'4107', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4107@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1165, N'4107', N'VI', N'VI WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4107@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1166, N'4108', N'I', N'I WYDZIA£ CYWILNY', N'4108@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1167, N'4108', N'II', N'II WYDZIA£ KARNY', N'4108@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1168, N'4108', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4108@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1169, N'4108', N'IV', N'IV WYDZIA£ PRACY', N'4108@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1170, N'4108', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4108@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1171, N'4108', N'VI', N'VI WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4108@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1172, N'4109', N'I', N'I WYDZIA£ CYWILNY', N'4109@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1173, N'4109', N'II', N'II WYDZIA£ KARNY', N'4109@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1174, N'4109', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4109@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1175, N'4109', N'IV', N'IV WYDZIA£ PRACY', N'4109@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1176, N'4109', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4109@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1177, N'4109', N'VI', N'VI WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4109@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1178, N'4110', N'I', N'I WYDZIA£ CYWILNY', N'4110@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1179, N'4110', N'II', N'II WYDZIA£ KARNY', N'4110@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1180, N'4110', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4110@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1181, N'4110', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4110@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1182, N'4111', N'I', N'I WYDZIA£ CYWILNY', N'4111@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1183, N'4111', N'II', N'II WYDZIA£ CYWILNY', N'4111@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1184, N'4111', N'III', N'III WYDZIA£ KARNY', N'4111@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1185, N'4111', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4111@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1186, N'4111', N'V', N'V WYDZIA£ PRACY I UBEZPIECZEÑ SPO£.', N'4111@V', N'SUBE', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1187, N'4111', N'IX', N'IX WYDZIA£ KARNY', N'4111@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1188, N'4111', N'X', N'X WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4111@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1189, N'4111', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4111@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1190, N'4112', N'I', N'I WYDZIA£ CYWILNY', N'4112@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1191, N'4112', N'II', N'II WYDZIA£ KARNY', N'4112@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1192, N'4112', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4112@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1193, N'4112', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4112@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1194, N'4112', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4112@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1195, N'4112', N'VI', N'VI WYDZIA£ GOSPODARCZY', N'4112@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1196, N'4112', N'VII', N'VII WYDZIA£ KARNY', N'4112@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1197, N'4112', N'VIII', N'VIII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4112@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1198, N'4112', N'IX', N'IX WYDZIA£ CYWILNY', N'4112@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1199, N'4113', N'I', N'I WYDZIA£ CYWILNY', N'4113@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1200, N'4113', N'II', N'II WYDZIA£ KARNY', N'4113@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1201, N'4113', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4113@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1202, N'4113', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4113@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1203, N'4113', N'V', N'OZ KAZIMIERZA WIELKA V WYDZIA£ CYWILNY', N'4113@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1204, N'4113', N'VI', N'OZ KAZIMIERZA WIELKA VI WYDZIA£ KARNY', N'4113@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1205, N'4113', N'VII', N'OZ KAZIMIERZA WIELKA VII WYDZ. RODZ.', N'4113@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1206, N'4113', N'VIII', N'OZ KAZIMIERZA WIELKA VIII WYDZ KSI¥G', N'4113@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1207, N'4113', N'IX', N'OZ PIÑCZÓW IX WYDZIA£ CYWILNY', N'4113@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1208, N'4113', N'X', N'OZ PIÑCZÓW X WYDZIA£ KARNY', N'4113@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1209, N'4113', N'XI', N'OZ PIÑCZÓW XI WYDZIA£ RODZINNY I NIELET', N'4113@XI', N'SROD', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1210, N'4113', N'XII', N'OZ PIÑCZÓW XII WYDZIA£ KSI¥G WIECZYSTYCH', N'4113@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1211, N'4114', N'I', N'I WYDZIA£ CYWILNY', N'4114@I', N'SCYW', 1)
GO
print 'Processed 1200 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1212, N'4114', N'II', N'II WYDZIA£ KARNY', N'4114@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1213, N'4114', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4114@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1214, N'4114', N'IV', N'IV WYDZIA£ S¥D PRACY', N'4114@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1215, N'4114', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4114@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1216, N'4114', N'VI', N'OZ W£OSZCZOWA VI WYDZIA£ CYWILNY', N'4114@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1217, N'4114', N'VII', N'OZ W£OSZCZOWA VII WYDZIA£ KARNY', N'4114@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1218, N'4114', N'VIII', N'OZ W£OSZCZOWA VIII WYDZ. RODZ. I NIELET', N'4114@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1219, N'4114', N'IX', N'OZ W£OSZCZOWA IX WYDZ KSI¥G WIECZYSTYCH', N'4114@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1220, N'4115', N'I', N'I WYDZIA£ CYWILNY', N'4115@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1221, N'4115', N'II', N'II WYDZIA£ KARNY', N'4115@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1222, N'4115', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4115@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1223, N'4115', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4115@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1224, N'4115', N'V', N'V WYDZIA£ GOSPODARCZY', N'4115@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1225, N'4115', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4115@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1226, N'4115', N'VII', N'VII WYDZIA£ CYWILNY', N'4115@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1227, N'4115', N'VIII', N'VIII WYDZIA£ CYWILNY', N'4115@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1228, N'4115', N'IX', N'IX WYDZIA£ KARNY', N'4115@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1229, N'4115', N'X', N'X WYDZIA£ KRS', N'4115@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1230, N'4115', N'XI', N'XI WYDZIA£ KARNY', N'4115@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1231, N'4115', N'XII', N'XII WYDZIA£ KARNY', N'4115@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1232, N'4115', N'XIII', N'XIII WYDZIA£ WYKONAWCZY', N'4115@XIII', N'SKAR', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1233, N'4116', N'I', N'I WYDZIA£ CYWILNY', N'4116@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1234, N'4116', N'II', N'II WYDZIA£ KARNY', N'4116@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1235, N'4116', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4116@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1236, N'4116', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4116@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1237, N'4117', N'I', N'I WYDZIA£ CYWILNY', N'4117@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1238, N'4117', N'II', N'II WYDZIA£ KARNY', N'4117@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1239, N'4117', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4117@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1240, N'4117', N'IV', N'IV WYDZIA£ PRACY', N'4117@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1241, N'4117', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4117@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1242, N'4117', N'VI', N'OZ OPATÓW VI WYDZIA£ CYWILNY', N'4117@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1243, N'4117', N'VII', N'OZ OPATÓW VII WYDZIA£ KARNY', N'4117@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1244, N'4117', N'VIII', N'OZ OPATÓW VIII WYDZ. RODZ. I NIELETNICH', N'4117@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1245, N'4117', N'IX', N'OZ OPATÓW IX WYDZ KSI¥G WIECZYSTYCH', N'4117@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1246, N'4118', N'I', N'I WYDZIA£ CYWILNY', N'4118@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1247, N'4118', N'II', N'II WYDZIA£ KARNY', N'4118@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1248, N'4118', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4118@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1249, N'4118', N'IV', N'IV WYDZIA£ PRACY', N'4118@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1250, N'4118', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4118@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1251, N'4118', N'VI', N'OZ STASZÓW VI WYDZIA£ CYWILNY', N'4118@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1252, N'4118', N'VII', N'OZ STASZÓW VII WYDZIA£ KARNY', N'4118@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1253, N'4118', N'VIII', N'OZ STASZÓW VIII WYDZ. RODZ. I NIELETNICH', N'4118@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1254, N'4118', N'IX', N'OZ STASZÓW IX WYDZ KSI¥G WIECZYSTYCH', N'4118@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1255, N'4119', N'I', N'I WYDZIA£ CYWILNY', N'4119@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1256, N'4119', N'II', N'II WYDZIA£ KARNY', N'4119@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1257, N'4119', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4119@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1258, N'4119', N'IV', N'IV WYDZIA£ PRACY', N'4119@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1259, N'4119', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4119@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1260, N'4120', N'I', N'I WYDZIA£ CYWILNY', N'4120@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1261, N'4120', N'II', N'II WYDZIA£ KARNY', N'4120@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1262, N'4120', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4120@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1263, N'4120', N'IV', N'IV WYDZIA£ S¥DU PRACY', N'4120@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1264, N'4120', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4120@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1265, N'4121', N'I', N'I WYDZIA£ CYWILNY', N'4121@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1266, N'4121', N'II', N'II WYDZIA£ KARNY', N'4121@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1267, N'4121', N'III', N'III WYDZIA£ RODZINNY', N'4121@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1268, N'4121', N'IV', N'IV WYDZIA£ PRACY', N'4121@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1269, N'4121', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4121@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1270, N'4122', N'I', N'I WYDZIA£ CYWILNY', N'4122@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1271, N'4122', N'II', N'II WYDZIA£ KARNY', N'4122@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1272, N'4122', N'III', N'III WYDZIA£ RODZINNY', N'4122@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1273, N'4122', N'IX', N'IX WYDZIA£ KARNY', N'4122@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1274, N'4122', N'VI', N'OZ KRZESZOWICE VI WYDZIA£ KSI¥G WIECZ', N'4122@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1275, N'4122', N'VII', N'OZ CZAERNIKÓW VII WYDZIA£ KSI¥G WIECZ', N'4122@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1276, N'4123', N'I', N'I WYDZIA£ CYWILNY', N'4123@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1277, N'4123', N'II', N'II WYDZIA£ KARNY', N'4123@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1278, N'4123', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4123@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1279, N'4123', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4123@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1280, N'4123', N'VIII', N'VIII WYDZIA£ KARNY', N'4123@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1281, N'4123', N'V', N'OZ PROSZOWICE V WYDZIA£ KSI¥G WIECZ', N'4123@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1282, N'4124', N'I', N'I WYDZIA£ CYWILNY', N'4124@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1283, N'4124', N'II', N'II WYDZIA£ KARNY', N'4124@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1284, N'4124', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4124@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1285, N'4124', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4124@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1286, N'4124', N'VIII', N'VIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4124@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1287, N'4124', N'XI', N'XI WYDZIA£ KARNY', N'4124@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1288, N'4125', N'I', N'I WYDZIA£ CYWILNY', N'4125@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1289, N'4125', N'II', N'II WYDZIA£ KARNY', N'4125@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1290, N'4125', N'III', N'III WYDZIA£ RODZINNY', N'4125@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1291, N'4125', N'IV', N'IV WYDZIA£ GOSPODARCZY', N'4125@IV', N'SGOS', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1292, N'4125', N'V', N'V WYDZIA£ GOSPODARCZY', N'4125@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1293, N'4125', N'VI', N'VI WYDZIA£ CYWILNY', N'4125@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1294, N'4125', N'VII', N'VII WYDZIA£ GOSPODARCZY REJESTRU ZAST', N'4125@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1295, N'4125', N'VIII', N'VIII WYDZIA£ GOSPODARCZY DS.UPAD£OŒ.-NAP', N'4125@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1296, N'4125', N'XI', N'XI WYDZIA£ GOSPODARCZY KRS', N'4125@XI', N'SGOS', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1297, N'4125', N'XII', N'XII WYDZIA£ GOSPODARCZY KRS', N'4125@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1298, N'4125', N'XIV', N'XIV WYDZIA£ KARNY', N'4125@XIV', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1299, N'4125', N'XV', N'OZ MIECHÓW XV WYDZIA£ CYWILNY', N'4125@XV', N'SCYW', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1300, N'4125', N'XVI', N'OZ MIECHÓW XVI WYDZIA£ KARNY', N'4125@XVI', N'SKAR', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1301, N'4125', N'XVII', N'OZ MIECHÓW XVII WYDZ. RODZ. I NIELET.', N'4125@XVII', N'SROD', 17)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1302, N'4125', N'IX', N'OZ S£OMNIKI IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4125@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1303, N'4125', N'XVIII', N'OZ MIECHÓW XVIII WYDZIA£ KSI¥G WIECZ', N'4125@XVIII', N'SCYW', 18)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1304, N'4126', N'I', N'I WYDZIA£ CYWILNY', N'4126@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1305, N'4126', N'II', N'II WYDZIA£ KARNY', N'4126@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1306, N'4126', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4126@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1307, N'4126', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4126@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1308, N'4126', N'V', N'OZ DOBCZYCE V WYDZIA£ KSI¥G WIECZYSTYCH', N'4126@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1309, N'4127', N'I', N'I WYDZIA£ CYWILNY', N'4127@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1310, N'4127', N'II', N'II WYDZIA£ KARNY', N'4127@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1311, N'4127', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4127@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1312, N'4127', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4127@V', N'SCYW', 5)
GO
print 'Processed 1300 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1313, N'4128', N'I', N'I WYDZIA£ CYWILNY', N'4128@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1314, N'4128', N'II', N'II WYDZIA£ KARNY', N'4128@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1315, N'4128', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4128@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1316, N'4128', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4128@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1317, N'4128', N'VI', N'OZ VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4128@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1318, N'4129', N'I', N'I WYDZIA£ CYWILNY', N'4129@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1319, N'4129', N'II', N'II WYDZIA£ KARNY', N'4129@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1320, N'4129', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4129@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1321, N'4129', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4129@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1322, N'4129', N'VI', N'OZ SUCHA BESKIDZKA VI WYDZIA£ CYWILNY', N'4129@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1323, N'4129', N'VII', N'OZ SUCHA BESKIDZKA VII WYDZIA£ KARNY', N'4129@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1324, N'4129', N'VIII', N'OZ SUCHA BESKIDZKA VIII WYDZ. RODZ', N'4129@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1325, N'4129', N'IX', N'OZ SUCHA BESKIDZKA IX WYDZ KSI¥G WIECZ', N'4129@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1326, N'4130', N'I', N'I WYDZIA£ CYWILNY', N'4130@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1327, N'4130', N'II', N'II WYDZIA£ KARNY', N'4130@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1328, N'4130', N'III', N'III WYDZIA£ KSI¥G WIECZYSTYCH', N'4130@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1329, N'4130', N'V', N'OZ SKAWINA V WYDZ KSI¥G WIECZYSTYCH', N'4130@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1330, N'4130', N'VII', N'OZ NIEPO£OMICE VII WYDZ KSI¥G WIECZ', N'4130@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1331, N'4131', N'I', N'I WYDZIA£ CYWILNY', N'4131@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1332, N'4131', N'II', N'II WYDZIA£ KARNY', N'4131@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1333, N'4131', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4131@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1334, N'4131', N'IV', N'IV WYDZIA£ PRACY', N'4131@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1335, N'4131', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4131@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1336, N'4132', N'I', N'I WYDZIA£ CYWILNY', N'4132@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1337, N'4132', N'II', N'II WYDZIA£ KARNY', N'4132@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1338, N'4132', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4132@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1339, N'4132', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4132@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1340, N'4132', N'VI', N'OZ MSZANA DOLNA VI WYDZIA£ KSI¥G WIECZ', N'4132@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1341, N'4133', N'I', N'I WYDZIA£ CYWILNY', N'4133@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1342, N'4133', N'II', N'II WYDZIA£ KARNY', N'4133@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1343, N'4133', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4133@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1344, N'4133', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4133@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1345, N'4133', N'V', N'V WYDZIA£ GOSPODARCZY', N'4133@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1346, N'4133', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4133@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1347, N'4133', N'VII', N'OZ MUSZYNA VII WYDZIA£ CYWILNY', N'4133@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1348, N'4133', N'VIII', N'OZ MUSZYNA VIII WYDZIA£ KARNY', N'4133@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1349, N'4133', N'IX', N'OZ MUSZYNA IX WYDZ. RODZ. I NIELETNICH', N'4133@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1350, N'4133', N'X', N'OZ MUSZYNA X WYDZ KSI¥G WIECZYSTYCH', N'4133@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1351, N'4134', N'I', N'I WYDZIA£ CYWILNY', N'4134@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1352, N'4134', N'II', N'II WYDZIA£ KARNY', N'4134@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1353, N'4134', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4134@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1354, N'4134', N'IV', N'IV WYDZIA£ PRACY', N'4134@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1355, N'4134', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4134@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1356, N'4135', N'I', N'I WYDZIA£ CYWILNY', N'4135@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1357, N'4135', N'II', N'II WYDZIA£ KARNY', N'4135@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1358, N'4135', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4135@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1359, N'4135', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4135@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1360, N'4136', N'I', N'I WYDZIA£ CYWILNY', N'4136@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1361, N'4136', N'II', N'II WYDZIA£ KARNY', N'4136@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1362, N'4136', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4136@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1363, N'4136', N'IV', N'IV WYDZIA£ PRACY', N'4136@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1364, N'4136', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4136@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1365, N'4137', N'I', N'I WYDZIA£ CYWILNY', N'4137@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1366, N'4137', N'II', N'II WYDZIA£ KARNY', N'4137@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1367, N'4137', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4137@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1368, N'4137', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4137@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1369, N'4138', N'I', N'I WYDZIA£ CYWILNY', N'4138@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1370, N'4138', N'II', N'II WYDZIA£ KARNY', N'4138@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1371, N'4138', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4138@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1372, N'4138', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4138@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1373, N'4138', N'V', N'V WYDZIA£ GOSPODARCZY', N'4138@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1374, N'4138', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4138@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1375, N'4138', N'IX', N'OZ TUCHÓW IX WYDZ KSI¥G WIECZYSTYCH', N'4138@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1376, N'4138', N'X', N'OZ TUCHÓW X WYDZIA£ CYWILNY', N'4138@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1377, N'4138', N'XI', N'XI WYDZIA£ WYKONAWCZY', N'4138@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1378, N'4138', N'XII', N'XII WYDZIA£  D¥BROWA TARNOWSKA', N'4138@XII', NULL, 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1381, N'4138', N'XIII', N'OZ D¥BROWA TARNOWSKA XIII WYDZIA£ KARNY', N'4138@XIII', N'SKAR', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1382, N'4138', N'XIV', N'OZ D¥BROWA TARNOWSKA XIV WYDZ. RODZ.', N'4138@XIV', N'SROD', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1383, N'4138', N'XV', N'', N'4138@XV', NULL, 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1384, N'OZ', N'D¥BROWA', N'TARNOWSKA XV WYDZ KSI¥GWIECZ', N'OZ@D¥BROWA', N'SCYW', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1385, N'4139', N'I', N'I WYDZIA£ CYWILNY', N'4139@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1386, N'4139', N'II', N'II WYDZIA£ KARNY', N'4139@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1387, N'4139', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4139@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1388, N'4139', N'IV', N'IV WYDZIA£ PRACY', N'4139@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1389, N'4139', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4139@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1390, N'4139', N'VII', N'VII WYDZIA£ KARNY', N'4139@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1391, N'4139', N'VIII', N'VIII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4139@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1392, N'4140', N'I', N'I WYDZIA£ CYWILNY', N'4140@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1393, N'4140', N'II', N'II WYDZIA£ KARNY', N'4140@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1394, N'4140', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4140@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1395, N'4140', N'IV', N'IV WYDZIA£ PRACY', N'4140@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1396, N'4140', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4140@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1397, N'4140', N'VII', N'VII WYDZIA£ KARNY', N'4140@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1398, N'4140', N'VIII', N'OZ W£ODAWA VIII WYDZIA£ CYWILNY', N'4140@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1399, N'4140', N'IX', N'OZ W£ODAWA IX WYDZIA£ KARNY', N'4140@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1400, N'4140', N'X', N'OZ W£ODAWA X WYDZ. RODZ. I NIELETNICH', N'4140@X', N'SROD', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1401, N'4140', N'XI', N'OZ W£ODAWA XI WYDZ KSI¥G WIECZYSTYCH', N'4140@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1402, N'4141', N'I', N'I WYDZIA£ CYWILNY', N'4141@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1403, N'4141', N'II', N'II WYDZIA£ KARNY', N'4141@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1404, N'4141', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4141@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1405, N'4141', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4141@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1406, N'4141', N'VI', N'OZ OPOLE LUBELSKIE VI WYDZIA£ CYWILNY', N'4141@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1407, N'4141', N'VII', N'OZ OPOLE LUBELSKIE VII WYDZIA£ KARNY', N'4141@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1408, N'4141', N'VIII', N'OZ OPOLE LUBELSKIE VIII WYDZ. RODZ.', N'4141@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1409, N'4141', N'IX', N'OZ OPOLE LUBELSKIE IX WYDZ KSI¥G WIECZ.', N'4141@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1410, N'4142', N'I', N'I WYDZIA£ CYWILNY', N'4142@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1411, N'4142', N'II', N'II WYDZIA£ KARNY', N'4142@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1412, N'4142', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4142@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1413, N'4142', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4142@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1414, N'4143', N'I', N'I WYDZIA£ CYWILNY', N'4143@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1415, N'4143', N'II', N'II WYDZIA£ KARNY', N'4143@II', N'SKAR', 2)
GO
print 'Processed 1400 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1416, N'4143', N'III', N'III WYDZIA£ KARNY', N'4143@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1417, N'4143', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4143@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1418, N'4143', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4143@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1419, N'4143', N'VI', N'VI WYDZIA£ GOSPODARCZY KRS', N'4143@VI', N'SGOS', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1420, N'4143', N'VII', N'VII WYDZIA£ GOSPODARCZY REJESTR ZASTAWÓW', N'4143@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1421, N'4143', N'VIII', N'VIII WYDZIA£ GOSPODARCZY', N'4143@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1422, N'4143', N'IX', N'IX WYDZIA£ GOSP. DS..UPAD£OŒCIOWYCH', N'4143@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1423, N'4144', N'I', N'I WYDZIA£ CYWILNY', N'4144@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1424, N'4144', N'II', N'II WYDZIA£ CYWILNY', N'4144@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1425, N'4144', N'III', N'III WYDZIA£ KARNY', N'4144@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1426, N'4144', N'IV', N'IV WYDZIA£ KARNY', N'4144@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1427, N'4144', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4144@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1428, N'4144', N'VI', N'VI WYDZIA£ CYWILNY', N'4144@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1429, N'4144', N'VII', N'VII WYDZ PRACY I UBEZPECZEÑ SPO£ECZNYCH', N'4144@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1430, N'4144', N'VIII', N'VIII WYDZIA£ CYWILNY', N'4144@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1431, N'4144', N'IX', N'IX WYDZIA£ KARNY', N'4144@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1432, N'4144', N'X', N'X WYDZIA£ KSI¥G WIECZYSTYCH', N'4144@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1433, N'4145', N'I', N'I WYDZIA£ CYWILNY', N'4145@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1434, N'4145', N'II', N'II WYDZIA£ KARNY', N'4145@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1435, N'4145', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4145@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1436, N'4145', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4145@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1437, N'4146', N'I', N'I WYDZIA£ CYWILNY', N'4146@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1438, N'4146', N'II', N'II WYDZIA£ KARNY', N'4146@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1439, N'4146', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4146@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1440, N'4146', N'IV', N'IV WYDZIA£ PRACY', N'4146@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1441, N'4146', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4146@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1442, N'4146', N'VI', N'OZ RYKI VI WYDZIA£ CYWILNY', N'4146@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1443, N'4146', N'VII', N'OZ RYKI VII WYDZIA£ KARNY', N'4146@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1444, N'4146', N'VIII', N'OZ RYKI VIII WYDZ. RODZ. I NIELETNICH', N'4146@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1445, N'4146', N'IX', N'OZ RYKI IX WYDZ KSI¥G WIECZYSTYCH', N'4146@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1446, N'4147', N'I', N'I WYDZIA£ CYWILNY', N'4147@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1447, N'4147', N'II', N'II WYDZIA£ KARNY', N'4147@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1448, N'4147', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4147@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1449, N'4147', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4147@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1450, N'4148', N'I', N'I WYDZIA£ CYWILNY', N'4148@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1451, N'4148', N'II', N'II WYDZIA£ KARNY', N'4148@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1452, N'4148', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4148@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1453, N'4148', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4148@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1454, N'4148', N'VI', N'VI WYDZIA£ KARNY', N'4148@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1455, N'4148', N'VIII', N'OZ BIA£OBRZEGI VIII WYDZ KSI¥G WIECZ.', N'4148@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1456, N'4149', N'I', N'I WYDZIA£ CYWILNY', N'4149@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1457, N'4149', N'II', N'II WYDZIA£ KARNY', N'4149@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1458, N'4149', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4149@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1459, N'4149', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4149@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1460, N'4149', N'VI', N'OZ LIPSK VI WYDZIA£ CYWILNY', N'4149@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1461, N'4149', N'VII', N'OZ LIPSK VII WYDZIA£ KARNY', N'4149@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1462, N'4149', N'VIII', N'OZ LIPSK VIII WYDZ. RODZ. I NIELETNICH', N'4149@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1463, N'4149', N'IX', N'OZ LIPSK IX WYDZ KSI¥G WIECZYSTYCH', N'4149@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1464, N'4149', N'X', N'OZ ZWOLEÑ X WYDZIA£ CYWILNY', N'4149@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1465, N'4149', N'XI', N'OZ ZWOLEÑ XI WYDZIA£ KARNY', N'4149@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1466, N'4149', N'XII', N'OZ ZWOLEÑ XII WYDZ. RODZ. I NIELETNICH', N'4149@XII', N'SROD', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1467, N'4149', N'XIII', N'OZ ZWOLEÑ XIII WYDZ KSI¥G WIECZYSTYCH', N'4149@XIII', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1468, N'4149', N'XIV', N'OZ PIONKACH XIV WYDZ KSI¥G WIECZYSTYCH', N'4149@XIV', N'SCYW', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1469, N'4150', N'I', N'I WYDZIA£ CYWILNY', N'4150@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1470, N'4150', N'II', N'II WYDZIA£ KARNY', N'4150@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1471, N'4150', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4150@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1472, N'4150', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4150@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1473, N'4150', N'VI', N'OZ SZYD£OWIEC VI WYDZIA£ CYWILNY', N'4150@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1474, N'4150', N'VII', N'OZ SZYD£OWIEC VII WYDZIA£ KARNY', N'4150@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1475, N'4150', N'VIII', N'OZ SZYD£OWIEC VIII WYDZ. RODZ. I NIELET.', N'4150@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1476, N'4150', N'IX', N'OZ SZYD£OWIEC IX WYDZ KSI¥G WIECZYSTYCH', N'4150@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1477, N'4151', N'I', N'I WYDZIA£ CYWILNY', N'4151@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1478, N'4151', N'II', N'II WYDZIA£ KARNY', N'4151@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1479, N'4151', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4151@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1480, N'4151', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4151@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1481, N'4151', N'V', N'V WYDZIA£ GOSPODARCZY', N'4151@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1482, N'4151', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4151@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1483, N'4151', N'VII', N'VII WYDZIA£ CYWILNY', N'4151@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1484, N'4151', N'VIII', N'VIII WYDZIA£ KARNY', N'4151@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1485, N'4151', N'IX', N'IX WYDZIA£ WYKONANIA ORZECZEÑ', N'4151@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1486, N'4151', N'X', N'X WYDZIA£ KARNY', N'4151@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1487, N'4152', N'I', N'I WYDZIA£ CYWILNY', N'4152@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1488, N'4152', N'II', N'II WYDZIA£ KARNY', N'4152@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1489, N'4152', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4152@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1490, N'4152', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4152@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1491, N'4153', N'I', N'I WYDZIA£ CYWILNY', N'4153@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1492, N'4153', N'II', N'II WYDZIA£ KARNY', N'4153@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1493, N'4153', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4153@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1494, N'4153', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4153@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1495, N'4154', N'I', N'I WYDZIA£ CYWILNY', N'4154@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1496, N'4154', N'II', N'II WYDZIA£ KARNY', N'4154@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1497, N'4154', N'II.1', N'SEKCJA WYKONAWCZA II WYDZIA£ KARNY', N'4154@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1498, N'4154', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4154@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1499, N'4154', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4154@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1500, N'4154', N'V', N'V WYDZIA£ GOSPODARCZY', N'4154@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1501, N'4154', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4154@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1502, N'4154', N'VII', N'VII WYDZIA£ KARNY', N'4154@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1503, N'4154', N'VIII', N'OZ £OSICE VIII WYDZ KSI¥G WIECZYSTYCH', N'4154@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1504, N'4155', N'I', N'I WYDZIA£ CYWILNY', N'4155@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1505, N'4155', N'II', N'II WYDZIA£ KARNY', N'4155@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1506, N'4155', N'II.1', N'SEKCJA WYKONAWCZA II WYDZIA£ KARNY', N'4155@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1507, N'4155', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4155@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1508, N'4155', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4155@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1509, N'4155', N'IV', N'OZ SOKO£ÓW PODLASKI VI WYDZIA£ CYWILNY', N'4155@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1510, N'4155', N'VII', N'OZ SOKO£ÓW PODLASKI VII WYDZIA£ KARNY', N'4155@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1511, N'4155', N'VIII', N'OZ SOKO£ÓW PODLASKI VIII WYDZ.RODZ.I NIE', N'4155@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1512, N'4155', N'IX', N'OZ SOKO£ÓW PODLASKI IX WYDZ KSI¥G WIECZ', N'4155@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1513, N'4156', N'I', N'I WYDZIA£ CYWILNY', N'4156@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1514, N'4156', N'II', N'II WYDZIA£ KARNY', N'4156@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1515, N'4156', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4156@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1516, N'4156', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4156@V', N'SCYW', 5)
GO
print 'Processed 1500 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1517, N'4156', N'VI', N'OZ JANÓW LUBELSKI VI WYDZIA£ CYWILNY', N'4156@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1518, N'4156', N'VII', N'OZ JANÓW LUBELSKI VII WYDZIA£ KARNY', N'4156@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1519, N'4156', N'VIII', N'OZ JANÓW LUBELSKI VIII WYDZ.RODZ. NIELET', N'4156@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1520, N'4156', N'IX', N'OZ JANÓW LUBELSKI IX WYDZ KSI¥G WIECZ.', N'4156@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1521, N'4157', N'I', N'I WYDZIA£ CYWILNY', N'4157@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1522, N'4157', N'II', N'II WYDZIA£ KARNY', N'4157@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1523, N'4157', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4157@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1524, N'4157', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4157@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1525, N'4158', N'I', N'I WYDZIA£ CYWILNY', N'4158@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1526, N'4158', N'II', N'II WYDZIA£ KARNY', N'4158@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1527, N'4158', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4158@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1528, N'4158', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4158@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1529, N'4159', N'I', N'I WYDZIA£ CYWILNY', N'4159@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1530, N'4159', N'II', N'II WYDZIA£ KARNY', N'4159@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1531, N'4159', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4159@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1532, N'4159', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4159@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1533, N'4159', N'V', N'V WYDZIA£ GOSPODARCZY', N'4159@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1534, N'4159', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4159@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1535, N'4159', N'VII', N'VII WYDZIA£ KARNY', N'4159@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1536, N'4159', N'VIII', N'VIII WYDZIA£ WYKONANIA ORZECZEÑ', N'4159@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1537, N'4159', N'IX', N'OZ KRASNYSTWA IX WYDZIA£ CYWILNY', N'4159@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1538, N'4159', N'X', N'OZ KRASNYSTAW X WYDZIA£ KARNY', N'4159@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1539, N'4159', N'XI', N'OZ KRASNYSTAW XI WYDZ. RODZ. I NIELET.', N'4159@XI', N'SROD', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1540, N'4159', N'XII', N'OZ KRASNYSTAW XII WYDZ KSI¥G WIECZYSTYCH', N'4159@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1541, N'4160', N'I', N'I WYDZIA£ CYWILNY', N'4160@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1542, N'4160', N'II', N'II WYDZIA£ KARNY', N'4160@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1543, N'4160', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4160@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1544, N'4160', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4160@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1545, N'4160', N'VI', N'OZ PLESZEW VI WYDZIA£ CYWILNY', N'4160@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1546, N'4160', N'VII', N'OZ PLESZEW VII WYDZIA£ KARNY', N'4160@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1547, N'4160', N'VIII', N'OZ PLESZEW VIII WYDZ. RODZ. I NIELETNICH', N'4160@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1548, N'4160', N'IX', N'OZ PLESZEW IX WYDZ KSI¥G WIECZYSTYCH', N'4160@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1549, N'4161', N'I', N'I WYDZIA£ CYWILNY', N'4161@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1550, N'4161', N'II', N'II WYDZIA£ KARNY', N'4161@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1551, N'4161', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4161@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1552, N'4161', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4161@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1553, N'4161', N'V', N'V WYDZIA£ GOSPODARCZY', N'4161@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1554, N'4161', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4161@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1555, N'4161', N'VII', N'VII WYDZIA£ KARNY', N'4161@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1556, N'4161', N'VIII', N'VIII WYDZIA£ WYKONANIA ORZECZEÑ', N'4161@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1557, N'4162', N'I', N'I WYDZIA£ CYWILNY', N'4162@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1558, N'4162', N'II', N'II WYDZIA£ KARNY', N'4162@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1559, N'4162', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4162@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1560, N'4162', N'IV', N'IV WYDZIA£ S¥D PRACY', N'4162@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1561, N'4162', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4162@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1562, N'4162', N'VI', N'OZ OSTRZESZÓW VI WYDZIA£ CYWILNY', N'4162@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1563, N'4162', N'VII', N'OZ OSTRZESZÓW VII WYDZIA£ KARNY', N'4162@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1564, N'4162', N'VIII', N'OZ OSTRZESZÓW VIII WYDZ. RODZ. I NIELET', N'4162@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1565, N'4162', N'IX', N'OZ OSTRZESZÓW IX WYDZ KSI¥G WIECZYSTYCH', N'4162@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1566, N'4163', N'I', N'I WYDZIA£ CYWILNY', N'4163@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1567, N'4163', N'II', N'II WYDZIA£ KARNY', N'4163@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1568, N'4163', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4163@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1569, N'4163', N'IV', N'IV WYDZIA£ PRACY', N'4163@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1570, N'4163', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4163@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1571, N'4163', N'VI', N'OZ KROTOSZYN VI WYDZIA£ CYWILNY', N'4163@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1572, N'4163', N'VII', N'OZ KROTOSZYN VII WYDZIA£ KARNY', N'4163@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1573, N'4163', N'VIII', N'OZ KROTOSZYN VIII WYDZ. RODZ. I NIELET', N'4163@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1574, N'4163', N'IX', N'OZ KROTOSZYN IX WYDZ KSI¥G WIECZYSTYCH', N'4163@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1575, N'4164', N'I', N'I WYDZIA£ CYWILNY', N'4164@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1576, N'4164', N'II', N'II WYDZIA£ KARNY', N'4164@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1577, N'4164', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4164@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1578, N'4164', N'IV', N'IV WYDZIA£ PRACY', N'4164@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1579, N'4164', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4164@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1580, N'4164', N'VI', N'OZ £ÊCZYCA VI WYDZIA£ CYWILNY', N'4164@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1581, N'4164', N'VII', N'OZ £ÊCZYCA VII WYDZIA£ KARNY', N'4164@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1582, N'4164', N'VIII', N'OZ £ÊCZYCA VIII WYDZ. RODZ. I NIELETNICH', N'4164@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1583, N'4164', N'IX', N'OZ £ÊCZYCA IX WYDZ KSI¥G WIECZYSTYCH', N'4164@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1584, N'4165', N'I', N'I WYDZIA£ CYWILNY', N'4165@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1585, N'4165', N'II', N'II WYDZIA£ KARNY', N'4165@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1586, N'4165', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4165@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1587, N'4165', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4165@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1588, N'4166', N'I', N'I WYDZIA£ CYWILNY', N'4166@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1589, N'4166', N'II', N'II WYDZIA£ CYWILNY', N'4166@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1590, N'4166', N'III', N'III WYDZIA£ CYWILNY', N'4166@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1591, N'4166', N'IV', N'IV WYDZIA£ KARNY', N'4166@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1592, N'4166', N'V', N'V WYDZIA£ KARNY', N'4166@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1593, N'4166', N'VI', N'VI WYDZIA£ KARNY', N'4166@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1594, N'4166', N'VII', N'VII WYDZIA£ RODZINNY I NIELETNICH', N'4166@VII', N'SROD', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1595, N'4166', N'VIII', N'VIII WYDZIA£ RODZINNY I NIELETNICH', N'4166@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1596, N'4166', N'IX', N'IX WYDZIA£ RODZINNY I NIELETNICH', N'4166@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1597, N'4166', N'X', N'X WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4166@X', N'SUBE', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1598, N'4166', N'XI', N'XI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4166@XI', N'SUBE', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1599, N'4166', N'XII', N'XII WYDZIA£ GOSPODARCZY', N'4166@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1600, N'4166', N'XIII', N'XIII WYDZIA£ GOSPODARCZY', N'4166@XIII', N'SGOS', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1601, N'4166', N'XIV', N'XIV WYDZIA£ GOSPODAR. DS. UPAD£OŒ.I NAP', N'4166@XIV', N'SGOS', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1602, N'4166', N'XV', N'', N'4166@XV', NULL, 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1603, N'XV', N'WYDZIA£', N'GOSPODARCZY REJESTRUZASTAWÓW', N'XV@WYDZIA£', N'SGOS', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1604, N'4166', N'XVI', N'XVI WYDZIA£ KSI¥G WIECZYSTYCH', N'4166@XVI', N'SCYW', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1605, N'4166', N'XVII', N'XVII WYDZIA£ KARNY', N'4166@XVII', N'SKAR', 17)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1606, N'4166', N'XVIII', N'XVIII WYDZIA£ CYWILNY', N'4166@XVIII', N'SCYW', 18)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1607, N'4166', N'XIX', N'OZ BRZEZINY XIX WYDZIA£ KSI¥G WIECZ', N'4166@XIX', N'SCYW', 19)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1608, N'4166', N'XX', N'XX WYDZIA£ GOSPODARCZY KRS', N'4166@XX', N'SGOS', 20)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1609, N'4167', N'I', N'I WYDZIA£ CYWILNY', N'4167@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1610, N'4167', N'II', N'II WYDZIA£ CYWILNY', N'4167@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1611, N'4167', N'III', N'III WYDZIA£ KARNY', N'4167@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1612, N'4167', N'IV', N'IV WYDZIA£ KARNY', N'4167@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1613, N'4167', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4167@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1614, N'4167', N'VI', N'VI WYDZIA£ RODZINNY I NIELETNICH', N'4167@VI', N'SROD', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1615, N'4167', N'VII', N'VII WYDZIA£ KARNY', N'4167@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1616, N'4167', N'VIII', N'VIII WYDZIA£ CYWILNY', N'4167@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1617, N'4167', N'IX', N'OZ BRZEZINY IX WYDZIA£ CYWILNY', N'4167@IX', N'SCYW', 9)
GO
print 'Processed 1600 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1618, N'4167', N'X', N'OZ BRZEZINY X WYDZIA£ KARNY', N'4167@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1619, N'4167', N'XI', N'OZ BRZEZINY XI WYDZIA£ RODZINNY I NIELET', N'4167@XI', N'SROD', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1620, N'4168', N'I', N'I WYDZIA£ CYWILNY', N'4168@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1621, N'4168', N'II', N'II WYDZIA£ KARNY', N'4168@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1622, N'4168', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4168@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1623, N'4168', N'IV', N'IV WYDZIA£ PRACY', N'4168@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1624, N'4168', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4168@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1625, N'4169', N'I', N'I WYDZIA£ CYWILNY', N'4169@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1626, N'4169', N'II', N'II WYDZIA£ KARNY', N'4169@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1627, N'4169', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4169@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1628, N'4169', N'IV', N'IV WYDZIA£ PRACY', N'4169@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1629, N'4169', N'V', N'V WYDZIA£ GOSPODARCZY', N'4169@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1630, N'4169', N'VII', N'VII WYDZIA£ KSI¥G WIECZYSTYCH', N'4169@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1631, N'4169', N'VIII', N'OZ RAWA MAZOWIECKA VIII WYDZIA£ CYWILNY', N'4169@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1632, N'4169', N'IX', N'OZ RAWA MAZOWIECKA IX WYDZIA£ KARNY', N'4169@IX', N'SKAR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1633, N'4169', N'X', N'OZ RAWA MAZ. X WYDZIA£ RODZINNY I NIELET', N'4169@X', N'SROD', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1634, N'4169', N'XI', N'OZ RAWA MAZ. XI WYDZIA£ KSI¥G WIECZ', N'4169@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1635, N'4170', N'I', N'I WYDZIA£ CYWILNY', N'4170@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1636, N'4170', N'II', N'II WYDZIA£ KARNY', N'4170@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1637, N'4170', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4170@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1638, N'4170', N'IV', N'IV WYDZIA£ PRACY', N'4170@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1639, N'4170', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4170@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1640, N'4171', N'I', N'I WYDZIA£ CYWILNY', N'4171@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1641, N'4171', N'II', N'II WYDZIA£ KARNY', N'4171@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1642, N'4171', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4171@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1643, N'4171', N'IV', N'IV WYDZIA£ PRACY', N'4171@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1644, N'4171', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4171@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1645, N'4172', N'I', N'I WYDZIA£ CYWILNY', N'4172@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1646, N'4172', N'II', N'II WYDZIA£ KARNY', N'4172@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1647, N'4172', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4172@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1648, N'4172', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4172@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1649, N'4173', N'I', N'I WYDZIA£ CYWILNY', N'4173@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1650, N'4173', N'II', N'II WYDZIA£ KARNY', N'4173@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1651, N'4173', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4173@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1652, N'4173', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4173@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1653, N'4173', N'V', N'V WYDZIA£ GOSPODARCZY', N'4173@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1654, N'4173', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4173@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1655, N'4173', N'VII', N'VII WYDZIA£ KARNY', N'4173@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1656, N'4174', N'I', N'I WYDZIA£ CYWILNY', N'4174@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1657, N'4174', N'II', N'II WYDZIA£ KARNY', N'4174@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1658, N'4174', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4174@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1659, N'4174', N'IV', N'IV WYDZIA£ PRACY', N'4174@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1660, N'4174', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4174@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1661, N'4174', N'VI', N'VI WYDZIA£ KARNY', N'4174@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1662, N'4175', N'I', N'I WYDZIA£ CYWILNY', N'4175@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1663, N'4175', N'II', N'II WYDZIA£ KARNY', N'4175@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1664, N'4175', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4175@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1665, N'4175', N'IV', N'IV WYDZIA£ PRACY', N'4175@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1666, N'4175', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4175@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1667, N'4176', N'I', N'I WYDZIA£ CYWILNY', N'4176@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1668, N'4176', N'II', N'II WYDZIA£ KARNY', N'4176@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1669, N'4176', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4176@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1670, N'4176', N'IV', N'IV WYDZIA£ PRACY', N'4176@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1671, N'4176', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4176@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1672, N'4176', N'VIII', N'VIII WYDZIA£ WYKONANIA ORZECZEÑ', N'4176@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1673, N'4177', N'I', N'I WYDZIA£ CYWILNY', N'4177@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1674, N'4177', N'II', N'II WYDZIA£ KARNY', N'4177@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1675, N'4177', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4177@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1676, N'4177', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4177@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1677, N'4178', N'I', N'I WYDZIA£ CYWILNY', N'4178@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1678, N'4178', N'II', N'II WYDZIA£ KARNY', N'4178@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1679, N'4178', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4178@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1680, N'4178', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4178@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1681, N'4178', N'VIII', N'VIII WYDZIA£ WYKONANIA ORZECZEÑ', N'4178@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1682, N'4178', N'IX', N'OZ DZIA£DOWO IX WYDZIA£ CYWILNY', N'4178@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1683, N'4178', N'X', N'OZ DZIA£DOWO X WYDZIA£ KARNY', N'4178@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1684, N'4178', N'XI', N'OZ DZIA£DOWO XI WYDZIA£ RODZINNY I NIELE', N'4178@XI', N'SROD', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1685, N'4178', N'XII', N'OZ DZIA£DOWO XII WYDZIA£ KSI¥G WIECZ', N'4178@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1686, N'4178', N'VII', N'OZ ¯UROMIN VII WYDZIA£ KSI¥G WIECZ', N'4178@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1687, N'4179', N'I', N'I WYDZIA£ CYWILNY', N'4179@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1688, N'4179', N'II', N'II WYDZIA£ KARNY', N'4179@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1689, N'4179', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4179@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1690, N'4179', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4179@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1691, N'4179', N'V', N'V WYDZIA£ GOSPODARCZY', N'4179@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1692, N'4179', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4179@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1693, N'4179', N'VII', N'VII WYDZIA£ KARNY', N'4179@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1694, N'4179', N'VIII', N'VIII WYDZIA£ WYKONANIA ORZECZEÑ', N'4179@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1695, N'4179', N'IX', N'OZ SIERPC IX WYDZIA£ CYWILNY', N'4179@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1696, N'4179', N'X', N'OZ SIERPC X WYDZIA£ KARNY', N'4179@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1697, N'4179', N'XI', N'OZ SIERPC XI WYDZ. RODZ. I NIELETNICH', N'4179@XI', N'SROD', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1698, N'4179', N'XII', N'OZ SIERPC XII WYDZ KSI¥G WIECZYSTYCH', N'4179@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1699, N'4180', N'I', N'I WYDZIA£ CYWILNY', N'4180@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1700, N'4180', N'II', N'II WYDZIA£ KARNY', N'4180@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1701, N'4180', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4180@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1702, N'4180', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4180@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1703, N'4181', N'I', N'I WYDZIA£ CYWILNY', N'4181@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1704, N'4181', N'II', N'II WYDZIA£ KARNY', N'4181@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1705, N'4181', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4181@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1706, N'4181', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4181@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1707, N'4182', N'I', N'I WYDZIA£ CYWILNY', N'4182@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1708, N'4182', N'II', N'II WYDZIA£ KARNY', N'4182@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1709, N'4182', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4182@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1710, N'4182', N'IV', N'IV WYDZIA£ PRACY', N'4182@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1711, N'4182', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4182@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1712, N'4183', N'I', N'I WYDZIA£ CYWILNY', N'4183@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1713, N'4183', N'II', N'II WYDZIA£ KARNY', N'4183@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1714, N'4183', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4183@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1715, N'4183', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4183@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1716, N'4183', N'VI', N'OZ PODDÊBICE VI WYDZIA£ KSI¥G WIECZ', N'4183@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1717, N'4183', N'VII', N'OZ PODDÊBICE VII WYDZIA£ KARNY', N'4183@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1718, N'4184', N'I', N'I WYDZIA£ CYWILNY', N'4184@I', N'SCYW', 1)
GO
print 'Processed 1700 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1719, N'4184', N'II', N'II WYDZIA£ KARNY', N'4184@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1720, N'4184', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4184@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1721, N'4184', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4184@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1722, N'4184', N'V', N'V WYDZIA£ GOSPODARCZY', N'4184@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1723, N'4184', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4184@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1724, N'4185', N'I', N'I WYDZIA£ CYWILNY', N'4185@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1725, N'4185', N'II', N'II WYDZIA£ KARNY', N'4185@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1726, N'4185', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4185@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1727, N'4185', N'IV', N'IV WYDZIA£ PRACY', N'4185@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1728, N'4185', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4185@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1729, N'4185', N'VI', N'OZ PAJÊCZNO VI WYDZIA£ KARNY', N'4185@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1730, N'4185', N'VII', N'OZ PAJÊCZNO VII WYDZ KSI¥G WIECZYSTYCH', N'4185@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1731, N'4185', N'VIII', N'OZ PAJÊCZNO VIII WYDZIA£ CYWILNY', N'4185@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1732, N'4186', N'I', N'I WYDZIA£ CYWILNY', N'4186@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1733, N'4186', N'II', N'II WYDZIA£ KARNY', N'4186@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1734, N'4186', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4186@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1735, N'4186', N'IV', N'IV WYDZIA£ PRACY', N'4186@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1736, N'4186', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4186@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1737, N'4187', N'I', N'I WYDZIA£ CYWILNY', N'4187@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1738, N'4187', N'II', N'II WYDZIA£ KARNY', N'4187@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1739, N'4187', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4187@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1740, N'4187', N'IV', N'IV WYDZIA£ PRACY I UBEZPIECZEÑ SPO£ECZ', N'4187@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1741, N'4187', N'V', N'V WYDZIA£ GOSPODARCZY', N'4187@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1742, N'4187', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4187@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1743, N'4187', N'VII', N'OZ S£UPCA VII WYDZIA£ CYWILNY', N'4187@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1744, N'4187', N'VIII', N'OZ S£UPCA VIII WYDZIA£ KARNY', N'4187@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1745, N'4187', N'IX', N'OZ S£UPCA IX WYDZ. RODZ. I NIELET.', N'4187@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1746, N'4187', N'X', N'OZ S£UPCA X WYDZIA£ KSI¥G WIECZYSTYCH', N'4187@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1747, N'4188', N'I', N'I WYDZIA£ CYWILNY', N'4188@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1748, N'4188', N'II', N'II WYDZIA£ KARNY', N'4188@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1749, N'4188', N'II.1', N'SEKCJA WYKONAWCZA PRZY II WYDZ KARNYM', N'4188@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1750, N'4188', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4188@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1751, N'4188', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4188@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1752, N'4189', N'I', N'I WYDZIA£ CYWILNY', N'4189@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1753, N'4189', N'II', N'II WYDZIA£ KARNY', N'4189@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1754, N'4189', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4189@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1755, N'4189', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4189@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1756, N'4190', N'I', N'I WYDZIA£ CYWILNY', N'4190@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1757, N'4190', N'II', N'II WYDZIA£ KARNY', N'4190@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1758, N'4190', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4190@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1759, N'4190', N'IV', N'IV WYDZIA£ PRACY', N'4190@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1760, N'4190', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4190@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1761, N'4191', N'I', N'I WYDZIA£ CYWILNY', N'4191@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1762, N'4191', N'II', N'II WYDZIA£ KARNY', N'4191@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1763, N'4191', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4191@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1764, N'4191', N'IV', N'IV WYDZIA£ PRACY', N'4191@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1765, N'4191', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4191@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1766, N'4191', N'VI', N'OZ NOWY TOMYŒL VI WYDZIA£ CYWILNY', N'4191@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1767, N'4191', N'VII', N'OZ NOWY TOMYŒL VII WYDZIA£ KARNY', N'4191@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1768, N'4191', N'VIII', N'OZ NOWY TOMYŒL VIII WYDZIA£ RODZ I NIELE', N'4191@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1769, N'4191', N'IX', N'OZ NOWY TOMYŒL IX WYDZ KSI¥G WIECZ', N'4191@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1770, N'4191', N'X', N'OZ WOLSZTYN X WYDZIA£ CYWILNY', N'4191@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1771, N'4191', N'XI', N'OZ WOLSZTYN XI WYDZIA£ KARNY', N'4191@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1772, N'4191', N'XII', N'OZ WOLSZTYN XII WYDZIA£ RODZINNY I NIELE', N'4191@XII', N'SROD', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1773, N'4191', N'XIII', N'OZ WOLSZTYN XIII WYDZIA£ KSI¥G WIECZ', N'4191@XIII', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1774, N'4192', N'I', N'I WYDZIA£ CYWILNY', N'4192@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1775, N'4192', N'II', N'II WYDZIA£ KARNY', N'4192@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1776, N'4192', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4192@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1777, N'4192', N'IV', N'IV WYDZIA£ PRACY', N'4192@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1778, N'4192', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4192@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1779, N'4192', N'VI', N'OZ ŒREM VI WYDZIA£ CYWILNY', N'4192@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1780, N'4192', N'VII', N'OZ ŒREM VII WYDZIA£ KARNY', N'4192@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1781, N'4192', N'VIII', N'OZ ŒREM VIII WYDZ RODZ I NIELETNICH', N'4192@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1782, N'4192', N'IX', N'OZ ŒREM IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4192@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1783, N'4193', N'I', N'I WYDZIA£ CYWILNY', N'4193@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1784, N'4193', N'II', N'II WYDZIA£ KARNY', N'4193@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1785, N'4193', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4193@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1786, N'4193', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4193@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1787, N'4193', N'V', N'V WYDZIA£ GOSPODARCZY', N'4193@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1788, N'4193', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4193@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1789, N'4193', N'VII', N'OZ GOSTYÑ VII WYDZIA£ CYWILNY', N'4193@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1790, N'4193', N'VIII', N'OZ GOSTYÑ VIII WYDZIA£ KARNY', N'4193@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1791, N'4193', N'IX', N'OZ GOSTYÑ IX WYDZIA£ RODZINNY I NIELE', N'4193@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1792, N'4193', N'X', N'OZ GOSTYÑ X WYDZIA£ KSI¥G WIECZYSTYCH', N'4193@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1793, N'4193', N'XI', N'OZ RAWICZ XI WYDZIA£ CYWILNY', N'4193@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1794, N'4193', N'XII', N'OZ RAWICZ XII WYDZIA£ KARNY', N'4193@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1795, N'4193', N'XIII', N'OZ RAWICZ XIII WYDZIA£ RODZINNY I NIELET', N'4193@XIII', N'SROD', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1796, N'4193', N'XIV', N'OZ RAWICZ XIV WYDZIA£ KSI¥G WIECZYSTYCH', N'4193@XIV', N'SCYW', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1797, N'4194', N'I', N'I WYDZIA£ CYWILNY', N'4194@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1798, N'4194', N'II', N'II WYDZIA£ KARNY', N'4194@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1799, N'4194', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4194@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1800, N'4194', N'IV', N'IV WYDZIA£ PRACY', N'4194@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1801, N'4194', N'V', N'V WYDZIA£ GOSPODARCZY', N'4194@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1802, N'4194', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4194@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1803, N'4194', N'VII', N'OZ Z£OTÓW VII WYDZIA£ CYWILNY', N'4194@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1804, N'4194', N'VIII', N'OZ Z£OTÓW VIII WYDZIA£ KARNY', N'4194@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1805, N'4194', N'IX', N'OZ Z£OTÓW IX WYDZIA£ RODZINNY I NIELET', N'4194@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1806, N'4194', N'X', N'OZ Z£OTÓW X WYDZIA£ KSI¥G WIECZYSTYCH', N'4194@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1807, N'4195', N'I', N'I WYDZIA£ CYWILNY', N'4195@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1808, N'4195', N'II', N'II WYDZIA£ CYWILNY', N'4195@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1809, N'4195', N'III', N'III WYDZIA£ KARNY', N'4195@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1810, N'4195', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4195@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1811, N'4195', N'V', N'V WYDZIA£ PRACY', N'4195@V', N'SPPR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1812, N'4195', N'VI', N'VI WYDZIA£ UBEZPIECZEÑ SPO£ECZNYCH', N'4195@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1813, N'4195', N'VIII', N'VIII WYDZIA£ KARNY', N'4195@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1814, N'4195', N'IX', N'IX WYDZIA£ CYWILNY', N'4195@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1815, N'4196', N'I', N'I WYDZIA£ CYWILNY', N'4196@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1816, N'4196', N'II', N'II WYDZIA£ CYWILNY', N'4196@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1817, N'4196', N'III', N'III WYDZIA£ KARNY', N'4196@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1818, N'4196', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4196@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1819, N'4196', N'V', N'V WYDZIA£ CYWILNY', N'4196@V', N'SCYW', 5)
GO
print 'Processed 1800 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1820, N'4196', N'VI', N'VI WYDZIA£ KARNY', N'4196@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1821, N'4196', N'VII', N'VII WYDZIA£ GOSPODARCZY- REJESTRU ZASTAW', N'4196@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1822, N'4196', N'VIII', N'VIII WYDZIA£ GOSPODARCZY - KRS', N'4196@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1823, N'4196', N'IX', N'IX WYDZIA£ GOSPODARCZY - KRS', N'4196@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1824, N'4197', N'I', N'I WYDZIA£ CYWILNY', N'4197@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1825, N'4197', N'II', N'II WYDZIA£ CYWILNY', N'4197@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1826, N'4197', N'III', N'III WYDZIA£ KARNY', N'4197@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1827, N'4197', N'IV', N'IV WYDZIA£ RODZINNY I NIELETNICH', N'4197@IV', N'SROD', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1828, N'4197', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4197@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1829, N'4197', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4197@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1830, N'4197', N'VII', N'VII WYDZIA£ CYWILNY', N'4197@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1831, N'4197', N'VIII', N'VIII WYDZIA£ KARNY', N'4197@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1832, N'4197', N'IX', N'IX WYDZIA£ GOSPODARCZY', N'4197@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1833, N'4197', N'X', N'X WYDZIA£ GOSPODARCZY', N'4197@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1834, N'4197', N'XI', N'XI WYDZIA£ GOSPODARCZY DS. UPAD£.I NAPR.', N'4197@XI', N'SGOS', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1835, N'4197', N'XII', N'XII WYDZIA£ CYWILNY', N'4197@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1836, N'4198', N'I', N'I WYDZIA£ CYWILNY', N'4198@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1837, N'4198', N'II', N'II WYDZIA£ KARNY', N'4198@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1838, N'4198', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4198@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1839, N'4198', N'IV', N'IV WYDZIA£ S¥DU PRACY', N'4198@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1840, N'4198', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4198@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1841, N'4198', N'VI', N'OZ MIÊDZYCHÓD VI WYDZIA£ KSI¥G WIECZYSTY', N'4198@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1842, N'4198', N'VII', N'OZ OBORNIKI VII WYDZIA£ CYWILNY', N'4198@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1843, N'4198', N'VIII', N'OZ OBORNIKI VIII WYDZIA£ KARNY', N'4198@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1844, N'4198', N'IX', N'OZ OBORNIKI IX WYDZIA£ RODZINNY I NIELE', N'4198@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1845, N'4198', N'X', N'OZ OBORNIKI X WYDZIA£ KSI¥G WIECZYS', N'4198@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1846, N'4199', N'I', N'I WYDZIA£ CYWILNY', N'4199@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1847, N'4199', N'II', N'II WYDZIA£ KARNY', N'4199@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1848, N'4199', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4199@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1849, N'4199', N'IV', N'IV WYDZIA£ S¥DU PRACY', N'4199@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1850, N'4199', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4199@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1851, N'4199', N'VI', N'OZ WRZEŒNIA VI WYDZIA£ CYWILNY', N'4199@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1852, N'4199', N'VII', N'OZ WRZEŒNIA VII WYDZIA£ KARNY', N'4199@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1853, N'4199', N'VIII', N'OZ WRZEŒNIA VIII WYDZ. RODZ. I NIELET.', N'4199@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1854, N'4199', N'IX', N'OZ WRZEŒNIA IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4199@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1855, N'4200', N'I', N'I WYDZIA£ CYWILNY', N'4200@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1856, N'4200', N'II', N'II WYDZIA£ KARNY', N'4200@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1857, N'4200', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4200@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1858, N'4200', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4200@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1859, N'4200', N'VI', N'OZ CZARNKÓW VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4200@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1860, N'4201', N'I', N'I WYDZIA£ CYWILNY', N'4201@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1861, N'4201', N'II', N'II WYDZIA£ KARNY', N'4201@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1862, N'4201', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4201@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1863, N'4201', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4201@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1864, N'4201', N'VI', N'OZ CHODZIE¯ VI WYDZIA£ CYWILNY', N'4201@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1865, N'4201', N'VII', N'OZ CHODZIE¯ VII WYDZIA£ KARNY', N'4201@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1866, N'4201', N'VIII', N'OZ CHODZIE¯ VIII WYDZ. RODZ. I NIELET.', N'4201@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1867, N'4201', N'IX', N'OZ CHODZIE¯ IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4201@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1868, N'4201', N'X', N'OZ WYRZYSK X WYDZIA£ KSI¥G WIECZYSTYCH', N'4201@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1869, N'4202', N'I', N'I WYDZIA£ CYWILNY', N'4202@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1870, N'4202', N'II', N'II WYDZIA£ KARNY', N'4202@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1871, N'4202', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4202@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1872, N'4202', N'IV', N'OZ GUBIN IV WYDZIA£ KARNY', N'4202@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1873, N'4202', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4202@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1874, N'4202', N'VI', N'OZ GUBIN VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4202@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1875, N'4203', N'I', N'I WYDZIA£ CYWILNY', N'4203@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1876, N'4203', N'II', N'II WYDZIA£ KARNY', N'4203@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1877, N'4203', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4203@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1878, N'4203', N'IV', N'IV WYDZIA£ PRACY', N'4203@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1879, N'4203', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4203@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1880, N'4203', N'VI', N'OZ WSCHOWA VI WYDZIA£ CYWILNY', N'4203@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1881, N'4203', N'VII', N'OZ WSCHOWA VII WYDZIA£ KARNY', N'4203@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1882, N'4203', N'VIII', N'OZ WSCHOWA VIII WYDZ. RODZ. I NIELET.', N'4203@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1883, N'4203', N'IX', N'OZ WSCHOWA IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4203@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1884, N'4204', N'I', N'I WYDZIA£ CYWILNY', N'4204@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1885, N'4204', N'II', N'II WYDZIA£ KARNY', N'4204@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1886, N'4204', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4204@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1887, N'4204', N'IV', N'IV WYDZIA£ PRACY', N'4204@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1888, N'4204', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4204@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1889, N'4204', N'VI', N'OZ SULECHOWIE VI WYDZIA£ KARNY', N'4204@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1890, N'4204', N'VII', N'OZ SULECHOWIE VII WYDZIA£ KSI¥G WIECZ', N'4204@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1891, N'4205', N'I', N'I WYDZIA£ CYWILNY', N'4205@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1892, N'4205', N'II', N'II WYDZIA£ KARNY', N'4205@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1893, N'4205', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4205@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1894, N'4205', N'IV', N'IV WYDZIA£ PRACY I UBEZPIECZEÑ SPO£', N'4205@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1895, N'4205', N'V', N'V WYDZIA£ GOSPODARCZY', N'4205@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1896, N'4205', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4205@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1897, N'4205', N'VII', N'VII WYDZIA£ KARNY', N'4205@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1898, N'4205', N'VIII', N'VIII WYDZIA£ GOSPODARCZY KRS', N'4205@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1899, N'4205', N'IX', N'IX WYDZIA£ EGZEKUCYJNY', N'4205@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1900, N'4206', N'I', N'I WYDZIA£ CYWILNY', N'4206@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1901, N'4206', N'II', N'II WYDZIA£ KARNY', N'4206@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1902, N'4206', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4206@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1903, N'4206', N'IV', N'IV WYDZIA£ PRACY', N'4206@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1904, N'4206', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4206@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1905, N'4207', N'I', N'I WYDZIA£ CYWILNY', N'4207@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1906, N'4207', N'II', N'II WYDZIA£ KARNY', N'4207@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1907, N'4207', N'III', N'III WYDZIA£ RODZINNY', N'4207@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1908, N'4207', N'IV', N'IV WYDZIA£ PRACY', N'4207@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1909, N'4207', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4207@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1910, N'4208', N'I', N'I WYDZIA£ CYWILNY', N'4208@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1911, N'4208', N'II', N'II WYDZIA£ KARNY', N'4208@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1912, N'4208', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4208@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1913, N'4208', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4208@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1914, N'4209', N'I', N'I WYDZIA£ CYWILNY', N'4209@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1915, N'4209', N'II', N'II WYDZIA£ KARNY', N'4209@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1916, N'4209', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4209@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1917, N'4209', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4209@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1918, N'4209', N'V', N'V WYDZIA£ GOSPODARCZY', N'4209@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1919, N'4209', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4209@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1920, N'4209', N'VII', N'OZ BRZOZÓW VII WYDZIA£ CYWILNY', N'4209@VII', N'SCYW', 7)
GO
print 'Processed 1900 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1921, N'4209', N'VIII', N'OZ BRZOZÓW VIII WYDZIA£ KARNY', N'4209@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1922, N'4209', N'IX', N'OZ BRZOZÓW IX WYDZ. RODZ. I NIELETNICH', N'4209@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1923, N'4209', N'X', N'OZ BRZOZÓW X WYDZ KSI¥G WIECZYSTYCH', N'4209@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1924, N'4209', N'II.1', N'SEKCJA WYKONAWCZA II WYDZIA£ KARNY', N'4209@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1925, N'4210', N'I', N'I WYDZIA£ CYWILNY', N'4210@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1926, N'4210', N'II', N'II WYDZIA£ KARNY', N'4210@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1927, N'4210', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4210@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1928, N'4210', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4210@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1929, N'4210', N'VI', N'OZ USTRZYKI DOLNE VI WYDZIA£ KARNY', N'4210@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1930, N'4210', N'VII', N'OZ USTRZYKI DOLNE VII WYDZ KSI¥G WIECZ', N'4210@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1931, N'4211', N'I', N'I WYDZIA£ CYWILNY', N'4211@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1932, N'4211', N'II', N'II WYDZIA£ KARNY', N'4211@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1933, N'4211', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4211@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1934, N'4211', N'IV', N'IV WYDZIA£ PRACY', N'4211@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1935, N'4211', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4211@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1936, N'4212', N'I', N'I WYDZIA£ CYWILNY', N'4212@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1937, N'4212', N'II', N'II WYDZIA£ KARNY', N'4212@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1938, N'4212', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4212@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1939, N'4212', N'IV', N'IV WYDZIA£ PRACY', N'4212@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1940, N'4212', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4212@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1941, N'4212', N'VI', N'OZ LUBACZÓW VI WYDZIA£ CYWILNY', N'4212@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1942, N'4212', N'VII', N'OZ LUBACZÓW VII WYDZIA£ KARNY', N'4212@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1943, N'4212', N'VIII', N'OZ LUBACZÓW VIII WYDZ. RODZ. I NIELETNIC', N'4212@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1944, N'4212', N'IX', N'OZ LUBACZÓW IX WYDZ KSI¥G WIECZYSTYCH', N'4212@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1945, N'4212', N'X', N'OZ PRZEWORSK X WYDZIA£ CYWILNY', N'4212@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1946, N'4212', N'XI', N'OZ PRZEWORSK XI WYDZIA£ KARNY', N'4212@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1947, N'4212', N'XII', N'OZ PRZEWORSK XII WYDZ. RODZ. I NIELETNIC', N'4212@XII', N'SROD', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1948, N'4212', N'XIII', N'OZ PRZEWORSK XIII WYDZ KSI¥G WIECZYSTYCH', N'4212@XIII', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1949, N'4212', N'XIV', N'OZ SIENIAWA XIV WYDZ KSI¥G WIECZYSTYCH', N'4212@XIV', N'SCYW', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1950, N'4213', N'I', N'I WYDZIA£ CYWILNY', N'4213@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1951, N'4213', N'II', N'II WYDZIA£ KARNY', N'4213@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1952, N'4213', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4213@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1953, N'4213', N'IV', N'IV WYDZIA£ PRACY', N'4213@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1954, N'4213', N'V', N'V WYDZIA£ GOSPODARCZY', N'4213@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1955, N'4213', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4213@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1956, N'4214', N'I', N'I WYDZIA£ CYWILNY', N'4214@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1957, N'4214', N'II', N'II WYDZIA£ KARNY', N'4214@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1958, N'4214', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4214@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1959, N'4214', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4214@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1960, N'4214', N'VI', N'OZ ROPCZYCE VI WYDZIA£ CYWILNY', N'4214@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1961, N'4214', N'VII', N'OZ ROPCZYCE VII WYDZIA£ KARNY', N'4214@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1962, N'4214', N'VIII', N'OZ ROPCZYCE VIII WYDZ. RODZ. I NIELET', N'4214@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1963, N'4214', N'IX', N'OZ ROPCZYCE IX WYDZ KSI¥G WIECZYSTYCH', N'4214@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1964, N'4215', N'I', N'I WYDZIA£ CYWILNY', N'4215@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1965, N'4215', N'II', N'II WYDZIA£ KARNY', N'4215@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1966, N'4215', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4215@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1967, N'4215', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4215@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1968, N'4215', N'VI', N'OZ LE¯AJSK VI WYDZIA£ CYWILNY', N'4215@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1969, N'4215', N'VII', N'OZ LE¯AJSK VII WYDZIA£ KARNY', N'4215@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1970, N'4215', N'VIII', N'OZ LE¯AJSK VIII WYDZ. RODZ. I NIELETNICH', N'4215@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1971, N'4215', N'IX', N'OZ LE¯AJSK IX WYDZ KSI¥G WIECZYSTYCH', N'4215@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1972, N'4216', N'I', N'I WYDZIA£ CYWILNY', N'4216@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1973, N'4216', N'II', N'II WYDZIA£ KARNY', N'4216@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1974, N'4216', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4216@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1975, N'4216', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4216@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1976, N'4216', N'V', N'V WYDZIA£ GOSPODARCZY', N'4216@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1977, N'4216', N'VI', N'VI WYDZIA£ KARNY WYKONAWCZY', N'4216@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1978, N'4216', N'VII', N'VII WYDZIA£ KSI¥G WIECZYSTYCH', N'4216@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1979, N'4216', N'VIII', N'OZ TYCZYN VIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4216@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1980, N'4216', N'IX', N'IX WYDZIA£ GOSPODARCZY I REJESTRU ZASTAW', N'4216@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1981, N'4216', N'X', N'X WYDZIA£ KARNY', N'4216@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1982, N'4216', N'XII', N'XII WYDZIA£ GOSPODARCZY KRS', N'4216@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1983, N'4216', N'XIII', N'OZ STRZY¯ÓW XIII WYDZIA£ CYWILNY', N'4216@XIII', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1984, N'4216', N'XIV', N'OZ STRZY¯ÓW XIV WYDZIA£ KARNY', N'4216@XIV', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1985, N'4216', N'XV', N'OZ STRZY¯ÓW XV WYDZ. RODZ. I NIELETNICH', N'4216@XV', N'SROD', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1986, N'4216', N'XVI', N'OZ STRZY¯ÓW XVI WYDZ KSI¥G WIECZYSTYCH', N'4216@XVI', N'SCYW', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1987, N'4216', N'I.1', N'SEKCJA EGZEK PRZY I WYDZIALE CYWILNYM', N'4216@I.1', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1988, N'4216', N'V.1', N'SEKCJA UPAD£.PRZY V WYDZ GOSPOD.', N'4216@V.1', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1989, N'4217', N'I', N'I WYDZIA£ CYWILNY', N'4217@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1990, N'4217', N'II', N'II WYDZIA£ KARNY', N'4217@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1991, N'4217', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4217@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1992, N'4217', N'IV', N'IV WYDZIA£ PRACY', N'4217@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1993, N'4217', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4217@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1994, N'4217', N'VI', N'OZ KOLBUSZOWA VI WYDZIA£ CYWILNY', N'4217@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1995, N'4217', N'VII', N'OZ KOLBUSZOWA VII WYDZIA£ KARNY', N'4217@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1996, N'4217', N'VIII', N'OZ KOLBUSZOWA VIII WYDZ. RODZ. I NIELE', N'4217@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1997, N'4217', N'IX', N'OZ KOLBUSZOWA IX WYDZ KSI¥G WIECZYSTYCH', N'4217@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1998, N'4218', N'I', N'I WYDZIA£ CYWILNY', N'4218@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (1999, N'4218', N'II', N'II WYDZIA£ KARNY', N'4218@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2000, N'4218', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4218@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2001, N'4218', N'IV', N'IV WYDZIA£ PRACY', N'4218@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2002, N'4218', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4218@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2003, N'4218', N'VII', N'OZ NISKO VII WYDZIA£ CYWILNY', N'4218@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2004, N'4218', N'VIII', N'OZ NISKO VIII WYDZIA£ KARNY', N'4218@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2005, N'4218', N'IX', N'OZ NISKO IX WYDZ. RODZ. I NIELETNICH', N'4218@IX', N'SROD', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2006, N'4218', N'X', N'OZ NISKO X WYDZ KSI¥G WIECZYSTYCH', N'4218@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2007, N'4219', N'I', N'I WYDZIA£ CYWILNY', N'4219@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2008, N'4219', N'II', N'II WYDZIA£ KARNY', N'4219@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2009, N'4219', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4219@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2010, N'4219', N'IV', N'IV WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4219@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2011, N'4219', N'V', N'V WYDZIA£ GOSPODARCZY', N'4219@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2012, N'4219', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4219@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2013, N'4220', N'I', N'I WYDZIA£ CYWILNY', N'4220@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2014, N'4220', N'II', N'II WYDZIA£ KARNY', N'4220@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2015, N'4220', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4220@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2016, N'4220', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£.', N'4220@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2017, N'4220', N'V', N'V WYDZIA£ GOSPODARCZY', N'4220@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2018, N'4220', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4220@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2019, N'4220', N'VII', N'VII WYDZIA£ KARNY', N'4220@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2020, N'4220', N'VIII', N'VIII WYDZIA£ EGZEKUCYJNY', N'4220@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2021, N'4220', N'IX', N'IX WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4220@IX', N'SKAR', 9)
GO
print 'Processed 2000 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2022, N'4220', N'X', N'X WYDZIA£ CYWILNY', N'4220@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2023, N'4220', N'XI', N'OZ STRZELCE KRAJEÑSKIE XI WYDZIA£ CYW', N'4220@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2024, N'4220', N'XII', N'OZ STRZELCE KRAJEÑSKIE XII WYDZ. KARNY', N'4220@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2025, N'4220', N'XIII', N'OZ STRZELCE KRAJEÑSKIE XIII RODZ.I NIEL', N'4220@XIII', N'SROD', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2026, N'4220', N'XIV', N'OZ STRZELCE KRAJEÑSKIE XIV WYDZ.KS.WIECZ', N'4220@XIV', N'SCYW', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2027, N'4221', N'I', N'I WYDZIA£ CYWILNY', N'4221@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2028, N'4221', N'II', N'II WYDZIA£ KARNY', N'4221@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2029, N'4221', N'II.1', N'SEKCJA WYKONAWCZA II WYDZIA£ KARNY', N'4221@II.1', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2030, N'4221', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4221@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2031, N'4221', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4221@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2032, N'4221', N'VI', N'OZ MIÊDZYRZECZ VI WYDZIA£ CYWILNY', N'4221@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2033, N'4221', N'VII', N'OZ MIÊDZYRZECZ VII WYDZIA£ KARNY', N'4221@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2034, N'4221', N'VIII', N'OZ MIÊDZYRZECZ VIII WYDZ. RODZ. I NIELET', N'4221@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2035, N'4221', N'IX', N'OZ MIÊDZYRZECZ IX WYDZ KSI¥G WIECZYSTYCH', N'4221@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2036, N'4222', N'I', N'I WYDZIA£ CYWILNY', N'4222@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2037, N'4222', N'II', N'II WYDZIA£ KARNY', N'4222@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2038, N'4222', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4222@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2039, N'4222', N'IV', N'IV WYDZIA£ PRACY', N'4222@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2040, N'4222', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4222@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2041, N'4223', N'I', N'I WYDZIA£ CYWILNY', N'4223@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2042, N'4223', N'II', N'II WYDZIA£ KARNY', N'4223@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2043, N'4223', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4223@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2044, N'4223', N'IV', N'IV WYDZIA£ PRACY', N'4223@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2045, N'4223', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4223@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2046, N'4223', N'VI', N'OZ ŒWIDWIN VI WYDZ KSI¥G WIECZYSTYCH', N'4223@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2047, N'4223', N'VII', N'OZ ŒWIDWIN VII WYDZIA£ KARNY', N'4223@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2048, N'4224', N'I', N'I WYDZIA£ CYWILNY', N'4224@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2049, N'4224', N'II', N'II WYDZIA£ KARNY', N'4224@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2050, N'4224', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4224@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2051, N'4224', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4224@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2052, N'4225', N'I', N'I WYDZIA£ CYWILNY', N'4225@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2053, N'4225', N'II', N'II WYDZIA£ KARNY', N'4225@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2054, N'4225', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4225@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2055, N'4225', N'IV', N'IV WYDZIA£ PRACY', N'4225@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2056, N'4225', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4225@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2057, N'4226', N'I', N'I WYDZIA£ CYWILNY', N'4226@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2058, N'4226', N'II', N'II WYDZIA£ KARNY', N'4226@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2059, N'4226', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4226@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2060, N'4226', N'IV', N'IV WYDZIA£ PRACY I UBEZPECZEÑ SPO£.', N'4226@IV', N'SUBE', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2061, N'4226', N'V', N'V WYDZIA£ GOSPODARCZY', N'4226@V', N'SGOS', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2062, N'4226', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4226@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2063, N'4226', N'VII', N'VII WYDZIA£ GOSPODARCZY', N'4226@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2064, N'4226', N'VIII', N'VIII WYDZIA£ CYWILNY', N'4226@VIII', N'SCYW', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2065, N'4226', N'IX', N'IX WYDZIA£ GOSPODARCZY KRS', N'4226@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2066, N'4226', N'X', N'X WYDZIA£ KARNY', N'4226@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2067, N'4226', N'XI', N'XI WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4226@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2068, N'4226', N'XII', N'OZ S£AWNO XII WYDZIA£ CYWILNY', N'4226@XII', N'SCYW', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2069, N'4226', N'XIII', N'OZ S£AWNO XIII WYDZIA£ KARNY', N'4226@XIII', N'SKAR', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2070, N'4226', N'XIV', N'OZ S£AWNO XIV WYDZ. RODZ. I NIELETNICH', N'4226@XIV', N'SROD', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2071, N'4226', N'XV', N'OZ S£AWNO XV WYDZ KSI¥G WIECZYSTYCH', N'4226@XV', N'SCYW', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2072, N'4227', N'I', N'I WYDZIA£ CYWILNY', N'4227@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2073, N'4227', N'II', N'II WYDZIA£ KARNY', N'4227@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2074, N'4227', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4227@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2075, N'4227', N'IV', N'IV WYDZIA£ PRACY', N'4227@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2076, N'4227', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4227@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2077, N'4228', N'I', N'I WYDZIA£ CYWILNY', N'4228@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2078, N'4228', N'II', N'II WYDZIA£ KARNY', N'4228@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2079, N'4228', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4228@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2080, N'4228', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4228@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2081, N'4229', N'I', N'I WYDZIA£ CYWILNY', N'4229@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2082, N'4229', N'II', N'II WYDZIA£ KARNY', N'4229@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2083, N'4229', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4229@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2084, N'4229', N'IV', N'IV WYDZIA£ PRACY', N'4229@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2085, N'4229', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4229@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2086, N'4230', N'I', N'I WYDZIA£ CYWILNY', N'4230@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2087, N'4230', N'II', N'II WYDZIA£ KARNY', N'4230@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2088, N'4230', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4230@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2089, N'4230', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4230@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2090, N'4230', N'VI', N'OZ £OBEZ VI WYDZIA£ CYWILNY', N'4230@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2091, N'4230', N'VII', N'OZ £OBEZ VII WYDZIA£ KARNY', N'4230@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2092, N'4230', N'VIII', N'OZ £OBEZ VIII WYDZ. RODZ. I NIELETNICH', N'4230@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2093, N'4230', N'IX', N'OZ £OBEZ IX WYDZ KSI¥G WIECZYSTYCH', N'4230@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2094, N'4231', N'I', N'I WYDZIA£ CYWILNY', N'4231@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2095, N'4231', N'II', N'II WYDZIA£ KARNY', N'4231@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2096, N'4231', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4231@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2097, N'4231', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4231@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2098, N'4231', N'VI', N'OZ CHOJNA VI WYDZIA£ KARNY', N'4231@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2099, N'4232', N'I', N'I WYDZIA£ CYWILNY', N'4232@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2100, N'4232', N'II', N'II WYDZIA£ KARNY', N'4232@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2101, N'4232', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4232@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2102, N'4232', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4232@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2103, N'4232', N'VI', N'OZ CHOSZCZNO VI WYDZIA£ CYWILNY', N'4232@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2104, N'4232', N'VII', N'OZ CHOSZCZNO VII WYDZIA£ KARNY', N'4232@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2105, N'4232', N'VIII', N'OZ CHOSZCZNO VIII WYDZ.RODZ.I NIELETNICH', N'4232@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2106, N'4232', N'IX', N'OZ CHOSZCZNO IX WYDZIA£ PRACY', N'4232@IX', N'SPPR', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2107, N'4232', N'X', N'OZ CHOSZCZNO X WYDZ KSI¥G WIECZYSTYCH', N'4232@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2108, N'4233', N'I', N'I WYDZIA£ CYWILNY', N'4233@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2109, N'4233', N'II', N'II WYDZIA£ KARNY', N'4233@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2110, N'4233', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4233@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2111, N'4233', N'IV', N'IV WYDZIA£ PRACY', N'4233@IV', N'SPPR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2112, N'4233', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4233@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2113, N'4233', N'VI', N'OZ PYRZYCE VI WYDZ KSI¥G WIECZYSTYCH', N'4233@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2114, N'4233', N'VII', N'OZ PYRZYCE VII WYDZIA£ KARNY', N'4233@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2115, N'4234', N'I', N'I WYDZIA£ CYWILNY', N'4234@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2116, N'4234', N'II', N'II WYDZIA£ CYWILNY', N'4234@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2117, N'4234', N'III', N'III WYDZIA£ CYWILNY', N'4234@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2118, N'4234', N'IV', N'IV WYDZIA£ KARNY', N'4234@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2119, N'4234', N'V', N'V WYDZIA£ KARNY', N'4234@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2120, N'4234', N'VI', N'VI WYDZIA£ EGZEKUCYJNY', N'4234@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2121, N'4234', N'VII', N'VII WYDZIA£ KARNY', N'4234@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2122, N'4234', N'VIII', N'VIII WYDZIA£ RODZINNY I NIELETNICH', N'4234@VIII', N'SROD', 8)
GO
print 'Processed 2100 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2123, N'4234', N'IX', N'IX WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4234@IX', N'SUBE', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2124, N'4234', N'X', N'X WYDZIA£ GOSPODARCZY', N'4234@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2125, N'4234', N'XI', N'XI WYDZIA£ GOSPODARCZY', N'4234@XI', N'SGOS', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2126, N'4234', N'XII', N'XII WYDZIA£ GOSPODARCZY', N'4234@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2127, N'4234', N'XIII', N'XIII WYDZIA£ GOSPODARCZY KRS', N'4234@XIII', N'SGOS', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2128, N'4234', N'XIV', N'XIV WYDZIA£ GOSP.REJESTRU ZASTAWÓW', N'4234@XIV', N'SGOS', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2129, N'4235', N'I', N'I WYDZIA£ CYWILNY', N'4235@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2130, N'4235', N'II', N'II WYDZIA£ CYWILNY', N'4235@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2131, N'4235', N'III', N'III WYDZIA£ CYWILNY', N'4235@III', N'SCYW', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2132, N'4235', N'IV', N'IV WYDZIA£ KARNY', N'4235@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2133, N'4235', N'V', N'V WYDZIA£ KARNY', N'4235@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2134, N'4235', N'VI', N'VI WYDZIA£ KARNY', N'4235@VI', N'SKAR', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2135, N'4235', N'VII', N'OZ POLICE VII WYDZIA£ KARNY', N'4235@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2136, N'4235', N'VIII', N'VIII WYDZIA£ RODZINNY I NIELETNICH', N'4235@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2137, N'4235', N'IX', N'IX WYDZIA£ EGZEKUCYJNY', N'4235@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2138, N'4235', N'X', N'X WYDZIA£ KSI¥G WIECZYSTYCH', N'4235@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2139, N'4235', N'XI', N'OZ POLICE XI WYDZ KSI¥G WIECZYSTYCH', N'4235@XI', N'SCYW', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2140, N'4236', N'I', N'I WYDZIA£ CYWILNY', N'4236@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2141, N'4236', N'II', N'II WYDZIA£ KARNY', N'4236@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2142, N'4236', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4236@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2143, N'4236', N'V', N'V WYDZIA£ KSI¥G WIECZYSTYCH', N'4236@V', N'SCYW', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2144, N'4236', N'VI', N'OZ KAMIEÑ POMORSKI VI WYDZIA£ CYWILNY', N'4236@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2145, N'4236', N'VII', N'OZ KAMIEÑ POMORSKI VII WYDZIA£ KARNY', N'4236@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2146, N'4236', N'VIII', N'OZ KAMIEÑ POMORSKI VIII WYDZ.RODZ.I NIEL', N'4236@VIII', N'SROD', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2147, N'4236', N'IX', N'OZKAMIEÑ POMORSKI IX WYDZ KSI¥G WIECZ', N'4236@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2148, N'4237', N'I', N'I WYDZIA£ CYWILNY', N'4237@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2149, N'4237', N'II', N'II WYDZIA£ KARNY', N'4237@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2150, N'4237', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4237@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2151, N'4237', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4237@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2152, N'4238', N'I', N'I WYDZIA£ CYWILNY', N'4238@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2153, N'4238', N'II', N'II WYDZIA£ KARNY', N'4238@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2154, N'4238', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4238@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2155, N'4238', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4238@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2156, N'4239', N'I', N'I WYDZIA£ CYWILNY', N'4239@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2157, N'4239', N'II', N'II WYDZIA£ KARNY', N'4239@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2158, N'4239', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4239@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2159, N'4239', N'V', N'V WYDZIA£ KARNY', N'4239@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2160, N'4239', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4239@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2161, N'4240', N'I', N'I WYDZIA£ CYWILNY', N'4240@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2162, N'4240', N'II', N'II WYDZIA£ CYWILNY', N'4240@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2163, N'4240', N'III', N'III WYDZIA£ KARNY', N'4240@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2164, N'4240', N'IV', N'IV WYDZIA£ KARNY', N'4240@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2165, N'4240', N'V', N'V WYDZIA£ KARNY', N'4240@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2166, N'4240', N'VI', N'VI WYDZIA£ RODZINNY I NIELETNICH', N'4240@VI', N'SROD', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2167, N'4240', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4240@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2168, N'4240', N'VIII', N'VIII WYDZIA£ GOSPODARCZY', N'4240@VIII', N'SGOS', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2169, N'4240', N'IX', N'IX WYDZIA£ GOSPODARCZY', N'4240@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2170, N'4240', N'X', N'X WYDZ.GOSP.DS.UPAD£OŒCIOWYCH I NAPR.', N'4240@X', N'SGOS', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2171, N'4240', N'XI', N'XI WYDZ. GOSPODARCZY I REJESTRU ZASTAWÓW', N'4240@XI', N'SGOS', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2172, N'4240', N'XII', N'XII WYDZIA£ GOSPODARCZY KRS', N'4240@XII', N'SGOS', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2173, N'4240', N'XIII', N'XIII WYDZIA£ GOSPODARCZY KRS', N'4240@XIII', N'SGOS', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2174, N'4240', N'XIV', N'XIV WYDZIA£ GOSPODARCZY KRS', N'4240@XIV', N'SGOS', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2175, N'4240', N'XV', N'XV WYDZIA£ GOSPODARCZY', N'4240@XV', N'SGOS', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2176, N'4240', N'XVI', N'XVI WYDZIA£ GOSPODARCZY', N'4240@XVI', N'SGOS', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2177, N'4241', N'I', N'I WYDZIA£ CYWILNY', N'4241@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2178, N'4241', N'II', N'II WYDZIA£ CYWILNY', N'4241@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2179, N'4241', N'III', N'III WYDZIA£ KARNY', N'4241@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2180, N'4241', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4241@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2181, N'4241', N'VI', N'VI WYDZIA£ KSI¥G WIECZYSTYCH', N'4241@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2182, N'4241', N'VII', N'VII WYDZIA£ KSI¥G WIECZYSTYCH', N'4241@VII', N'SCYW', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2183, N'4241', N'VIII', N'VIII WYDZIA£ KARNY', N'4241@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2184, N'4241', N'IX', N'IX WYDZIA£ KSI¥G WIECZYSTYCH', N'4241@IX', N'SCYW', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2185, N'4241', N'X', N'X WYDZIA£ KSI¥G WIECZYSTYCH', N'4241@X', N'SCYW', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2186, N'4241', N'XII', N'XII WYDZIA£ DS. WYKONYWANIA ORZECZEÑ', N'4241@XII', N'SKAR', 12)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2187, N'4241', N'XIII', N'XIII WYDZIA£ KSI¥G WIECZYSTYCH', N'4241@XIII', N'SCYW', 13)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2188, N'4241', N'XIV', N'XIV WYDZIA£ KARNY', N'4241@XIV', N'SKAR', 14)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2189, N'4241', N'XV', N'XV WYDZIA£ KSI¥G WIECZYSTYCH', N'4241@XV', N'SCYW', 15)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2190, N'4241', N'XVI', N'XVI WYDZIA£ CYWILNY', N'4241@XVI', N'SCYW', 16)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2191, N'4242', N'I', N'I WYDZIA£ CYWILNY', N'4242@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2192, N'4242', N'II', N'II WYDZIA£ KARNY', N'4242@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2193, N'4242', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4242@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2194, N'4242', N'V', N'V WYDZIA£ KARNY', N'4242@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2195, N'4242', N'VI', N'VI WYDZIA£ CYWILNY', N'4242@VI', N'SCYW', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2196, N'4242', N'VIII', N'VIII WYDZ PRACY I UBEZPIECZEÑ SPO£', N'4242@VIII', N'SUBE', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2197, N'4242', N'X', N'X WYDZIA£ KARNY', N'4242@X', N'SKAR', 10)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2198, N'4242', N'XI', N'XI WYDZIA£ KARNY', N'4242@XI', N'SKAR', 11)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2199, N'4243', N'I', N'I WYDZIA£ CYWILNY', N'4243@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2200, N'4243', N'II', N'II WYDZIA£ CYWILNY', N'4243@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2201, N'4243', N'III', N'III WYDZIA£ KARNY', N'4243@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2202, N'4243', N'IV', N'IV WYDZIA£ KARNY', N'4243@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2203, N'4243', N'V', N'V WYDZIA£ KARNY', N'4243@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2204, N'4243', N'VI', N'VI WYDZIA£ RODZINNY I NIELETNICH', N'4243@VI', N'SROD', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2205, N'4243', N'VII', N'VII WYDZIA£ WYKONYWANIA ORZECZEÑ', N'4243@VII', N'SKAR', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2206, N'4244', N'I', N'I WYDZIA£ CYWILNY', N'4244@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2207, N'4244', N'II', N'II WYDZIA£ CYWILNY', N'4244@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2208, N'4244', N'III', N'III WYDZIA£ KARNY', N'4244@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2209, N'4244', N'IV', N'IV WYDZIA£ KARNY', N'4244@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2210, N'4244', N'V', N'V WYDZIA£ DS. WYKONYWANIA ORZECZEÑ', N'4244@V', N'SKAR', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2211, N'4244', N'VI', N'VI WYDZIA£ RODZINNY I NIELETNICH', N'4244@VI', N'SROD', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2212, N'4244', N'VII', N'VII WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4244@VII', N'SUBE', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2213, N'4245', N'I', N'I WYDZIA£ CYWILNY', N'4245@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2214, N'4245', N'II', N'II WYDZIA£ KARNY', N'4245@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2215, N'4245', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4245@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2216, N'4245', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4245@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2217, N'4246', N'I', N'I WYDZIA£ CYWILNY', N'4246@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2218, N'4246', N'II', N'II WYDZIA£ KARNY', N'4246@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2219, N'4246', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4246@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2220, N'4246', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4246@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2221, N'4247', N'I', N'I WYDZIA£ CYWILNY', N'4247@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2222, N'4247', N'II', N'II WYDZIA£ KARNY', N'4247@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2223, N'4247', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4247@III', N'SROD', 3)
GO
print 'Processed 2200 total records'
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2224, N'4247', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4247@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2225, N'4248', N'I', N'I WYDZIA£ CYWILNY', N'4248@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2226, N'4248', N'II', N'II WYDZIA£ CYWILNY', N'4248@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2227, N'4248', N'III', N'III WYDZIA£ KARNY', N'4248@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2228, N'4248', N'IV', N'IV WYDZIA£ KARNY', N'4248@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2229, N'4248', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4248@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2230, N'4248', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4248@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2231, N'4249', N'I', N'I WYDZIA£ CYWILNY', N'4249@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2232, N'4249', N'II', N'II WYDZIA£ CYWILNY', N'4249@II', N'SCYW', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2233, N'4249', N'III', N'III WYDZIA£ KARNY', N'4249@III', N'SKAR', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2234, N'4249', N'IV', N'IV WYDZIA£ KARNY', N'4249@IV', N'SKAR', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2235, N'4249', N'V', N'V WYDZIA£ RODZINNY I NIELETNICH', N'4249@V', N'SROD', 5)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2236, N'4249', N'VI', N'VI WYDZ PRACY I UBEZPIECZEÑ SPO£ECZNYCH', N'4249@VI', N'SUBE', 6)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2237, N'4249', N'VII', N'VII WYDZIA£ GOSPODARCZY', N'4249@VII', N'SGOS', 7)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2238, N'4249', N'VIII', N'VIII WYDZIA£ KARNY', N'4249@VIII', N'SKAR', 8)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2239, N'4249', N'IX', N'IX WYDZ. GOSPODARCZY DS.UPAD£.I NAPRAW.', N'4249@IX', N'SGOS', 9)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2240, N'4250', N'I', N'I WYDZIA£ CYWILNY', N'4250@I', N'SCYW', 1)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2241, N'4250', N'II', N'II WYDZIA£ KARNY', N'4250@II', N'SKAR', 2)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2242, N'4250', N'III', N'III WYDZIA£ RODZINNY I NIELETNICH', N'4250@III', N'SROD', 3)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2243, N'4250', N'IV', N'IV WYDZIA£ KSI¥G WIECZYSTYCH', N'4250@IV', N'SCYW', 4)
INSERT [dbo].[SAPWydzial] ([Id], [kodSad], [numer], [nazwa], [sadWydzial], [rodzajSprawy], [numerWydz]) VALUES (2244, N'4250', N'V', N'V WYDZIA£ KARNY', N'4250@V', N'SKAR', 5)
SET IDENTITY_INSERT [dbo].[SAPWydzial] OFF
/****** Object:  Table [dbo].[SAPTomyAkt]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPTomyAkt](
	[Kod] [varchar](3) NOT NULL,
	[Opis] [varchar](50) NULL,
 CONSTRAINT [PK_SAPTomyAkt] PRIMARY KEY CLUSTERED 
(
	[Kod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[SAPTomyAkt] ([Kod], [Opis]) VALUES (N'001', N'Nie dotyczy')
INSERT [dbo].[SAPTomyAkt] ([Kod], [Opis]) VALUES (N'002', N'do 5 tomów')
INSERT [dbo].[SAPTomyAkt] ([Kod], [Opis]) VALUES (N'003', N'5-20 tomów')
INSERT [dbo].[SAPTomyAkt] ([Kod], [Opis]) VALUES (N'004', N'powy¿ej 20 tomów')
/****** Object:  Table [dbo].[SAPStanNal]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPStanNal](
	[Kod] [varchar](50) NOT NULL,
	[Opis] [varchar](50) NULL,
	[grzywnakoszty] [char](1) NULL,
 CONSTRAINT [PK_SAPStanNal] PRIMARY KEY CLUSTERED 
(
	[Kod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[SAPStanNal] ([Kod], [Opis], [grzywnakoszty]) VALUES (N'A', N'Wezwania', N'a')
INSERT [dbo].[SAPStanNal] ([Kod], [Opis], [grzywnakoszty]) VALUES (N'B', N'Roz³o¿ono na raty', N'a')
INSERT [dbo].[SAPStanNal] ([Kod], [Opis], [grzywnakoszty]) VALUES (N'C', N'Egzekucja komornicza', N'a')
INSERT [dbo].[SAPStanNal] ([Kod], [Opis], [grzywnakoszty]) VALUES (N'D', N'Grzywny odroczone', N'g')
INSERT [dbo].[SAPStanNal] ([Kod], [Opis], [grzywnakoszty]) VALUES (N'E', N'Inne', N'a')
INSERT [dbo].[SAPStanNal] ([Kod], [Opis], [grzywnakoszty]) VALUES (N'F', N'Grzywna uprzednio odpisana', N'g')
/****** Object:  Table [dbo].[SAPSad]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPSad](
	[kod] [char](4) NOT NULL,
	[sad] [varchar](100) NULL,
	[miasto] [varchar](60) NULL,
	[miastSad] [varchar](160) NULL,
	[typSad] [varchar](2) NULL,
	[JEGO] [char](4) NULL,
 CONSTRAINT [PK_SAPSady] PRIMARY KEY CLUSTERED 
(
	[kod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'    ', N'<Nieokreœlony>', N'<Brak>', N'<Nieokreœlony>', NULL, N'    ')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'1000', N'Ministerstwo Sprawiedliw', N'Warszawa', N'Warszawa Ministerstwo Sprawiedliw', N'MS', N'1000')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2002', N'S¹d Apelacyjny Warszawa', N'Warszawa', N'Warszawa S¹d Apelacyjny Warszawa', N'SA', N'2002')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2003', N'S¹d Apelacyjny Katowice', N'Katowice', N'Katowice S¹d Apelacyjny Katowice', N'SA', N'2003')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2004', N'S¹d Apelacyjny Gdañsk', N'Gdañsk', N'Gdañsk S¹d Apelacyjny Gdañsk', N'SA', N'2004')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2005', N'S¹d Apelacyjny Poznaniu', N'Poznaniu', N'Poznaniu S¹d Apelacyjny Poznaniu', N'SA', N'2005')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2006', N'S¹d Apelacyjny Kraków', N'Kraków', N'Kraków S¹d Apelacyjny Kraków', N'SA', N'2006')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2007', N'S¹d Apelacyjny Wroc³aw', N'Wroc³aw', N'Wroc³aw S¹d Apelacyjny Wroc³aw', N'SA', N'2007')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2008', N'S¹d Apelacyjny £ódŸ', N'£ódŸ', N'£ódŸ S¹d Apelacyjny £ódŸ', N'SA', N'2008')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2009', N'S¹d Apelacyjny Rzeszów', N'Rzeszów', N'Rzeszów S¹d Apelacyjny Rzeszów', N'SA', N'2009')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2010', N'S¹d Apelacyjny Bia³ystok', N'Bia³ystok', N'Bia³ystok S¹d Apelacyjny Bia³ystok', N'SA', N'2010')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2011', N'S¹d Apelacyjny Lublin', N'Lublin', N'Lublin S¹d Apelacyjny Lublin', N'SA', N'2011')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'2012', N'S¹d Apelacyjny Szczecin', N'Szczecin', N'Szczecin S¹d Apelacyjny Szczecin', N'SA', N'2012')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3001', N'SO w Jeleniej Górze', N'Jelenia Góra', N'Jelenia Góra SO w Jeleniej Górze', N'SO', N'3001')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3002', N'SO w Legnicy', N'Legnica', N'Legnica SO w Legnicy', N'SO', N'3002')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3003', N'SO w Opolu', N'Opole', N'Opole SO w Opolu', N'SO', N'3003')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3004', N'SO w Œwidnicy', N'Œwidnica', N'Œwidnica SO w Œwidnicy', N'SO', N'3004')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3005', N'SO we Wroc³awiu', N'Wroc³aw', N'Wroc³aw SO we Wroc³awiu', N'SO', N'3005')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3006', N'SO w Bia³ymstoku', N'Bia³ystok', N'Bia³ystok SO w Bia³ymstoku', N'SO', N'3006')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3007', N'SO w £om¿y', N'£om¿a', N'£om¿a SO w £om¿y', N'SO', N'3007')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3008', N'SO w Olsztynie', N'Olsztyn', N'Olsztyn SO w Olsztynie', N'SO', N'3008')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3009', N'SO w Suwa³kach', N'Suwa³ki', N'Suwa³ki SO w Suwa³kach', N'SO', N'3009')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3010', N'SO w Bydgoszczy', N'Bydgoszcz', N'Bydgoszcz SO w Bydgoszczy', N'SO', N'3010')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3011', N'SO w Elbl¹gu', N'Elbl¹g', N'Elbl¹g SO w Elbl¹gu', N'SO', N'3011')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3012', N'SO w Gdañsku', N'Gdañsk', N'Gdañsk SO w Gdañsku', N'SO', N'3012')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3013', N'SO w S³upsku', N'S³upsk', N'S³upsk SO w S³upsku', N'SO', N'3013')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3014', N'SO w Toruniu', N'Toruñ', N'Toruñ SO w Toruniu', N'SO', N'3014')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3015', N'SO we W³oc³awku', N'W³oc³awek', N'W³oc³awek SO we W³oc³awku', N'SO', N'3015')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3016', N'SO w Bielsku-Bia³ej', N'Bielsko-Bia³a', N'Bielsko-Bia³a SO w Bielsku-Bia³ej', N'SO', N'3016')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3017', N'SO w Czêstochowie', N'Czêstochowa', N'Czêstochowa SO w Czêstochowie', N'SO', N'3017')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3018', N'SO w Gliwicach', N'Gliwice', N'Gliwice SO w Gliwicach', N'SO', N'3018')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3019', N'SO w Katowicach', N'Katowice', N'Katowice SO w Katowicach', N'SO', N'3019')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3020', N'SO w Kielcach', N'Kielce', N'Kielce SO w Kielcach', N'SO', N'3020')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3021', N'SO w Krakowie', N'Kraków', N'Kraków SO w Krakowie', N'SO', N'3021')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3022', N'SO w Nowym S¹czu', N'Nowy S¹cz', N'Nowy S¹cz SO w Nowym S¹czu', N'SO', N'3022')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3023', N'SO w Tarnowie', N'Tarnów', N'Tarnów SO w Tarnowie', N'SO', N'3023')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3024', N'SO w Lublinie', N'Lublin', N'Lublin SO w Lublinie', N'SO', N'3024')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3025', N'SO w Radomiu', N'Radom', N'Radom SO w Radomiu', N'SO', N'3025')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3026', N'SO w Siedlcach', N'Siedlce', N'Siedlce SO w Siedlcach', N'SO', N'3026')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3027', N'SO w Zamoœciu', N'Zamoœæ', N'Zamoœæ SO w Zamoœciu', N'SO', N'3027')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3028', N'SO w Kaliszu', N'Kalisz', N'Kalisz SO w Kaliszu', N'SO', N'3028')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3029', N'SO w £odzi', N'£ódŸ', N'£ódŸ SO w £odzi', N'SO', N'3029')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3030', N'SO w Piotrkowie Tryb.', N'Piotrków Tryb.', N'Piotrków Tryb. SO w Piotrkowie Tryb.', N'SO', N'3030')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3031', N'SO w Sieradzu', N'Sieradz', N'Sieradz SO w Sieradzu', N'SO', N'3031')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3032', N'SO w Koninie', N'Konin', N'Konin SO w Koninie', N'SO', N'3032')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3033', N'SO w Poznaniu', N'Poznañ', N'Poznañ SO w Poznaniu', N'SO', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3034', N'SO w Zielonej Górze', N'Zielona Góra', N'Zielona Góra SO w Zielonej Górze', N'SO', N'3034')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3035', N'SO w Kroœnie', N'Krosno', N'Krosno SO w Kroœnie', N'SO', N'3035')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3036', N'SO w Przemyœlu', N'Przemyœl', N'Przemyœl SO w Przemyœlu', N'SO', N'3036')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3037', N'SO w Rzeszowie', N'Rzeszów', N'Rzeszów SO w Rzeszowie', N'SO', N'3037')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3038', N'SO w Tarnobrzegu', N'Tarnobrzeg', N'Tarnobrzeg SO w Tarnobrzegu', N'SO', N'3038')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3039', N'SO w Gorzowie Wlkp.', N'Gorzów Wlkp.', N'Gorzów Wlkp. SO w Gorzowie Wlkp.', N'SO', N'3039')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3040', N'SO w Koszalinie', N'Koszalin', N'Koszalin SO w Koszalinie', N'SO', N'3040')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3041', N'SO w Szczecinie', N'Szczecin', N'Szczecin SO w Szczecinie', N'SO', N'3041')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3042', N'SO w Ostro³êce', N'Ostro³êka', N'Ostro³êka SO w Ostro³êce', N'SO', N'3042')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3043', N'SO w P³ocku', N'P³ock', N'P³ock SO w P³ocku', N'SO', N'3043')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3044', N'SO w Warszawie', N'Warszawa', N'Warszawa SO w Warszawie', N'SO', N'3044')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'3045', N'SO Warszawa - Praga', N'Warszawa', N'Warszawa SO Warszawa - Praga', N'SO', N'3045')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4001', N'SR w Jeleniej Górze', N'Jelenia Góra', N'Jelenia Góra SR w Jeleniej Górze', N'SR', N'4001')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4002', N'SR w Legnicy', N'Legnica', N'Legnica SR w Legnicy', N'SR', N'4002')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4003', N'SR w G³ogowie', N'G³ogów', N'G³ogów SR w G³ogowie', N'SR', N'4003')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4004', N'SR w Kêdzierzynie-KoŸlu', N'Kêdzierzyn-KoŸle', N'Kêdzierzyn-KoŸle SR w Kêdzierzynie-KoŸlu', N'SR', N'4004')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4005', N'SR w Kluczborku', N'Kluczbork', N'Kluczbork SR w Kluczborku', N'SR', N'4005')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4006', N'SR w Nysie', N'Nysa', N'Nysa SR w Nysie', N'SR', N'4006')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4007', N'SR w Opolu', N'Opole', N'Opole SR w Opolu', N'SR', N'4007')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4008', N'SR w K³odzku', N'K³odzko', N'K³odzko SR w K³odzku', N'SR', N'4008')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4009', N'SR w Œwidnicy', N'Œwidnica', N'Œwidnica SR w Œwidnicy', N'SR', N'4009')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4010', N'SR w Wa³brzychu', N'Wa³brzych', N'Wa³brzych SR w Wa³brzychu', N'SR', N'4010')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4011', N'SR Wroc³aw-Fabryczna', N'Wroc³aw', N'Wroc³aw SR Wroc³aw-Fabryczna', N'SR', N'4011')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4012', N'SR Wroc³aw-Krzyki', N'Wroc³aw', N'Wroc³aw SR Wroc³aw-Krzyki', N'SR', N'4012')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4013', N'SR Wroc³aw-Œródmieœcie', N'Wroc³aw', N'Wroc³aw SR Wroc³aw-Œródmieœcie', N'SR', N'4013')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4014', N'SR w Oleœnicy', N'Oleœnica', N'Oleœnica SR w Oleœnicy', N'SR', N'4014')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4016', N'SR w Lubinie', N'Lubin', N'Lubin SR w Lubinie', N'SR', N'4016')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4017', N'SR w Dzier¿oniowie', N'Dzier¿oniów', N'Dzier¿oniów SR w Dzier¿oniowie', N'SR', N'4017')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4019', N'SR w O³awie', N'O³awa', N'O³awa SR w O³awie', N'SR', N'4019')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4021', N'SR w Œrodzie Œl¹skie', N'Œroda Œl¹ska', N'Œroda Œl¹ska SR w Œrodzie Œl¹skie', N'SR', N'4021')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4022', N'SR w Trzebnicy', N'Trzebnica', N'Trzebnica SR w Trzebnicy', N'SR', N'4022')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4024', N'SR w Boles³awcu', N'Boles³awiec', N'Boles³awiec SR w Boles³awcu', N'SR', N'4024')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4026', N'SR w Lubaniu', N'Lubañ', N'Lubañ SR w Lubaniu', N'SR', N'4026')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4027', N'SR w Zgorzelcu', N'Zgorzelec', N'Zgorzelec SR w Zgorzelcu', N'SR', N'4027')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4029', N'SR w Z³otoryi', N'Z³otoryja', N'Z³otoryja SR w Z³otoryi', N'SR', N'4029')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4030', N'SR w Brzegu', N'Brzeg', N'Brzeg SR w Brzegu', N'SR', N'4030')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4033', N'SR w Prudniku', N'Prudnik', N'Prudnik SR w Prudniku', N'SR', N'4033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4034', N'SR w Strzelcach Opolskich', N'Strzelce Opolskie', N'Strzelce Opolskie SR w Strzelcach Opolskich', N'SR', N'4034')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4035', N'SR w Z¹bkowicach', N'Z¹bkowice', N'Z¹bkowice SR w Z¹bkowicach', N'SR', N'4035')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4036', N'SR w Bia³ymstoku', N'Bia³ystok', N'Bia³ystok SR w Bia³ymstoku', N'SR', N'4036')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4037', N'SR w Bielsku Podlaskim', N'Bielsk Podlaski', N'Bielsk Podlaski SR w Bielsku Podlaskim', N'SR', N'4037')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4038', N'SR w Sokó³ce', N'Sokó³ka', N'Sokó³ka SR w Sokó³ce', N'SR', N'4038')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4039', N'SR w £om¿y', N'£om¿a', N'£om¿a SR w £om¿y', N'SR', N'4039')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4040', N'SR w Zambrowie', N'Zambrów', N'Zambrów SR w Zambrowie', N'SR', N'4040')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4041', N'SR w Bartoszycach', N'Bartoszyce', N'Bartoszyce SR w Bartoszycach', N'SR', N'4041')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4042', N'SR w Gi¿ycku', N'Gi¿ycko', N'Gi¿ycko SR w Gi¿ycku', N'SR', N'4042')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4043', N'SR w Kêtrzynie', N'Kêtrzyn', N'Kêtrzyn SR w Kêtrzynie', N'SR', N'4043')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4044', N'SR w Mr¹gowie', N'Mr¹gowo', N'Mr¹gowo SR w Mr¹gowie', N'SR', N'4044')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4045', N'SR w Olsztynie', N'Olsztyn', N'Olsztyn SR w Olsztynie', N'SR', N'4045')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4046', N'SR w Szczytnie', N'Szczytno', N'Szczytno SR w Szczytnie', N'SR', N'4046')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4047', N'SR w Ostro³êce', N'Ostro³êka', N'Ostro³êka SR w Ostro³êce', N'SR', N'4047')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4048', N'SR w Ostrowi Mazow.', N'Ostrów Mazow.', N'Ostrów Mazow. SR w Ostrowi Mazow.', N'SR', N'4048')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4049', N'SR w Przasnyszu', N'Przasnysz', N'Przasnysz SR w Przasnyszu', N'SR', N'4049')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4050', N'SR w Wyszkowie', N'Wyszków', N'Wyszków SR w Wyszkowie', N'SR', N'4050')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4051', N'SR w Augustowie', N'Augustów', N'Augustów SR w Augustowie', N'SR', N'4051')
GO
print 'Processed 100 total records'
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4052', N'SR w E³ku', N'E³k', N'E³k SR w E³ku', N'SR', N'4052')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4053', N'SR w Olecku', N'Olecko', N'Olecko SR w Olecku', N'SR', N'4053')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4054', N'SR w Suwa³kach', N'Suwa³ki', N'Suwa³ki SR w Suwa³kach', N'SR', N'4054')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4055', N'SR w Bydgoszczy', N'Bydgoszcz', N'Bydgoszcz SR w Bydgoszczy', N'SR', N'4055')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4056', N'SR w Inowroc³awiu', N'Inowroc³aw', N'Inowroc³aw SR w Inowroc³awiu', N'SR', N'4056')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4057', N'SR w Szubinie', N'Szubin', N'Szubin SR w Szubinie', N'SR', N'4057')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4058', N'SR w Œwieciu', N'Œwiecie', N'Œwiecie SR w Œwieciu', N'SR', N'4058')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4059', N'SR w Braniewie', N'Braniewo', N'Braniewo SR w Braniewie', N'SR', N'4059')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4060', N'SR w Elbl¹gu', N'Elbl¹g', N'Elbl¹g SR w Elbl¹gu', N'SR', N'4060')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4061', N'SR w I³awie', N'I³awa', N'I³awa SR w I³awie', N'SR', N'4061')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4062', N'SR w Ostródzie', N'Ostróda', N'Ostróda SR w Ostródzie', N'SR', N'4062')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4063', N'SR Gdañsk - Po³udnie', N'Gdañsk', N'Gdañsk SR Gdañsk - Po³udnie', N'SR', N'4063')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4064', N'SR Gdañsk - Pó³noc', N'Gdañsk', N'Gdañsk SR Gdañsk - Pó³noc', N'SR', N'4064')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4065', N'SR w Gdyni', N'Gdyni', N'Gdyni SR w Gdyni', N'SR', N'4065')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4066', N'SR w Kartuzach', N'Kartuzy', N'Kartuzy SR w Kartuzach', N'SR', N'4066')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4067', N'SR w Kwidzynie', N'Kwidzyn', N'Kwidzyn SR w Kwidzynie', N'SR', N'4067')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4068', N'SR w Malborku', N'Malbork', N'Malbork SR w Malborku', N'SR', N'4068')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4069', N'SR w Sopocie', N'Sopot', N'Sopot SR w Sopocie', N'SR', N'4069')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4070', N'SR Starogard Gdañski', N'Starogard Gdañski', N'Starogard Gdañski SR Starogard Gdañski', N'SR', N'4070')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4071', N'SR w Tczewie', N'Tczew', N'Tczew SR w Tczewie', N'SR', N'4071')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4072', N'SR w Wejherowie', N'Wejherowo', N'Wejherowo SR w Wejherowie', N'SR', N'4072')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4073', N'SR w Chojnicach', N'Chojnice', N'Chojnice SR w Chojnicach', N'SR', N'4073')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4074', N'SR w Cz³uchowie', N'Cz³uchów', N'Cz³uchów SR w Cz³uchowie', N'SR', N'4074')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4075', N'SR w Lêborku', N'Lêbork', N'Lêbork SR w Lêborku', N'SR', N'4075')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4076', N'SR w S³upsku', N'S³upsk', N'S³upsk SR w S³upsku', N'SR', N'4076')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4077', N'SR w Brodnicy', N'Brodnica', N'Brodnica SR w Brodnicy', N'SR', N'4077')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4078', N'SR w Che³mnie', N'Che³mno', N'Che³mno SR w Che³mnie', N'SR', N'4078')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4079', N'SR w Grudzi¹dzu', N'Grudzi¹dz', N'Grudzi¹dz SR w Grudzi¹dzu', N'SR', N'4079')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4080', N'SR w Toruniu', N'Toruñ', N'Toruñ SR w Toruniu', N'SR', N'4080')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4081', N'SR w Aleksandrowie Kuj.', N'Aleksandrów Kuj.', N'Aleksandrów Kuj. SR w Aleksandrowie Kuj.', N'SR', N'4081')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4082', N'SR w Lipnie', N'Lipno', N'Lipno SR w Lipnie', N'SR', N'4082')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4083', N'SR we W³oc³awku', N'W³oc³awek', N'W³oc³awek SR we W³oc³awku', N'SR', N'4083')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4084', N'SR w Bielsku-Bia³ej', N'Bielsko-Bia³a', N'Bielsko-Bia³a SR w Bielsku-Bia³ej', N'SR', N'4084')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4085', N'SR w Cieszynie', N'Cieszyn', N'Cieszyn SR w Cieszynie', N'SR', N'4085')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4086', N'SR w ¯ywcu', N'¯ywiec', N'¯ywiec SR w ¯ywcu', N'SR', N'4086')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4087', N'SR w Czêstochowie', N'Czêstochowa', N'Czêstochowa SR w Czêstochowie', N'SR', N'4087')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4088', N'SR w Lubliñcu', N'Lubliniec', N'Lubliniec SR w Lubliñcu', N'SR', N'4088')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4089', N'SR w Myszkowie', N'Myszków', N'Myszków SR w Myszkowie', N'SR', N'4089')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4090', N'SR w Zawierciu', N'Zawiercie', N'Zawiercie SR w Zawierciu', N'SR', N'4090')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4091', N'SR w Gliwicach', N'Gliwice', N'Gliwice SR w Gliwicach', N'SR', N'4091')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4092', N'SR w Jastrzêbiu-Zdroju', N'Jastrzêbie-Zdrój', N'Jastrzêbie-Zdrój SR w Jastrzêbiu-Zdroju', N'SR', N'4092')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4093', N'SR w Raciborzu', N'Racibórz', N'Racibórz SR w Raciborzu', N'SR', N'4093')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4094', N'SR w Rudzie Œl¹skiej', N'Ruda Œl¹ska', N'Ruda Œl¹ska SR w Rudzie Œl¹skiej', N'SR', N'4094')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4095', N'SR w Rybniku', N'Rybnik', N'Rybnik SR w Rybniku', N'SR', N'4095')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4096', N'SR w Tarnowskich Górach', N'Tarnowskie Góry', N'Tarnowskie Góry SR w Tarnowskich Górach', N'SR', N'4096')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4097', N'SR w Wodzis³awiu Œl¹skim', N'Wodzis³aw Œl¹ski', N'Wodzis³aw Œl¹ski SR w Wodzis³awiu Œl¹skim', N'SR', N'4097')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4098', N'SR w Zabrzu', N'Zabrze', N'Zabrze SR w Zabrzu', N'SR', N'4098')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4099', N'SR w ¯orach', N'¯ory', N'¯ory SR w ¯orach', N'SR', N'4099')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4100', N'SR w Bêdzinie', N'Bêdzin', N'Bêdzin SR w Bêdzinie', N'SR', N'4100')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4101', N'SR w Bytomiu', N'Bytom', N'Bytom SR w Bytomiu', N'SR', N'4101')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4102', N'SR w Chorzowie', N'Chorzów', N'Chorzów SR w Chorzowie', N'SR', N'4102')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4103', N'SR w D¹browie Górniczej', N'D¹browa Górnicza', N'D¹browa Górnicza SR w D¹browie Górniczej', N'SR', N'4103')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4104', N'SR w Jaworznie', N'Jaworzno', N'Jaworzno SR w Jaworznie', N'SR', N'4104')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4105', N'SR w Katowicach Wsch.', N'Katowice', N'Katowice SR w Katowicach Wsch.', N'SR', N'4105')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4106', N'SR w Katowicach Zach.', N'Katowice', N'Katowice SR w Katowicach Zach.', N'SR', N'4106')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4107', N'SR w Miko³owie', N'Miko³ów', N'Miko³ów SR w Miko³owie', N'SR', N'4107')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4108', N'SR w Mys³owicach', N'Mys³owice', N'Mys³owice SR w Mys³owicach', N'SR', N'4108')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4109', N'SR w Pszczynie', N'Pszczyna', N'Pszczyna SR w Pszczynie', N'SR', N'4109')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4110', N'SR w Siemianowicach Œl.', N'Siemianowice Œl.', N'Siemianowice Œl. SR w Siemianowicach Œl.', N'SR', N'4110')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4111', N'SR w Sosnowcu', N'Sosnowiec', N'Sosnowiec SR w Sosnowcu', N'SR', N'4111')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4112', N'SR w Tychach', N'Tychy', N'Tychy SR w Tychach', N'SR', N'4112')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4113', N'SR w Busku Zdroju', N'Busk Zdrój', N'Busk Zdrój SR w Busku Zdroju', N'SR', N'4113')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4114', N'SR w Jêdrzejowie', N'Jêdrzejów', N'Jêdrzejów SR w Jêdrzejowie', N'SR', N'4114')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4115', N'SR w Kielcach', N'Kielce', N'Kielce SR w Kielcach', N'SR', N'4115')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4116', N'SR w Koñskich', N'Koñskie', N'Koñskie SR w Koñskich', N'SR', N'4116')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4117', N'SR w Ostrowcu Œwiêtokrz.', N'Ostrowiec Œwiêtokrz.', N'Ostrowiec Œwiêtokrz. SR w Ostrowcu Œwiêtokrz.', N'SR', N'4117')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4118', N'SR w Sandomierzu', N'Sandomierz', N'Sandomierz SR w Sandomierzu', N'SR', N'4118')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4119', N'SR w Skra¿ysku-Kamiennej', N'Skra¿ysko-Kamienna', N'Skra¿ysko-Kamienna SR w Skra¿ysku-Kamiennej', N'SR', N'4119')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4120', N'SR w Starachowicach', N'Starachowice', N'Starachowice SR w Starachowicach', N'SR', N'4120')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4121', N'SR w Chrzanowie', N'Chrzanów', N'Chrzanów SR w Chrzanowie', N'SR', N'4121')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4122', N'SR dla Krakowa-Krowodrzy', N'Kraków', N'Kraków SR dla Krakowa-Krowodrzy', N'SR', N'4122')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4123', N'SR dla Krakowa-Nowej Huty', N'Kraków', N'Kraków SR dla Krakowa-Nowej Huty', N'SR', N'4123')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4124', N'SR dla Krakowa-Podgórza', N'Kraków', N'Kraków SR dla Krakowa-Podgórza', N'SR', N'4124')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4125', N'SR dla Krakowa-Œródmieœc.', N'Kraków', N'Kraków SR dla Krakowa-Œródmieœc.', N'SR', N'4125')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4126', N'SR w Myœlenicach', N'Myœlenice', N'Myœlenice SR w Myœlenicach', N'SR', N'4126')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4127', N'SR w Olkuszu', N'Olkusz', N'Olkusz SR w Olkuszu', N'SR', N'4127')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4128', N'SR w Oœwiêcimiu', N'Oœwiêcim', N'Oœwiêcim SR w Oœwiêcimiu', N'SR', N'4128')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4129', N'SR w Wadowicach', N'Wadowice', N'Wadowice SR w Wadowicach', N'SR', N'4129')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4130', N'SR w Wieliczce', N'Wieliczka', N'Wieliczka SR w Wieliczce', N'SR', N'4130')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4131', N'SR w Gorlicach', N'Gorlice', N'Gorlice SR w Gorlicach', N'SR', N'4131')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4132', N'SR w Limanowej', N'Limanowa', N'Limanowa SR w Limanowej', N'SR', N'4132')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4133', N'SR w Nowym S¹czu', N'Nowy S¹cz', N'Nowy S¹cz SR w Nowym S¹czu', N'SR', N'4133')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4134', N'SR w Nowym Targu', N'Nowy Targ', N'Nowy Targ SR w Nowym Targu', N'SR', N'4134')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4135', N'SR w Zakopanem', N'Zakopane', N'Zakopane SR w Zakopanem', N'SR', N'4135')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4136', N'SR w Bochni', N'Bochnia', N'Bochnia SR w Bochni', N'SR', N'4136')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4137', N'SR w Brzesku', N'Brzesko', N'Brzesko SR w Brzesku', N'SR', N'4137')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4138', N'SR w Tarnowie', N'Tarnów', N'Tarnów SR w Tarnowie', N'SR', N'4138')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4139', N'SR w Bia³ej Podlaskiej', N'Bia³a Podlaska', N'Bia³a Podlaska SR w Bia³ej Podlaskiej', N'SR', N'4139')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4140', N'SR w Che³mie', N'Che³m', N'Che³m SR w Che³mie', N'SR', N'4140')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4141', N'SR w Kraœniku', N'Kraœnik', N'Kraœnik SR w Kraœniku', N'SR', N'4141')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4142', N'SR w Lubartowie', N'Lubartów', N'Lubartów SR w Lubartowie', N'SR', N'4142')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4143', N'SR Lublin-Wschód', N'Lublin', N'Lublin SR Lublin-Wschód', N'SR', N'4143')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4144', N'SR Lublin-Zachód', N'Lublin', N'Lublin SR Lublin-Zachód', N'SR', N'4144')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4145', N'SR w £ukowie', N'£uków', N'£uków SR w £ukowie', N'SR', N'4145')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4146', N'SR w Pu³awach', N'Pu³awy', N'Pu³awy SR w Pu³awach', N'SR', N'4146')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4147', N'SR w Radzyniu Podl.', N'Radzyñ Podl.', N'Radzyñ Podl. SR w Radzyniu Podl.', N'SR', N'4147')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4148', N'SR w Grójcu', N'Grójec', N'Grójec SR w Grójcu', N'SR', N'4148')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4149', N'SR w Kozienicach', N'Kozienice', N'Kozienice SR w Kozienicach', N'SR', N'4149')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4150', N'SR w Przysusze', N'Przysucha', N'Przysucha SR w Przysusze', N'SR', N'4150')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4151', N'SR w Radomiu', N'Radom', N'Radom SR w Radomiu', N'SR', N'4151')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4152', N'SR w Garwolinie', N'Garwolin', N'Garwolin SR w Garwolinie', N'SR', N'4152')
GO
print 'Processed 200 total records'
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4153', N'SR w Miñsku Maz.', N'Miñsk Maz.', N'Miñsk Maz. SR w Miñsku Maz.', N'SR', N'4153')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4154', N'SR w Siedlcach', N'Siedlce', N'Siedlce SR w Siedlcach', N'SR', N'4154')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4155', N'SR w Wêgrowie', N'Wêgrów', N'Wêgrów SR w Wêgrowie', N'SR', N'4155')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4156', N'SR w Bi³goraju', N'Bi³goraj', N'Bi³goraj SR w Bi³goraju', N'SR', N'4156')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4157', N'SR w Hrubieszowie', N'Hrubieszów', N'Hrubieszów SR w Hrubieszowie', N'SR', N'4157')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4158', N'SR w Tomaszowie Lub.', N'Tomaszów Lub.', N'Tomaszów Lub. SR w Tomaszowie Lub.', N'SR', N'4158')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4159', N'SR w Zamoœciu', N'Zamoœæ', N'Zamoœæ SR w Zamoœciu', N'SR', N'4159')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4160', N'SR w Jarocinie', N'Jarocin', N'Jarocin SR w Jarocinie', N'SR', N'4160')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4161', N'SR w Kaliszu', N'Kalisz', N'Kalisz SR w Kaliszu', N'SR', N'4161')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4162', N'SR w Kêpnie', N'Kêpno', N'Kêpno SR w Kêpnie', N'SR', N'4162')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4163', N'SR w Ostrowie Wlkp.', N'Ostrów Wlkp.', N'Ostrów Wlkp. SR w Ostrowie Wlkp.', N'SR', N'4163')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4164', N'SR w Kutnie', N'Kutno', N'Kutno SR w Kutnie', N'SR', N'4164')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4165', N'SR w £owiczu', N'£owicz', N'£owicz SR w £owiczu', N'SR', N'4165')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4166', N'SR dla £odzi-Œródmieœcia', N'£ódŸ', N'£ódŸ SR dla £odzi-Œródmieœcia', N'SR', N'4166')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4167', N'SR dla £odzi-Widzewa', N'£ódŸ', N'£ódŸ SR dla £odzi-Widzewa', N'SR', N'4167')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4168', N'SR w Pabianicach', N'Pabianice', N'Pabianice SR w Pabianicach', N'SR', N'4168')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4169', N'SR w Skierniewicach', N'Skierniewice', N'Skierniewice SR w Skierniewicach', N'SR', N'4169')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4170', N'SR w Zgierzu', N'Zgierz', N'Zgierz SR w Zgierzu', N'SR', N'4170')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4171', N'SR w Be³chatowie', N'Be³chatów', N'Be³chatów SR w Be³chatowie', N'SR', N'4171')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4172', N'SR w Opocznie', N'Opoczno', N'Opoczno SR w Opocznie', N'SR', N'4172')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4173', N'SR w Piotrkowie Tryb.', N'Piotrków Tryb.', N'Piotrków Tryb. SR w Piotrkowie Tryb.', N'SR', N'4173')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4174', N'SR w Radomsku', N'Radomsko', N'Radomsko SR w Radomsku', N'SR', N'4174')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4175', N'SR w Tomaszowie Maz.', N'Tomaszów Maz.', N'Tomaszów Maz. SR w Tomaszowie Maz.', N'SR', N'4175')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4176', N'SR w Ciechanowie', N'Ciechanów', N'Ciechanów SR w Ciechanowie', N'SR', N'4176')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4177', N'SR w Gostyninie', N'Gostynin', N'Gostynin SR w Gostyninie', N'SR', N'4177')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4178', N'SR w M³awie', N'M³awa', N'M³awa SR w M³awie', N'SR', N'4178')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4179', N'SR w P³ocku', N'P³ock', N'P³ock SR w P³ocku', N'SR', N'4179')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4180', N'SR w P³oñsku', N'P³oñsk', N'P³oñsk SR w P³oñsku', N'SR', N'4180')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4181', N'SR w Sochaczewie', N'Sochaczew', N'Sochaczew SR w Sochaczewie', N'SR', N'4181')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4182', N'SR w ¯yrardowie', N'¯yrardów', N'¯yrardów SR w ¯yrardowie', N'SR', N'4182')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4183', N'SR w £asku', N'£ask', N'£ask SR w £asku', N'SR', N'4183')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4184', N'SR w Sieradzu', N'Sieradz', N'Sieradz SR w Sieradzu', N'SR', N'4184')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4185', N'SR w Wieluniu', N'Wieluñ', N'Wieluñ SR w Wieluniu', N'SR', N'4185')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4186', N'SR w Zduñskiej Woli', N'Zduñska Wola', N'Zduñska Wola SR w Zduñskiej Woli', N'SR', N'4186')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4187', N'SR w Koninie', N'Konin', N'Konin SR w Koninie', N'SR', N'4187')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4188', N'SR w Kole', N'Ko³o', N'Ko³o SR w Kole', N'SR', N'4188')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4189', N'SR w Turku', N'Turek', N'Turek SR w Turku', N'SR', N'4189')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4190', N'SR w GnieŸnie', N'Gniezno', N'Gniezno SR w GnieŸnie', N'SR', N'4190')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4191', N'SR w Grodzisku Wlkp.', N'Grodzisk Wlkp.', N'Grodzisk Wlkp. SR w Grodzisku Wlkp.', N'SR', N'4191')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4192', N'SR w Koœcianie', N'Koœcian', N'Koœcian SR w Koœcianie', N'SR', N'4192')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4193', N'SR w Lesznie', N'Leszno', N'Leszno SR w Lesznie', N'SR', N'4193')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4194', N'SR w Pile', N'Pi³a', N'Pi³a SR w Pile', N'SR', N'4194')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4195', N'SR Poznañ Grunwald Je¿yce', N'Poznañ', N'Poznañ SR Poznañ Grunwald Je¿yce', N'SR', N'4195')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4196', N'SR Poznañ Nw Miasto Wilda', N'Poznañ', N'Poznañ SR Poznañ Nw Miasto Wilda', N'SR', N'4196')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4197', N'SR Poznañ Stare Miasto', N'Poznañ', N'Poznañ SR Poznañ Stare Miasto', N'SR', N'4197')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4198', N'SR w Szamotu³ach', N'Szamotu³y', N'Szamotu³y SR w Szamotu³ach', N'SR', N'4198')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4199', N'SR w Œrodzie Wlkp.', N'Œroda Wlkp.', N'Œroda Wlkp. SR w Œrodzie Wlkp.', N'SR', N'4199')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4200', N'SR w Trzciance', N'Trzcianka', N'Trzcianka SR w Trzciance', N'SR', N'4200')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4201', N'SR w W¹growcu', N'W¹growiec', N'W¹growiec SR w W¹growcu', N'SR', N'4201')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4202', N'SR w Kroœnie Odrzañskim', N'Krosno Odrzañskie', N'Krosno Odrzañskie SR w Kroœnie Odrzañskim', N'SR', N'4202')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4203', N'SR w Nowej Soli', N'Nowa Sól', N'Nowa Sól SR w Nowej Soli', N'SR', N'4203')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4204', N'SR w Œwiebodzinie', N'Œwiebodzin', N'Œwiebodzin SR w Œwiebodzinie', N'SR', N'4204')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4205', N'SR w Zielonej Górze', N'Zielona Góra', N'Zielona Góra SR w Zielonej Górze', N'SR', N'4205')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4206', N'SR w ¯aganiu', N'¯agañ', N'¯agañ SR w ¯aganiu', N'SR', N'4206')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4207', N'SR w ¯arach', N'¯ary', N'¯ary SR w ¯arach', N'SR', N'4207')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4208', N'SR w Jaœle', N'Jas³o', N'Jas³o SR w Jaœle', N'SR', N'4208')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4209', N'SR w Kroœnie', N'Krosno', N'Krosno SR w Kroœnie', N'SR', N'4209')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4210', N'SR w Lesku', N'Lesko', N'Lesko SR w Lesku', N'SR', N'4210')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4211', N'SR w Sanoku', N'Sanok', N'Sanok SR w Sanoku', N'SR', N'4211')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4212', N'SR w Jaros³awiu', N'Jaros³aw', N'Jaros³aw SR w Jaros³awiu', N'SR', N'4212')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4213', N'SR w Przemyœlu', N'Przemyœl', N'Przemyœl SR w Przemyœlu', N'SR', N'4213')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4214', N'SR w Dêbicy', N'Dêbica', N'Dêbica SR w Dêbicy', N'SR', N'4214')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4215', N'SR w £añcucie', N'£añcut', N'£añcut SR w £añcucie', N'SR', N'4215')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4216', N'SR w Rzeszowie', N'Rzeszów', N'Rzeszów SR w Rzeszowie', N'SR', N'4216')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4217', N'SR w Mielcu', N'Mielec', N'Mielec SR w Mielcu', N'SR', N'4217')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4218', N'SR w Stalowej Woli', N'Stalowa Wola', N'Stalowa Wola SR w Stalowej Woli', N'SR', N'4218')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4219', N'SR w Tarnobrzegu', N'Tarnobrzeg', N'Tarnobrzeg SR w Tarnobrzegu', N'SR', N'4219')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4220', N'SR w Gorzowie Wlkp.', N'Gorzów Wlkp.', N'Gorzów Wlkp. SR w Gorzowie Wlkp.', N'SR', N'4220')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4221', N'SR w Miêdzyrzeczu', N'Miêdzyrzecz', N'Miêdzyrzecz SR w Miêdzyrzeczu', N'SR', N'4221')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4222', N'SR w S³ubicach', N'S³ubice', N'S³ubice SR w S³ubicach', N'SR', N'4222')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4223', N'SR w Bia³ogardzie', N'Bia³ogard', N'Bia³ogard SR w Bia³ogardzie', N'SR', N'4223')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4224', N'SR w Drawsku Pomorskim', N'Drawsko Pomorskie', N'Drawsko Pomorskie SR w Drawsku Pomorskim', N'SR', N'4224')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4225', N'SR w Ko³obrzegu', N'Ko³obrzeg', N'Ko³obrzeg SR w Ko³obrzegu', N'SR', N'4225')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4226', N'SR w Koszalinie', N'Koszalin', N'Koszalin SR w Koszalinie', N'SR', N'4226')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4227', N'SR w Szczecinku', N'Szczecinek', N'Szczecinek SR w Szczecinku', N'SR', N'4227')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4228', N'SR w Wa³czu', N'Wa³cz', N'Wa³cz SR w Wa³czu', N'SR', N'4228')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4229', N'SR w Goleniowie', N'Goleniów', N'Goleniów SR w Goleniowie', N'SR', N'4229')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4230', N'SR w Gryficach', N'Gryfice', N'Gryfice SR w Gryficach', N'SR', N'4230')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4231', N'SR w Gryfinie', N'Gryfino', N'Gryfino SR w Gryfinie', N'SR', N'4231')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4232', N'SR w Myœliborzu', N'Myœlibórz', N'Myœlibórz SR w Myœliborzu', N'SR', N'4232')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4233', N'SR w Stargardzie Szczec.', N'Stargard Szczec.', N'Stargard Szczec. SR w Stargardzie Szczec.', N'SR', N'4233')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4234', N'SR Szczecin Centrum', N'Szczecin', N'Szczecin SR Szczecin Centrum', N'SR', N'4234')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4235', N'SR Szczecin Praw i Zachód', N'Szczecin', N'Szczecin SR Szczecin Praw i Zachód', N'SR', N'4235')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4236', N'SR w Œwinoujœciu', N'Œwinoujœcie', N'Œwinoujœcie SR w Œwinoujœciu', N'SR', N'4236')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4237', N'SR w Grodzisku Maz.', N'Grodzisk Maz.', N'Grodzisk Maz. SR w Grodzisku Maz.', N'SR', N'4237')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4238', N'SR w Piasecznie', N'Piaseczno', N'Piaseczno SR w Piasecznie', N'SR', N'4238')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4239', N'SR w Pruszkowie', N'Pruszków', N'Pruszków SR w Pruszkowie', N'SR', N'4239')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4240', N'SR dla m.st. Warszawy', N'Warszawa', N'Warszawa SR dla m.st. Warszawy', N'SR', N'4240')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4241', N'SR dla Warszawy Mokotowa', N'Warszawa', N'Warszawa SR dla Warszawy Mokotowa', N'SR', N'4241')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4242', N'SR dla Warszawy Œródm.', N'Warszawa', N'Warszawa SR dla Warszawy Œródm.', N'SR', N'4242')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4243', N'SR dla Warszawy Woli', N'Warszawa', N'Warszawa SR dla Warszawy Woli', N'SR', N'4243')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4244', N'SR dla Warszawy ¯oliborza', N'Warszawa', N'Warszawa SR dla Warszawy ¯oliborza', N'SR', N'4244')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4245', N'SR w Legionowie', N'Legionowo', N'Legionowo SR w Legionowie', N'SR', N'4245')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4246', N'SR w Nowym Dworze Maz.', N'Nowy Dwór Maz.', N'Nowy Dwór Maz. SR w Nowym Dworze Maz.', N'SR', N'4246')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4247', N'SR w Otwocku', N'Otwock', N'Otwock SR w Otwocku', N'SR', N'4247')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4248', N'SR dla Warszawy Pragi Pd.', N'Warszawa', N'Warszawa SR dla Warszawy Pragi Pd.', N'SR', N'4248')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4249', N'SR dla Warszawy Pragi Pn', N'Warszawa', N'Warszawa SR dla Warszawy Pragi Pn', N'SR', N'4249')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4250', N'SR w Wo³ominie', N'Wo³omin', N'Wo³omin SR w Wo³ominie', N'SR', N'4250')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'4251', N'SR w Zwoleniu', N'Zwoleñ', N'Zwoleñ SR w Zwoleniu', N'SR', N'4251')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5001', N'SF w Grajewie', N'Grajewo', N'Grajewo SF w Grajewie', N'SF', N'3007')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5002', N'SF w Wysokiem Maz.', N'Wysokie Mazowieckie', N'Wysokie Mazowieckie SF w Wysokiem Maz.', N'SF', N'3007')
GO
print 'Processed 300 total records'
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5003', N'SF w Zambrowie', N'Zambrów', N'Zambrów SF w Zambrowie', N'SF', N'3007')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5004', N'SF w Piszu', N'Pisz', N'Pisz SF w Piszu', N'SF', N'3008')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5005', N'SF w Pu³tusku', N'Pu³tusk', N'Pu³tusk SF w Pu³tusku', N'SF', N'3042')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5006', N'SF w Nakle n Noteci¹', N'Nalk³o n Noteci¹', N'Nalk³o n Noteci¹ SF w Nakle n Noteci¹', N'SF', N'3010')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5007', N'SF w Tucholi', N'Tuchola', N'Tuchola SF w Tucholi', N'SF', N'3010')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5008', N'SF w Dzia³dowie', N'Dzia³dowo', N'Dzia³dowo SF w Dzia³dowie', N'SF', N'3011')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5009', N'SF w Koœcierzynie', N'Koœcierzyna', N'Koœcierzyna SF w Koœcierzynie', N'SF', N'3012')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5010', N'SF w Opatowie', N'Opatów', N'Opatów SF w Opatowie', N'SF', N'3020')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5011', N'SF w Staszowie', N'Staszów', N'Staszów SF w Staszowie', N'SF', N'3020')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5012', N'SF w Suchej Beskidz.', N'Sucha Beskidzka', N'Sucha Beskidzka SF w Suchej Beskidz.', N'SF', N'3021')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5013', N'SF w D¹browie Tarn.', N'D¹browa Tarn.', N'D¹browa Tarn. SF w D¹browie Tarn.', N'SF', N'3023')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5014', N'SF w Opolu Lubelskim', N'Opole Lubelskie', N'Opole Lubelskie SF w Opolu Lubelskim', N'SF', N'3024')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5015', N'SF w Rykach', N'Ryki', N'Ryki SF w Rykach', N'SF', N'3024')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5016', N'SF w Soko³owie Podl.', N'Soko³ów Podl.', N'Soko³ów Podl. SF w Soko³owie Podl.', N'SF', N'3026')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5017', N'SF w Krasnymstawie', N'Krasnystaw', N'Krasnystaw SF w Krasnymstawie', N'SF', N'3027')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5018', N'SF w Jarocinie', N'Jarocin', N'Jarocin SF w Jarocinie', N'SF', N'3028')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5019', N'SF w Krotoszynie', N'Krotoszyn', N'Krotoszyn SF w Krotoszynie', N'SF', N'3028')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5020', N'SF w Pleszewie', N'Pleszew', N'Pleszew SF w Pleszewie', N'SF', N'3028')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5021', N'SF w Brzezinach', N'Brzeziny', N'Brzeziny SF w Brzezinach', N'SF', N'3029')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5022', N'SF w £êczycy', N'£êczyca', N'£êczyca SF w £êczycy', N'SF', N'3029')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5023', N'SF w Rawie Mazowiec.', N'Rawa Mazowiecka', N'Rawa Mazowiecka SF w Rawie Mazowiec.', N'SF', N'3029')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5024', N'SF w Sierpcu', N'Sierpc', N'Sierpc SF w Sierpcu', N'SF', N'3043')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5025', N'SF w S³upcy', N'S³upca', N'S³upca SF w S³upcy', N'SF', N'3032')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5026', N'SF w Chodzie¿y', N'Chodzie¿', N'Chodzie¿ SF w Chodzie¿y', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5027', N'SF w Gostyniu', N'Gostyñ', N'Gostyñ SF w Gostyniu', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5028', N'SF w Grodzisku Wlkp', N'Grodzisk Wlkp.', N'Grodzisk Wlkp. SF w Grodzisku Wlkp', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5029', N'SF w Koœcianie', N'Koœcian', N'Koœcian SF w Koœcianie', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5030', N'SF w Nowym Tomyœlu', N'Nowy Tomyœl', N'Nowy Tomyœl SF w Nowym Tomyœlu', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5031', N'SF w Obornikach', N'Oborniki', N'Oborniki SF w Obornikach', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5032', N'SF w Rawiczu', N'Rawicz', N'Rawicz SF w Rawiczu', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5033', N'SF w Œremie', N'Œrem', N'Œrem SF w Œremie', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5034', N'SF w Œrodzie Wlkp', N'Œroda Wlkp.', N'Œroda Wlkp. SF w Œrodzie Wlkp', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5035', N'SF w Wolsztynie', N'Wolsztyn', N'Wolsztyn SF w Wolsztynie', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5036', N'SF w Z³otowie', N'Z³otów', N'Z³otów SF w Z³otowie', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5037', N'SF we Wrzeœni', N'Wrzeœnia', N'Wrzeœnia SF we Wrzeœni', N'SF', N'3033')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5038', N'SF w Przeworsku', N'Przeworsk', N'Przeworsk SF w Przeworsku', N'SF', N'3036')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5039', N'SF w Le¿ajsku', N'Le¿ajsk', N'Le¿ajsk SF w Le¿ajsku', N'SF', N'3037')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5040', N'SF w Nisku', N'Nisko', N'Nisko SF w Nisku', N'SF', N'3038')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5041', N'SF w Strzelcach Kraj', N'Strzelce Kraj.', N'Strzelce Kraj. SF w Strzelcach Kraj', N'SF', N'3039')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5042', N'SF w Choszcznie', N'Choszczno', N'Choszczno SF w Choszcznie', N'SF', N'3041')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5043', N'SF w Kamiennej Górze', N'Kamienna Góra', N'Kamienna Góra SF w Kamiennej Górze', N'SF', N'3001')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5044', N'SF w Lwówku Œl¹skim', N'Lwówek œl¹ski', N'Lwówek œl¹ski SF w Lwówku Œl¹skim', N'SF', N'3001')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5045', N'SF w Oleœnie', N'Olesno', N'Olesno SF w Oleœnie', N'SF', N'3003')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5046', N'SF Wschowa ', N'Wschowa', N'Wschowa SF we Wschowej', N'SF', N'3034')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5050', N'SF w Piñczowie', N'Piñczów', N'Piñczów SF w Piñczowie', N'SF', N'3020')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5051', N'SF we W³oszczowej', N'W³oszczowa', N'W³oszczowa SF we W³oszczowej', N'SF', N'3020')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5071', N'SF w Ostrzeszowie', N'Ostrzeszów', N'Ostrzeszów SF w Ostrzeszowie', N'SF', N'3028')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5077', N'SF w G³ubczycach', N'G³ubczyce', N'G³ubczyce SF w G³ubczycach', N'SF', N'3003')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5078', N'SF w Miliczu', N'Milicz', N'Milicz SF w Miliczu', N'SF', N'3005')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5079', N'SF w Strzelinie', N'Strzelin', N'Strzelin SF w Strzelinie', N'SF', N'3005')
INSERT [dbo].[SAPSad] ([kod], [sad], [miasto], [miastSad], [typSad], [JEGO]) VALUES (N'5080', N'SF w Wo³owie', N'Wo³ów', N'Wo³ów SF w Wo³owie', N'SF', N'3005')
/****** Object:  Table [dbo].[SAPRodzajSprawy]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPRodzajSprawy](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[kod] [varchar](5) NULL,
	[opis] [varchar](30) NULL,
	[repertorium] [varchar](10) NULL,
	[typSad] [varchar](2) NULL,
 CONSTRAINT [PK_SAPRodzajSprawy] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[SAPRodzajSprawy] ON
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (2, N'1C012', N'SR-C do5tom', N'C', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (3, N'1C013', N'SR-C5-20tom', N'C', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (4, N'1C014', N'SR-Cpow20tom', N'C', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (6, N'1C022', N'SR-Ns-do5tom', N'Ns', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (7, N'1C023', N'SR-Ns5-20tom', N'Ns', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (8, N'1C024', N'SR-Ns pow20tom', N'Ns', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (9, N'1C031', N'SR-Nc i Nc-e', N'Nc', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (10, N'1C041', N'SR-Co', N'Co', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (11, N'1C051', N'SR-Cps', N'Cps', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (12, N'1C131', N'SR-CG-G', N'CG', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (21, N'2C012', N'SO-Cdo5tom', N'C', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (22, N'2C013', N'SO-C5-20tom', N'C', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (23, N'2C014', N'SO-Cpow20tom', N'C', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (24, N'2C021', N'SO-Ns', N'Ns', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (25, N'2C031', N'SO-Nc', N'Nc', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (26, N'2C131', N'SO-CG-G', N'CG', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (34, N'2C041', N'SO-Co', N'Co', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (35, N'4C081', N'SA-WSC', N'WSC', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (36, N'4C091', N'SA-S', N'S', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (38, N'4C102', N'SA-ACado5tom', N'ACa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (39, N'4C103', N'SA-ACa5-20tom', N'ACa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (40, N'4C104', N'SA-ACapow20tom', N'ACa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (41, N'4C111', N'SA-ACz', N'ACz', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (42, N'4C121', N'SA-ACo', N'ACo', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (43, N'3C081', N'SO2i.-WSC', N'WSC', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (44, N'3C091', N'SO2i.-S', N'S', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (45, N'3C131', N'SO2i.-CG-G', N'CG', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (53, N'3C041', N'SO2i.-Co', N'Co', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (55, N'3C062', N'SO2i.-Cado5tom', N'Ca', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (56, N'3C063', N'SO2i.-Ca5-20tom', N'Ca', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (57, N'3C064', N'SO2i.-Capow20tom', N'Ca', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (58, N'3C071', N'SO2i.-Cz', N'Cz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (59, N'2E011', N'SO-WykonywaniaOrzeczeñ', N'', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (60, N'1E011', N'SR-WykonywaniaOrzeczeñ', N'', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (61, N'4E011', N'SA-WykonywaniaOrzeczeñ', N'', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (62, N'3E011', N'SO2i.-WykonywaniaOrzeczeñ', N'', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (63, N'1G051', N'SR-Gco', N'Gco', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (64, N'2G051', N'SO-Gco', N'Gco', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (65, N'2G011', N'SO-GC', N'GC', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (66, N'2G031', N'SO-GNc', N'GNc', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (67, N'2G041', N'SO-GNs', N'GNs', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (73, N'1G012', N'SR-GCdo5tom', N'GC', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (74, N'1G014', N'SR-GCpow20tom', N'GC', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (75, N'1G021', N'SR-GN', N'GN', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (76, N'1G031', N'SR-GNc', N'GNc', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (77, N'1G041', N'SR-GNs', N'GNs', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (78, N'1G061', N'SR-GCps', N'GCps', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (79, N'1G091', N'SR-GU', N'GU', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (80, N'1G101', N'SR-GUp', N'GUp', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (81, N'1G111', N'SR-Gzd', N'Gzd', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (82, N'1G121', N'SR-Guo', N'Guo', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (83, N'1G131', N'SR-GUz', N'GUz', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (84, N'1G141', N'SR-GUk', N'GUk', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (85, N'1G151', N'SR-GUu', N'GUu', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (92, N'3G191', N'SO2i.-S', N'S', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (93, N'3G201', N'SO2i.-WSC', N'WSC', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (95, N'3G051', N'SO2i.-Gco', N'Gco', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (96, N'3G071', N'SO2i.-Ga', N'Ga', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (97, N'3G081', N'SO2i.-Gz', N'Gz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (98, N'2H041', N'SO-Ns-Rej.Ew.R', N'Ns-Rej.EwR', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (99, N'2H051', N'SO-Ns-Rej.FE', N'Ns-Rej.FE', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (100, N'2H061', N'SO-Ns-Rej.FI', N'Ns-Rej.FI', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (101, N'2H071', N'SO-Ns-Rej.Pr', N'Ns-Rej.Pr', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (102, N'1H011', N'SR-Ns-Rej', N'Ns-Rej', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (103, N'1H021', N'SR-Ns-Rej.KRS', N'Ns-Rej.KRS', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (104, N'1H031', N'SR-Ns-Rej.Za', N'Ns-Rej.Za', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (105, N'3H041', N'SO2i.-Ns-Rej.Ew.R', N'Ns-Rej.EwR', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (106, N'3H051', N'SO2i.-Ns-Rej.FE', N'Ns-Rej.FE', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (107, N'3H061', N'SO2i.-Ns-Rej.FI', N'Ns-Rej.FI', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (108, N'3H071', N'SO2i.-Ns-Rej.Pr', N'Ns-Rej.Pr', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (111, N'1J011', N'SR-Ko wykrocz.', N'Ko', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (115, N'3J042', N'SO2i.-Ka-wykr.do5tom', N'Ka', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (116, N'3J043', N'SO2i.-Ka-wykr.5-20tom', N'Ka', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (117, N'3J044', N'SO2i.-Ka-wykr.pow20tom', N'Ka', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (118, N'3J031', N'SO2i.-Kz', N'Kz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (121, N'2K021', N'SO-Kbez335,387,KKS', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (122, N'2K031', N'SO-K-335KPK', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (123, N'2K041', N'SO-K-387KPK', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (125, N'2K072', N'SO-K-wy.³aczny do5tom', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (126, N'2K073', N'SO-K-wy.³aczny 5-20tom', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (127, N'2K074', N'SO-K-wy.³aczny pow20tom', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (128, N'2K111', N'SO-Kp', N'Kp', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (129, N'2K131', N'SO-K-bezwy.³acznego', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (131, N'2K261', N'SO-Kop', N'Kop', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (134, N'2K121', N'SO-Ko', N'Ko', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (136, N'1K021', N'SR-Kbez335,387,KKS', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (137, N'1K031', N'SR-K-335KPK', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (138, N'1K041', N'SR-K-387KPK', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (139, N'1K051', N'SR-K-KKS', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (140, N'1K061', N'SR-K-wy.nakazowy', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (142, N'1K072', N'SR-K-wy.³aczny do5tom', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (143, N'1K073', N'SR-K-wy.³aczny 5-20tom', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (144, N'1K074', N'SR-K-wy.³aczny pow20tom', N'K', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (145, N'1J051', N'SR-WbezKKS,wy.nak.', N'W', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (146, N'1J061', N'SR-W-wy.nak.', N'W', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (147, N'1J071', N'SR-W-KKS', N'W', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (148, N'1K111', N'SR-Kp', N'Kp', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (149, N'1K261', N'SR-Kop', N'Kop', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (154, N'1K121', N'SR-Ko', N'Ko', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (155, N'4K181', N'SA-WKK', N'WKK', N'SA')
GO
print 'Processed 100 total records'
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (156, N'4K191', N'SA-Aka-bezwyr.³acznego', N'Aka', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (158, N'4K202', N'SA-AKa-wyr.³aczny do5tom', N'AKa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (159, N'4K204', N'SA-AKa-wyr.³aczny pow20tom', N'AKa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (160, N'4K211', N'SA-AKz', N'AKz', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (161, N'4K221', N'SA-AKzw', N'AKzw', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (162, N'4K231', N'SA-S', N'S', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (164, N'4K271', N'SA-AKo', N'AKo', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (165, N'4K281', N'SA-AKp', N'AKp', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (166, N'3K121', N'SO2i.-Ko', N'Ko', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (169, N'3K142', N'SO2i.-Ka-bezwykr.do5tom', N'Ka', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (170, N'3K143', N'SO2i.-Ka-bezwykr.5-20tom', N'Ka', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (171, N'3K144', N'SO2i.-Ka-bezwykr.pow20tom', N'Ka', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (172, N'3K181', N'SO2i.-WKK', N'WKK', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (173, N'3K231', N'SO2i.-S', N'S', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (174, N'3K261', N'SO2i.-Kop', N'Kop', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (177, N'3K161', N'SO2i.-Kz', N'Kz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (178, N'3K171', N'SO2i.-Kzw', N'Kzw', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (185, N'2L081', N'SO-Wz', N'Wz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (186, N'2L091', N'SO-D', N'D', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (192, N'1L011', N'SR-Kow', N'Kow', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (193, N'1L021', N'SR-Pen', N'Pen', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (199, N'1L081', N'SR-Wz', N'Wz', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (200, N'1L091', N'SR-D', N'D', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (218, N'2N011', N'SO-Dz.Ko', N'Dz.Ko', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (219, N'2N021', N'SO-Dz.Kw', N'Dz.Kw', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (220, N'2N031', N'SO-Dz.Odp', N'Dz.Odp', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (221, N'2N041', N'SO-Dz.Zd', N'Dz.Zd', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (222, N'2N051', N'SO-Ar', N'Ar', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (223, N'2N061', N'SO-Kw', N'Kw', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (224, N'2N071', N'SO-Zd', N'Zd', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (226, N'3N011', N'SO2i.-Dz.Ko', N'Dz.Ko', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (227, N'3N021', N'SO2i.-Dz.Kw', N'Dz.Kw', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (228, N'3N031', N'SO2i.-Dz.Odp', N'Dz.Odp', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (229, N'3N041', N'SO2i.-Dz.Zd', N'Dz.Zd', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (230, N'3N051', N'SO2i.-Ar', N'Ar', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (231, N'3N061', N'SO2i.-Kw', N'Kw', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (232, N'3N071', N'SO2i.-Zd', N'Zd', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (233, N'2O011', N'SO-AmA', N'AmA', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (234, N'2O031', N'SO-AmE', N'AmE', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (235, N'2O041', N'SO-AmK', N'AmK', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (236, N'2O061', N'SO-AmT', N'AmT', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (237, N'2O021', N'SO-AmC', N'AmC', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (238, N'2O051', N'SO-Amo', N'Amo', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (239, N'2O071', N'SO-Amz', N'Amz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (242, N'2P071', N'SO-Kas-z', N'Kas', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (247, N'3P071', N'SO2i.-Kas-z', N'Kas', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (248, N'1P121', N'SR-Po-Uo', N'Po', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (249, N'1P161', N'SR-Uo', N'Uo', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (250, N'2P121', N'SO-Po-Uo', N'Po', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (251, N'2P161', N'SO-Uo', N'Uo', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (252, N'2P081', N'SO-Np', N'Np', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (253, N'2P091', N'SO-P', N'P', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (254, N'2P111', N'SO-Po', N'Po', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (255, N'2P141', N'SO-U', N'U', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (256, N'1P081', N'SR-Np', N'Np', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (257, N'1P091', N'SR-P', N'P', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (258, N'1P111', N'SR-Po', N'Po', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (259, N'1P141', N'SR-U', N'U', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (260, N'4P011', N'SA-APa', N'APa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (261, N'4P021', N'SA-APo', N'APo', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (262, N'4P031', N'SA-APz', N'APz', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (263, N'4P041', N'SA-AUa', N'AUa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (264, N'4P051', N'SA-AUo', N'AUo', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (265, N'4P061', N'SA-AUz', N'AUz', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (266, N'4P181', N'SA-S', N'S', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (267, N'4P191', N'SA-WSC', N'WSC', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (268, N'3P101', N'SO2i.-Pa', N'Pa', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (269, N'3P121', N'SO2i.-Po-Uo', N'Po', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (270, N'3P131', N'SO2i.-Pz', N'Pz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (271, N'3P151', N'SO2i.-Ua', N'Ua', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (272, N'3P171', N'SO2i.-Uz', N'Uz', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (274, N'2R041', N'SO-Nmo', N'Nmo', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (277, N'2R081', N'SO-Nsm', N'Nsm', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (278, N'2R111', N'SO-OPM', N'OPM', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (279, N'2R121', N'SO-RC', N'RC', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (280, N'2R131', N'SO-Rco', N'Rco', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (281, N'2R141', N'SO-RCps', N'RCps', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (282, N'2R161', N'SO-RNs', N'RNs', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (288, N'1R041', N'SR-Nmo', N'Nmo', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (292, N'1R081', N'SR-Nsm', N'Nsm', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (294, N'1R101', N'SR-Op', N'Op', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (295, N'1R111', N'SR-OPM', N'OPM', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (296, N'1R121', N'SR-RC', N'RC', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (297, N'1R131', N'SR-Rco', N'Rco', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (298, N'1R141', N'SR-RCps', N'RCps', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (299, N'1R151', N'SR-RNc', N'RNc', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (300, N'1R161', N'SR-RNs', N'RNs', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (301, N'1R171', N'SR-WSC', N'WSC', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (308, N'2U011', N'SO-Zespó³Kuratorski', N'', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (309, N'1U011', N'SR-Zespó³Kuratorski', N'', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (310, N'3U011', N'SO2i.-Zespó³Kuratorski', N'', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (314, N'1G013', N'SR-GC 5-20 tom', N'GC', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (315, N'1R221', N'SR-Nkd', N'Nkd', N'SR')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (316, N'2K051', N'SO 1in.-K-KKS', N'K', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (317, N'2L011', N'SO 1in.-Kow', N'Kow', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (318, N'2L021', N'SO 1in.-Pen', N'Pen', N'SO')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (319, N'4K203', N'SA-AKa-wyr.³¹czny 5-20 tom', N'AKa', N'SA')
INSERT [dbo].[SAPRodzajSprawy] ([id], [kod], [opis], [repertorium], [typSad]) VALUES (320, N'1C031', N'SR-Nc i Nc-e', N'Nc-e', N'SR')
SET IDENTITY_INSERT [dbo].[SAPRodzajSprawy] OFF
/****** Object:  Table [dbo].[SAPRepertorium]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPRepertorium](
	[kod] [varchar](50) NOT NULL,
	[SymbolRodzajPrzedmiotu] [varchar](4) NULL,
 CONSTRAINT [PK_SAPRepertorium] PRIMARY KEY CLUSTERED 
(
	[kod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'AKo', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'AKp', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Alk', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'APa', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'APo', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'APz', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ar', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Are', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'AUa', N'SUBE')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'AUo', N'SUBE')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'AUz', N'SUBE')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Bp', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'C', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ca', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'CG-G', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Co', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Cps', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Cz', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'D', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Dz.Ko', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Dz.Odp', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Dz.Zd', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ga', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GC', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GCo', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GCps', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GN', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GNc', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GNs', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GU', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GUk', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GUo', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GUp', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GUu', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GUz', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GWo', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GWwp', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'GWzt', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Gz', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Gzd', N'SGOS')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'K', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ka', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Kas-z', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ko', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Kop', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Kow', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Kp', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Kw', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Kz', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Kzw', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'MED', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nc', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nc-e', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'NF', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nk', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nkd', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nmo', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Now', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Np', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Npw', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns', N'SCYW')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns.R.D', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns.R.E', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns.R.I', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns.R.K', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns.R.P', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns.R.Z', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nsch', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nsm', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ns-Rej', N'SRES')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Nw', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Op', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Opm', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'OZ', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'P', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Pa', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Pen', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Po', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Po-Uo', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Pz', N'SPPR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'RC', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Rco', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'RCps', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'RNc', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'RNs', N'SROD')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'S', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Œr.Zab', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'U', N'SUBE')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Ua', N'SUBE')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Uo', N'SUBE')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Uz', N'SUBE')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'W', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Wab', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'WKK', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Wo', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Wp', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Wpkz', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'WSC', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Wu', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Wz', N'SKAR')
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Wzaw', N'SKAR')
GO
print 'Processed 100 total records'
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Zd', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Zpc', NULL)
INSERT [dbo].[SAPRepertorium] ([kod], [SymbolRodzajPrzedmiotu]) VALUES (N'Zpk', NULL)
/****** Object:  Table [dbo].[SAPRBN]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPRBN](
	[kod] [varchar](2) NOT NULL,
	[opis] [varchar](255) NULL,
 CONSTRAINT [PK_SAPRBN] PRIMARY KEY CLUSTERED 
(
	[kod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'01', N'01-grupa I  ORGANY W£ADZY PUBLICZNEJ, RZADOWEJ, KONTROLI, OCHRONY PRAWA, SADY, JEDNOSTKI BUD¯ETOWE…')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'02', N'02-grupa II UCZELNIE PUBLICZNE, SAM. PUBLICZNE ZAK£. OPIEKI ZDROW, ORGANY ADMIN RZ¥DOWEJ...')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'03', N'03-grupa III JEDN. SAM. TERYT., SAM. JEDN. BUD_., SAM. PUBL. ZAK£. OPIEKI ZDROW…')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'04', N'04-grupa IV ZUS , KRUS , NARODOWY FUNDUSZ ZDROWIA…')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'05', N'05-bank centralny NARODOWY BANK POLSKI')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'06', N'06-banki  BANKI NA TERYT. RP, BANKI PANSTWOWE, SPÓ£DZIELCZE, W FORMIE SPÓ£EK AKCYJNYCH…')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'07', N'07-pozosta³e krajowe instytucje finansowe POSREDNICTWO FINANSOWE, FUNDUSZ INWEST, NAROD. FUNDUSZ INWEST., ZAK£. UBEZP…')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'08', N'08-przedsiêbiorstwa niefinansowe PROD. I OBRÓT DOBRAMI, SWIADCZENIE US£. NIEFINAN., PRZEDSI. PANSTW., SPÓ£KI, SPÓ£DZIELNIE...')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'09', N'09-gospodarstwa domowe OSOBY BEDACE KONSUMENT., PRODUCENTAMI, OSOBY FIZYCZNE, W TYM OSOBY FIZYCZNE…')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'10', N'10-instytucje niekomercyjne dzia³aj¹ce na rzecz gospodarstw domowych ZWIAZKI ZAWODOWE, FUNDACJE, STOWARZYSZENIA, PARTIE POLITYCZNE, KOSCIO£Y LUB ZWIZKI WYZNANIOWE')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'11', N'11-podmioty nale¿¹ce do strefy euro NIEREZYD. MAJACY SIEDZIBE LUB MIEJ. ZAM. W PANSTWIE CZ£ONK. UE, W KTÓRYM OBOW. SR. P£. JEST EURO')
INSERT [dbo].[SAPRBN] ([kod], [opis]) VALUES (N'12', N'12-pozosta³e podmioty zagraniczne  NIEREZYD. MAJACY MIEJ. ZAM. ZA GRANICA ORAZ OS. PRAWNE MAJACE SIEDZIBE ZA GRANICA W INNYM PANST. NI_ UE')
/****** Object:  Table [dbo].[SAPOpisPrzedmiotu]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPOpisPrzedmiotu](
	[Symbol] [varchar](4) NOT NULL,
	[Opis] [varchar](20) NOT NULL,
 CONSTRAINT [PK_SAPOpisPrzedmiotu] PRIMARY KEY CLUSTERED 
(
	[Symbol] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[SAPOpisPrzedmiotu] ([Symbol], [Opis]) VALUES (N'SCYW', N'Cywilna')
INSERT [dbo].[SAPOpisPrzedmiotu] ([Symbol], [Opis]) VALUES (N'SGOS', N'Gospodarcza')
INSERT [dbo].[SAPOpisPrzedmiotu] ([Symbol], [Opis]) VALUES (N'SKAR', N'Karna')
INSERT [dbo].[SAPOpisPrzedmiotu] ([Symbol], [Opis]) VALUES (N'SPPR', N'Pracy')
INSERT [dbo].[SAPOpisPrzedmiotu] ([Symbol], [Opis]) VALUES (N'SRES', N'Rejestrowa')
INSERT [dbo].[SAPOpisPrzedmiotu] ([Symbol], [Opis]) VALUES (N'SROD', N'Rodzinna')
INSERT [dbo].[SAPOpisPrzedmiotu] ([Symbol], [Opis]) VALUES (N'SUBE', N'Ubezpieczenia')
/****** Object:  Table [dbo].[SAPKodyOpr]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPKodyOpr](
	[kod] [char](4) NOT NULL,
	[nazwa] [varchar](30) NULL,
	[grzywnakoszty] [char](1) NULL,
	[samoistna] [char](1) NULL,
	[operacjaGlowna] [char](4) NULL,
	[oznaczenieOpGlownej] [varchar](30) NULL,
	[id] [varchar](8) NOT NULL,
 CONSTRAINT [PK_SAPKodyOpr] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0010', N'Grzywna cywilna os. fizyczna', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100010')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0020', N'Grzywna karna os. fizyczna', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100020')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0030', N'Uiszcz.grzyw.odpisanej karnej', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100030')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0040', N'Grzywna samoistna karna', N'g', N's', N'N010', N'KNS - Przypis', N'N0100040')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0050', N'Grzyw.karna wykrocz- os.fiz ', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100050')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0060', N'Uiszczgrzyw.odpisanej wykrocz', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100060')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0070', N'Grzywna samoistna wykroczenia', N'g', N's', N'N010', N'KNS - Przypis', N'N0100070')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0080', N'Kara ograniczenia wolnoœci', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100080')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0090', N'Grzywna- os.praw.i in.jed.org', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100090')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0100', N'In.kary pien-os. praw.i in.jed', N'g', N' ', N'N010', N'KNS - Przypis', N'N0100100')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0110', N'Op³aty cywilne- os.fiz - wezw', N'k', N' ', N'N010', N'KNS - Przypis', N'N0100110')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0120', N'K.s¹d.w spr.rodzin.i niel.', N'k', N' ', N'N010', N'KNS - Przypis', N'N0100120')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0130', N'Op³aty i koszty karne', N'k', N' ', N'N010', N'KNS - Przypis', N'N0100130')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0010', N'Grzywna cywilna os. fizyczna', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200010')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0020', N'Grzywna karna os. fizyczna', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200020')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0030', N'Grzywna karna skaz.na wolnoœci', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200030')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0031', N'Grzywna karna skaz.pozb.wolno.', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200031')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0040', N'Grzywna samoistna karna', N'g', N's', N'N020', N'KNS - Odpis (720)', N'N0200040')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0050', N'Grzyw.karna wykrocz- os.fiz', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200050')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0060', N'Grzywna wykro.skaz.na wolnoœci', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200060')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0061', N'Grzywna wykro.skaz.pozb.wolno.', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200061')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0070', N'Grzywna samoistna wykroczenia', N'g', N's', N'N020', N'KNS - Odpis (720)', N'N0200070')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0080', N'Kara oGraniczenia wolnoœci', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200080')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0090', N'Grzywna- os.praw.i in.jed.orG.', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200090')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0100', N'In.kary pien-os. praw.i in.jed', N'g', N' ', N'N020', N'KNS - Odpis (720)', N'N0200100')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0110', N'Op³aty cywilne- os.fiz - wezw', N'k', N' ', N'N020', N'KNS - Odpis (720)', N'N0200110')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0120', N'K.s¹d.w spr.rodzin.i niel.', N'k', N' ', N'N020', N'KNS - Odpis (720)', N'N0200120')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0130', N'Op³aty i koszty karne', N'k', N' ', N'N020', N'KNS - Odpis (720)', N'N0200130')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0010', N'Grzywna cywilna os. fizyczna', N'g', N' ', N'N021', N'KNS - Odpis (761)', N'N0210010')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0020', N'Grzywna karna os. fizyczna', N'g', N' ', N'N021', N'KNS - Odpis (761)', N'N0210020')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0040', N'Grzywna samoistna karna', N'g', N's', N'N021', N'KNS - Odpis (761)', N'N0210040')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0050', N'Grzyw.karna wykrocz- os.fiz', N'g', N' ', N'N021', N'KNS - Odpis (761)', N'N0210050')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0070', N'Grzywna samoistna wykroczenia', N'g', N's', N'N021', N'KNS - Odpis (761)', N'N0210070')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0080', N'Kara oGraniczenia wolnoœci', N'g', N' ', N'N021', N'KNS - Odpis (761)', N'N0210080')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0090', N'Grzywna- os.praw.i in.jed.orG.', N'g', N' ', N'N021', N'KNS - Odpis (761)', N'N0210090')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0100', N'In.kary pien-os. praw.i in.jed', N'g', N' ', N'N021', N'KNS - Odpis (761)', N'N0210100')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0110', N'Op³aty i koszty cywilne', N'k', N' ', N'N021', N'KNS - Odpis (761)', N'N0210110')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0120', N'K.s¹d.w spr.rodzin.i niel.', N'k', N' ', N'N021', N'KNS - Odpis (761)', N'N0210120')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0130', N'Op³aty i koszty karne', N'k', N' ', N'N021', N'KNS - Odpis (761)', N'N0210130')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0010', N'Grzywna cywilna os. fizyczna', N'g', N' ', N'N030', N'KNS - Umorzenie', N'N0300010')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0020', N'Grzywna karna os. fizyczna', N'g', N' ', N'N030', N'KNS - Umorzenie', N'N0300020')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0040', N'Grzywna samoistna karna', N'g', N's', N'N030', N'KNS - Umorzenie', N'N0300040')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0050', N'Grzyw.karna wykrocz- os.fiz', N'g', N' ', N'N030', N'KNS - Umorzenie', N'N0300050')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0070', N'Grzywna samoistna wykroczenia', N'g', N's', N'N030', N'KNS - Umorzenie', N'N0300070')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0080', N'Kara oGraniczenia wolnoœci', N'g', N' ', N'N030', N'KNS - Umorzenie', N'N0300080')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0090', N'Grzywna- os.praw.i in.jed.orG.', N'g', N' ', N'N030', N'KNS - Umorzenie', N'N0300090')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0100', N'In.kary pien-os. praw.i in.jed', N'g', N' ', N'N030', N'KNS - Umorzenie', N'N0300100')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0110', N'Op³aty cywilne- os.fiz - wezw', N'k', N' ', N'N030', N'KNS - Umorzenie', N'N0300110')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0120', N'K.s¹d.w spr.rodzin.i niel.', N'k', N' ', N'N030', N'KNS - Umorzenie', N'N0300120')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0130', N'Op³aty i koszty karne', N'k', N' ', N'N030', N'KNS - Umorzenie', N'N0300130')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0040', N'Op³aty i koszty cywilne', N'k', N' ', N'P020', N'Doch.Nieprzyp. Odpis', N'P0200040')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'0050', N'Op³aty i koszty karne', N'k', N' ', N'P020', N'Doch.Nieprzyp. Odpis', N'P0200050')
INSERT [dbo].[SAPKodyOpr] ([kod], [nazwa], [grzywnakoszty], [samoistna], [operacjaGlowna], [oznaczenieOpGlownej], [id]) VALUES (N'1040', N'Op³aty i koszty cywilne-EPU', N'k', N' ', N'P020', N'Doch.Nieprzyp. Odpis', N'P0201040')
/****** Object:  Table [dbo].[SAPKodKraju]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SAPKodKraju](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[kod] [varchar](2) NULL,
	[kraj] [varchar](50) NULL,
 CONSTRAINT [PK_SAPKodKraju] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[SAPKodKraju] ON
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (1, N'AD', N'Andora')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (2, N'AE', N'Zj. Emir. Arab.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (3, N'AF', N'Afganistan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (4, N'AG', N'Antigua/Barbuda')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (5, N'AI', N'Anguilla')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (6, N'AL', N'Albania')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (7, N'AM', N'Armenia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (8, N'AN', N'Antyle Holend.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (9, N'AO', N'Angola')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (10, N'AQ', N'Antarktyka  Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (11, N'AR', N'Argentyna')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (12, N'AS', N'Samoa ameryk.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (13, N'AT', N'Austria')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (14, N'AU', N'Australia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (15, N'AW', N'Aruba')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (16, N'AZ', N'Azerbejd¿an')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (17, N'BA', N'Boœnia-Herceg.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (18, N'BB', N'Barbados')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (19, N'BD', N'Bangladesz')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (20, N'BE', N'Belgia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (21, N'BF', N'Burkina Faso')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (22, N'BG', N'Bu³garia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (23, N'BH', N'Bahrajn')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (24, N'BI', N'Burundi')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (25, N'BJ', N'Benin')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (26, N'BL', N'Blue')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (27, N'BM', N'Bermudy')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (28, N'BN', N'Brunei')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (29, N'BO', N'Boliwia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (30, N'BR', N'Brazylia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (31, N'BS', N'Bahama')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (32, N'BT', N'Bhutan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (33, N'BV', N'Wyspy Bouveta')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (34, N'BW', N'Botswana')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (35, N'BY', N'Bia³oruœ')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (36, N'BZ', N'Belize')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (37, N'CA', N'Kanada')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (38, N'CC', N'Wyspy Kokosowe   Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (39, N'CD', N'Republika Kongo')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (40, N'CF', N'Republ. Sr.Afr.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (41, N'CG', N'Kongo')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (42, N'CH', N'Szwajcaria')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (43, N'CI', N'Wyb. Koœci S³.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (44, N'CK', N'Wyspy Cooka')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (45, N'CL', N'Chile')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (46, N'CM', N'Kamerun')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (47, N'CN', N'Chiny')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (48, N'CO', N'Kolumbia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (49, N'CR', N'Kostaryka')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (50, N'CS', N'Serbia/Czarnog.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (51, N'CU', N'Kuba')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (52, N'CV', N'Ziel. Przyl. Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (53, N'CX', N'Wyspa Bo¿ Narodz')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (54, N'CY', N'Cypr')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (55, N'CZ', N'Republ. Czeska')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (56, N'DE', N'Niemcy')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (57, N'DJ', N'D¿ibuti')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (58, N'DK', N'Dania')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (59, N'DM', N'Dominika')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (60, N'DO', N'Rep. Dominikany')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (61, N'DZ', N'Algieria')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (62, N'EC', N'Ekwador')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (63, N'EE', N'Estonia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (64, N'EG', N'Egipt')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (65, N'EH', N'Zachod. Sahara')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (66, N'ER', N'Erytrea Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (67, N'ES', N'Hiszpania')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (68, N'ET', N'Etiopia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (69, N'EU', N'Unia Europejska')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (70, N'FI', N'Finlandia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (71, N'FJ', N'Fid¿i')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (72, N'FK', N'Falklandy')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (73, N'FM', N'Mikronezja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (74, N'FO', N'Wyspy Owcze')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (75, N'FR', N'Francja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (76, N'GA', N'Gabon')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (77, N'GB', N'Zjedn. Królest.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (78, N'GD', N'Grenada')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (79, N'GE', N'Gruzja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (80, N'GF', N'Gujana Franc. Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (81, N'GH', N'Ghana')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (82, N'GI', N'Gibraltar')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (83, N'GL', N'Grenlandia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (84, N'GM', N'Gambia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (85, N'GN', N'Gwinea')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (86, N'GP', N'Gwadelupa')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (87, N'GQ', N'Gwinea Równik.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (88, N'GR', N'Grecja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (89, N'GS', N'Sandwich Po³ud.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (90, N'GT', N'Gwatemala')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (91, N'GU', N'Guam')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (92, N'GW', N'Gwinea Bissau')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (93, N'GY', N'Gujana')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (94, N'HK', N'Hongkong Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (95, N'HM', N'Heard/McDonald')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (96, N'HN', N'Honduras')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (97, N'HR', N'Chorwacja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (98, N'HT', N'Haiti')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (99, N'HU', N'Wegry')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (100, N'ID', N'Indonezja')
GO
print 'Processed 100 total records'
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (101, N'IE', N'Irlandia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (102, N'IL', N'Izrael')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (103, N'IN', N'Indie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (104, N'IO', N'Bryt.Ter.Oc.Ind')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (105, N'IQ', N'Irak')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (106, N'IR', N'Iran')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (107, N'IS', N'Islandia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (109, N'KE', N'Kenia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (110, N'KG', N'Kirgistan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (111, N'KH', N'Kambod¿a')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (112, N'KI', N'Kiribati')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (113, N'KM', N'Komory')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (114, N'KN', N'St.Kitts&Nevis')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (115, N'KP', N'Korea Pó³nocna')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (116, N'KR', N'Korea Po³udn.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (117, N'KW', N'Kuwejt')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (118, N'KY', N'Kajmany')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (119, N'KZ', N'Kazachstan Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (120, N'LA', N'Laos')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (121, N'LB', N'Liban')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (122, N'LC', N'Saint Lucia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (123, N'LI', N'Lichtenstein')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (124, N'LK', N'Sri Lanka')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (125, N'LR', N'Liberia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (126, N'LS', N'Lesoto')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (127, N'LT', N'Litwa')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (128, N'LU', N'Luksemburg')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (129, N'LV', N'£otwa')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (130, N'LY', N'Libia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (131, N'MA', N'Maroko')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (132, N'MC', N'Monako')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (133, N'MD', N'Mo³dawia Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (134, N'MG', N'Madagaskar')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (135, N'MH', N'Wyspy Marshalla')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (136, N'MK', N'Macedonia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (137, N'ML', N'Mali')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (138, N'MM', N'Myanmar')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (139, N'MN', N'Mongolia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (140, N'MO', N'Makau')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (141, N'MP', N'Mariany Pó³noc.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (142, N'MQ', N'Martynika')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (143, N'MR', N'Mauretania')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (144, N'MS', N'Montserrat')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (145, N'MT', N'Malta')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (146, N'MU', N'Mauritius')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (147, N'MV', N'Malediwy Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (148, N'MW', N'Malawi')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (149, N'MX', N'Meksyk')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (150, N'MY', N'Malezja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (151, N'MZ', N'Mozambik')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (152, N'NA', N'Namibia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (153, N'NC', N'Nowa Kaledonia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (154, N'NE', N'Nigeria')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (155, N'NF', N'Wyspy Norfolk')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (156, N'NG', N'Nigeria')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (157, N'NI', N'Nikaragua')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (158, N'NL', N'Holandia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (159, N'NO', N'Norwegia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (160, N'NP', N'Nepal')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (161, N'NR', N'Nauru Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (162, N'NT', N'NATO')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (163, N'NU', N'Wyspy Niue')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (164, N'NZ', N'Nowa Zelandia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (165, N'OM', N'Oman')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (166, N'OR', N'Orange')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (167, N'PA', N'Panama')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (168, N'PE', N'Peru')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (169, N'PF', N'Polinezja Fran.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (170, N'PG', N'Papua-Nowa Gw.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (171, N'PH', N'Filipiny')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (172, N'PK', N'Pakistan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (173, N'PL', N'Polska')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (174, N'PM', N'StPier.,Miquel.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (175, N'PN', N'Wyspy Pitcairn Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (176, N'PR', N'Puerto Rico')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (177, N'PS', N'Palestyna')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (178, N'PT', N'Portugalia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (179, N'PW', N'Palau')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (180, N'PY', N'Paragwaj')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (181, N'QA', N'Katar')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (182, N'RE', N'Reunion')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (183, N'RO', N'Rumunia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (184, N'RU', N'Federacja Ros.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (185, N'RW', N'Ruanda')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (186, N'SA', N'Arabia Saudyjs.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (187, N'SB', N'Wyspy Salomona')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (188, N'SC', N'Seszele')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (189, N'SD', N'Sudan Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (190, N'SE', N'Szwecja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (191, N'SG', N'Singapur')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (192, N'SH', N'Sw. Helena')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (193, N'SI', N'S³owenia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (194, N'SJ', N'Svalbard')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (195, N'SK', N'S³owacja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (196, N'SL', N'Sierra Leone')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (197, N'SM', N'San Marino')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (198, N'SN', N'Senegal')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (199, N'SO', N'Somalia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (200, N'SR', N'Surinam')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (201, N'ST', N'S.Tome,Principe')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (202, N'SV', N'Salwador')
GO
print 'Processed 200 total records'
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (203, N'SY', N'Syria Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (204, N'SZ', N'Suazi')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (205, N'TC', N'Turks i Caicos')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (206, N'TD', N'Czad')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (207, N'TF', N'Franc. Teryt.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (208, N'TG', N'Togo')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (209, N'TH', N'Tajlandia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (210, N'TJ', N'Tad¿ykistan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (211, N'TK', N'Wyspy Tokelau')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (212, N'TL', N'Timor')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (213, N'TM', N'Turkmenia')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (214, N'TN', N'Tunezja')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (215, N'TO', N'Wyspy Tonga')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (216, N'TP', N'Timor Wschodni')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (217, N'TR', N'Turcja Klucz kraju Oznaczenie')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (218, N'TT', N'Trinidad,Tobago')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (219, N'TV', N'Tuvalu')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (220, N'TW', N'Tajwan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (221, N'TZ', N'Tanzania')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (222, N'UA', N'Ukraina')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (223, N'UG', N'Uganda')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (224, N'UM', N'Wys.Minor Outl.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (225, N'UN', N'Narody Zjedn.')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (226, N'US', N'USA')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (227, N'UY', N'Urugwaj')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (228, N'UZ', N'Uzbekistan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (229, N'VA', N'Watykan')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (230, N'VC', N'W. Sw. Wincenta')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (231, N'VE', N'Wenezuela ')
INSERT [dbo].[SAPKodKraju] ([id], [kod], [kraj]) VALUES (232, N'IT', N'W³ochy')
SET IDENTITY_INSERT [dbo].[SAPKodKraju] OFF
/****** Object:  Table [dbo].[RL_Schemat]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RL_Schemat](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[nazwa] [varchar](100) NULL,
	[kod] [char](1) NULL,
	[wzorzec] [varchar](255) NULL,
	[priority] [int] NULL,
	[detailsPattern] [varchar](100) NULL,
	[NextIfYes] [int] NULL,
	[NextIfNo] [int] NULL,
	[MatchMode] [varchar](1) NULL,
 CONSTRAINT [PK_RL_Schemat] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RL_Konfig]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RL_Konfig](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[typDB] [int] NULL,
	[rodzajDB] [int] NULL,
	[srvName] [varchar](100) NULL,
	[DbName] [varchar](100) NULL,
	[pwd] [varchar](100) NULL,
	[logId] [varchar](100) NULL,
	[WinLogon] [bit] NULL,
	[srvAlias] [varchar](100) NULL,
	[EndpointWS] [varchar](512) NULL,
	[WSLogon] [varchar](100) NULL,
	[WSpwd] [varchar](100) NULL,
	[ERPLogon] [varchar](100) NULL,
	[dbversion] [varchar](30) NULL,
	[sp_name] [varchar](100) NULL,
 CONSTRAINT [PK_RL_Konfig] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OperacjaKonfig]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OperacjaKonfig](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[typKns] [int] NULL,
	[srvName] [varchar](100) NULL,
	[DbName] [varchar](100) NULL,
	[pwd] [varchar](100) NULL,
	[logId] [varchar](100) NULL,
	[WinLogon] [bit] NULL,
	[NazwaOpr] [varchar](255) NULL,
	[KodOpr] [varchar](2) NULL,
	[OdDo] [bit] NULL,
	[spName] [varchar](255) NULL,
	[numerOpr] [int] NULL,
 CONSTRAINT [PK_OperacjaKonfig] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[OperacjaKonfig] ON
INSERT [dbo].[OperacjaKonfig] ([id], [typKns], [srvName], [DbName], [pwd], [logId], [WinLogon], [NazwaOpr], [KodOpr], [OdDo], [spName], [numerOpr]) VALUES (1, 0, N'XPROGLEX', N'ww7', N'DQ2d4OrGopU=', N'sa', 0, N'Zwroty 34 op³aty', N'ZP', 1, N'sp_Dupa', NULL)
SET IDENTITY_INSERT [dbo].[OperacjaKonfig] OFF
/****** Object:  Table [dbo].[Konfiguracja]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Konfiguracja](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[typKns] [int] NULL,
	[srvName] [varchar](100) NULL,
	[DbName] [varchar](100) NULL,
	[pwd] [varchar](100) NULL,
	[logId] [varchar](100) NULL,
	[WinLogon] [bit] NULL,
	[JednostkaGospodarcza] [varchar](4) NULL,
	[StartImportDate] [datetime] NULL,
	[srvAlias] [varchar](100) NULL,
	[EndpointWS] [varchar](512) NULL,
	[WSLogon] [varchar](100) NULL,
	[WSpwd] [varchar](100) NULL,
	[typImportSAP] [int] NULL,
	[PrzypisFile] [varchar](255) NULL,
	[OdpisFile] [varchar](255) NULL,
	[skipraty] [bit] NULL,
	[StanowiskoFin] [varchar](4) NULL,
	[ERPLogon] [varchar](100) NULL,
	[defSad] [bit] NULL,
	[dbversion] [varchar](30) NULL,
	[czyautoks] [int] NULL,
	[czyautoprzyp] [int] NULL,
 CONSTRAINT [PK_Konfiguracja] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Konfiguracja] ON
INSERT [dbo].[Konfiguracja] ([id], [typKns], [srvName], [DbName], [pwd], [logId], [WinLogon], [JednostkaGospodarcza], [StartImportDate], [srvAlias], [EndpointWS], [WSLogon], [WSpwd], [typImportSAP], [PrzypisFile], [OdpisFile], [skipraty], [StanowiskoFin], [ERPLogon], [defSad], [dbversion], [czyautoks], [czyautoprzyp]) VALUES (1, 0, N'XPROGLEX', N'ww_jaslo', N'DQ2d4OrGopU=', N'sa', 0, N'4208', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'2.5.0', NULL, NULL)
SET IDENTITY_INSERT [dbo].[Konfiguracja] OFF
/****** Object:  Table [dbo].[KonfigImport]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[KonfigImport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NazwaOpr] [varchar](255) NULL,
	[KodOpr] [varchar](2) NULL,
	[OdDo] [bit] NULL,
	[spC] [varchar](255) NULL,
	[spZ] [varchar](255) NULL,
	[spO] [varchar](255) NULL,
	[spA] [varchar](255) NULL,
	[spI1] [varchar](255) NULL,
	[czyFinDB] [bit] NULL,
	[numer] [int] NULL,
 CONSTRAINT [PK_KonfigImport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[KonfigImport] ON
INSERT [dbo].[KonfigImport] ([Id], [NazwaOpr], [KodOpr], [OdDo], [spC], [spZ], [spO], [spA], [spI1], [czyFinDB], [numer]) VALUES (1, N'Zwroty 3_4', N'ZS', 0, N'sp_Dupa', NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[KonfigImport] OFF
/****** Object:  Table [dbo].[KnsSad]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[KnsSad](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sad_Id] [int] NOT NULL,
	[Nazwa] [varchar](100) NULL,
	[SAPSad_Id] [varchar](4) NULL,
	[SAPWydz_Id] [varchar](10) NULL,
	[SAPtypSad] [varchar](2) NULL,
 CONSTRAINT [PK_KnsSad] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[KnsKsiegi]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[KnsKsiegi](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nazwa] [varchar](60) NULL,
	[id_sadu] [int] NULL,
	[sad] [varchar](100) NULL,
	[wydzial] [varchar](100) NULL,
	[kodSad] [varchar](4) NULL,
	[numWydz] [varchar](10) NULL,
	[oprKosztFiz] [varchar](4) NULL,
	[oprKosztPraw] [varchar](4) NULL,
	[oprGrzFiz] [varchar](4) NULL,
	[oprGrzPraw] [varchar](4) NULL,
	[rodzajPrzedmiotu] [varchar](4) NULL,
	[Id_Ksiegi] [int] NULL,
	[oprGrzSamoistna] [varchar](4) NULL,
	[czyFPP] [int] NULL,
 CONSTRAINT [PK_KnsKsiegi] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[KnsKomornik]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[KnsKomornik](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](200) NOT NULL,
	[Miasto] [varchar](50) NULL,
	[Ulica] [varchar](50) NULL,
	[NIP] [varchar](10) NULL,
	[Komornik_id] [int] NOT NULL,
 CONSTRAINT [PK_KnsKomornik] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_SadWydzOr]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SadWydzOr]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		   @nextday Datetime,
		   @query varchar(MAX),
		   @nextdayString varchar(12),
		   @SAPjednostka varchar(4)
		   set @dbname = 'OrComNS'
		   IF  CHARINDEX ( '@@' , @sourcesrv ) > 0  
		BEGIN
		 set @SAPjednostka = Substring(@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) +2,4)
		 set @sourcesrv = left (@sourcesrv,CHARINDEX ( '@@' , @sourcesrv ) -1 )
		END
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		    

		   
   
  		  
  		  
set @query = ' select s.Adresatid as Id, max(rtrim(s.nazwa + '' '' +  rtrim(isnull(s.skrot,'''')+ '' '' )) ) + rtrim(isnull(a.adres,'''')) as nazwa , count(*) as ile ' +
             ' from  ' + @sourcesrv + '.' + @dbname + '.dbo.adresat s ' +
             ' inner join  ' + @sourcesrv + '.' + @dbname + '.dbo.Adres a on a.AdresId = s.AdresId ' +
             ' inner join  ' + @sourcesrv + '.' + @dbname + '.dbo.Sprawa spr on spr.OrzeczenieSadAdresatId = s.AdresatId ' +
             ' inner join  ' + @sourcesrv + '.' + @dbname + '.dbo.JednostkaWydzial jw on jw.JednostkaWydzialId = spr.JednostkaWydzialId ' +
             ' inner join  ' + @sourcesrv + '.' + @dbname + '.dbo.Jednostka j on j.JednostkaId = jw.JednostkaId ' +
             ' where j.SAP_JednGospId = ' + @SAPjednostka + ' and ( ( select isnull(sum(zapis.przypis), 0) - isnull(sum(zapis.uiszczenie), 0) - isnull(sum(zapis.odpis), 0) from  ' + @sourcesrv + '.' + @dbname + '.dbo.zapis where zapis.naleznoscTypId = 0 and zapis.SprawaId = spr.SprawaId and data < ' + @nextdaystring + ' ) > 0 ' +
             ' or (select isnull(sum (zapis.przypis), 0) - isnull(sum (zapis.uiszczenie), 0) - isnull(sum(zapis.odpis), 0) from  ' + @sourcesrv + '.' + @dbname + '.dbo.zapis where zapis.naleznoscTypId = 1 and zapis.SprawaId = spr.SprawaId and data < ' + @nextdaystring + ' )> 0 )'  +
              ' group by s.AdresatId, j.SAP_JednGospId, a.adres  order by ile desc '

 
print @query
Exec (@query)			   


end
GO
/****** Object:  StoredProcedure [dbo].[sp_SadWydzCR]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SadWydzCR]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		   @nextday Datetime,
		   @query varchar(MAX),
		   @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		    

		   
   
  		  
  		  
set @query = ' select s.id as Id, max(rtrim(isnull(s.nazwa,'''')+ '' '' + isnull(s.nazwa2,'''') +  isnull('', '' + s.miejsce,''''))) as nazwa , count(*) as ile ' +
             ' from  ' + @sourcesrv + '.' + @dbname + '.dbo.skor s inner join  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_sprawa spr on spr.id_sad = s.id ' +
             ' where ( select isnull(sum(kns_dz_nal.przypis_grzywny ), 0) - isnull(sum(kns_dz_nal.uiszczenia_grzywny), 0) - isnull(sum(kns_dz_nal.odpisanie_grzywny), 0) from  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r < ' + @nextdaystring + ' ) > 0 ' +
             ' or (select isnull(sum (kns_dz_nal.przypis_kosztow), 0) - isnull(sum (kns_dz_nal.uiszczenia_kostow), 0) - isnull(sum(kns_dz_nal.odpisanie_kosztow), 0) from  ' + @sourcesrv + '.' + @dbname + '.dbo.kns_dz_nal where kns_dz_nal.id_sprawy = spr.id and data_r  < ' + @nextdaystring + ' )> 0 '  +
              ' group by s.id  order by ile desc '

 
print @query
Exec (@query)			   
end
GO
/****** Object:  StoredProcedure [dbo].[sp_SadWydz]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SadWydz]
	
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @dataDo DateTime
	 
AS
BEGIN
  DECLARE 
		   @nextday Datetime,
		   @query varchar(MAX),
		   @nextdayString varchar(12)
  		  
  		  set @nextday  = DateAdd(d,1,@dataDo) 
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @sourcesrv  = '"' + @sourcesrv + '"' 
  		    

		   
   
  		  
  		  
set @query = ' select  slas.kod  as Id, max(rtrim(isnull(slas.nazwa,'''')))  +  '' '' + max(rtrim(isnull(slas.nazwa1,''''))) + '' '' + max(rtrim(isnull(slas.miejscowosc,''''))) as nazwa, ' +
              ' count(*) as ile ' +
              ' from ( select nal.id_dluznik, sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis )   as grzywna, ' +
              '  sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis )	as koszty ' +
              ' from ' + @sourcesrv + '.' + @dbname + '.dbo.naleznosci_dziennik nal where  isnull(nal.data_operacji,nal.data_wprow_zapisu) <  ' + @nextdaystring + '  and isnull ( nal.data_usun_zapisu,''2099-12-31'') > ' + @nextdaystring  +
              ' group by id_dluznik  ' +
              '  having sum(nal.grzywna_przypis  -  nal.grzywna_uiszcz - nal.grzywna_odpis ) > 0 or sum(nal.oplatakoszty_przypis  -  nal.oplatakoszty_uiszcz - nal.oplatakoszty_odpis ) > 0 ) nals ' +
              ' LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.Dluznik dlu  ON  dlu.id_dluznik=  nals.id_dluznik ' +
              ' LEFT JOIN ' + @sourcesrv + '.' + @dbname + '.dbo.DLUZNIK_SPRAWA_SADOWA dlss ON dlss.id_dluznik  = dlu.id_dluznik ' +
              ' LEFT Join ' + @sourcesrv + '.' + @dbname + '.dbo.SL_ADR_SADOW slas ON dlss.id_sad_obcy = slas.kod ' +
              ' where slas.kod is not null ' +
              ' group by slas.kod ' +
              ' order by ile desc '

 
print @query
Exec (@query)			   
end
GO
/****** Object:  Table [dbo].[WalidSaldo]    Script Date: 08/13/2015 17:01:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WalidSaldo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Klucz] [uniqueidentifier] NULL,
	[KartaDl] [varchar](50) NULL,
	[Sygnatura] [varchar](50) NULL,
	[Naleznosc] [varchar](20) NULL,
	[Kwota] [decimal](12, 2) NULL,
	[Status] [varchar](255) NULL,
	[OpGlowna] [varchar](4) NULL,
	[OpCzesc] [varchar](4) NULL,
	[DataKsiegowania] [datetime] NULL,
	[Dluznik] [varchar](255) NULL,
	[SprawaId] [int] NULL,
	[Ksiega] [int] NULL,
	[SAPKwota] [decimal](12, 2) NULL,
	[KsiegaOpis] [varchar](100) NULL,
 CONSTRAINT [PK_WalidSaldo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [dbo].[SplitNumbers]    Script Date: 08/13/2015 17:01:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[SplitNumbers] 
(  -- separator = , 
   @List      VARCHAR(MAX)
  
)
RETURNS TABLE
AS
  RETURN ( SELECT Item = CONVERT(INT, Item) FROM
      ( SELECT Item = x.i.value('(./text())[1]', 'varchar(max)')
        FROM ( SELECT [XML] = CONVERT(XML, '<i>'
        + REPLACE(@List, ',' , '</i><i>') + '</i>').query('.')
          ) AS a CROSS APPLY [XML].nodes('i') AS x(i) ) AS y
      WHERE Item IS NOT NULL
  );
GO
/****** Object:  Table [dbo].[BankiKonfig]    Script Date: 08/13/2015 17:01:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BankiKonfig](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Label] [varchar](100) NULL,
	[ExePath] [varchar](255) NULL,
	[Folder] [varchar](255) NULL,
	[LastRunDate] [datetime] NULL,
	[LastRunStatus] [int] NULL,
	[Message] [varchar](255) NULL,
 CONSTRAINT [PK_BankiKonfig] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Wplata]    Script Date: 08/13/2015 17:01:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Wplata](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SAPDocPRef] [varchar](20) NULL,
	[SAPDokRozliczeniowy] [varchar](20) NULL,
	[SAPRodzajDok] [varchar](10) NULL,
	[DataRozlicz] [datetime] NULL,
	[SAPDokRozliczany] [varchar](20) NULL,
	[Kwota] [decimal](18, 2) NULL,
	[tytulem] [varchar](255) NULL,
	[Transfer_Id] [int] NULL,
	[DataWplaty] [datetime] NULL,
 CONSTRAINT [PK_Wplata] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Dluznik]    Script Date: 08/13/2015 17:01:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Dluznik](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FizPraw] [char](1) NOT NULL,
	[Imie] [varchar](40) NULL,
	[Nazwisko] [varchar](40) NULL,
	[Ulica] [varchar](60) NULL,
	[NrDomu] [varchar](10) NULL,
	[NrMieszkania] [varchar](10) NULL,
	[KodPocztowy] [varchar](10) NULL,
	[Miejscowosc] [varchar](40) NULL,
	[KluczKraju] [varchar](2) NULL,
	[Iban] [varchar](28) NULL,
	[Nip] [varchar](10) NULL,
	[Pesel] [varchar](11) NULL,
	[RBN] [varchar](2) NULL,
	[Sprawa_Id] [int] NULL,
	[KnsDluz_Id] [int] NULL,
	[SAPKontoPartnera] [varchar](50) NULL,
 CONSTRAINT [PK_Dluznik] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Dokument]    Script Date: 08/13/2015 17:01:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Dokument](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DocGuid] [uniqueidentifier] NOT NULL,
	[DataDokumentu] [datetime] NULL,
	[DataKsiegowania] [datetime] NULL,
	[grzSamoistna] [varchar](1) NULL,
	[kwota] [decimal](18, 2) NULL,
	[OperacjaCzesciowa] [varchar](4) NULL,
	[DataPlatnosci] [datetime] NULL,
	[Stan] [varchar](1) NULL,
	[Opis] [varchar](50) NULL,
	[typFakt] [varchar](2) NULL,
	[Sprawa_Id] [int] NULL,
	[SrcSystemId] [int] NULL,
	[SrcDocumentHash] [int] NULL,
	[Document_Id] [int] NULL,
	[DocGuid_Ref] [uniqueidentifier] NULL,
	[SAPDocId] [varchar](100) NULL,
	[KnsPozDzNal] [int] NULL,
	[KnsRokDzNal] [int] NULL,
	[KnsKsiegaDzNal] [int] NULL,
	[Transfer_Id] [int] NULL,
	[Dluznik_Id] [int] NULL,
	[RataKwota1] [decimal](18, 2) NULL,
	[RataData1] [datetime] NULL,
	[RataKwota2] [decimal](18, 2) NULL,
	[RataData2] [datetime] NULL,
	[RataKwota3] [decimal](18, 2) NULL,
	[RataData3] [datetime] NULL,
	[RataKwota4] [decimal](18, 2) NULL,
	[RataData4] [datetime] NULL,
	[RataKwota5] [decimal](18, 2) NULL,
	[RataData5] [datetime] NULL,
	[RataKwota6] [decimal](18, 2) NULL,
	[RataData6] [datetime] NULL,
	[RataKwota7] [decimal](18, 2) NULL,
	[RataData7] [datetime] NULL,
	[RataKwota8] [decimal](18, 2) NULL,
	[RataData8] [datetime] NULL,
	[RataKwota9] [decimal](18, 2) NULL,
	[RataData9] [datetime] NULL,
	[RataKwota10] [decimal](18, 2) NULL,
	[RataData10] [datetime] NULL,
	[RataKwota11] [decimal](18, 2) NULL,
	[RataData11] [datetime] NULL,
	[RataKwota12] [decimal](18, 2) NULL,
	[RataData12] [datetime] NULL,
	[RataKwota13] [decimal](18, 2) NULL,
	[RataData13] [datetime] NULL,
	[RataKwota14] [decimal](18, 2) NULL,
	[RataData14] [datetime] NULL,
	[RataKwota15] [decimal](18, 2) NULL,
	[RataData15] [datetime] NULL,
	[RataKwota16] [decimal](18, 2) NULL,
	[RataData16] [datetime] NULL,
	[RataKwota17] [decimal](18, 2) NULL,
	[RataData17] [datetime] NULL,
	[RataKwota18] [decimal](18, 2) NULL,
	[RataData18] [datetime] NULL,
	[RataKwota19] [decimal](18, 2) NULL,
	[RataData19] [datetime] NULL,
	[RataKwota20] [decimal](18, 2) NULL,
	[RataData20] [datetime] NULL,
	[RataKwota21] [decimal](18, 2) NULL,
	[RataData21] [datetime] NULL,
	[RataKwota22] [decimal](18, 2) NULL,
	[RataData22] [datetime] NULL,
	[RataKwota23] [decimal](18, 2) NULL,
	[RataData23] [datetime] NULL,
	[RataKwota24] [decimal](18, 2) NULL,
	[RataData24] [datetime] NULL,
	[RataKwota25] [decimal](18, 2) NULL,
	[RataData25] [datetime] NULL,
	[RataKwota26] [decimal](18, 2) NULL,
	[RataData26] [datetime] NULL,
	[RataKwota27] [decimal](18, 2) NULL,
	[RataData27] [datetime] NULL,
	[RataKwota28] [decimal](18, 2) NULL,
	[RataData28] [datetime] NULL,
	[RataKwota29] [decimal](18, 2) NULL,
	[RataData29] [datetime] NULL,
	[RataKwota30] [decimal](18, 2) NULL,
	[RataData30] [datetime] NULL,
	[RataKwota31] [decimal](18, 2) NULL,
	[RataData31] [datetime] NULL,
	[RataKwota32] [decimal](18, 2) NULL,
	[RataData32] [datetime] NULL,
	[RataKwota33] [decimal](18, 2) NULL,
	[RataData33] [datetime] NULL,
	[RataKwota34] [decimal](18, 2) NULL,
	[RataData34] [datetime] NULL,
	[RataKwota35] [decimal](18, 2) NULL,
	[RataData35] [datetime] NULL,
	[RataKwota36] [decimal](18, 2) NULL,
	[RataData36] [datetime] NULL,
	[SAPRatyId] [varchar](20) NULL,
	[SAPDocIdRef] [varchar](20) NULL,
	[Info] [varchar](255) NULL,
	[OperacjaGlowna] [varchar](4) NULL,
	[SAPImportInfo] [varchar](1024) NULL,
	[SAPImportStatus] [int] NULL,
	[SAPImportDate] [datetime] NULL,
	[SAPImportPonowne] [varchar](1) NULL,
	[SAPRodzajDokumentu] [varchar](2) NULL,
	[SAPKontoKG] [varchar](10) NULL,
	[SAPWaluta] [varchar](3) NULL,
	[SAPKluczUzgodnienia] [varchar](12) NULL,
 CONSTRAINT [PK_Dokument] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[v_DokDoOdpisu]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view  [dbo].[v_DokDoOdpisu]
as
select 
0 as status, '                                                                        ' as informacja, do.typfakt, do.SAPDocId ,
do.kwota, 0.00 as SaldoSAP,  do.DataKsiegowania, do.operacjaglowna as OperacjaGlowna,
 do.Operacjaczesciowa as OperacjaCzesciowa, do.SapRodzajdokumentu as RodzajDokumentu,  sp.karta as Karta, sp.sygnatura as Sygnatura, 
 sp.SAPSadId as Sad_sygn, sp.SAPPrzedmiotUmowy as PrzedmiotUmowy, sp.SAPKontoUmowy as KontoUmowy, dl.Imie, dl.Nazwisko, dl.Pesel , sp.KnsKsiega, sp.KdRok, 
sp.KdNumer 
from dokument do 
left join Dluznik dl on dl.Id = do.Dluznik_Id
left join Sprawa sp on sp.Id = dl.Sprawa_Id
where DataKsiegowania >= '2014-05-31'
and typFakt in ('GS','KS', 'GP', 'KP') and  len(isnull(sp.SAPPrzedmiotUmowy,'')) > 0 and  len(isnull(sp.SAPKontoUmowy,'')) > 0 and  len(isnull(do.SAPDocId,'')) > 0
GO
/****** Object:  StoredProcedure [dbo].[sp_RozpoznajPrzelewCR]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sp_RozpoznajPrzelewCR]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @key varchar(100),
	 @wydzial varchar(10),
	 @repertorium varchar (10),
	 @numer int,
	 @rok   int,
	 @skipkns int,
	 @idList varchar(max)
	 
AS
BEGIN
  DECLARE @sygnatura varchar(50),
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		   @query4 varchar(MAX),
		   @oznKontaUm varchar(50),
		  --@query text,
  		  @dzienString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortdzienString varchar(12),
  		  @searchString varchar(60),
  		  @expression	varchar(100)
  		  /*
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dzien,120)  +''''
  		  */
  		  set @sourcesrv  = '"' + @sourcesrv + '"'
if (left(@key,1) = 'S' or left(@key,1) = 'N'  or  left(@key,1) = 'U' or  left(@key,1) = 'M' or   left(@key,1) = 'I' or   left(@key,1) = 'W' or   left(@key,1) = 'Z' or   left(@key,1) = 'F' )
BEGIN  		  	

set @oznKontaUm = (select JednostkaGospodarcza + case when len(StanowiskoFin)> 0 then '/' + StanowiskoFin else '' end + ' Dochody nieprzypisane'  from konfiguracja )

set @query1 = 
' WITH  pelnomocnicy as '+
' (  SELECT br.ident as id_podmioty, ' +
'      br.id_strony as id_strony, ' +
'      br.id_sprawy as id_sprawy, ' +
'         stat.nazwa as rodzaj, ' + 
' '''' as uwagi,  ' + 
'        case when len(isnull(rtrim(ob.instytucja),'''')) > 0  then rtrim(ob.instytucja)  else   isnull(rtrim(ob.nazwisko),'''') end as Nazwa1,  ' +   
'   case when  len(isnull(rtrim(ob.instytucja),'''')) > 0    then rtrim(ob.oddzial)    else    ltrim(isnull(rtrim(ob.imie),''''))  end as Nazwa2, ' +   
'   case when  len(isnull(rtrim(ob.instytucja),'''')) > 0 and len(isnull(rtrim(ob.nazwisko),'''')) > 0  then isnull(rtrim(ob.nazwisko),'''') + '' '' + isnull(rtrim(ob.imie),'''')    else ''''  end as Nazwa3, '+
'	'''' as  nip, ob.pesel as pesel, ' +
'    0 as typ ,  ' + 
'    rtrim(ob.ulica) as ulica, ' +
'    rtrim(ob.numer) as nr_domu, ' +
'    ''''  as nr_mieszkania, ' +
'   rtrim(ob.miejscowosc) as miejscowosc, ' + 
'	rtrim(ob.poczta) as poczta, ' + 
'   rtrim(ob.kod) as kod, ' + 
'   rtrim(ob.kraj) as kraj, '+	 
'		 stat.typ_roli, '  +
'  ''''  as mikro_iban,  ' +   
'  ''''  as nr_konta, ' +
'		  case when len(isnull(rtrim(ob.instytucja),'''')) > 0  then ''I'' else ''O'' end as fizpraw '  +	 
' FROM   ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.broni br  left outer join '  +
'		 ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.status stat on br.id_statusu = stat.ident inner join   ' +  
'         ' + rtrim(@sourcesrv) + '.' + rtrim (@dbname) + '.dbo.obronca ob on  br.id_obroncy = ob.ident ' +
'   WHERE  ( br.czyus = 0  AND   ob.czyus = 0  ) ),  ' +
' strony as  ' +
			    ' ( ' +
'	SELECT  ' +
' st.ident as id_podmioty, ' +
' 0 as id_strony , '+
' st.id_sprawy as id_sprawy , ' +
'  stat.nazwa as rodzaj, ' +
' '''' as uwagi,  ' + 
' IsNull( rtrim(ds.nazwisko) ,'''' )  as Nazwa1 ,' + 
' IsNull( rtrim(ds.imie) ,'''') as Nazwa2 ,' + 
' '''' as Nazwa3 ,' + 
' rtrim(ds.nip) as nip, ' +
' rtrim(ds.pesel) as pesel, ' + 
' 0 as typ, ' +
'    rtrim(ad.ulica) as ulica, ' +
'    rtrim(ad.numer) as nr_domu, ' +
'    ''''  as nr_mieszkania, ' +
'   rtrim(ad.miejscowosc) as miejscowosc, ' + 
'	rtrim(ad.poczta) as poczta, ' + 
'   rtrim(ad.kod) as kod, ' + 
'   rtrim(ad.kraj) as kraj, ' +
'  stat.typ_roli, ' +  
'  rtrim(st.mikro_iban) as mikro_iban,  ' +   
'  rtrim(st.nr_konta) as nr_konta, '  +   
'  case when ds.fizpraw = 0 then  ''O''  else ''I'' end as fizpraw ' +    
' FROM   ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.strona st  left outer join '  +
'		  ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.status stat on st.id_statusu = stat.ident inner join   ' +  
'         ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.dane_strony ds on st.id_danych = ds.ident left outer join ' +
'          ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.adres ad on ad.id_strony = st.ident ' +
'   WHERE    ( st.czyus = 0  AND   ds.czyus = 0 and isnull(ad.czybiezacy,1) = 1 and ad.czyus = 0 )   ' +
' union all ' +
' SELECT e.ident as  id_podmioty, ' + 
' 0 as id_strony , '+
' e.id_sprawy as id_sprawy , ' +
'        stat.nazwa as rodzaj, ' +
' '''' as uwagi,  ' + 
'	rtrim(isnull(ins.nazwisko,'''')) as Nazwa1,  ' +   
'         rtrim(isnull(ins.imie,'''')) as Nazwa2,   ' +
'         rtrim(isnull(ins.instytucja,'''')) as Nazwa3,   ' +
'	'''' as  nip, ins.pesel as pesel, ' +
'    0 as typ ,  ' + 
'    rtrim(ad.ulica) as ulica, ' +
'    rtrim(ad.numer) as nr_domu, ' +
'    ''''  as nr_mieszkania, ' +
'   rtrim(ad.miejscowosc) as miejscowosc, ' + 
'	rtrim(ad.poczta) as poczta, ' + 
'   rtrim(ad.kod) as kod, ' + 
'   rtrim(ad.kraj) as kraj, ' +
'		 stat.typ_roli, '  +
'  ''''  as mikro_iban,  ' +   
'  ''''  as nr_konta, ' +
'	case when fizpraw = 0  then 	 ''O'' else ''I'' end  as fizpraw '  +	 			   
' FROM  ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.ekspertyza  e  left outer join '  +
'		 ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.status stat on e.id_statusu = stat.ident inner join   ' +  
'         ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.inna_strona ins on  e.id_innej = ins.ident left outer join ' +
'         ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.adres ad on ad.id_ekspertyzy = e.ident ' +
'   WHERE ( e.czyus = 0 and  len(rtrim(isnull(ins.nazwisko,''''))) > 0 ) ' +
'  UNION ALL ' +  
' SELECT e.ident as  id_podmioty, ' + 
' 0 as id_strony , '+
' e.id_sprawy as id_sprawy , ' +
'        stat.nazwa as rodzaj, ' +
' '''' as uwagi,  ' + 
'	rtrim(isnull(ins.Instytucja,'''')) as Nazwa1,  ' +   
'         rtrim(isnull(ins.oddzial,'''')) as Nazwa2,   ' +
'         ''''  as Nazwa3,   ' +
'	'''' as  nip, ins.pesel as pesel, ' +
'    0 as typ ,  ' + 
'    rtrim(ad.ulica) as ulica, ' +
'    rtrim(ad.numer) as nr_domu, ' +
'    ''''  as nr_mieszkania, ' +
'   rtrim(ad.miejscowosc) as miejscowosc, ' + 
'	rtrim(ad.poczta) as poczta, ' + 
'   rtrim(ad.kod) as kod, ' + 
'   rtrim(ad.kraj) as kraj, ' +
'		 stat.typ_roli, '  +
'  ''''  as mikro_iban,  ' +   
'  ''''  as nr_konta, ' +
'	 ''I''    as fizpraw '  +	 			   
' FROM  ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.ekspertyza  e  left outer join '  +
'		 ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.status stat on e.id_statusu = stat.ident inner join   ' +  
'         ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.inna_strona ins on  e.id_innej = ins.ident left outer join ' +
'         ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.adres ad on ad.id_ekspertyzy = e.ident ' +
'   WHERE ( e.czyus = 0 and  len(rtrim(isnull(ins.nazwisko,''''))) = 0 ) ' +
' union all ' + 
' select   id_podmioty, ' +
'         id_strony, ' +
'          id_sprawy, ' +
'            rodzaj, ' + 
'  ( select rtrim(isnull(dans.nazwisko,'''')) + '' '' + rtrim(isnull(dans.imie,''''))  from  ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.dane_strony dans inner join ' + rtrim(@sourcesrv) + '.' + rtrim(@dbname) + '.dbo.strona str on str.id_danych = dans.ident where str.ident = pelnomocnicy.id_strony ) as uwagi, ' +   
'         Nazwa1,  ' +   
'    Nazwa2, ' +   
'    Nazwa3, '+
'	  nip,  pesel, ' +
'     typ ,  ' + 
'     ulica, ' +
'     nr_domu, ' +
'     nr_mieszkania, ' +
'    miejscowosc, ' + 
'	 poczta, ' + 
'    kod, ' + 
'	 kraj, '+	 
'   typ_roli, '  +
'    mikro_iban,  ' +   
'     nr_konta, ' +
'		 fizpraw '  +	
' from pelnomocnicy ),'  
  
  
set @query2 = 
' sprawaS as (SELECT      SPRAWa.ident as id_sprawy,  ' + 
'                      SPRAWa.numer as nr, SPRAWa.rok as rok, ' + 
' rtrim(ko.oznaczenie)  +rtrim(re.symbol) +'' ''+rtrim(cast(sprawa.numer as varchar)) + ''/'' + right(cast(sprawa.rok as varchar(4)),2) as sygnatura_sprawy, ' +
'                      isnull(SPRAWa.d_zakreslenia,''2050-01-01'') as data_kon, ' + 
'                     rtrim(re.symbol) AS repertorium, ' + 
'                      rtrim(ko.oznaczenie) AS kodWydzial, ' + 
'                      case re.wydzial when 1 then ''WK'' when 2 then ''WC'' else '''' end  as rodzWydz ,  ' +
'					   replace(rtrim(ko.oznaczenie)  +rtrim(re.symbol) +'' ''+rtrim(cast(sprawa.numer as varchar)) + ''/'' + right(cast(sprawa.rok as varchar(4)),2),'' '','''') as sygnShort, ' +
'					   Sprawa.sygnat_powoda as sygnObca, ' +
'					   Sprawa.kwota_spr as WPS ' +
' FROM                   ' +   @sourcesrv +'.'  + @dbname +'.dbo.sprawa INNER JOIN ' + 
'                        ' +   @sourcesrv +'.'  + @dbname +'.dbo.repertorium re ON sprawa.repertorium = re.numer cross join' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.konfig ko ' + 
' WHERE  sprawa.czyus = 0 and re.rodzaj = 0' +
' ) ' 

set @query3 =  ' select ''ORZCZ'' as ZrodloDanych, s.id_podmioty as IdStrony, case  s.fizpraw when ''I'' then ''X'' else '''' end as typPartnera, s.id_sprawy as IdSprawy, s.Nazwa1 COLLATE Polish_CI_AS as Nazwa1 , s.Nazwa2 COLLATE Polish_CI_AS as Nazwa2, s.Nazwa3 COLLATE Polish_CI_AS as Nazwa3 , replace(s.nip,''-'','''') COLLATE Polish_CI_AS as nip, s.pesel COLLATE Polish_CI_AS as pesel, s.rodzaj COLLATE Polish_CI_AS as rola , s.uwagi COLLATE Polish_CI_AS as uwagi, ' + 
'   spr.kodWydzial COLLATE Polish_CI_AS as kodWydzial, spr.repertorium COLLATE Polish_CI_AS as repertorium , spr.nr, spr.rok, spr.sygnatura_sprawy COLLATE Polish_CI_AS as sygnatura,spr.rodzWydz COLLATE Polish_CI_AS as rodzWydz, spr.data_Kon,s.ulica COLLATE Polish_CI_AS as ulica , s.nr_domu COLLATE Polish_CI_AS as nr_domu , ' +
'    s.nr_mieszkania COLLATE Polish_CI_AS as nr_mieszkania ,s.miejscowosc COLLATE Polish_CI_AS as miejscowosc, s.kod COLLATE Polish_CI_AS as kod , s.kraj COLLATE Polish_CI_AS as kraj, 0 as Ksiega , ''' +   @oznKontaUm  + ''' COLLATE Polish_CI_AS as  OznKontaUmowy , ''DO'' COLLATE Polish_CI_AS as TypKontaUmowy, '''' COLLATE Polish_CI_AS as RelacjaKonta , '''' COLLATE Polish_CI_AS as IBAN, '''' COLLATE Polish_CI_AS as RBN, '''' as OperacjaGlowna, '''' COLLATE Polish_CI_AS as OperacjaCzesciowa, '''' COLLATE Polish_CI_AS as RodzajDokumentu, 0 as kwota, spr.WPS as Roszczenie, ' + 
'   '''' COLLATE Polish_CI_AS as NumerPartnera, '''' COLLATE Polish_CI_AS as KontoUmowy,'''' COLLATE Polish_CI_AS as PrzedmiotUmowy , '''' COLLATE Polish_CI_AS as NrDokumentu , ' +
'   spr.sygnObca as sygnObca ' +
' from    strony  s inner join sprawaS spr on spr.id_sprawy = s.id_sprawy '  


	
	-- dodanie zapuyania o sprawy KNS
	set @query4 = 
	
	' select ''KNS'' as ZrodloDanych, 0 as IdStrony, dl.FizPraw as typPartnera, sp.KnsSprawa_Id as IdSprawy, isnull(dl.nazwisko,'''') COLLATE Polish_CI_AS as Nazwa1,   rtrim(ltrim(isnull(dl.imie,''''))) COLLATE Polish_CI_AS as Nazwa2,'''' COLLATE Polish_CI_AS as Nazwa3,dl.nip COLLATE Polish_CI_AS  as  nip, dl.pesel  COLLATE Polish_CI_AS as pesel, ''D³u¿nik'' COLLATE Polish_CI_AS as rola, ' +
	'	case left(dok.typFakt,1)  when ''K'' then ''koszty''  when ''G'' then ''grzywna''  end + '' '' + cast (dok.kwota as varchar (10) ) COLLATE Polish_CI_AS  as		uwagi, '+
	'	sp.SAPWydzia³ COLLATE Polish_CI_AS  as kodWydzial, sp.SAPRepertorium COLLATE Polish_CI_AS as repertorium, sp.Numer as nr, sp.Rok as rok,sp.sygnatura COLLATE Polish_CI_AS as sygnatura, '''' COLLATE Polish_CI_AS as rodzWydz, ''2050-01-01'' as data_Kon, dl.ulica COLLATE Polish_CI_AS as ulica, dl.nrDomu COLLATE Polish_CI_AS as nr_domu, isnull(dl.nrMieszkania,'''') COLLATE Polish_CI_AS as nr_mieszkania,  '+	
	'	isnull(dl.miejscowosc,'''') COLLATE Polish_CI_AS as miejscowosc, dl.kodpocztowy COLLATE Polish_CI_AS as kod, dl.kluczkraju COLLATE Polish_CI_AS as kraj, knsKsiega as ksiega,   sp.karta COLLATE Polish_CI_AS as OznKontaUmowy, isnull(sp.SAPTYPKontaUmowy,''KN'') COLLATE Polish_CI_AS as TypKontaUmowy,  isnull(sp.SAPRelacjaKontaUmowy,''99'') COLLATE Polish_CI_AS as RelacjaKonta, dl.IBAN COLLATE Polish_CI_AS as IBAN, dl.RBN  COLLATE Polish_CI_AS as RBN, dok.OperacjaGlowna COLLATE Polish_CI_AS as OperacjaGlowna, dok.OperacjaCzesciowa COLLATE Polish_CI_AS as OperacjaCzesciowa, isnull(dok.SAPRodzajDokumentu,''NS'') COLLATE Polish_CI_AS as RodzajDokumentu, 0 as kwota, dok.kwota as Roszczenie, ' +
	'	 dl.SapKontoPartnera COLLATE Polish_CI_AS as NumerPartnera, sp.SapKontoUmowy COLLATE Polish_CI_AS as KontoUmowy,sp.SAPPrzedmiotumowy COLLATE Polish_CI_AS as PrzedmiotUmowy , SAPDocId COLLATE Polish_CI_AS as NrDokumentu , '''' COLLATE Polish_CI_AS as sygnObca ' +
	'	  from dluznik dl inner join sprawa sp on dl.Sprawa_id = sp.id  left outer join dokument dok  on dok.sprawa_id = sp.id and dok.dluznik_id = dl.id ' + 
	'		where  len(dok.SAPDocId)  > 0 and typfakt   in (''KS'',''GS'',''KP'',''GP'')  '
	
set @searchString  = '%' + ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) + '%'

if left(@key,1) = 'S'           --or left(@key,1) = 'N'
BEGIN
set @expression = ''
	if (@numer > 0 )
	BEGIN 
		if  len(@wydzial) > 0
		BEGIN
			set @expression = ' kodWydzial =  ''' + @wydzial + ''''
		END 	
		if  len(@repertorium) > 0
		BEGIN
			if len(@expression) > 0 
			BEGIN
			set @expression = @expression + ' and '
			END
			set @expression = @expression + ' repertorium =  ''' + @repertorium + ''''	
		END
		if len(@expression) > 0 
			BEGIN
			set	@expression = @expression + ' and '
			END
			set @expression = @expression + ' nr =  ' + cast(@numer as varchar(10)	)
		if @rok > 0 
		BEGIN
		if len(@expression) > 0 
			BEGIN
				set @expression = @expression + ' and '
			END
			set @expression = @expression + ' rok =  ' + cast(@rok as varchar(10)	)
		END
	 set @query3 = @query3 + ' where ' + @expression	
	END
	ELSE
	BEGIN
		set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
		set @query3 = @query3 + ' where sygnShort =  ''' + @sygnatura + '''' 	
	END
	set @query4 = @query4 + ' and (  ltrim(rtrim(replace(sp.sygnatura,'' '','''')))  like ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'N'   -- nazwisko	
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where Nazwa1 =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.nazwisko,'' '','''')))  like ''' + @searchstring + ''' ) '
END	

if left(@key,1) = 'U'   -- ulica
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where ulica =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.ulica,'' '','''')))  like ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'I'   -- IBAN
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where 1  =  2 '  -- set @query3 = @query3 + ' where IBAN =  ''' + @sygnatura + '''' 
 set @searchString  =   ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) 
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.IBAN,'' '','''')))  =  ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'W'   -- WPS
BEGIN
 set @sygnatura  = replace(replace(rtrim(substring (@key,3,50)),' ',''),',','.')
 
 set @query3 = @query3 + ' where Roszczenie =  ' + @sygnatura 
 set @query4 = @query4 + ' and ( Roszczenie  =   ' + @sygnatura +')'
END	
if left(@key,1) = 'M'   -- Miejscowosc
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where miejscowosc =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.miejscowosc,'' '','''')))  like ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'Z'   -- sygnatura d³u¿nika
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where sygnObca =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(sygnObca,'' '','''')))  like ''' + @searchstring + ''' ) '
END
if left(@key,1) = 'F'   -- fragment nazwy  d³u¿nika
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @sygnatura  = '%'+ @sygnatura + '%'
 set @query3 = @query3 + ' where Nazwa1 like  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.nazwisko,'' '','''')))  like ''' + @searchstring + ''' ) '
END

if len(@idList) > 0 
BEGIN
	set @query3 = @query3  + ' and s.id_sprawy in (select item from dbo.SplitNumbers(''' + @idList +''' )) '
END 
	print @query1 
	print @query2
	if (@skipkns = 0 )
	BEGIN
		print @query4
		print ' UNION ALL ' 
	END 
	print @query3
	if ( @skipkns = 0 )
		BEGIN	
			EXEC (@query1+ @query2 +@query4 + ' UNION ALL' + @query3)
		END
	else
		BEGIN
			EXEC (@query1+ @query2 + @query3)
		END
	 

END 

if (left(@key,1) = 'K'  and @skipkns = 0) -- karta d³u¿nika
BEGIN

set @searchString  = '%' + ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) + '%'
 
		select 'KNS' as ZrodloDanych, 0 as IdStrony, dl.FizPraw as typPartnera, sp.KnsSprawa_Id as IdSprawy, isnull(dl.nazwisko,'') as Nazwa1,   rtrim(ltrim(isnull(dl.imie,'')))  as Nazwa2,'' as Nazwa3,dl.nip as  nip, dl.pesel as pesel, 'D³u¿nik' as rola, 
		case left(dok.typFakt,1)  when 'K' then 'koszty'  when 'G' then 'grzywna'  end + ' ' + cast (dok.kwota as varchar (10) ) as		uwagi,
		sp.SAPWydzia³ as kodWydzial, sp.SAPRepertorium as repertorium, sp.Numer as nr, sp.Rok as rok,sp.sygnatura as sygnatura, '' as rodzWydz, '2050-01-01' as data_Kon, dl.ulica as ulica, dl.nrDomu  as nr_domu, isnull(dl.nrMieszkania,'') as nr_mieszkania,  	
		isnull(dl.miejscowosc,'') as miejscowosc, dl.kodpocztowy as kod, dl.kluczkraju  as kraj, knsKsiega as ksiega,   sp.karta as OznKontaUmowy, isnull(sp.SAPTYPKontaUmowy,'KN') as TypKontaUmowy,  isnull(sp.SAPRelacjaKontaUmowy,'99') as RelacjaKonta, dl.IBAN as IBAN, dl.RBN as RBN, dok.OperacjaGlowna as OperacjaGlowna, dok.OperacjaCzesciowa as OperacjaCzesciowa, isnull(dok.SAPRodzajDokumentu,'NS') as RodzajDokumentu, 0 as kwota, dok.kwota as Roszczenie,
		 dl.SapKontoPartnera as NumerPartnera, sp.SapKontoUmowy as KontoUmowy,sp.SAPPrzedmiotumowy as PrzedmiotUmowy , SAPDocId  as NrDokumentu , '' as  sygnObca
		  from dluznik dl inner join sprawa sp on dl.Sprawa_id = sp.id  left outer join dokument dok  on dok.sprawa_id = sp.id and dok.dluznik_id = dl.id 
where len(dok.SAPDocId)  > 0 and typfakt   in ('KS','GS','KP','GP') and (  ltrim(rtrim(replace(sp.karta,' ',''))) like @searchstring )
	/*
	set @searchString  = '%' + ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) + '%'
 
		select 'KNS' as ZrodloDanych, 0 as IdStrony, dl.FizPraw as typPatnera, sp.KnsSprawa_Id as IdSprawy, isnull(dl.nazwisko,'') as Nazwa1,   rtrim(ltrim(isnull(dl.imie,'')))  as Nazwa2,'' as Nazwa3,dl.nip as  nip, dl.pesel as pesel, 'D³u¿nik' as rola, 
		case left(dok.typFakt,1)  when 'K' then 'koszty'  when 'G' then 'grzywna'  end + ' ' + cast (dok.kwota as varchar (10) ) as		uwagi,
		sp.SAPWydzia³ as kodWydzial, sp.SAPRepertorium as repertorium, sp.Numer as nr, sp.Rok as rok,sp.sygnatura as sygnatura, '' as rodzWydz, '2050-01-01' as data_Kon, dl.ulica as ulica, dl.nrDomu  as nr_domu, isnull(dl.nrMieszkania,'') as nr_mieszkania,  	
		isnull(dl.miejscowosc,'') as miejscowosc, dl.kodpocztowy as kod, dl.kluczkraju  as kraj, knsKsiega as ksiega,   sp.karta as OznKontaUmowy, isnull(sp.SAPTYPKontaUmowy,'KN') as TypKontaUmowy,  isnull(sp.SAPRelacjaKontaUmowy,'99') as RelacjaKonta, dl.IBAN as IBAN, dl.RBN as RBN, dok.OperacjaGlowna as OperacjaGlowna, dok.OperacjaCzesciowa as OperacjaCzesciowa, dok.SAPRodzajDokumentu as RodzajDokumentu, 0 as kwota, dok.kwota as Roszczenie,
		 dl.SapKontoPartnera as NumerPartnera, sp.SapKontoUmowy as KontoUmowy,sp.SAPPrzedmiotumowy as PrzedmiotUmowy , SAPDocId  as NrDokumentu 
		  from dluznik dl inner join sprawa sp on dl.Sprawa_id = sp.id  left outer join dokument dok  on dok.sprawa_id = sp.id and dok.dluznik_id = dl.id 
where  typfakt   in ('KS','GS','KP','GP') and (
ltrim(rtrim(replace(dl.Nazwisko,' ',''))) like @searchstring or ltrim(rtrim(replace(sp.sygnatura,' ',''))) like @searchstring or ltrim(rtrim(replace(sp.karta,' ',''))) like @searchstring
)
 */
  		  
 end

end
GO
/****** Object:  StoredProcedure [dbo].[sp_RozpoznajPrzelew]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_RozpoznajPrzelew]
	-- Add the parameters for the stored procedure here
	 @sourcesrv varchar(50),
	 @dbname varchar(50),
	 @key varchar(100),
	 @wydzial varchar(10),
	 @repertorium varchar (10),
	 @numer int,
	 @rok   int,
	 @skipkns int,
	 @idList varchar(max)
	 
AS
BEGIN
  DECLARE @sygnatura varchar(50),
		  @nextday Datetime,
		   @query1 varchar(MAX),
		   @query2 varchar(MAX),
		   @query3 varchar(MAX),
		   @query4 varchar(MAX),
		   @oznKontaUm varchar(50),
		  --@query text,
  		  @dzienString varchar(30),
  		  @nextdayString varchar(12),
  		  @shortdzienString varchar(12),
  		  @searchString varchar(60),
  		  @expression	varchar(100)
  		  /*
  		  set @nextday  = DateAdd(d,1,@dzien) 
  		  set @dzienString =  '''' + convert ( varchar(20),@dzien,120)  +''''
  		  set @nextdayString = '''' + convert ( varchar(10),@nextday,120)  + ''''
  		  set @shortDzienString = '''' + convert ( varchar(10),@dzien,120)  +''''
  		  */
  		  set @sourcesrv  = '"' + @sourcesrv + '"'
if (left(@key,1) = 'S' or left(@key,1) = 'N'  or  left(@key,1) = 'U' or  left(@key,1) = 'M' or   left(@key,1) = 'I' or   left(@key,1) = 'W' or   left(@key,1) = 'Z' or   left(@key,1) = 'F' )
BEGIN  		  	

set @oznKontaUm = (select JednostkaGospodarcza + case when len(StanowiskoFin)> 0 then '/' + StanowiskoFin else '' end + ' Dochody nieprzypisane'  from konfiguracja )

set @query1 =  ' WITH  pelnomocnicy as '+
			   ' ( SELECT     PODMIOTY.id_podmioty,  PODMIOTY.rodzaj  AS rodzaj, ' +
               '        INSTYTUCJE.nazwa1 AS Nazwa1, INSTYTUCJE.nazwa2 AS Nazwa2, INSTYTUCJE.nazwa3 AS Nazwa3, INSTYTUCJE.nip, '''' as pesel, ' +
               '        RTRIM(INSTYTUCJE.nazwa1 + '' '' + ISNULL(INSTYTUCJE.nazwa2, '''')) + '' '' + ISNULL(INSTYTUCJE.nazwa3, '''') AS PelnaNazwa '+
			   ' 		FROM         ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY INNER JOIN '+
               '        ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY_REPREZENTANCI ON PODMIOTY.id_podmioty = PODMIOTY_REPREZENTANCI.id_podmioty_inni INNER JOIN  '+
               '        ' +   @sourcesrv +'.'  + @dbname +'.dbo.INSTYTUCJE ON INSTYTUCJE.id_instytucje = PODMIOTY.id_podmioty '+
			   '			UNION ALL '+
			   '			SELECT     PODMIOTY_1.id_podmioty,  PODMIOTY_1.rodzaj  AS rodzaj, '+
               '       OSOBY.nazwisko AS Nazwa1, OSOBY.imie1 AS Nazwa2, OSOBY.imie2 AS Nazwa3, OSOBY.nip, OSOBY.pesel ,'+
               '       OSOBY.nazwisko + '' '' + RTRIM(OSOBY.imie1 + '' '' + ISNULL(OSOBY.imie2, '''')) AS PelnaNazwa '+
			   '		FROM         ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY AS PODMIOTY_1 INNER JOIN '+
               '       ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY_REPREZENTANCI ON PODMIOTY_1.id_podmioty = PODMIOTY_REPREZENTANCI.id_podmioty_inni INNER JOIN '+
               '       ' +   @sourcesrv +'.'  + @dbname +'.dbo.OSOBY ON OSOBY.id_osoby = PODMIOTY_1.id_podmioty '+
			   '		UNION ALL '+
			   '		SELECT     PODMIOTY.id_podmioty,  PODMIOTY.rodzaj AS rodzaj, ' +
               '       INSTYTUCJE.nazwa1 AS Nazwa1, INSTYTUCJE.nazwa2 AS Nazwa2, INSTYTUCJE.nazwa3 AS Nazwa3, INSTYTUCJE.nip, '''' as pesel, ' + 
               '       RTRIM(INSTYTUCJE.nazwa1 + '' '' + ISNULL(INSTYTUCJE.nazwa2, '''')) + '' '' + ISNULL(INSTYTUCJE.nazwa3, '''') AS PelnaNazwa '+
				'		FROM         ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY INNER JOIN ' +
                '      ' +   @sourcesrv +'.'  + @dbname +'.dbo.INSTYTUCJE ON INSTYTUCJE.id_instytucje = PODMIOTY.id_podmioty ' +
				'		WHERE     (PODMIOTY.id_sprawy IS NULL) ' +
				'		UNION ALL '+
				'		SELECT     PODMIOTY_1.id_podmioty,  PODMIOTY_1.rodzaj AS rodzaj, ' +
                '      OSOBY.nazwisko AS Nazwa1, OSOBY.imie1 AS Nazwa2, OSOBY.imie2 AS Nazwa3, OSOBY.nip, OSOBY.pesel, ' +
                '      OSOBY.nazwisko + '' '' + RTRIM(OSOBY.imie1 + '' '' + ISNULL(OSOBY.imie2, '''')) AS PelnaNazwa '+
				'		FROM         ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY AS PODMIOTY_1 INNER JOIN ' +
                '      ' +   @sourcesrv +'.'  + @dbname +'.dbo.OSOBY ON OSOBY.id_osoby = PODMIOTY_1.id_podmioty '+
				'		WHERE     (PODMIOTY_1.id_sprawy IS NULL) ) ,' +
				' strony as  '+
			    ' ( ' +
				'	SELECT     PODMIOTY_1.id_podmioty, PODMIOTY_1.rodzaj as P_rodzaj , PODMIOTY_1.id_sprawy as P_id_sprawy, INSTYTUCJE.nazwa1 AS Nazwa1, '+
                '      INSTYTUCJE.nazwa2 AS Nazwa2, INSTYTUCJE.nazwa3 AS Nazwa3, INSTYTUCJE.nip, INSTYTUCJE.regon AS pesel, '+
                '      S_ROLE_1.kod AS rola, '+  
                '       RTRIM(INSTYTUCJE.nazwa1 + '' '' + RTRIM(ISNULL(INSTYTUCJE.nazwa2, '''')  ' +
                '      + '' '' + ISNULL(INSTYTUCJE.nazwa3, ''''))) AS NazwaPodmiotu, PODMIOTY_1.identyfikator, '+
                '      '''' as uwagi '+
				'	FROM ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY AS PODMIOTY_1 INNER JOIN  '+
					   @sourcesrv +'.'  + @dbname +'.dbo.INSTYTUCJE ON INSTYTUCJE.id_instytucje = PODMIOTY_1.id_podmioty LEFT OUTER JOIN '+
					   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY_ROLE AS PODMIOTY_ROLE_1 ON PODMIOTY_1.id_podmioty = PODMIOTY_ROLE_1.id_podmioty LEFT OUTER JOIN '+
					   @sourcesrv +'.'  + @dbname +'.dbo.S_ROLE AS S_ROLE_1 ON S_ROLE_1.id_role = PODMIOTY_ROLE_1.id_role '+
' UNION ALL ' + 
' SELECT     PODMIOTY.id_podmioty, PODMIOTY.rodzaj P_rodzaj , PODMIOTY.id_sprawy as P_id_sprawy, ' + 
'					OSOBY.nazwisko AS Nazwa1, OSOBY.imie1 AS Nazwa2, ' + 
'                      OSOBY.imie2 AS Nazwa3, OSOBY.nip, OSOBY.pesel, S_ROLE.kod AS rola, ' +
'                      RTRIM(OSOBY.nazwisko + '' '' + RTRIM(ISNULL(OSOBY.imie1, '''') + '' '' + ISNULL(OSOBY.imie2, ''''))) AS NazwaPodmiotu, ' +
'                      PODMIOTY.identyfikator, '''' as uwagi ' + 
' FROM          ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY AS PODMIOTY INNER JOIN ' + 
'                      ' +   @sourcesrv +'.'  + @dbname +'.dbo.OSOBY ON OSOBY.id_osoby = PODMIOTY.id_podmioty LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY_ROLE AS PODMIOTY_ROLE ON PODMIOTY.id_podmioty = PODMIOTY_ROLE.id_podmioty LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_ROLE AS S_ROLE ON S_ROLE.id_role = PODMIOTY_ROLE.id_role ' + 
' UNION ALL ' + 
' SELECT    PODMIOTY_REPREZENTANCI.id_podmioty, pelnomocnicy.rodzaj as P_rodzaj, Sprawy.id_sprawy as P_id_sprawy, ' + 
'					  pelnomocnicy.Nazwa1 as Nazwa1, ' +
'					  pelnomocnicy.Nazwa2  as Nazwa2,  '+
'					  pelnomocnicy.Nazwa3  as Nazwa3,  '+
'					 pelnomocnicy.nip,  pelnomocnicy.pesel,S_TYPY_REPREZENTANTOW.kod as rola, ' + 
'                      pelnomocnicy.PelnaNazwa AS NazwaPodmiotu, 0 as identyfikator,  V_OBIEKT_PODMIOT.PelnaNazwa AS uwagi ' + 
'                   FROM         ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY_REPREZENTANCI LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.V_OBIEKT_PODMIOT INNER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.SPRAWY  ON V_OBIEKT_PODMIOT.id_sprawy = SPRAWY.id_sprawy ON  ' + 
'						PODMIOTY_REPREZENTANCI.id_podmioty = V_OBIEKT_PODMIOT.id_podmioty LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_REPREZENTANTOW AS S_TYPY_REPREZENTANTOW_1 ON ' + 
'						PODMIOTY_REPREZENTANCI.id_typy_reprezentantow = S_TYPY_REPREZENTANTOW_1.id_typy_reprezentantow LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_REPREZENTANCI INNER JOIN ' + 
'                       pelnomocnicy ON S_REPREZENTANCI.id_reprezentanci = pelnomocnicy.id_podmioty INNER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_REPREZENTANTOW ON S_REPREZENTANCI.id_typy_reprezentantow = S_TYPY_REPREZENTANTOW.id_typy_reprezentantow ON  ' + 
'                       PODMIOTY_REPREZENTANCI.id_reprezentanci = S_REPREZENTANCI.id_reprezentanci ' + 
' WHERE PODMIOTY_REPREZENTANCI.id_podmioty_inni is null ' + 
' ), '
/*
' UNION ALL ' + 
' SELECT    PODMIOTY_REPREZENTANCI.id_podmioty , left(pelnomocnicy.rodzaj,1) as P_rodzj,Sprawy.id_sprawy as P_id_sprawy, ' + 
'					  pelnomocnicy.Nazwa1 as Nazwa1, ' +
'					  pelnomocnicy.Nazwa2  as Nazwa2,  '+
'					  pelnomocnicy.Nazwa3  as Nazwa3,  '+
'					pelnomocnicy.nip, pelnomocnicy.pesel, S_TYPY_REPREZENTANTOW.kod as rola, ' + 
'                      pelnomocnicy.PelnaNazwa as  NazwaPodmiotu, 0 as identyfikator,     V_OBIEKT_PODMIOT.PelnaNazwa AS uwagi ' + 
' FROM        ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_REPREZENTANCI INNER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_REPREZENTANTOW ON S_REPREZENTANCI.id_typy_reprezentantow = S_TYPY_REPREZENTANTOW.id_typy_reprezentantow RIGHT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_TYPY_REPREZENTANTOW AS S_TYPY_REPREZENTANTOW_1 RIGHT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.PODMIOTY_REPREZENTANCI INNER JOIN ' + 
'                       pelnomocnicy ON PODMIOTY_REPREZENTANCI.id_podmioty_inni = pelnomocnicy.id_podmioty LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.V_OBIEKT_PODMIOT INNER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.SPRAWY  ON V_OBIEKT_PODMIOT.id_sprawy = SPRAWY.id_sprawy ON  ' + 
'                       PODMIOTY_REPREZENTANCI.id_podmioty = V_OBIEKT_PODMIOT.id_podmioty ON  ' + 
'                       S_TYPY_REPREZENTANTOW_1.id_typy_reprezentantow = PODMIOTY_REPREZENTANCI.id_typy_reprezentantow ON ' + 
'                       S_REPREZENTANCI.id_reprezentanci = PODMIOTY_REPREZENTANCI.id_reprezentanci ' + 
' WHERE  PODMIOTY_REPREZENTANCI.id_podmioty_inni is not null ' + 
*/
  
set @query2 = 
' sprawaS as (SELECT      SPRAWY.id_sprawy,  ' + 
'                      SPRAWY.nr, SPRAWY.rok, SPRAWY.sygnatura_sprawy, SPRAWY.data_zak, SPRAWY.data_zakr, ' + 
'                      isnull(SPRAWY.data_zakr,''2050-01-01'') as data_kon, ' + 
'                      S_REPERTORIA.nazwa AS repertorium, ' + 
'                      S_WYDZIALY.nazwa AS wydzial, ' + 
'                      S_WYDZIALY.kod AS kodWydzial, ' + 
'                      S_REPERTORIA.SYSTEM as rodzWydz ,  ' +
'					   replace(SPRAWY.sygnatura_sprawy,'' '','''') as sygnShort, ' +
'					   SPRAWY.przedmiot_spr_wartosc as WPS ' +
' FROM                   ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_WYDZIALY INNER JOIN ' + 
'                        ' +   @sourcesrv +'.'  + @dbname +'.dbo.S_REPERTORIA ON S_WYDZIALY.id_wydzialy = S_REPERTORIA.id_wydzialy INNER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.SPRAWY ON S_REPERTORIA.id_repertoria = SPRAWY.id_repertoria ' + 
' ), ' +
' adresyStron as( ' + 
' SELECT              V_OBIEKT_PODMIOT.id_podmioty,  ' + 
'                    ADRESY.typ, ADRESY.ulica, rtrim(ADRESY.nr_domu) as nr_domu,  ' +
'                      rtrim(ADRESY.nr_mieszkania) as nr_mieszkania, ADRESY.miejscowosc, ADRESY.poczta, ADRESY.kod,  ADRESY.kraj ' + 
' FROM          ' +   @sourcesrv +'.'  + @dbname +'.dbo.ADRESY LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.V_OBIEKT_PODMIOT ON ADRESY.id_instytucje = V_OBIEKT_PODMIOT.id_podmioty ' + 
' WHERE     ( V_OBIEKT_PODMIOT.id_sprawy IS NOT NULL) ' + 
' UNION ' + 
' SELECT     V_OBIEKT_PODMIOT_1.id_podmioty,  ' + 
'                       ADRESY_1.typ, ADRESY_1.ulica, rtrim(ADRESY_1.nr_domu) as nr_domu, rtrim(ADRESY_1.nr_mieszkania) as nr_mieszkania, ' + 
'                      ADRESY_1.miejscowosc, ADRESY_1.poczta, ADRESY_1.kod,  ADRESY_1.kraj ' + 
' FROM          ' +   @sourcesrv +'.'  + @dbname +'.dbo.ADRESY AS ADRESY_1 LEFT OUTER JOIN ' + 
'                       ' +   @sourcesrv +'.'  + @dbname +'.dbo.V_OBIEKT_PODMIOT AS V_OBIEKT_PODMIOT_1 ON ADRESY_1.id_osoby = V_OBIEKT_PODMIOT_1.id_podmioty ' + 
' WHERE     ( V_OBIEKT_PODMIOT_1.id_sprawy IS NOT NULL) ' + 
' ) '
set @query3 =  ' select ''ORZCZ'' as ZrodloDanych, s.id_podmioty as IdStrony, case  s.P_rodzaj when ''I'' then ''X'' else '''' end as typPartnera, s.P_id_sprawy as IdSprawy, s.Nazwa1 COLLATE Polish_CI_AS as Nazwa1 , s.Nazwa2 COLLATE Polish_CI_AS as Nazwa2, s.Nazwa3 COLLATE Polish_CI_AS as Nazwa3 , replace(nip,''-'','''') COLLATE Polish_CI_AS as nip, s.pesel COLLATE Polish_CI_AS as pesel, s.rola COLLATE Polish_CI_AS as rola , s.uwagi COLLATE Polish_CI_AS as uwagi, ' + 
'   spr.kodWydzial COLLATE Polish_CI_AS as kodWydzial, spr.repertorium COLLATE Polish_CI_AS as repertorium , spr.nr, spr.rok, spr.sygnatura_sprawy COLLATE Polish_CI_AS as sygnatura,spr.rodzWydz COLLATE Polish_CI_AS as rodzWydz, spr.data_Kon,astr.ulica COLLATE Polish_CI_AS as ulica , astr.nr_domu COLLATE Polish_CI_AS as nr_domu , ' +
'    astr.nr_mieszkania COLLATE Polish_CI_AS as nr_mieszkania ,astr.miejscowosc COLLATE Polish_CI_AS as miejscowosc, astr.kod COLLATE Polish_CI_AS as kod , astr.kraj COLLATE Polish_CI_AS as kraj, 0 as Ksiega , ''' +   @oznKontaUm  + ''' COLLATE Polish_CI_AS as  OznKontaUmowy , ''DO'' COLLATE Polish_CI_AS as TypKontaUmowy, '''' COLLATE Polish_CI_AS as RelacjaKonta , '''' COLLATE Polish_CI_AS as IBAN, '''' COLLATE Polish_CI_AS as RBN, '''' as OperacjaGlowna, '''' COLLATE Polish_CI_AS as OperacjaCzesciowa, '''' COLLATE Polish_CI_AS as RodzajDokumentu, 0 as kwota, spr.WPS as Roszczenie, ' + 
'   '''' COLLATE Polish_CI_AS as NumerPartnera, '''' COLLATE Polish_CI_AS as KontoUmowy,'''' COLLATE Polish_CI_AS as PrzedmiotUmowy , '''' COLLATE Polish_CI_AS as NrDokumentu , ' +
'   (select top 1 pow.sygnatura_sprawy  from  	' +   @sourcesrv +'.'  + @dbname +'.dbo.SPRAWY_POWIAZANE pow where pow.id_sprawy  = spr.id_sprawy and pow.rodzaj_powiazania = ''DOW'' order by pow.data_powiazania) COLLATE Polish_CI_AS as sygnObca ' +
' from    strony  s inner join sprawaS spr on spr.id_sprawy = s.P_id_sprawy ' +
'       left outer join  adresyStron astr on astr.id_podmioty = s.id_podmioty ' 


	
	-- dodanie zapuyania o sprawy KNS
	set @query4 = 
	
	' select ''KNS'' as ZrodloDanych, 0 as IdStrony, dl.FizPraw as typPartnera, sp.KnsSprawa_Id as IdSprawy, isnull(dl.nazwisko,'''') COLLATE Polish_CI_AS as Nazwa1,   rtrim(ltrim(isnull(dl.imie,''''))) COLLATE Polish_CI_AS as Nazwa2,'''' COLLATE Polish_CI_AS as Nazwa3,dl.nip COLLATE Polish_CI_AS  as  nip, dl.pesel  COLLATE Polish_CI_AS as pesel, ''D³u¿nik'' COLLATE Polish_CI_AS as rola, ' +
	'	case left(dok.typFakt,1)  when ''K'' then ''koszty''  when ''G'' then ''grzywna''  end + '' '' + cast (dok.kwota as varchar (10) ) COLLATE Polish_CI_AS  as		uwagi, '+
	'	sp.SAPWydzia³ COLLATE Polish_CI_AS  as kodWydzial, sp.SAPRepertorium COLLATE Polish_CI_AS as repertorium, sp.Numer as nr, sp.Rok as rok,sp.sygnatura COLLATE Polish_CI_AS as sygnatura, '''' COLLATE Polish_CI_AS as rodzWydz, ''2050-01-01'' as data_Kon, dl.ulica COLLATE Polish_CI_AS as ulica, dl.nrDomu COLLATE Polish_CI_AS as nr_domu, isnull(dl.nrMieszkania,'''') COLLATE Polish_CI_AS as nr_mieszkania,  '+	
	'	isnull(dl.miejscowosc,'''') COLLATE Polish_CI_AS as miejscowosc, dl.kodpocztowy COLLATE Polish_CI_AS as kod, dl.kluczkraju COLLATE Polish_CI_AS as kraj, knsKsiega as ksiega,   sp.karta COLLATE Polish_CI_AS as OznKontaUmowy, isnull(sp.SAPTYPKontaUmowy,''KN'') COLLATE Polish_CI_AS as TypKontaUmowy,  isnull(sp.SAPRelacjaKontaUmowy,''99'') COLLATE Polish_CI_AS as RelacjaKonta, dl.IBAN COLLATE Polish_CI_AS as IBAN, dl.RBN  COLLATE Polish_CI_AS as RBN, dok.OperacjaGlowna COLLATE Polish_CI_AS as OperacjaGlowna, dok.OperacjaCzesciowa COLLATE Polish_CI_AS as OperacjaCzesciowa, isnull(dok.SAPRodzajDokumentu,''NS'') COLLATE Polish_CI_AS as RodzajDokumentu, 0 as kwota, dok.kwota as Roszczenie, ' +
	'	 dl.SapKontoPartnera COLLATE Polish_CI_AS as NumerPartnera, sp.SapKontoUmowy COLLATE Polish_CI_AS as KontoUmowy,sp.SAPPrzedmiotumowy COLLATE Polish_CI_AS as PrzedmiotUmowy , SAPDocId COLLATE Polish_CI_AS as NrDokumentu , '''' COLLATE Polish_CI_AS as sygnObca ' +
	'	  from dluznik dl inner join sprawa sp on dl.Sprawa_id = sp.id  left outer join dokument dok  on dok.sprawa_id = sp.id and dok.dluznik_id = dl.id ' + 
	'		where  len(dok.SAPDocId)  > 0 and typfakt   in (''KS'',''GS'',''KP'',''GP'')  '
	
set @searchString  = '%' + ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) + '%'

if left(@key,1) = 'S'           --or left(@key,1) = 'N'
BEGIN
set @expression = ''
	if (@numer > 0 )
	BEGIN 
		if  len(@wydzial) > 0
		BEGIN
			set @expression = ' kodWydzial =  ''' + @wydzial + ''''
		END 	
		if  len(@repertorium) > 0
		BEGIN
			if len(@expression) > 0 
			BEGIN
			set @expression = @expression + ' and '
			END
			set @expression = @expression + ' repertorium =  ''' + @repertorium + ''''	
		END
		if len(@expression) > 0 
			BEGIN
			set	@expression = @expression + ' and '
			END
			set @expression = @expression + ' nr =  ' + cast(@numer as varchar(10)	)
		if @rok > 0 
		BEGIN
		if len(@expression) > 0 
			BEGIN
				set @expression = @expression + ' and '
			END
			set @expression = @expression + ' rok =  ' + cast(@rok as varchar(10)	)
		END
	 set @query3 = @query3 + ' where ' + @expression	
	END
	ELSE
	BEGIN
		set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
		set @query3 = @query3 + ' where sygnShort =  ''' + @sygnatura + '''' 	
	END
	set @query4 = @query4 + ' and (  ltrim(rtrim(replace(sp.sygnatura,'' '','''')))  like ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'N'   -- nazwisko	
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where Nazwa1 =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.nazwisko,'' '','''')))  like ''' + @searchstring + ''' ) '
END	

if left(@key,1) = 'U'   -- ulica
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where ulica =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.ulica,'' '','''')))  like ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'I'   -- IBAN
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where 1  =  2 '  -- set @query3 = @query3 + ' where IBAN =  ''' + @sygnatura + '''' 
 set @searchString  =   ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) 
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.IBAN,'' '','''')))  =  ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'W'   -- WPS
BEGIN
 set @sygnatura  = replace(replace(rtrim(substring (@key,3,50)),' ',''),',','.')
 
 set @query3 = @query3 + ' where Roszczenie =  ' + @sygnatura 
 set @query4 = @query4 + ' and ( Roszczenie  =   ' + @sygnatura +')'
END	
if left(@key,1) = 'M'   -- Miejscowosc
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where miejscowosc =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.miejscowosc,'' '','''')))  like ''' + @searchstring + ''' ) '
END	
if left(@key,1) = 'Z'   -- sygnatura d³u¿nika
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @query3 = @query3 + ' where sygnObca =  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(sygnObca,'' '','''')))  like ''' + @searchstring + ''' ) '
END
if left(@key,1) = 'F'   -- fragment nazwy  d³u¿nika
BEGIN
 set @sygnatura  = replace(rtrim(substring (@key,3,50)),' ','')
 set @sygnatura  = '%'+ @sygnatura + '%'
 set @query3 = @query3 + ' where Nazwa1 like  ''' + @sygnatura + ''''
 set @query4 = @query4 + ' and (  ltrim(rtrim(replace(dl.nazwisko,'' '','''')))  like ''' + @searchstring + ''' ) '
END

if len(@idList) > 0 
BEGIN
	set @query3 = @query3  + ' and s.P_id_sprawy in (select item from dbo.SplitNumbers(''' + @idList +''' )) '
END 
	print @query1 
	print @query2
	if (@skipkns = 0 )
	BEGIN
		print @query4
		print ' UNION ALL ' 
	END 
	print @query3
	if ( @skipkns = 0 )
		BEGIN	
			EXEC (@query1+ @query2 +@query4 + ' UNION ALL' + @query3)
		END
	else
		BEGIN
			EXEC (@query1+ @query2 + @query3)
		END
	 

END 

if (left(@key,1) = 'K'  and @skipkns = 0) -- karta d³u¿nika
BEGIN

set @searchString  = '%' + ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) + '%'
 
		select 'KNS' as ZrodloDanych, 0 as IdStrony, dl.FizPraw as typPartnera, sp.KnsSprawa_Id as IdSprawy, isnull(dl.nazwisko,'') as Nazwa1,   rtrim(ltrim(isnull(dl.imie,'')))  as Nazwa2,'' as Nazwa3,dl.nip as  nip, dl.pesel as pesel, 'D³u¿nik' as rola, 
		case left(dok.typFakt,1)  when 'K' then 'koszty'  when 'G' then 'grzywna'  end + ' ' + cast (dok.kwota as varchar (10) ) as		uwagi,
		sp.SAPWydzia³ as kodWydzial, sp.SAPRepertorium as repertorium, sp.Numer as nr, sp.Rok as rok,sp.sygnatura as sygnatura, '' as rodzWydz, '2050-01-01' as data_Kon, dl.ulica as ulica, dl.nrDomu  as nr_domu, isnull(dl.nrMieszkania,'') as nr_mieszkania,  	
		isnull(dl.miejscowosc,'') as miejscowosc, dl.kodpocztowy as kod, dl.kluczkraju  as kraj, knsKsiega as ksiega,   sp.karta as OznKontaUmowy, isnull(sp.SAPTYPKontaUmowy,'KN') as TypKontaUmowy,  isnull(sp.SAPRelacjaKontaUmowy,'99') as RelacjaKonta, dl.IBAN as IBAN, dl.RBN as RBN, dok.OperacjaGlowna as OperacjaGlowna, dok.OperacjaCzesciowa as OperacjaCzesciowa, dok.SAPRodzajDokumentu as RodzajDokumentu, 0 as kwota, dok.kwota as Roszczenie,
		 dl.SapKontoPartnera as NumerPartnera, sp.SapKontoUmowy as KontoUmowy,sp.SAPPrzedmiotumowy as PrzedmiotUmowy , SAPDocId  as NrDokumentu , '' as  sygnObca
		  from dluznik dl inner join sprawa sp on dl.Sprawa_id = sp.id  left outer join dokument dok  on dok.sprawa_id = sp.id and dok.dluznik_id = dl.id 
where len(dok.SAPDocId)  > 0 and typfakt   in ('KS','GS','KP','GP') and (  ltrim(rtrim(replace(sp.karta,' ',''))) like @searchstring )
	/*
	set @searchString  = '%' + ltrim(rtrim(replace(replace(Substring(@key,3,60),' ',''),'\','/'))) + '%'
 
		select 'KNS' as ZrodloDanych, 0 as IdStrony, dl.FizPraw as typPatnera, sp.KnsSprawa_Id as IdSprawy, isnull(dl.nazwisko,'') as Nazwa1,   rtrim(ltrim(isnull(dl.imie,'')))  as Nazwa2,'' as Nazwa3,dl.nip as  nip, dl.pesel as pesel, 'D³u¿nik' as rola, 
		case left(dok.typFakt,1)  when 'K' then 'koszty'  when 'G' then 'grzywna'  end + ' ' + cast (dok.kwota as varchar (10) ) as		uwagi,
		sp.SAPWydzia³ as kodWydzial, sp.SAPRepertorium as repertorium, sp.Numer as nr, sp.Rok as rok,sp.sygnatura as sygnatura, '' as rodzWydz, '2050-01-01' as data_Kon, dl.ulica as ulica, dl.nrDomu  as nr_domu, isnull(dl.nrMieszkania,'') as nr_mieszkania,  	
		isnull(dl.miejscowosc,'') as miejscowosc, dl.kodpocztowy as kod, dl.kluczkraju  as kraj, knsKsiega as ksiega,   sp.karta as OznKontaUmowy, isnull(sp.SAPTYPKontaUmowy,'KN') as TypKontaUmowy,  isnull(sp.SAPRelacjaKontaUmowy,'99') as RelacjaKonta, dl.IBAN as IBAN, dl.RBN as RBN, dok.OperacjaGlowna as OperacjaGlowna, dok.OperacjaCzesciowa as OperacjaCzesciowa, dok.SAPRodzajDokumentu as RodzajDokumentu, 0 as kwota, dok.kwota as Roszczenie,
		 dl.SapKontoPartnera as NumerPartnera, sp.SapKontoUmowy as KontoUmowy,sp.SAPPrzedmiotumowy as PrzedmiotUmowy , SAPDocId  as NrDokumentu 
		  from dluznik dl inner join sprawa sp on dl.Sprawa_id = sp.id  left outer join dokument dok  on dok.sprawa_id = sp.id and dok.dluznik_id = dl.id 
where  typfakt   in ('KS','GS','KP','GP') and (
ltrim(rtrim(replace(dl.Nazwisko,' ',''))) like @searchstring or ltrim(rtrim(replace(sp.sygnatura,' ',''))) like @searchstring or ltrim(rtrim(replace(sp.karta,' ',''))) like @searchstring
)
 */
  		  
 end

end
GO
/****** Object:  StoredProcedure [dbo].[sp_Search]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Search]
	
	 @instring varchar(50)
	 
AS
BEGIN
  DECLARE 
		@searchString varchar(60)
		   
		   set @searchString  = '%' + ltrim(rtrim(replace(replace(@instring,' ',''),'\','/'))) + '%'
 
		select  rtrim(ltrim(isnull(dl.imie,'')  + ' ' + isnull(dl.nazwisko,''))) as Podmiot, rtrim(isnull(dl.miejscowosc,'') + ' ' +  isnull(dl.ulica  + ' ' +  dl.nrDomu + case when len(isnull(dl.nrMieszkania,'')) > 0 then '/'  +isnull(dl.nrMieszkania,'') else '' end ,''))   as  Adres  , sp.sygnatura as Sygnatura, sp.karta as "Nr karty d³u¿nika", 
case left(dok.typFakt,1)  when 'K' then 'koszty'  when 'G' then 'grzywna'  end as "Nale¿noœæ", dok.kwota as Kwota
 , dl.SapKontoPartnera as "Numer Partnera", sp.SapKontoUmowy as "Konto umowy",sp.SAPPrzedmiotumowy as "Przedmiot Umowy" , SAPDocId  as "Dokument" from dluznik dl inner join sprawa sp on dl.Sprawa_id = sp.id  left outer join dokument dok  on dok.sprawa_id = sp.id and dok.dluznik_id = dl.id 
where  typfakt   in ('KS','GS','KP','GP') and (
ltrim(rtrim(replace(dl.Nazwisko,' ',''))) like @searchstring or ltrim(rtrim(replace(replace(sp.sygnatura,' ',''),'\','/'))) like @searchstring or ltrim(rtrim(replace(replace(sp.karta,' ',''),'\','/'))) like @searchstring
)
 
  		  
 end
GO
/****** Object:  StoredProcedure [dbo].[sp_kns_wb_zakresCR]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_kns_wb_zakresCR]

                -- Add the parameters for the stored procedure here

                -- Add the parameters for the stored procedure here


@sourcesrv varchar(50),
@dbname varchar(50),
@dataOd DateTime,
@dataDo DateTime         

AS

BEGIN

declare 
/*
@sourcesrv varchar(50),
@dbname varchar(50),
*/


  

	@nextday Datetime,
	@sql varchar(MAX)
	

--set @sourcesrv  = 'Fin1'
--set @dbname = 'wcyw_j'

--select data_r, * from [ww_lan].dbo.kns_dz_nal


set @sql =                                          
' set QUOTED_IDENTIFIER on ' +
' select * into ##kns_wb from ( ' +
--uijszczenie grzywny odpisanej

' select ' +
'  dz.id  ' +
' , dz.pos as ''poz'' ' +
' , dz.rok  ' +
' , dz.data_r ' +
' , dz.data_zapisu ' +
' , dz.data_dowodu ' +
' , dz.dow ' +
' , ''UG'' as ''UNS'' ' +
' , dz.uiszczenia_grzywny as ''uiszczenie_grzywny''  ' +
' , dz.uiszczenia_kostow as ''uiszczenie_kosztow''  ' +
' , dz.grzywna_areszt as ''grzywna_areszt''  ' +
' , dz.ksiega ' +
' , dz.id_sprawy ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.kns_dz_nal dz ' +
' where dz.dow like ''WB%'' ' +
' and isnull(dz.grzywna_areszt,0) <> 0 ' +
' and dz.data_r >= 2014 ' +
' and month(dz.data_dowodu) >= 6 ' +
' union all' +

--uiszczenie grzywny 

' select ' +
'  dz.id  ' +
' , dz.pos as ''poz'' ' +
' , dz.rok  ' +
' , dz.data_r ' +
' , dz.data_zapisu ' +
' , dz.data_dowodu ' +
' , dz.dow ' +
' , ''UG'' as ''UNS'' ' +
' , dz.uiszczenia_grzywny as ''uiszczenie_grzywny''  ' +
' , dz.uiszczenia_kostow as ''uiszczenie_kosztow''  ' +
' , dz.grzywna_areszt as ''grzywna_areszt''  ' +
' , dz.ksiega ' +
' , dz.id_sprawy ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.kns_dz_nal dz ' +
' where dz.dow like ''WB%'' ' +
' and isnull(dz.uiszczenia_grzywny,0) <> 0 ' +
' and dz.rok >= 2015 '  +
' union all' +

--uiszczenie kosztow

' select ' +
'  dz.id  ' +
' , dz.pos as ''poz'' ' +
' , dz.rok  ' +
' , dz.data_r ' +
' , dz.data_zapisu ' +
' , dz.data_dowodu ' +
' , dz.dow ' +
' , ''UK'' as ''UNS'' ' +
' , dz.uiszczenia_grzywny as ''uiszczenie_grzywny''  ' +
' , dz.uiszczenia_kostow as ''uiszczenie_kosztow''  ' +
' , dz.grzywna_areszt as ''grzywna_areszt''  ' +
' , dz.ksiega ' +
' , dz.id_sprawy ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.kns_dz_nal dz ' +
' where dz.dow like ''WB%'' ' +
' and isnull(dz.uiszczenia_kostow,0) <> 0 '  +
' and dz.rok >= 2015 ) wb ' +

'set QUOTED_IDENTIFIER off ' 


print  @sql

--select * from dbo.##kns_wb
exec (@sql)


select 
distinct 
wb.data_dowodu
,case when wb.uns = 'UG' then isnull(wb.uiszczenie_grzywny,0) + isnull(wb.grzywna_areszt,0) 
when wb.uns = 'UK' then isnull(wb.uiszczenie_kosztow,0)
end as kwota
, 'c' as 't1'
, sp.SAPKontoUmowy as 'KontoUmowy'
--, sp.SAPPrzedmiotUmowy as 'Umowa'
--, 'b' as 't2'
--, dl.SAPKontoPartnera as 'Partner'
, 'd' as 't3'
, case when wb.uns = 'UG' and do1.SAPRatyId IS not null then do1.SAPRatyId
	when wb.uns = 'UG' and do1.SAPRatyId IS null  then do1.SAPDocId 
	when wb.uns = 'UK' and do1.SAPRatyId IS not null then do2.SAPRatyId 
	when wb.uns = 'UK' and do1.SAPRatyId IS null then do2.SAPDocId 
	
 end as Dokument

, 'Nr.Kd '+ sp.Karta +', ' + sp.Sygnatura +', ' + convert(varchar, wb.data_dowodu, 103)  +', '+ rtrim(wb.dow) collate DATABASE_DEFAULT 
+', '+
case when wb.uns = 'UG' then 'grzywna' when wb.uns = 'UK' then 'koszty' end 
as 'TekstDod'
--, wb.*
from dbo.##kns_wb wb
left join sprawa sp on sp.KnsSprawa_id = wb.id_sprawy
left join dokument do1 on do1.sprawa_id = sp.id and do1.SAPDocId is not null and do1.typFakt in ('GP','GS') and wb.uns = 'UG' and do1.SAPImportStatus = 1
and isnull(wb.uiszczenie_grzywny,0) <> 0
left join dokument do2 on do2.sprawa_id = sp.id and do2.SAPDocId is not null and do2.typFakt in ('KP','KS') and wb.uns = 'UK' and do2.SAPImportStatus = 1
and isnull(wb.uiszczenie_kosztow,0) <> 0
left join Dluznik dl on dl.Sprawa_Id = sp.id
where sp.SAPKontoUmowy is not null
and wb.data_dowodu between @dataOd and @dataDo
and (do1.SAPDocId  is not null or do2.SAPDocId is not null)
order by wb.data_dowodu, sp.SAPKontoUmowy
--and wb.poz = 61
--and do1.SAPImportStatus = 1
--and do2.SAPImportStatus = 1
--and wb.poz = 61
--and do1.SAPImportStatus = 1
--and do2.SAPImportStatus = 1


--and (wb_g.uiszczenie_grzywny <> 0 or 

--select * from Dluznik
--select * from Dokument


/*
select * from dluznik
select * from sprawa
*/




--select * from ##kns_wb where data_dowodu = '2014-06-27'
drop table ##kns_wb

/*
exec sp_kns_wb 'serwerlex', 'ww', '2014-06-27'
exec sp_PrzypisyCR_cr 'fin1', 'ww_lan', '1900-01-01', '2014-03-31'                 
*/
 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_kns_wb_zakres1]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROCEDURE [dbo].[sp_kns_wb_zakres1]

                -- Add the parameters for the stored procedure here

                -- Add the parameters for the stored procedure here


@sourcesrv varchar(50),
@dbname varchar(50),
@dataOd DateTime,
@dataDo DateTime         

AS

BEGIN

declare 
/*
@sourcesrv varchar(50),
@dbname varchar(50),
*/


  

	@nextday Datetime,
	@sql varchar(MAX)
	

--set @sourcesrv  = 'Fin1'
--set @dbname = 'wcyw_j'

--select data_r, * from [ww_lan].dbo.kns_dz_nal


set @sql =                                          
' set QUOTED_IDENTIFIER on ' +
' select * into ##kns_wb from ( ' +
--uijszczenie grzywny odpisanej

' select ' +
'  dz.id_naleznosci  ' +
' , dz.nr_poz as ''poz'' ' +
' , dz.rok_naleznosci as rok  ' +
' , dz.data_operacji as data_r ' +
' , dz.data_wprow_zapisu as data_zapisu ' +
' , dz.data_operacji as data_dowodu ' +
' , dz.nr_dowodu as dow' +
' , ''UG'' as ''UNS'' ' +
' , dz.grzywna_uiszcz as ''uiszczenie_grzywny''  ' +
' , dz.oplatakoszty_uiszcz as ''uiszczenie_kosztow''  ' +
' , dz.uiszcz_grzywny_odpis as ''grzywna_areszt''  ' +
--' , dz.ksiega ' +
' , dz.id_dluznik ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.naleznosci_dziennik dz ' +
' where  ' +
'  isnull(dz.uiszcz_grzywny_odpis,0) <> 0 ' +
' and year(dz.data_operacji) >= 2015 ' +
' union all' +

--uiszczenie grzywny 

' select ' +
'  dz.id_naleznosci  ' +
' , dz.nr_poz as ''poz'' ' +
' , dz.rok_naleznosci as rok  ' +
' , dz.data_operacji as data_r ' +
' , dz.data_wprow_zapisu as data_zapisu ' +
' , dz.data_operacji as data_dowodu ' +
' , dz.nr_dowodu as dow' +
' , ''UG'' as ''UNS'' ' +
' , dz.grzywna_uiszcz as ''uiszczenie_grzywny''  ' +
' , dz.oplatakoszty_uiszcz as ''uiszczenie_kosztow''  ' +
' , dz.uiszcz_grzywny_odpis as ''grzywna_areszt''  ' +
--' , dz.ksiega ' +
' , dz.id_dluznik ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.naleznosci_dziennik dz ' +
' where isnull(dz.grzywna_uiszcz,0) <> 0 ' +
--' and	dz.nr_dowodu like ''WB%'' ' +
' and year(dz.data_operacji) >= 2015 ' +

' union all' +

--uiszczenie kosztow

' select ' +
'  dz.id_naleznosci  ' +
' , dz.nr_poz as ''poz'' ' +
' , dz.rok_naleznosci as rok  ' +
' , dz.data_operacji as data_r ' +
' , dz.data_wprow_zapisu as data_zapisu ' +
' , dz.data_operacji as data_dowodu ' +
' , dz.nr_dowodu as dow' +
' , ''UK'' as ''UNS'' ' +
' , dz.grzywna_uiszcz as ''uiszczenie_grzywny''  ' +
' , dz.oplatakoszty_uiszcz as ''uiszczenie_kosztow''  ' +
' , dz.uiszcz_grzywny_odpis as ''grzywna_areszt''  ' +
--' , dz.ksiega ' +
' , dz.id_dluznik ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.naleznosci_dziennik dz ' +
' where ' +
' isnull(dz.oplatakoszty_uiszcz,0) <> 0 ' +
' and year(dz.data_operacji) >= 2015 '  +

' ) wb ' +


'set QUOTED_IDENTIFIER off ' 


--print  @sql

--select * from dbo.##kns_wb
exec (@sql)


select 
distinct 
wb.data_dowodu
,case when wb.uns = 'UG' then isnull(wb.uiszczenie_grzywny,0) + isnull(wb.grzywna_areszt,0) 
when wb.uns = 'UK' then isnull(wb.uiszczenie_kosztow,0)
end as kwota
, 'c' as 't1'
, sp.SAPKontoUmowy as 'KontoUmowy'
--, sp.SAPPrzedmiotUmowy as 'Umowa'
--, 'b' as 't2'
--, dl.SAPKontoPartnera as 'Partner'
, 'd' as 't3'
, case when wb.uns = 'UG' and do1.SAPRatyId IS not null then do1.SAPRatyId
	when wb.uns = 'UG' and do1.SAPRatyId IS null  then do1.SAPDocId 
	when wb.uns = 'UK' and do1.SAPRatyId IS not null then do2.SAPRatyId 
	when wb.uns = 'UK' and do1.SAPRatyId IS null then do2.SAPDocId 
	
 end as Dokument

, 'Nr.Kd '+ sp.Karta +', ' + sp.Sygnatura +', ' + convert(varchar, wb.data_dowodu, 103)  +', '+ rtrim(wb.dow) collate DATABASE_DEFAULT 
+', '+
case when wb.uns = 'UG' then 'grzywna' when wb.uns = 'UK' then 'koszty' end 
as 'TekstDod'
--, wb.*
--select *
from dbo.##kns_wb wb
left join sprawa sp on wb.id_dluznik = sp.KnsSprawa_id   
left join Dluznik dl on dl.Sprawa_id = sp.id
left join dokument do1 on do1.sprawa_id = sp.id and do1.SAPDocId is not null and do1.typFakt in ('GP','GS') and wb.uns = 'UG' and do1.SAPImportStatus >= 1
and isnull(wb.uiszczenie_grzywny,0) <> 0
left join dokument do2 on do2.sprawa_id = sp.id and do2.SAPDocId is not null and do2.typFakt in ('KP','KS') and wb.uns = 'UK' and do2.SAPImportStatus >= 1
and isnull(wb.uiszczenie_kosztow,0) <> 0

where sp.SAPKontoUmowy is not null
and wb.data_dowodu between @dataOd and @dataDo
and (do1.SAPDocId  is not null or do2.SAPDocId is not null)
order by wb.data_dowodu, sp.SAPKontoUmowy



drop table ##kns_wb


 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_kns_wb_zakres]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_kns_wb_zakres]

                -- Add the parameters for the stored procedure here

                -- Add the parameters for the stored procedure here


@sourcesrv varchar(50),
@dbname varchar(50),
@dataOd DateTime,
@dataDo DateTime         

AS

BEGIN

declare 
/*
@sourcesrv varchar(50),
@dbname varchar(50),
*/


  

	@nextday Datetime,
	@sql varchar(MAX)
	

--set @sourcesrv  = 'Fin1'
--set @dbname = 'wcyw_j'

--select data_r, * from [ww_lan].dbo.kns_dz_nal


set @sql =                                          
' set QUOTED_IDENTIFIER on ' +
' select * into ##kns_wb from ( ' +
--uijszczenie grzywny odpisanej

' select ' +
'  dz.id_naleznosci  ' +
' , dz.nr_poz as ''poz'' ' +
' , dz.rok_naleznosci as rok  ' +
' , dz.data_operacji as data_r ' +
' , dz.data_wprow_zapisu as data_zapisu ' +
' , dz.data_operacji as data_dowodu ' +
' , dz.nr_dowodu as dow' +
' , ''UG'' as ''UNS'' ' +
' , dz.grzywna_uiszcz as ''uiszczenie_grzywny''  ' +
' , dz.oplatakoszty_uiszcz as ''uiszczenie_kosztow''  ' +
' , dz.uiszcz_grzywny_odpis as ''grzywna_areszt''  ' +
--' , dz.ksiega ' +
' , dz.id_dluznik ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.naleznosci_dziennik dz ' +
' where  ' +
'  isnull(dz.uiszcz_grzywny_odpis,0) <> 0 ' +
' and year(dz.data_operacji) >= 2015 and year(dz.data_usun_zapisu) > 2050 ' +
' union all' +

--uiszczenie grzywny 

' select ' +
'  dz.id_naleznosci  ' +
' , dz.nr_poz as ''poz'' ' +
' , dz.rok_naleznosci as rok  ' +
' , dz.data_operacji as data_r ' +
' , dz.data_wprow_zapisu as data_zapisu ' +
' , dz.data_operacji as data_dowodu ' +
' , dz.nr_dowodu as dow' +
' , ''UG'' as ''UNS'' ' +
' , dz.grzywna_uiszcz as ''uiszczenie_grzywny''  ' +
' , dz.oplatakoszty_uiszcz as ''uiszczenie_kosztow''  ' +
' , dz.uiszcz_grzywny_odpis as ''grzywna_areszt''  ' +
--' , dz.ksiega ' +
' , dz.id_dluznik ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.naleznosci_dziennik dz ' +
' where isnull(dz.grzywna_uiszcz,0) <> 0 ' +
--' and	dz.nr_dowodu like ''WB%'' ' +
' and year(dz.data_operacji) >= 2015 and year(dz.data_usun_zapisu) > 2050 ' +

' union all' +

--uiszczenie kosztow

' select ' +
'  dz.id_naleznosci  ' +
' , dz.nr_poz as ''poz'' ' +
' , dz.rok_naleznosci as rok  ' +
' , dz.data_operacji as data_r ' +
' , dz.data_wprow_zapisu as data_zapisu ' +
' , dz.data_operacji as data_dowodu ' +
' , dz.nr_dowodu as dow' +
' , ''UK'' as ''UNS'' ' +
' , dz.grzywna_uiszcz as ''uiszczenie_grzywny''  ' +
' , dz.oplatakoszty_uiszcz as ''uiszczenie_kosztow''  ' +
' , dz.uiszcz_grzywny_odpis as ''grzywna_areszt''  ' +
--' , dz.ksiega ' +
' , dz.id_dluznik ' +

' from "' + rtrim(@sourcesrv) + '"."' + rtrim(@dbname) + '".dbo.naleznosci_dziennik dz ' +
' where ' +
' isnull(dz.oplatakoszty_uiszcz,0) <> 0 ' +
' and year(dz.data_operacji) >= 2015 and year(dz.data_usun_zapisu) > 2050 '  +

' ) wb ' +


'set QUOTED_IDENTIFIER off ' 


-- print  @sql

--select * from dbo.##kns_wb
exec (@sql)

select 
distinct 
wb.data_dowodu
,case when wb.uns = 'UG' then isnull(wb.uiszczenie_grzywny,0) + isnull(wb.grzywna_areszt,0) 
when wb.uns = 'UK' then isnull(wb.uiszczenie_kosztow,0)
end as kwota
, 'c' as 't1'
, sp.SAPKontoUmowy as 'KontoUmowy'
--, sp.SAPPrzedmiotUmowy as 'Umowa'
--, 'b' as 't2'
--, dl.SAPKontoPartnera as 'Partner'
, 'd' as 't3'
, case when wb.uns = 'UG' and do1.SAPRatyId IS not null then do1.SAPRatyId
	when wb.uns = 'UG' and do1.SAPRatyId IS null  then do1.SAPDocId 
	
	
 end as Dokument

, 'Nr.Kd '+ sp.Karta +', ' + sp.Sygnatura +', ' + convert(varchar, wb.data_dowodu, 103)  +', '+ rtrim(wb.dow) collate DATABASE_DEFAULT 
+', '+
case when wb.uns = 'UG' then 'grzywna' when wb.uns = 'UK' then 'koszty' end 
as 'TekstDod'
--, wb.*
--select *
from dbo.##kns_wb wb
inner join  sprawa sp on wb.id_dluznik = sp.KnsSprawa_id   
inner join dokument do1 on do1.sprawa_id = sp.id and len(do1.SAPDocId) > 0  and do1.typFakt in ('GP','GS') and wb.uns = 'UG' and do1.SAPImportStatus >= 1 
and isnull(wb.uiszczenie_grzywny,0) <> 0 
left join Dluznik dl on dl.Sprawa_id = sp.id 

where sp.SAPKontoUmowy is not null
and wb.data_dowodu between @dataOd and @dataDo
and (do1.SAPDocId  is not null )


union all

select 
distinct 
wb.data_dowodu
,case when wb.uns = 'UG' then isnull(wb.uiszczenie_grzywny,0) + isnull(wb.grzywna_areszt,0) 
when wb.uns = 'UK' then isnull(wb.uiszczenie_kosztow,0)
end as kwota
, 'c' as 't1'
, sp.SAPKontoUmowy as 'KontoUmowy'
--, sp.SAPPrzedmiotUmowy as 'Umowa'
--, 'b' as 't2'
--, dl.SAPKontoPartnera as 'Partner'
, 'd' as 't3'
, case when wb.uns = 'UK' and do2.SAPRatyId IS not null then do2.SAPRatyId 
	when wb.uns = 'UK' and do2.SAPRatyId IS null then do2.SAPDocId 
	
 end as Dokument

, 'Nr.Kd '+ sp.Karta +', ' + sp.Sygnatura +', ' + convert(varchar, wb.data_dowodu, 103)  +', '+ rtrim(wb.dow) collate DATABASE_DEFAULT 
+', '+
case when wb.uns = 'UG' then 'grzywna' when wb.uns = 'UK' then 'koszty' end 
as 'TekstDod'
--, wb.*
--select *
from dbo.##kns_wb wb
inner join sprawa sp on wb.id_dluznik = sp.KnsSprawa_id  
inner join dokument do2 on do2.sprawa_id = sp.id and do2.SAPDocId is not null and do2.typFakt in ('KP','KS') and wb.uns = 'UK' and do2.SAPImportStatus >= 1
and isnull(wb.uiszczenie_kosztow,0) <> 0
left join Dluznik dl on dl.Sprawa_id = sp.id

where sp.SAPKontoUmowy is not null
and wb.data_dowodu between @dataOd and @dataDo
and ( do2.SAPDocId is not null)

order by wb.data_dowodu, sp.SAPKontoUmowy


--and wb.poz = 61
--and do1.SAPImportStatus = 1
--and do2.SAPImportStatus = 1
--and wb.poz = 61
--and do1.SAPImportStatus = 1
--and do2.SAPImportStatus = 1


--and (wb_g.uiszczenie_grzywny <> 0 or 

--select * from Dluznik
--select * from Dokument


/*
select * from dluznik
select * from sprawa
*/




--select * from ##kns_wb where data_dowodu = '2014-06-27'
drop table ##kns_wb

/*
exec sp_kns_wb 'serwerlex', 'ww', '2014-06-27'
exec sp_kns_wb_zeto_zakres 'fin1', 'knsZeto', '1900-01-01', '2014-03-31'                 
*/
end
GO
/****** Object:  Table [dbo].[Ekstrakcja]    Script Date: 08/13/2015 17:01:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Ekstrakcja](
	[Osoba fizyczna/Osoba prawna] [char](1) NULL,
	[Imiê/Nazwa1] [varchar](40) NULL,
	[Nazwisko/ Nazwa2] [varchar](40) NULL,
	[Ulica] [varchar](60) NULL,
	[Nrdomu] [varchar](10) NULL,
	[Nrmieszkania] [varchar](10) NULL,
	[Kodpocztowy] [varchar](10) NULL,
	[Miejscowoœæ] [varchar](40) NULL,
	[Kluczkraju] [varchar](2) NULL,
	[IBAN] [varchar](28) NULL,
	[NIP] [varchar](10) NULL,
	[Pesel] [varchar](11) NULL,
	[KwalifikatordoRBN] [varchar](2) NULL,
	[Typkontaumowy] [varchar](2) NULL,
	[Oznaczeniekontaumowy] [varchar](35) NULL,
	[Relacjakonta] [varchar](2) NULL,
	[GrupaJG] [varchar](4) NULL,
	[StandardowaJG] [varchar](4) NULL,
	[Rodzajprzedmiotuumowy] [varchar](4) NULL,
	[JednostkaGospodarcza] [varchar](4) NULL,
	[Nrwydzia³uisekcji] [varchar](10) NULL,
	[Repertorium] [varchar](6) NULL,
	[NrSprawy] [varchar](10) NULL,
	[Rok] [varchar](4) NULL,
	[Rodzajsprawy] [varchar](5) NULL,
	[Iloœætomów] [varchar](3) NULL,
	[Datadokumentu] [varchar](8) NULL,
	[Dataksiêgowania] [varchar](8) NULL,
	[Rodzajdokumentu] [varchar](2) NULL,
	[Waluta] [varchar](5) NULL,
	[Kluczuzgodnienia] [varchar](12) NULL,
	[JednostkaGospodarcza32] [varchar](4) NULL,
	[Operacjag³ówna] [varchar](4) NULL,
	[Operacjaczêœciowa] [varchar](4) NULL,
	[KwotawPLN] [varchar](21) NULL,
	[Datap³atnoœci] [varchar](8) NULL,
	[Stannale¿noœci] [char](1) NULL,
	[KontoKG] [bigint] NULL,
	[Opis] [varchar](50) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Dokument_Id] [int] NULL,
	[DocGuid] [uniqueidentifier] NULL,
	[NumerPartnera] [varchar](20) NULL,
	[NumerKontaUmowy] [varchar](20) NULL,
	[NumerPrzedmiotuUmowy] [varchar](20) NULL,
	[NumerDokumentu] [varchar](20) NULL,
	[NumerDokumentuReferencyjnego] [varchar](20) NULL,
	[NumerDokumentuPlanRat] [varchar](20) NULL,
	[RataKwota1] [varchar](21) NULL,
	[RataData1] [varchar](8) NULL,
	[RataKwota2] [varchar](21) NULL,
	[RataData2] [varchar](8) NULL,
	[RataKwota3] [varchar](21) NULL,
	[RataData3] [varchar](8) NULL,
	[RataKwota4] [varchar](21) NULL,
	[RataData4] [varchar](8) NULL,
	[RataKwota5] [varchar](21) NULL,
	[RataData5] [varchar](8) NULL,
	[RataKwota6] [varchar](21) NULL,
	[RataData6] [varchar](8) NULL,
	[RataKwota7] [varchar](21) NULL,
	[RataData7] [varchar](8) NULL,
	[RataKwota8] [varchar](21) NULL,
	[RataData8] [varchar](8) NULL,
	[RataKwota9] [varchar](21) NULL,
	[RataData9] [varchar](8) NULL,
	[RataKwota10] [varchar](21) NULL,
	[RataData10] [varchar](8) NULL,
	[RataKwota11] [varchar](21) NULL,
	[RataData11] [varchar](8) NULL,
	[RataKwota12] [varchar](21) NULL,
	[RataData12] [varchar](8) NULL,
	[RataKwota13] [varchar](21) NULL,
	[RataData13] [varchar](8) NULL,
	[RataKwota14] [varchar](21) NULL,
	[RataData14] [varchar](8) NULL,
	[RataKwota15] [varchar](21) NULL,
	[RataData15] [varchar](8) NULL,
	[RataKwota16] [varchar](21) NULL,
	[RataData16] [varchar](8) NULL,
	[RataKwota17] [varchar](21) NULL,
	[RataData17] [varchar](8) NULL,
	[RataKwota18] [varchar](21) NULL,
	[RataData18] [varchar](8) NULL,
	[RataKwota19] [varchar](21) NULL,
	[RataData19] [varchar](8) NULL,
	[RataKwota20] [varchar](21) NULL,
	[RataData20] [varchar](8) NULL,
	[RataKwota21] [varchar](21) NULL,
	[RataData21] [varchar](8) NULL,
	[RataKwota22] [varchar](21) NULL,
	[RataData22] [varchar](8) NULL,
	[RataKwota23] [varchar](21) NULL,
	[RataData23] [varchar](8) NULL,
	[RataKwota24] [varchar](21) NULL,
	[RataData24] [varchar](8) NULL,
	[RataKwota25] [varchar](21) NULL,
	[RataData25] [varchar](8) NULL,
	[RataKwota26] [varchar](21) NULL,
	[RataData26] [varchar](8) NULL,
	[RataKwota27] [varchar](21) NULL,
	[RataData27] [varchar](8) NULL,
	[RataKwota28] [varchar](21) NULL,
	[RataData28] [varchar](8) NULL,
	[RataKwota29] [varchar](21) NULL,
	[RataData29] [varchar](8) NULL,
	[RataKwota30] [varchar](21) NULL,
	[RataData30] [varchar](8) NULL,
	[RataKwota31] [varchar](21) NULL,
	[RataData31] [varchar](8) NULL,
	[RataKwota32] [varchar](21) NULL,
	[RataData32] [varchar](8) NULL,
	[RataKwota33] [varchar](21) NULL,
	[RataData33] [varchar](8) NULL,
	[RataKwota34] [varchar](21) NULL,
	[RataData34] [varchar](8) NULL,
	[RataKwota35] [varchar](21) NULL,
	[RataData35] [varchar](8) NULL,
	[RataKwota36] [varchar](21) NULL,
	[RataData36] [varchar](8) NULL,
	[SygnaturaPoprzednia] [varchar](25) NULL,
	[KodOperacji] [varchar](2) NULL,
	[SAPImportPonowne] [varchar](1) NULL,
	[UserId] [int] NULL,
	[IsDeleted] [bit] NULL,
	[StanowiskoFinansowePU] [varchar](4) NULL,
	[StanowiskoFinansoweKU] [varchar](4) NULL,
	[JeGoWindyk] [varchar](4) NULL,
	[StanowiskoFianasoweWindyk] [varchar](16) NULL,
 CONSTRAINT [PK_Ekstrakcja] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Ekstrakcja] ON
INSERT [dbo].[Ekstrakcja] ([Osoba fizyczna/Osoba prawna], [Imiê/Nazwa1], [Nazwisko/ Nazwa2], [Ulica], [Nrdomu], [Nrmieszkania], [Kodpocztowy], [Miejscowoœæ], [Kluczkraju], [IBAN], [NIP], [Pesel], [KwalifikatordoRBN], [Typkontaumowy], [Oznaczeniekontaumowy], [Relacjakonta], [GrupaJG], [StandardowaJG], [Rodzajprzedmiotuumowy], [JednostkaGospodarcza], [Nrwydzia³uisekcji], [Repertorium], [NrSprawy], [Rok], [Rodzajsprawy], [Iloœætomów], [Datadokumentu], [Dataksiêgowania], [Rodzajdokumentu], [Waluta], [Kluczuzgodnienia], [JednostkaGospodarcza32], [Operacjag³ówna], [Operacjaczêœciowa], [KwotawPLN], [Datap³atnoœci], [Stannale¿noœci], [KontoKG], [Opis], [id], [Dokument_Id], [DocGuid], [NumerPartnera], [NumerKontaUmowy], [NumerPrzedmiotuUmowy], [NumerDokumentu], [NumerDokumentuReferencyjnego], [NumerDokumentuPlanRat], [RataKwota1], [RataData1], [RataKwota2], [RataData2], [RataKwota3], [RataData3], [RataKwota4], [RataData4], [RataKwota5], [RataData5], [RataKwota6], [RataData6], [RataKwota7], [RataData7], [RataKwota8], [RataData8], [RataKwota9], [RataData9], [RataKwota10], [RataData10], [RataKwota11], [RataData11], [RataKwota12], [RataData12], [RataKwota13], [RataData13], [RataKwota14], [RataData14], [RataKwota15], [RataData15], [RataKwota16], [RataData16], [RataKwota17], [RataData17], [RataKwota18], [RataData18], [RataKwota19], [RataData19], [RataKwota20], [RataData20], [RataKwota21], [RataData21], [RataKwota22], [RataData22], [RataKwota23], [RataData23], [RataKwota24], [RataData24], [RataKwota25], [RataData25], [RataKwota26], [RataData26], [RataKwota27], [RataData27], [RataKwota28], [RataData28], [RataKwota29], [RataData29], [RataKwota30], [RataData30], [RataKwota31], [RataData31], [RataKwota32], [RataData32], [RataKwota33], [RataData33], [RataKwota34], [RataData34], [RataKwota35], [RataData35], [RataKwota36], [RataData36], [SygnaturaPoprzednia], [KodOperacji], [SAPImportPonowne], [UserId], [IsDeleted], [StanowiskoFinansowePU], [StanowiskoFinansoweKU], [JeGoWindyk], [StanowiskoFianasoweWindyk]) VALUES (N' ', N'£ukasz', N'Ptaszyñski', N'Trzebieszowice', N'84', N'', N'57-541    ', N'Bystrzyca K³odzka', N'PL', N'', N'', N'83012919533', N'09', N'KN', N'Kd K 1/14', N'99', N'', N'3004', N'SKAR', N'3004', N'III', N'Kop', N'56', N'2011', N'2K261', N'001', N'20140109', N'20140109', N'NS', N'PLN', N'', N'3004', N'N010', N'0130', N'8224.22', N'20140306', N'A', 7200000000, NULL, 11557, NULL, N'825c069a-8dea-451a-b620-71f80c01673b', NULL, NULL, NULL, NULL, NULL, NULL, N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'0', N'', N'III KOP 56/11', N'KP', N'', NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Ekstrakcja] OFF
/****** Object:  Default [DF_User_deleted]    Script Date: 08/13/2015 17:01:34 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_deleted]  DEFAULT ((0)) FOR [deleted]
GO
/****** Object:  Default [DF_User_PwdPeriodChange]    Script Date: 08/13/2015 17:01:34 ******/
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_PwdPeriodChange]  DEFAULT ((0)) FOR [PwdPeriodChange]
GO
/****** Object:  Default [DF_Sprawa_wyklucz]    Script Date: 08/13/2015 17:01:34 ******/
ALTER TABLE [dbo].[Sprawa] ADD  CONSTRAINT [DF_Sprawa_wyklucz]  DEFAULT ((0)) FOR [wyklucz]
GO
/****** Object:  Default [DF_Dokument_DocGuid]    Script Date: 08/13/2015 17:01:40 ******/
ALTER TABLE [dbo].[Dokument] ADD  CONSTRAINT [DF_Dokument_DocGuid]  DEFAULT (newid()) FOR [DocGuid]
GO
/****** Object:  Default [DF_Ekstrakcja_IsDeleted]    Script Date: 08/13/2015 17:01:45 ******/
ALTER TABLE [dbo].[Ekstrakcja] ADD  CONSTRAINT [DF_Ekstrakcja_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
/****** Object:  ForeignKey [FK_Wplata_Transfer]    Script Date: 08/13/2015 17:01:40 ******/
ALTER TABLE [dbo].[Wplata]  WITH CHECK ADD  CONSTRAINT [FK_Wplata_Transfer] FOREIGN KEY([Transfer_Id])
REFERENCES [dbo].[Transfer] ([Id])
GO
ALTER TABLE [dbo].[Wplata] CHECK CONSTRAINT [FK_Wplata_Transfer]
GO
/****** Object:  ForeignKey [FK_Dluznik_Sprawa]    Script Date: 08/13/2015 17:01:40 ******/
ALTER TABLE [dbo].[Dluznik]  WITH CHECK ADD  CONSTRAINT [FK_Dluznik_Sprawa] FOREIGN KEY([Sprawa_Id])
REFERENCES [dbo].[Sprawa] ([Id])
GO
ALTER TABLE [dbo].[Dluznik] CHECK CONSTRAINT [FK_Dluznik_Sprawa]
GO
/****** Object:  ForeignKey [FK_Dokument_Dluznik]    Script Date: 08/13/2015 17:01:40 ******/
ALTER TABLE [dbo].[Dokument]  WITH CHECK ADD  CONSTRAINT [FK_Dokument_Dluznik] FOREIGN KEY([Dluznik_Id])
REFERENCES [dbo].[Dluznik] ([Id])
GO
ALTER TABLE [dbo].[Dokument] CHECK CONSTRAINT [FK_Dokument_Dluznik]
GO
/****** Object:  ForeignKey [FK_Dokument_Sprawa]    Script Date: 08/13/2015 17:01:40 ******/
ALTER TABLE [dbo].[Dokument]  WITH CHECK ADD  CONSTRAINT [FK_Dokument_Sprawa] FOREIGN KEY([Sprawa_Id])
REFERENCES [dbo].[Sprawa] ([Id])
GO
ALTER TABLE [dbo].[Dokument] CHECK CONSTRAINT [FK_Dokument_Sprawa]
GO
/****** Object:  ForeignKey [FK_Dokument_Transfer]    Script Date: 08/13/2015 17:01:40 ******/
ALTER TABLE [dbo].[Dokument]  WITH CHECK ADD  CONSTRAINT [FK_Dokument_Transfer] FOREIGN KEY([Transfer_Id])
REFERENCES [dbo].[Transfer] ([Id])
GO
ALTER TABLE [dbo].[Dokument] CHECK CONSTRAINT [FK_Dokument_Transfer]
GO
/****** Object:  ForeignKey [FK_Ekstrakcja_Dokument]    Script Date: 08/13/2015 17:01:45 ******/
ALTER TABLE [dbo].[Ekstrakcja]  WITH CHECK ADD  CONSTRAINT [FK_Ekstrakcja_Dokument] FOREIGN KEY([Dokument_Id])
REFERENCES [dbo].[Dokument] ([id])
GO
ALTER TABLE [dbo].[Ekstrakcja] CHECK CONSTRAINT [FK_Ekstrakcja_Dokument]
GO
