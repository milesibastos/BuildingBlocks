using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;

namespace BuildingBlocks.Data.NHibernate
{
    public class SessionResolver
    {
        private static SessionResolver _instance = new SessionResolver();
        private ISessionResolver _current;

        public SessionResolver()
        {
            InnerSetResolver(new DefaultSessionResolver());
        }


        public static ISessionResolver Current { get { return _instance.InnerCurrent; } }

        public ISessionResolver InnerCurrent { get { return _current; } }

        public static void Register(ISessionFactory sessionFactory)
        {

        }

        public static void SetResolver(ISessionResolver resolver)
        {
            _instance.InnerSetResolver(resolver);
        }

        public void InnerSetResolver(ISessionResolver resolver)
        {
            Contract.Requires<ArgumentNullException>(resolver != null);
            _current = resolver;
        }

        private class DefaultSessionResolver : ISessionResolver
        {
            private static readonly ISet<ISessionFactory> _factories;

            static DefaultSessionResolver()
            {
                _factories = new HashSet<ISessionFactory>();
            }

            public ISession GetCurrentSessionFor(Type type)
            {
                var factory = GetSessionFactory(type);
                if (!CurrentSessionContext.HasBind(factory))
                    CurrentSessionContext.Bind(factory.OpenSession());

                return factory.GetCurrentSession();
            }

            public ISessionFactory GetSessionFactory(Type type)
            {
                var factory = _factories.SingleOrDefault(x => ((ISessionFactoryImplementor)x).TryGetEntityPersister(type.FullName) != null);
                return factory;
            }

            internal void SetFactoryToResolve(ISessionFactory factory)
            {
                _factories.Add(factory);
            }
        }
    }
}
