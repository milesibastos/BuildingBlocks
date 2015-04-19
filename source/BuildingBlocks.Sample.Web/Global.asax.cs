using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NHibernate;

namespace BuildingBlocks.Sample.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static readonly IEnumerable<ISessionFactory> _sessionFactory = new List<ISessionFactory>();

        static MvcApplication()
        {
            log4net.Config.XmlConfigurator.Configure();
            //var context = new LinqReadonlyTestsContext();
            //context.CreateNorthwindDb();
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
