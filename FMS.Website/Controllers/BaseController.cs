using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Code;
using FMS.Website.Models;
using System.Data.SqlClient;
using FMS.BusinessObject.Dto;
using System.Data.Entity.Core.EntityClient;
using FMS.BLL.Role;
using FMS.Website.Helpers;

namespace FMS.Website.Controllers
{
    [AuthorizeAD]
    public class BaseController : Controller
    {

        private IPageBLL _pageBLL;
        private Enums.MenuList _menuID;

        public BaseController(IPageBLL pageBll, Enums.MenuList menuID)
        {
            _pageBLL = pageBll;
            _menuID = menuID;
        }
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private bool IsAvailableDistrictCookie(string cookieName)
        {
            if (ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(cookieName))
            {
                return true;
            }
            return false;
        }

        protected void CreateCookie(string cookieName, string district)
        {
            HttpCookie cookie;
            if (!IsAvailableDistrictCookie(cookieName))
            {
                cookie = new HttpCookie(cookieName);


            }
            else
            {
                cookie = ControllerContext.HttpContext.Request.Cookies[cookieName];

            }
            if (cookie != null)
            {
                cookie.Value = district;
                cookie.Expires = DateTime.Now.AddYears(1);
                ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }

        public Login CurrentUser
        {
            get
            {

                SetLoginSession();
                return (Login)Session[Constans.SessionKey.CurrentUser];

            }
        }
        public void SetLoginSession()
        {
            if (Session[Core.Constans.SessionKey.CurrentUser] == null)
            {
                var userId = User.Identity.Name.Split('\\')[User.Identity.Name.Split('\\').Length - 1]; //User.Identity.Name.Remove(0, 4);
                IRoleBLL _userBll = MvcApplication.GetInstance<RoleBLL>();
                EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
                string connectionString = e.ProviderConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();
                var list = new List<String>();
                SqlCommand query = new SqlCommand("SELECT ROLE_NAME FROM ROLE_CONFIG ", con);
                SqlDataReader reader = query.ExecuteReader();
                while (reader.Read())
                {
                    var roleName = reader[0].ToString();
                    list.Add(roleName);
                }
                reader.Close();
                var getrole = new List<LdapDto>();
                foreach (var item in list)
                {
                   query = new SqlCommand("SELECT ADGroup = '"+ item +"', employeeID, login = sAMAccountName, displayName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + item + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ", con);
                    reader = query.ExecuteReader();
                    while (reader.Read())
                    {
                        var data = new LdapDto();
                        data.ADGroup = reader[0].ToString();
                        data.EmployeeId = reader[1].ToString();
                        data.Login = reader[2].ToString();
                        data.DisplayName = reader[3].ToString();
                        var arsplit = data.ADGroup.Split(' ').ToList();
                        arsplit.RemoveAt(arsplit.Count - 1);
                        arsplit.RemoveAt(arsplit.Count - 1);
                        data.RoleName = string.Join(" ", arsplit.ToArray());
                        data.RoleName = data.RoleName.Substring(23);
                        getrole.Add(data);
                    }
                    reader.Close();
                }
                var userrole = getrole.Where(x => x.Login == userId).FirstOrDefault();
                var loginResult = new Login();
                con.Close();
                if (userrole != null)
                {
                    loginResult.UserRole = _userBll.GetUserRole(userrole.RoleName);
                    loginResult.USERNAME = userrole.DisplayName;
                    loginResult.AuthorizePages = _userBll.GetRoles().Where(x => x.RoleName == userrole.RoleName).Select(x => x.ModulId).ToList();
                    loginResult.USER_ID = userrole.Login;
                    Session[Core.Constans.SessionKey.CurrentUser] = loginResult;
                }
            }
        }
        protected MST_MODUL PageInfo
        {
            get
            {
                return _pageBLL.GetPageByModulName( _menuID.ToString());
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;


            base.OnActionExecuted(filterContext);

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var descriptor = filterContext.ActionDescriptor;
            var actionName = descriptor.ActionName;
            var controllerName = descriptor.ControllerDescriptor.ControllerName;

            if (controllerName == "Login" && actionName == "Index") return;

            if (CurrentUser == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary { { "controller", "Login" }, { "action", "Index" } });

                return;
            }
           
            var isUsePageAuth = ConfigurationManager.AppSettings["UsePageAuth"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["UsePageAuth"]);
            if (isUsePageAuth)
            {
                CurrentUser.AuthorizePages = _pageBLL.GetAuthPages(CurrentUser);
                if (CurrentUser.AuthorizePages != null & CurrentUser.AuthorizePages.Count != 0)
                {
                    if (!CurrentUser.AuthorizePages.Contains(PageInfo.MST_MODUL_ID))
                    {
                        if (!CurrentUser.AuthorizePages.Contains(PageInfo.PARENT_MODUL_ID))
                        {
                            filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary { { "controller", "Error" }, { "action", "Unauthorized" } });

                        }
                    }
                }
            }
        }

        #region MessageInfo
        private List<MessageInfo> ListMessageInfo { get; set; }

        private void AddMessage(MessageInfo messageInfo)
        {
            ListMessageInfo = (List<MessageInfo>)TempData["MessageInfo"] ?? new List<MessageInfo>();
            ListMessageInfo.Add(messageInfo);

            TempData["MessageInfo"] = ListMessageInfo;
        }

        public void AddMessageInfo(MessageInfo messageinfo)
        {
            AddMessage(messageinfo);
        }

        public void AddMessageInfo(List<string> message, Enums.MessageInfoType messageinfotype)
        {
            AddMessage(new MessageInfo(message, messageinfotype));
        }

        public void AddMessageInfo(string message, Enums.MessageInfoType messageinfotype)
        {
            AddMessage(new MessageInfo(new List<string> { message }, messageinfotype));
        }


        public List<BaseModel> GetListMessageInfo()
        {
            var lsModel = new List<BaseModel>();
            ListMessageInfo = (List<MessageInfo>)TempData["MessageInfo"];

            if (ListMessageInfo != null)
                lsModel.AddRange(ListMessageInfo.Select(messageInfo => new BaseModel()
                {
                    MessageTitle =messageInfo.MessageInfoType.ToString(),// EnumsHelper.GetResourceDisplayEnums(messageInfo.MessageInfoType),
                    MessageBody = messageInfo.MessageText
                }));

            return lsModel;
        }
        #endregion

        #region ---------- Pdf Purpose --------

        protected ActionResult Pdf()
        {
            return Pdf(null, null, null);
        }

        protected ActionResult Pdf(string fileDownloadName)
        {
            return Pdf(fileDownloadName, null, null);
        }

        protected ActionResult Pdf(string fileDownloadName, string viewName)
        {
            return Pdf(fileDownloadName, viewName, null);
        }

        protected ActionResult Pdf(object model)
        {
            return Pdf(null, null, model);
        }

        protected ActionResult Pdf(string fileDownloadName, object model)
        {
            return Pdf(fileDownloadName, null, model);
        }

        protected ActionResult Pdf(string fileDownloadName, string viewName, object model)
        {
            // Based on View() code in Controller base class from MVC
            if (model != null)
            {
                ViewData.Model = model;
            }
            PdfResult pdf = new PdfResult()
            {
                FileDownloadName = fileDownloadName,
                ViewName = viewName,
                ViewData = ViewData,
                TempData = TempData,
                ViewEngineCollection = ViewEngineCollection
            };
            return pdf;
        }

        #endregion

    }
}