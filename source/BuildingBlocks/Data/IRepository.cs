using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain;

namespace BuildingBlocks.Data
{
    public interface IRepository<TEntity> :
        IDictionary<int, TEntity>
        where TEntity : Entity<int> { }

    public interface IRepository<TKey, TEntity> : 
        IDictionary<TKey, TEntity>
        where TKey : IEquatable<TKey>
        where TEntity : Entity<TKey>
    {
    }
}
