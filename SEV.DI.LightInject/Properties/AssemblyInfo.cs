using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SEV FWK - DI LightInject")]
[assembly: AssemblyDescription("The implementation of the Dependency Inversion pattern for the SEV Framework with LightInject")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("SEV")]
[assembly: AssemblyProduct("SEV.DI.LightInject")]
[assembly: AssemblyCopyright("Copyright © SEV 2017")]

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: Guid("d61790e8-7784-4552-9f8d-15a58bc47446")]
[assembly: AssemblyVersion("0.5.0.1")]
[assembly: AssemblyFileVersion("0.5.0.1")]

[assembly: InternalsVisibleTo("SEV.DI.LightInject.Tests")]
