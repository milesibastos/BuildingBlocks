﻿using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using Microsoft.Owin;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Tool.hbm2ddl;
using Owin;

[assembly: OwinStartup(typeof(BuildingBlocks.Sample.Web.Startup))]

namespace BuildingBlocks.Sample.Web
{
    public class Startup
    {
        /// <summary>
        /// Assembly to load mapping files from
        /// </summary>
        const string MAPPINGSASSEMBLY = "NHibernate.DomainModel";

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var northwind = Configure("~/App_Data/NorthwindData.cfg.xml",
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

            var misc = Configure("~/App_Data/MiscTestData.cfg.xml",
                "Northwind.Mappings.AnotherEntity.hbm.xml",
                "Northwind.Mappings.Role.hbm.xml",
                "Northwind.Mappings.User.hbm.xml",
                "Northwind.Mappings.TimeSheet.hbm.xml",
                "Northwind.Mappings.Animal.hbm.xml");

            var patient = Configure("~/App_Data/PatientData.cfg.xml",
                "Northwind.Mappings.Patient.hbm.xml");

            
            var builder = new SessionFactoryBuilder();
            builder.RegisterToBuild(northwind);
            builder.RegisterToBuild(misc);
            builder.RegisterToBuild(patient);
            builder.Build(app);

            new SchemaExport(northwind).Create(false, true);
            new SchemaExport(misc).Create(false, true);
            new SchemaExport(patient).Create(false, true);

            NorthwindCreateTestData();
            MiscTestCreateTestData();
            PatientCreateTestData();
        }

        private static Configuration Configure(string nhConfigPath, params string[] mappings)
        {
            var path = HostingEnvironment.MapPath(nhConfigPath);
            var configuration = new Configuration();
            configuration.Configure(path);

            configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider, typeof(DriverConnectionProvider).AssemblyQualifiedName);

            Assembly assembly = Assembly.Load(MAPPINGSASSEMBLY);

            foreach (string file in mappings.Select(mf => MAPPINGSASSEMBLY + "." + mf))
            {
                configuration.AddResource(file, assembly);
            }

            return configuration;
        }

        private static void NorthwindCreateTestData()
        {
            var factory = SessionResolver.Current.GetFactoryFor<Customer>();

            using (IStatelessSession session = factory.OpenStatelessSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreateNorthwindData(session);

                tx.Commit();
            }
        }

        private static void MiscTestCreateTestData()
        {
            using (ISession session = SessionResolver.Current.GetCurrentSessionFor<Animal>())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreateMiscTestData(session);
                tx.Commit();
            }
        }

        private static void PatientCreateTestData()
        {
            using (ISession session = SessionResolver.Current.GetCurrentSessionFor<Patient>())
            using (ITransaction tx = session.BeginTransaction())
            {
                NorthwindDbCreator.CreatePatientData(session);
                tx.Commit();
            }
        }

    }
}
