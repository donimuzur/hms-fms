using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class TraCtfController : BaseController
    {
        private IEpafBLL _epafBLL;
        private ITraCtfBLL _ctfBLL;
        private IRemarkBLL _remarkBLL;
        private IEmployeeBLL _employeeBLL;
        private IReasonBLL _reasonBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        public TraCtfController(IPageBLL pageBll, IEpafBLL epafBll, ITraCtfBLL ctfBll, IRemarkBLL RemarkBLL, IEmployeeBLL  EmployeeBLL, IReasonBLL ReasonBLL): base(pageBll, Core.Enums.MenuList.TraCtf)
        {
            _epafBLL = epafBll;
            _ctfBLL = ctfBll;
            _employeeBLL = EmployeeBLL;
            _pageBLL = pageBll;
            _remarkBLL = RemarkBLL;
            _reasonBLL = ReasonBLL;
            _mainMenu = Enums.MenuList.Transaction;
        }
        public ActionResult Index()
        {
            var model = new CtfModel();
            //model.TitleForm = "CSF Open Document";
            //model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new CtfItem();
            var EmployeeList= _employeeBLL.GetEmployee().Where(x => x.IS_ACTIVE == true).Select(x => new { x.EMPLOYEE_ID , employee=x.EMPLOYEE_ID+" == "+ x.FORMAL_NAME }).Distinct().ToList();
            model.EmployeeIdList = new SelectList(EmployeeList, "EMPLOYEE_ID", "employee");
            var ReasonList = _reasonBLL.GetReason().Where(x => x.IsActive == true && x.DocumentType == 6 ).ToList();
            model.ReasonList = new SelectList(ReasonList, "MstReasonId", "Reason");
            model.CreatedDate = DateTime.Now;
            model.CreatedDateS = model.CreatedDate.ToString("dd-MMM-yyyy");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        public JsonResult GetEmployee(string Id)
        {
            var model = _employeeBLL.GetByID(Id);
            return Json(model);
        }

        public ActionResult Dashboard()
        {
            var EpafData = _epafBLL.GetEpafByDocType(Enums.DocumentType.CTF).ToList();
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.RoleType == CurrentUser.UserRole.ToString()).ToList();
            
            var model = new CtfModel();
            model.RemarkList = new SelectList(RemarkList, "MstRemarkId", "Remark");
            foreach (var data in EpafData)
            {
                var item = new CtfItem();
                item.EPafData = data;
                model.Details.Add(item);
            }
            model.TitleForm = "CTF Dashboard"; 
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public ActionResult Completed()
        {
            var data = _epafBLL.GetEpaf().Where(x => x.DocumentType == 1);
            var model = new CsfIndexModel();
            //model.TitleForm = "CSF Completed Document";
            //model.EpafList = Mapper.Map<List<EpafData>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View("Index", model);
        }

        public ActionResult CloseEpaf(int MstEpafId, int RemarkId)
        {
            
            if(ModelState.IsValid)
            {
                try
                {
                    _epafBLL.DeactivateEpaf(MstEpafId, RemarkId, CurrentUser.USERNAME);
                }
                catch (Exception exp)
                {
                    
                }
                
            }
            return RedirectToAction("Dashboard", "TraCtf");
        }

    }
}
