using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using BuildingBlocks.Domain;
using NHibernate;
using NHibernate.Linq;

namespace BuildingBlocks.Data.NHibernate
{
    public class NHRepository<TEntity> : IRepository<TEntity>
        where TEntity : IEntity<object>
    {
        private readonly ISession _session;

        public NHRepository() : 
            this(SessionResolver.Current.GetCurrentSessionFor<TEntity>()) { }

        public NHRepository(ISession session)
        {
            _session = session;
        }

        public TEntity this[object id]
        {
            get { return _session.Get<TEntity>(id); }
        }

        public void Add(TEntity item)
        {
            Contract.Requires<ArgumentNullException>(item != null);
            _session.Save(item);
            _session.Flush();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(TEntity item)
        {
            Contract.Requires<ArgumentNullException>(item != null);
            return _session.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            Contract.Requires<ArgumentNullException>(array != null);
            var source = _session.Query<TEntity>().ToArray();
            Array.Copy(source, 0, array, arrayIndex, source.Count());
        }

        public int Count
        {
            get { return _session.Query<TEntity>().Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TEntity item)
        {
            Contract.Requires<ArgumentNullException>(item != null);
            _session.Delete(item);
            _session.Flush();
            return true;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _session.Query<TEntity>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _session.Query<TEntity>().GetEnumerator();
        }

        public Type ElementType
        {
            get { return _session.Query<TEntity>().ElementType; }
        }

        public Expression Expression
        {
            get { return _session.Query<TEntity>().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _session.Query<TEntity>().Provider; }
        }
    }
}
