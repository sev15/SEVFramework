using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SEV FWK - Service API")]
[assembly: AssemblyDescription("The SEV Framework Service API")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("SEV")]
[assembly: AssemblyProduct("SEV.Service")]
[assembly: AssemblyCopyright("Copyright © SEV 2017")]

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: Guid("6393cb10-73b4-47a6-9b7e-62cf792e637d")]
[assembly: AssemblyVersion("0.4.1.0")]
[assembly: AssemblyFileVersion("0.4.1.0")]

[assembly: InternalsVisibleTo("SEV.Service.Tests")]
[assembly: InternalsVisibleTo("SEV.Service.DI")]
[assembly: InternalsVisibleTo("SEV.Service.DI.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
