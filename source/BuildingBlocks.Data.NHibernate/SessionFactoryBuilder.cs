using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Engine;
using Owin;

namespace BuildingBlocks.Data.NHibernate
{
    public class SessionFactoryBuilder
    {
        private readonly ISet<Configuration> _configurations;

        public SessionFactoryBuilder()
        {
            _configurations = new HashSet<Configuration>();
        }

        public void RegisterConfiguration(Configuration configuration)
        {
            _configurations.Add(configuration);
        }

        public ISessionResolver Build(IAppBuilder app)
        {
            var resolver = new DefaultSessionResolver();
            foreach (var configuration in _configurations) {
                var factory = configuration.BuildSessionFactory();
                resolver.SetFactoryToResolve(factory);
            }

            app.Use<SessionMiddleware>(resolver);
            return resolver;
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
            public IEnumerable<ISessionFactory> GetAllFactories()
            {
                return _factories;
            }
        }

    }
}
