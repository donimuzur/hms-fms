using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Code;
using FMS.Website.Models;


namespace FMS.Website.Controllers
{

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
                return (Login)Session[Core.Constans.SessionKey.CurrentUser];
            }
            set
            {
                Session[Constans.SessionKey.CurrentUser] = value;
            }
        }

        protected MST_MODUL PageInfo
        {
            get
            {
                return _pageBLL.GetPageByModulName(_menuID.ToString());
            }
        }

        protected RoleDto CurrentPageAccess
        {
            get
            {
                var dataRole = CurrentUser.AuthorizePages.FirstOrDefault(x => x.IsActive && x.ModulId == (int) _menuID);
                if (dataRole == null)
                {
                    return new RoleDto()
                    {
                        ModulId = (int) _menuID,
                        ReadAccess = false,
                        WriteAccess = false,
                        UploadAccess = false
                    };
                }
                return dataRole;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;


            base.OnActionExecuted(filterContext);

        }

        public List<ChangesLogs> GetChangesHistory(int modulId, long formId)
        {
            var data = _pageBLL.GetChangesHistory(modulId, formId);

            return Mapper.Map<List<ChangesLogs>>(data);
        }

        public List<WorkflowLogs> GetWorkflowHistory(int modulId, long formId)
        {
            var data = _pageBLL.GetWorkflowHistory(modulId, formId);

            return Mapper.Map<List<WorkflowLogs>>(data);
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
                if (CurrentUser.AuthorizePages.Count > 0)
                {
                    if (!CurrentUser.AuthorizePages.Select(x=> x.ModulId).Contains(PageInfo.MST_MODUL_ID))
                    {
                        if (!CurrentUser.AuthorizePages.Select(x => x.ModulId).Contains(PageInfo.PARENT_MODUL_ID))
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