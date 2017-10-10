using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;

namespace FMS.Website.Controllers
{
    public class MstComplaintController : BaseController
    {
        private IComplaintCategoryBLL _complaintCategoryBLL;
        private Enums.MenuList _mainMenu;
        //
        // GET: /MstComplaint/

        public MstComplaintController(IPageBLL pageBll, IComplaintCategoryBLL complaintCategoryBLL) : base(pageBll, Enums.MenuList.MasterComplaintCategory)
        {
            _complaintCategoryBLL = complaintCategoryBLL;
            _mainMenu = Enums.MenuList.MasterComplaintCategory;
        }

        public ActionResult Index()
        {
            var data = _complaintCategoryBLL.GetComplaints();

            var model = new ComplaintCategoryModel();
            model.Details = Mapper.Map<List<ComplaintCategoryItem>>(data);
            return View(model);
        }

    }
}
