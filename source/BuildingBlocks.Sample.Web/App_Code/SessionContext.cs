using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Mvc;
using BuildingBlocks.Data.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Context;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;

namespace BuildingBlocks.Sample.Web
{
	public sealed class SessionContext
	{
        private static readonly IEnumerable<ISessionFactory> _sessionFactory;

        static SessionContext()
        {
            var provider = DependencyResolver.Current.GetService<SessionFactoryProvider>();
            _sessionFactory = provider;
        }

        public static ISessionFactory GetSessionFactory<TEntity>()
        {
            return GetSessionFactory(typeof(TEntity));
        }

        public static ISessionFactory GetSessionFactory(Type type)
        {
            var factory = _sessionFactory.SingleOrDefault(x => ((ISessionFactoryImplementor)x).TryGetEntityPersister(type.FullName) != null);
            return factory;
        }

        public static ISession GetCurrentSession<TEntity>()
        {
            return GetCurrentSession(typeof(TEntity));
        }
        
        public static ISession GetCurrentSession(Type type)
        {
            var factory = GetSessionFactory(type);
            if (!CurrentSessionContext.HasBind(factory))
                CurrentSessionContext.Bind(factory.OpenSession());

            return factory.GetCurrentSession();
        }

    }
}