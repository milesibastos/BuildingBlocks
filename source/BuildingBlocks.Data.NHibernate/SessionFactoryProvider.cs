using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace BuildingBlocks.Data.NHibernate
{
    public abstract class SessionFactoryProvider : IEnumerable<ISessionFactory>
    {
        private static IEnumerable<ISessionFactory> _sessionFactory;

        public abstract IEnumerable<ISessionFactory> BuildSessionFactory();

        public IEnumerator<ISessionFactory> GetEnumerator()
        {
            if (_sessionFactory == null)
                _sessionFactory = BuildSessionFactory();

            return _sessionFactory.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _sessionFactory.GetEnumerator();
        }
    }
}
