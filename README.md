The SEV Framework has a layered architecture :
 - UI Model layer;
 - Application or Service layer;
 - Business Domain layer (based on the Repository and Unit Of Work patterns, includes Business Domain Model (aka entities));
 - Data Access layer includes the implementation of the interfaces of the Business Domain layer for the Entity Framework.

The SEV Framework is SOA ready.

The SEV Framework is based on the use of the Dependency Inversion S.O.L.I.D principle. The SEV Framework introduces a generic IoC container IDIContainer. An implementation of the IDIContainer interface for LightInject is provided.

---

New features of SEV Framework currently in production :

- Support asynchronous queries;
- Support business rules and business processes.
