using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

namespace BuildingBlocks.Sample.Web
{
	public class LinqReadonlyTestsContext
	{
		/// <summary>
		/// Assembly to load mapping files from
		/// </summary>
		protected virtual string MappingsAssembly
		{
			get { return "NHibernate.DomainModel"; }
		}

        public void CreateNorthwindDb()
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

        }

        //private void ExecuteScriptFile(Configuration configuration, string scripFileName)
        //{
        //    var file = new FileInfo(scripFileName);
        //    string script = file.OpenText().ReadToEnd().Replace("GO", "");
        //    var connectionProvider = ConnectionProviderFactory.NewConnectionProvider(configuration.GetDerivedProperties());
        //    using (var conn = connectionProvider.GetConnection())
        //    {
        //        if (conn.State == ConnectionState.Closed)
        //        {
        //            conn.Open();
        //        }
        //        using (var command = conn.CreateCommand())
        //        {
        //            command.CommandText = script;
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public void DestroyNorthwindDb()
        //{
        //    string nhConfigPath = HostingEnvironment.MapPath("~/App_Data/NorthwindData.cfg.xml");
        //    Configuration configuration = NorthwindConfigure(nhConfigPath);
        //    string scripFileName = GetScripFileName(configuration, "LinqReadonlyDropScript");
        //    if (File.Exists(scripFileName))
        //    {
        //        ExecuteScriptFile(configuration, scripFileName);
        //    }
        //    else
        //    {
        //        new SchemaExport(configuration).Drop(false, true);
        //    }
        //}

		private string GetScripFileName(Configuration configuration,string postFix)
		{
			var dialect = Dialect.GetDialect(configuration.Properties);
			return Path.Combine("DbScripts", dialect.GetType().Name + postFix + ".sql");
		}

        private Configuration Configure(string nhConfigPath, params string[] mappings)
		{
			var configuration = new Configuration();
            configuration.Configure(nhConfigPath);

			configuration.SetProperty(Environment.ConnectionProvider, typeof (DriverConnectionProvider).AssemblyQualifiedName);

			Assembly assembly = Assembly.Load(MappingsAssembly);

            foreach (string file in mappings.Select(mf => MappingsAssembly + "." + mf))
			{
				configuration.AddResource(file, assembly);
			}

			return configuration;
		}

        private void NorthwindCreateTestData(ISessionFactory sessionFactory)
        {
            using (IStatelessSession session = sessionFactory.OpenStatelessSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreateNorthwindData(session);

                tx.Commit();
            }
        }

        private void MiscTestCreateTestData(ISessionFactory sessionFactory)
        {
            using (ISession session = sessionFactory.OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreateMiscTestData(session);
                tx.Commit();
            }
        }

        private void PatientCreateTestData(ISessionFactory sessionFactory)
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