using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

[assembly: AssemblyTitle("SmartIrc4net")]
[assembly: AssemblyDescription("IRC library for the .NET Framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("qNETp")]
[assembly: AssemblyProduct("SmartIrc4net")]
[assembly: AssemblyCopyright("2003-2007 (C) Mirco Bauer <meebey@meebey.net>")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.4.0.*")]

#if DELAY_SIGN
[assembly: AssemblyDelaySign(true)]
[assembly: AssemblyKeyFile("../SmartIrc4net-pub.snk")]
#else
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
#endif
