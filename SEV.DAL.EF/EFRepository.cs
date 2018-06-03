using SEV.Domain.Model;
using SEV.Domain.Services.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SEV.DAL.EF
{
    public class EFRepository<TEntity> : QueryBuilder<TEntity>, IRepository<TEntity>
        where TEntity : Entity
    {
        private readonly IDbContext m_context;
        private readonly IDbSet<TEntity> m_dbSet;

        public EFRepository(IDbContext context)
        {
            m_context = context;
            m_dbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> All()
        {
            return m_dbSet.ToList();
        }

        public TEntity GetById(object id)
        {
            return m_dbSet.Find(CastId(id));
        }

        private int CastId(object inputId)
        {
            if (inputId is string)
            {
                return int.Parse(inputId as string);
            }
            return (int)inputId;
        }

        public IEnumerable<TEntity> GetByIdList(IEnumerable<object> ids)
        {
            var idList = ids.Select(CastId).ToList();
            return m_dbSet.Where(x => idList.Contains(x.Id)).ToList();
        }

        public async Task<IEnumerable<TEntity>> AllAsync()
        {
            return await m_dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            var idValue = CastId(id);

            return await m_dbSet.FirstOrDefaultAsync(x => x.Id == idValue);
        }

        public async Task<IEnumerable<TEntity>> GetByIdListAsync(IEnumerable<object> ids)
        {
            var idList = ids.Select(CastId).ToList();
            return await m_dbSet.Where(x => idList.Contains(x.Id)).ToListAsync();
        }

        public RepositoryQuery<TEntity> Query()
        {
            var repositoryGetFluentHelper = new RepositoryQuery<TEntity>(this);

            return repositoryGetFluentHelper;
        }

        protected override IQueryable<TEntity> CreateQuery()
        {
            return m_dbSet;
        }

        public TEntity Insert(TEntity entity)
        {
            return m_dbSet.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            m_dbSet.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            m_context.SetEntityState(entity, EntityState.Modified);
        }
    }
}
