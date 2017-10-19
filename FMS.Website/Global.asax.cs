using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FMS.BLL.ComplaintCategory;
using FMS.BLL.Mapper;
using FMS.BLL.Page;
using FMS.Contract.BLL;
using FMS.Website.Code;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using FMS.Contract;
using FMS.DAL;

using NLog;
using FMS.BLL.Vendor;
using FMS.BLL.Fleet;
using FMS.BLL.Employee;
namespace FMS.Website
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static Container _container;
        public static TService GetInstance<TService>()
        where TService : class
        {
            return _container.GetInstance<TService>();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            Bootstrap();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_container));
        }

        private static void Bootstrap()
        {
            //initialize mappers
            ComplaintMapper.Initialize();
            RoleMapper.Initialize();
            FMSWebsiteMapper.Initialize();
            VendorMapper.Initialize();
            FleetMapper.Initialize();

            // 1. Create a new Simple Injector container
            var container = new Container();

            // register unit of work / context by request
            // http://simpleinjector.codeplex.com/wikipage?title=ObjectLifestyleManagement#PerThread
            var webLifestyle = new WebRequestLifestyle();

            //container.Register<IUnitOfWork, SqlUnitOfWork>(webLifestyle);
            //container.Register<ILogger, Logger>();
            container.Register<IUnitOfWork, SqlUnitOfWork>(webLifestyle);
            container.Register<IComplaintCategoryBLL,ComplaintCategoryBLL>();
            container.Register<IVendorBLL, VendorBLL>();
            container.Register<IEmployeeBLL,EmployeBLL>();
            container.Register<IPageBLL, PageBLL>();
            container.Register<IFleetBLL , FleetBLL>();
            // 3. Optionally verify the container's configuration.
            container.Verify();

            // 4. Store the container for use by Page classes.
            _container = container;
        }
    }
}