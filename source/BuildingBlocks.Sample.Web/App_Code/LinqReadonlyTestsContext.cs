using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;

namespace BuildingBlocks.Sample.Web
{
	public class LinqReadonlyTestsContext
	{
		/// <summary>
		/// Assembly to load mapping files from
		/// </summary>
		const string MAPPINGSASSEMBLY = "NHibernate.DomainModel";

        private static readonly IEnumerable<ISessionFactory> _sessionFactory;

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
            return GetSessionFactory<TEntity>().GetCurrentSession();
        }
        
        public static ISession GetCurrentSession(Type type)
        {
            return GetSessionFactory(type).GetCurrentSession();
        }

        static LinqReadonlyTestsContext()
        {
            string nhConfigPath = HostingEnvironment.MapPath("~/App_Data/NorthwindData.cfg.xml");
            Configuration configuration = Configure(nhConfigPath,
                "Northwind.Mappings.Customer.hbm.xml",
				"Northwind.Mappings.Employee.hbm.xml",
				"Northwind.Mappings.Order.hbm.xml",
				"Northwind.Mappings.OrderLine.hbm.xml",
				"Northwind.Mappings.Product.hbm.xml",
				"Northwind.Mappings.ProductCategory.hbm.xml",
				"Northwind.Mappings.Region.hbm.xml",
				"Northwind.Mappings.Shipper.hbm.xml",
				"Northwind.Mappings.Supplier.hbm.xml",
				"Northwind.Mappings.Territory.hbm.xml");

            new SchemaExport(configuration).Create(false, true);
            ISessionFactory northwindSessionFactory = configuration.BuildSessionFactory();
            NorthwindCreateTestData(northwindSessionFactory);

            nhConfigPath = HostingEnvironment.MapPath("~/App_Data/MiscTestData.cfg.xml");
            configuration = Configure(nhConfigPath,
                "Northwind.Mappings.AnotherEntity.hbm.xml",
                "Northwind.Mappings.Role.hbm.xml",
                "Northwind.Mappings.User.hbm.xml",
                "Northwind.Mappings.TimeSheet.hbm.xml",
                "Northwind.Mappings.Animal.hbm.xml");

            new SchemaExport(configuration).Create(false, true);
            ISessionFactory miscSessionFactory = configuration.BuildSessionFactory();
            MiscTestCreateTestData(miscSessionFactory);

            nhConfigPath = HostingEnvironment.MapPath("~/App_Data/PatientData.cfg.xml");
            configuration = Configure(nhConfigPath,
                "Northwind.Mappings.Patient.hbm.xml");
            new SchemaExport(configuration).Create(false, true);
            ISessionFactory patientSessionFactory = configuration.BuildSessionFactory();
            PatientCreateTestData(patientSessionFactory);

            _sessionFactory = new List<ISessionFactory>(new[] { northwindSessionFactory, miscSessionFactory, patientSessionFactory });

        }

        private static Configuration Configure(string nhConfigPath, params string[] mappings)
		{
			var configuration = new Configuration();
            configuration.Configure(nhConfigPath);

			configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, typeof (DriverConnectionProvider).AssemblyQualifiedName);

			Assembly assembly = Assembly.Load(MAPPINGSASSEMBLY);

            foreach (string file in mappings.Select(mf => MAPPINGSASSEMBLY + "." + mf))
			{
				configuration.AddResource(file, assembly);
			}

			return configuration;
		}

        private static void NorthwindCreateTestData(ISessionFactory sessionFactory)
        {
            using (IStatelessSession session = sessionFactory.OpenStatelessSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreateNorthwindData(session);

                tx.Commit();
            }
        }

        private static void MiscTestCreateTestData(ISessionFactory sessionFactory)
        {
            using (ISession session = sessionFactory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreateMiscTestData(session);
                tx.Commit();
            }
        }

        private static void PatientCreateTestData(ISessionFactory sessionFactory)
        {
            using (ISession session = sessionFactory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreatePatientData(session);
                tx.Commit();
            }
        }
    }
}