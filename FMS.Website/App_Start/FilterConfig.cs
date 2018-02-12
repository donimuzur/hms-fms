using System.Web;
using System.Web.Mvc;
using FMS.Website.Filters;
namespace FMS.Website
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UniversalErrorHandlerAttribute());
            filters.Add(new HandleErrorAttribute() {  });
        }
    }
}