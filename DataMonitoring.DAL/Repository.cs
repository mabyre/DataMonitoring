//
// https://docs.microsoft.com/fr-fr/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implemenation-entity-framework-core
// https://www.c-sharpcorner.com/article/net-entity-framework-core-generic-async-operations-with-unit-of-work-generic-re/
// https://entityframework.net/knowledge-base/51023223/the-provider-for-the-source-iqueryable-doesn-t-implement-iasyncqueryprovider
// https://code-maze.com/async-generic-repository-pattern/
// https://expertcodeblog.wordpress.com/2018/02/19/net-core-2-0-resolve-error-the-source-iqueryable-doesnt-implement-iasyncenumerable/
// The source IQueryable doesn’t implement IAsyncEnumerable.Only sources that implement IAsyncEnumerable can be used for Entity Framework asynchronous operations.
// https://github.com/SmartITAz/EFCoreGenericRepositorySolution/tree/master/EfCoreGenericRepository
// DataContext et DeleteAsyn, Update, UpdateAsyn, Count, Save
//
using DataMonitoring.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DataMonitoring.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public DbContext Context { get; set; }
        private DbSet<T> allRecords = null;

        public Repository( DbContext context )
        {
            Context = context;
            allRecords = Context.Set<T>(); // GetAll
        }

        #region HELPER_FUNCTIONS
        private IQueryable<T> findAll()
        {
            return allRecords;
        }

        private IQueryable<T> findByCondition( Expression<Func<T, bool>> predicate )
        {
            return allRecords.Where( predicate );
        }
        #endregion

        public virtual T Get( long id )
        {
            return allRecords.Find( id );
        }

        public virtual async Task<T> GetAsync( long id )
        {
            return await allRecords.FindAsync( id );
        }

        public virtual IEnumerable<T> GetAll()
        {
            return findAll();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await findAll().ToListAsync<T>();
        }

        //---------------------------------------------------------------------

        public IEnumerable<T> Find( Func<T, bool> predicat )
        {
            IEnumerable<T> dbQuery = findAll().Where<T>( predicat );
            return dbQuery;
        }

        public IEnumerable<T> Find( Func<T, bool> predicate, params Expression<Func<T, object>>[] includeProperties )
        {
            IQueryable<T> query = allRecords;

            foreach ( var includeProperty in includeProperties )
            {
                query = query.Include( includeProperty );
            }

            return query.Where<T>( predicate );
        }

        //---------------------------------------------------------------------

        public T SingleOrDefault( Func<T, bool> predicate )
        {
            IEnumerable<T> dbQuery = allRecords.Where<T>( predicate );
            return dbQuery.SingleOrDefault();
        }

        public Task<T> SingleOrDefaultAsync( Func<T, bool> predicate, params Expression<Func<T, object>>[] includeProperties )
        {
            IQueryable<T> query = allRecords;

            foreach ( var includeProperty in includeProperties )
            {
                query = query.Include( includeProperty );
            }

            return Task.FromResult( query.Where<T>( predicate ).SingleOrDefault() );
        }

        //---------------------------------------------------------------------

        public virtual void Delete( T entity )
        {
            allRecords.Remove( entity );
        }

        public virtual void Delete( long id )
        {
            T entity = Get( id );
            allRecords.Remove( entity );
        }

        public virtual void DeleteRange( IEnumerable<T> entities )
        {
            allRecords.RemoveRange( entities );
        }

        public void DeleteRange( List<T> entities )
        {
            allRecords.RemoveRange( entities );
        }

        public void Delete( int id )
        {
            T entity = Get( id );
            allRecords.Remove( entity );
        }

        public void DeleteRange( DbSet<T> entities )
        {
            allRecords.RemoveRange( entities );
        }

        public virtual void Update( T entity )
        {
            this.allRecords.Update( entity );
        }

        //---------------------------------------------------------------------

        public virtual void Create( T entity )
        {
            allRecords.Add( entity );
        }

        public virtual async Task CreateAsync( T entity )
        {
            await allRecords.AddAsync( entity );
        }

        public virtual void CreateRange( IEnumerable<T> entities )
        {
            allRecords.AddRange( entities );
        }

        public virtual async Task CreateRangeAsync( IEnumerable<T> entities )
        {
            await allRecords.AddRangeAsync( entities );
        }

        //--------------------------------------------------------------------------------
        // Fonction très intéressante
        // cet exemple montre comment se servir de toutes les possiblités
        // https://gist.github.com/pmbanugo/8456744
        //--------------------------------------------------------------------------------
        public virtual IEnumerable<T> Get( Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties )
        {
            IQueryable<T> query = allRecords;

            if ( filter != null )
            {
                query = query.Where( filter );
            }

            foreach ( var includeProperty in includeProperties )
            {
                query = query.Include( includeProperty );
            }

            if ( orderBy != null )
            {
                return orderBy( query ).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
    }

    //---------------------------------------------------------------------

    public static class EntityFrameworkCoreExtensions
    {
        public static Task<List<TSource>> ToListAsyncSafe<TSource>( this IQueryable<TSource> source )
        {
            if ( source == null )
                throw new ArgumentNullException( nameof( source ) );

            if ( !(source is IAsyncEnumerable<TSource>) )
                return Task.FromResult( source.ToList() );

            return source.ToListAsync();
        }
    }
}