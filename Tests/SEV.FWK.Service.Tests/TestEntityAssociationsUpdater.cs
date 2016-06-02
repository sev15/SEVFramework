using SEV.DAL.EF;
using SEV.Domain.Model;
using System.Data.Entity;

namespace SEV.FWK.Service.Tests
{
    public class TestEntityAssociationsUpdater : EntityAssociationsUpdater
    {
        public override void UpdateAssociations(Entity entity, IDbContext context)
        {
            UpdateReference((TestEntity)entity, (DbContext)context, x => x.Parent);
        }
    }
}