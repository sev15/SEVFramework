using System;

namespace SEV.Domain.Model
{
    [Flags]
    public enum DomainEvent
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 4,
        //Read = 8,
        //Append =16,
    }
}
