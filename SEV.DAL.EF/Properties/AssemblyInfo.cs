using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("SEV FWK - DAL EF")]
[assembly: AssemblyDescription("The implementation of the UnitOfWork and Repository patterns for the SEV Framework with Entity Framework")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("SEV")]
[assembly: AssemblyProduct("SEV.DAL.EF")]
[assembly: AssemblyCopyright("Copyright © SEV 2017")]

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

[assembly: Guid("c3e1515d-115a-400a-bbce-6895ac343ceb")]
[assembly: AssemblyVersion("0.5.1.0")]
[assembly: AssemblyFileVersion("0.5.1.0")]

[assembly: InternalsVisibleTo("SEV.DAL.EF.Tests")]
[assembly: InternalsVisibleTo("SEV.DAL.DI")]
