using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Integrator RUP")]
[assembly: AssemblyDescription("System integrujący oprogramowanie lokalnie eksploatowane w sądach z Modułem Rozrachunku z Uczestnikami Postępowań (RUP) systemu ZSRK")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Complex www.wroclaw.sa.gov.pl")]
[assembly: AssemblyProduct("RUP Integrator")]
[assembly: AssemblyCopyright("Copyright © Sąd Apelacyjny w Wrocławiu 2020-2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("38a51836-a246-426c-84fd-8bd4cbe7285a")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]
[assembly: AssemblyVersion("3.7.1.*")]
[assembly: AssemblyFileVersion("3.7.1")]
//2.5.6 - nowy import odpisów
//2.3.11 ksiegowanie sald z uwzględnieniem roku należności ( konto księgi głównej )
// 3.3 - dostosowanie do nowej wersji usługi sieciowej
// 3.4 - modyfikacje - zlecenie #1
//3.5  - modyfikacje zlecenie #2
//3.6  - modyfikacje zlecenie #3
//3.7 - CONS