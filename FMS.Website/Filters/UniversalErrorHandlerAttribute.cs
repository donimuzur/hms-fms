using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FMS.Logger;
using FMS.Logger.Models;

namespace FMS.Website.Filters
{
    public class UniversalErrorHandlerAttribute:Attribute,IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                try
                {
                    ExceptionInfo exceptionInfo = new ExceptionInfo
                    {
                        ControllerName = (string)filterContext.RouteData.Values["controller"],
                        ActionName = (string)filterContext.RouteData.Values["action"],
                        Message = string.Format(
                                "Please see more information at: {0}",
                                Path.Combine(HttpContext.Current.Server.MapPath("~/Log"), "main-log.xml")
                            ),
                        Title = "Internal Server Error (500)",
                        DateTime = DateTime.Now,
                        ExceptionType = filterContext.Exception.GetType().Name,
                        ErrorMessage = filterContext.Exception.Message,
                        StackTrace = filterContext.Exception.StackTrace
                    };
                    
                    WebLogger.CreateLog(exceptionInfo, HttpContext.Current.Server.MapPath("~/Log"));
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "ErrorHandler",
                        ViewData = new ViewDataDictionary(exceptionInfo)
                    };
                }
                catch { }
                filterContext.ExceptionHandled = true;
            }
        }
    }
}