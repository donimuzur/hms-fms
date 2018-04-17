using AutoMapper;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class EmailApprovalsController : Controller
    {
        private ITraCtfBLL _ctfBLL;
        private ITraCsfBLL _csfBLL;
        private ITraCrfBLL _crfBLL;
        private IUserBLL _userBLL;
        // GET: /EmailApproval/

        public EmailApprovalsController( ITraCtfBLL CtfBLL, ITraCsfBLL csfBLL, ITraCrfBLL crfBLL, IUserBLL UserBLL)
        {
            _ctfBLL = CtfBLL;
            _crfBLL = crfBLL;
            _csfBLL = csfBLL;
            _userBLL = UserBLL;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CTF()
        {   
            var id = Request.Form["id"];
            var email = Request.Form["email"];
            var action = Request.Form["action"];
            var ApprovedBy = Request.Form["ApprovedBy"];
            var StatusApproval = Request.Form["StatusApproval"];
            var Data = new EmailApprovals();

       
            
            try
            {
                Data.Status = "Success";
                Data.Messages = string.Empty;
            }
            catch (Exception e)
            {
                Data.Status = "Failed";
                Data.Messages = e.Message;
            }

            return Json(Data, JsonRequestBehavior.AllowGet);

        }

        public Login GetLoginByEmail()
        {
            var GetLogin = new Login();
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];
            
            var hrQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + hrRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";
            var fleetQuery = "SELECT 'PMI\\' + sAMAccountName AS sAMAccountName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " + fleetRole + ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ";

            if (typeEnv == "VTI")
            {
                hrQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + hrRole + "'";
                fleetQuery = "SELECT EMPLOYEE_ID FROM LOGIN_FOR_VTI WHERE AD_GROUP = '" + fleetRole + "'";
            }

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();

            //----------GetLogin------------//  
            var UserLogin = string.Empty;
            var Email = string.Empty;
            var EmployeeId = string.Empty;
            var FullName = string.Empty;

            var CreatorQuery = "SELECT EMAIL, FULL_NAME, ID FROM " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE EMAIL = 'PMI\\" + ctfData.CreatedBy + "'";
            if (typeEnv == "VTI")
            {
                CreatorQuery = "SELECT EMAIL, FULL_NAME FROM EMAIL_FOR_VTI WHERE FULL_NAME = 'PMI\\" + ctfData.CreatedBy + "'";
            }

            SqlCommand query = new SqlCommand(CreatorQuery, con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {

            }

            var GetCtf = _ctfBLL.GetCtfById(Convert.ToInt32(id));
            if (GetCtf == null)
            {
                Data.Status = "Failed";
                Data.Messages = "Document is not found";
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            if (action == "Approve")
            {
                if (StatusApproval == "6")
                {

                    GetCtf.DocumentStatus = Enums.DocumentStatus.WaitingFleetApproval;
                    GetCtf.ModifiedBy = CurrentUser.USER_ID;
                    GetCtf.ModifiedDate = DateTime.Now;
                    GetCtf.EmployeeIdFleetApproval = CurrentUser.EMPLOYEE_ID;
                    GetCtf.ApprovedFleet = CurrentUser.USER_ID;
                    GetCtf.ApprovedFleetDate = DateTime.Now;

                    var saveResult = _ctfBLL.Save(GetCtf, CurrentUser);
                }
                else if (StatusApproval == "5")
                {

                }
            }
            else
            {

            }

            return GetLogin;
        }
    }
}
