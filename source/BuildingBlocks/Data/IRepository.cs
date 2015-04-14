using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain;

namespace BuildingBlocks.Data
{
    public interface IRepository<TEntity> : IRepository<object, TEntity>
        where TEntity : IEntity<object> { }

    /// <summary>
    ///     Defines the public members of a class that implements the repository pattern for entities
    ///     of the specified type.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IRepository<TKey, TEntity> : 
        ICollection<TEntity>,
        IQueryable<TEntity>
        where TEntity : IEntity<TKey>
    {
        /// <summary>
        ///     Get the specified entity
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="id">The entity to load</param>
        TEntity this[TKey id] { get; }
    }
}
