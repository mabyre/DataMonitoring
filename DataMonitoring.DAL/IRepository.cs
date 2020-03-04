using DataMonitoring.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataMonitoring.DAL
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        Task<TEntity> GetAsync(long id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        TEntity Get(long id);

        IEnumerable<TEntity> Find(Func<TEntity, bool> p);

        IEnumerable<TEntity> Find(Func<TEntity, bool> p1, params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity SingleOrDefault(Func<TEntity, bool> p);

        Task<TEntity> SingleOrDefaultAsync( Func<TEntity, bool> predicat, params Expression<Func<TEntity, object>>[] includeProperties);

        void Update(TEntity entity);

        void Create(TEntity entity);
        
        Task CreateAsync(TEntity entity);

        void DeleteRange(List<TEntity> entity);

        void DeleteRange(IEnumerable<TEntity> entities);

        void DeleteRange(DbSet<TEntity> entities);

        void Delete(TEntity entity);

        void Delete(int id);

        void Delete(long id);
    }
}