using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BLL.Role;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;

namespace FMS.Website.Controllers
{
    public class LoginController : BaseController
    {
        public LoginController(IPageBLL pageBll)
            : base(pageBll, Enums.MenuList.Login)
        {

        }

        //
        // GET: /Login/
        public ActionResult Index()
        {
            var model = new LoginFormModel();
            var listLogin = new List<LdapDto>();

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand("SELECT LOGIN, CONCAT(LOGIN,'-',SUBSTRING(AD_GROUP,23,10)) AS DISPLAY_LOGIN FROM LOGIN_FOR_VTI", con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                var item = new LdapDto();
                item.Login = reader[0].ToString();
                item.DisplayName = reader[1].ToString();
                listLogin.Add(item);
            }
            reader.Close();
            con.Close();

            model.Users = new SelectList(listLogin, "Login", "DisplayName");
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(LoginFormModel model)
        {
            var item = new LdapDto();

            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand query = new SqlCommand("SELECT AD_GROUP, EMPLOYEE_ID, LOGIN, DISPLAY_NAME FROM LOGIN_FOR_VTI WHERE LOGIN = '" + model.Login.UserId + "'", con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                item.ADGroup = reader[0].ToString();
                item.EmployeeId = reader[1].ToString();
                item.Login = reader[2].ToString();
                item.DisplayName = reader[3].ToString();
                item.RoleName = "User";
                var arsplit = new List<string>();
                if (!string.IsNullOrEmpty(item.ADGroup))
                {
                    arsplit = item.ADGroup.Split(' ').ToList();
                    arsplit.RemoveAt(arsplit.Count - 1);
                    arsplit.RemoveAt(arsplit.Count - 1);
                    item.RoleName = string.Join(" ", arsplit.ToArray());
                    item.RoleName = item.RoleName.Substring(23);
                }

            }
            reader.Close();
            con.Close();

            IRoleBLL _roleBll = MvcApplication.GetInstance<RoleBLL>();

            if (item.Login != null)
            {
                CurrentUser = new Login();
                CurrentUser.UserRole = _roleBll.GetUserRole(item.RoleName);
                CurrentUser.AuthorizePages = _roleBll.GetRoles().Where(x => x.RoleName == item.RoleName).Select(x => x.ModulId).ToList();
                CurrentUser.EMPLOYEE_ID = item.EmployeeId;
                CurrentUser.USERNAME = item.DisplayName;
                CurrentUser.USER_ID = item.Login;

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Unauthorized", "Error");

        }

        public ActionResult MessageInfo()
        {
            var model = GetListMessageInfo();
            return PartialView("_MessageInfo", model);
        }
    }
}
