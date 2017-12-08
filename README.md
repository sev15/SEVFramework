SEV Framework is a domain centric & SOA ready framework for the development of business applications with onion architecture. The SEV Framework is based on the .NET framework. The SEV Framework follows object-oriented design and supports the following layers:
 - UI Model / Presentation layer;
 - Application / Service layer;
 - Business Domain layer (DDD-inspired, based on the Repository and Unit Of Work patterns);
 - Data Access layer includes the implementation of the interfaces of the Business Domain layer for the Entity Framework.

The SEV Framework is based on the use of the Dependency Inversion S.O.L.I.D principle. The SEV Framework introduces a generic IoC container IDIContainer. An implementation of the IDIContainer interface for LightInject is provided.

---

The SEV Framework is organized with the following nuget packages (currently unlisted)

- SEV.FWK.Service (NuGet.exe install SEV.FWK.Service -Version 0.5.0 -o Nupkgs)
- SEV.FWK.DAL.EF (NuGet.exe install SEV.FWK.DAL.EF -Version 0.5.0 -o Nupkgs)
- SEV.FWK.UI.Model (NuGet.exe install SEV.FWK.UI.Model -Version 0.5.0 -o Nupkgs)
- SEV.FWK.DI.Web (NuGet.exe install SEV.FWK.DI.Web -Version 0.5.0 -o Nupkgs)
- SEV.FWK.DI.LightInject (NuGet.exe install SEV.FWK.DI.LightInject -Version 0.5.0 -o Nupkgs)
- SEV.FWK.DI.Web.LightInject (NuGet.exe install SEV.FWK.DI.Web.LightInject -Version 0.5.0 -o Nupkgs)
