using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using BuildingBlocks.Data.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Tool.hbm2ddl;

namespace BuildingBlocks.Sample.Web
{
    public class NHProvider : SessionFactoryProvider
    {
        /// <summary>
        /// Assembly to load mapping files from
        /// </summary>
        const string MAPPINGSASSEMBLY = "NHibernate.DomainModel";

        public override IEnumerable<NHibernate.ISessionFactory> BuildSessionFactory()
        {
            ISessionFactory northwindSessionFactory = BuildSessionFactory("~/App_Data/NorthwindData.cfg.xml",
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

            ISessionFactory miscSessionFactory = BuildSessionFactory("~/App_Data/MiscTestData.cfg.xml",
                "Northwind.Mappings.AnotherEntity.hbm.xml",
                "Northwind.Mappings.Role.hbm.xml",
                "Northwind.Mappings.User.hbm.xml",
                "Northwind.Mappings.TimeSheet.hbm.xml",
                "Northwind.Mappings.Animal.hbm.xml");

            ISessionFactory patientSessionFactory = BuildSessionFactory("~/App_Data/PatientData.cfg.xml",
                "Northwind.Mappings.Patient.hbm.xml");

            NorthwindCreateTestData(northwindSessionFactory);
            MiscTestCreateTestData(miscSessionFactory);
            PatientCreateTestData(patientSessionFactory);

            return new[] { northwindSessionFactory, miscSessionFactory, patientSessionFactory };
        }

        private static ISessionFactory BuildSessionFactory(string nhConfigPath, params string[] mappings)
        {
            var path = HostingEnvironment.MapPath(nhConfigPath);
            var configuration = Configure(path, mappings);

            new SchemaExport(configuration).Create(false, true);
            ISessionFactory miscSessionFactory = configuration.BuildSessionFactory();
            return miscSessionFactory;
        }

        private static Configuration Configure(string nhConfigPath, params string[] mappings)
        {
            var configuration = new Configuration();
            configuration.Configure(nhConfigPath);

            configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, typeof(DriverConnectionProvider).AssemblyQualifiedName);

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