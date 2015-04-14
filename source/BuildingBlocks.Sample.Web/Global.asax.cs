using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.DomainModel.Northwind.Entities;

namespace BuildingBlocks.Sample.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static readonly IEnumerable<ISessionFactory> _sessionFactory = new List<ISessionFactory>();

        static MvcApplication()
        {
            log4net.Config.XmlConfigurator.Configure();
            var Configuration = new Configuration();
            string nhConfigPath = HostingEnvironment.MapPath("~/App_Data/hibernate.cfg.xml");
            if (File.Exists(nhConfigPath))
            {
                Configuration.Configure(nhConfigPath);
            }
            Configuration.SetDefaultAssembly(typeof(Animal).Assembly.FullName)
                .SetDefaultNamespace(typeof(Animal).Namespace)
                .AddDirectory(new DirectoryInfo(HostingEnvironment.MapPath("~/App_Data/")));

            var _factory = Configuration.BuildSessionFactory();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
