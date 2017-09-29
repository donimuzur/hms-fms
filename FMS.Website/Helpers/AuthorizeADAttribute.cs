using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Helpers
{
    public class AuthorizeADAttribute : AuthorizeAttribute
    {
        private bool _authenticated;
        private bool _authorized;
       
        

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);

            if (!_authenticated || !_authorized)
            {
                var baseUrl = ConfigurationManager.AppSettings["WebRootUrl"].ToString();
                filterContext.Result = new RedirectResult(baseUrl +"/Error/Unauthorized");
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            _authenticated = base.AuthorizeCore(httpContext); // AD Authorization

            //IUserAuthorizationBLL authorization;
            if (_authenticated)
            {

                _authorized = true;
                return _authorized;
            }

            _authorized = false;
            return _authorized;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }

    }
}