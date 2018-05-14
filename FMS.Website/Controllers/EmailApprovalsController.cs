using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using SpreadsheetLight;
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BLL.Role;

namespace FMS.Website.Controllers
{
    public class EmailApprovalsController : Controller
    {
        private ITraCtfBLL _ctfBLL;
        private ITraCsfBLL _csfBLL;
        private ITraCrfBLL _crfBLL;
        private ITraTemporaryBLL _tempBLL;
        private IReasonBLL _reasonBLL;
        private IVehicleSpectBLL _vehicleBLL;
        private IFleetBLL _fleetBLL;
        private IVendorBLL _vendorBLL;
        private IPriceListBLL _pricelistBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private ISettingBLL _settingBLL;

        // GET: /EmailApproval/
        public EmailApprovalsController( ITraCtfBLL CtfBLL, ITraCsfBLL csfBLL, ITraCrfBLL crfBLL, ITraTemporaryBLL TemporaryBLL, IReasonBLL ReasonBLL, IVehicleSpectBLL VehicleBLL, IFleetBLL FleetBLL, IVendorBLL VendorBLL, IPriceListBLL PricelistBLL, ILocationMappingBLL LocationMappingBLL,ISettingBLL SettingBLL)
        {
            _ctfBLL = CtfBLL;
            _crfBLL = crfBLL;
            _csfBLL = csfBLL;
            _tempBLL = TemporaryBLL;
            _reasonBLL = ReasonBLL;
            _vehicleBLL = VehicleBLL;
            _fleetBLL = FleetBLL;
            _pricelistBLL = PricelistBLL;
            _locationMappingBLL = LocationMappingBLL;
            _vendorBLL = VendorBLL;
            _settingBLL = SettingBLL;
        }

        public JsonResult CTF()
        {   
            var id = Request.Form["id"];
            var email = Request.Form["email"];
            var Response = Request.Form["action"];
            var ActionName = Request.Form["ActionName"];
            var StatusApproval = Request.Form["StatusApproval"];
            var Reason = Request.Form["Reason"];
            var Data = new EmailApprovals();
            var Login = new Login();
            
            Login = GetLoginByEmail(email);
            if(Login == null )
            {
                Data.Status = "Failed";
                Data.Messages = "User Not Found";
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var CtfDto = _ctfBLL.GetCtfById(Convert.ToInt32(id));
                var IsBenefit = CtfDto.VehicleType.ToUpper() == "BENEFIT";
                if(CtfDto ==null)
                {
                    Data.Status = "Failed";
                    Data.Messages = "Document Not Found";
                    return Json(Data, JsonRequestBehavior.AllowGet);
                }

               if(Response == "1")
                {
                    if(StatusApproval == "5")
                    {
                        try
                        {
                            CtfDto.DocumentStatus = Enums.DocumentStatus.WaitingHRApproval;
                            CtfDto.ModifiedBy = Login.USER_ID;
                            CtfDto.ModifiedDate = DateTime.Now;
                            CtfDto.EmployeeIdFleetApproval = Login.EMPLOYEE_ID;
                            CtfDto.ApprovedFleet = Login.USER_ID;
                            CtfDto.ApprovedFleetDate = DateTime.Now;
                            var saveResult = _ctfBLL.Save(CtfDto, Login);

                            CtfWorkflow(CtfDto.TraCtfId, Enums.ActionType.Approve, null, false, true, CtfDto.DocumentNumber, Login);
                        }
                        catch (Exception ex)
                        {
                            Data.Status = "Failed";
                            Data.Messages = ex.Message;
                            return Json(Data, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (StatusApproval == "6")
                    {
                        try
                        {
                            CtfDto.DocumentStatus = Enums.DocumentStatus.WaitingFleetApproval;
                            CtfDto.ModifiedBy = Login.USER_ID;
                            CtfDto.ModifiedDate = DateTime.Now;
                            CtfDto.EmployeeIdFleetApproval = Login.EMPLOYEE_ID;
                            CtfDto.ApprovedFleet = Login.USER_ID;
                            CtfDto.ApprovedFleetDate = DateTime.Now;

                            var saveResult = _ctfBLL.Save(CtfDto, Login);

                            var Reasons = _reasonBLL.GetReasonById(CtfDto.Reason.Value);
                            var reasonStr = "";
                            if (Reasons != null)
                                reasonStr = Reasons.Reason;
                            var IsEndRent = reasonStr.ToLower() == "end rent";
                            if (IsEndRent)
                            {
                                CtfWorkflow(CtfDto.TraCtfId, Enums.ActionType.Approve, null, IsEndRent, IsBenefit, CtfDto.DocumentNumber, Login);
                            }
                            CtfWorkflow(CtfDto.TraCtfId, Enums.ActionType.Approve, null, false, IsBenefit, CtfDto.DocumentNumber, Login);
                        }
                        catch (Exception ex)
                        {
                            Data.Status = "Failed";
                            Data.Messages = ex.Message;
                            return Json(Data, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else if(Response == "0")
                {
                    try
                    {
                        CtfDto.ApprovedFleet = Login.USER_ID;
                        CtfDto.ApprovedFleetDate = DateTime.Now;
                        CtfDto.EmployeeIdFleetApproval = Login.EMPLOYEE_ID;
                        var saveResult = _ctfBLL.Save(CtfDto, Login);

                        CtfWorkflow(CtfDto.TraCtfId, Enums.ActionType.Reject, Convert.ToInt32(Reason), false, IsBenefit, CtfDto.DocumentNumber, Login);
                    }
                    catch (Exception ex)
                    {
                        Data.Status = "Failed";
                        Data.Messages = ex.Message;
                        return Json(Data, JsonRequestBehavior.AllowGet);
                    }
                }
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

        public JsonResult CSF()
        {
            var id = Request.Form["id"];
            var email = Request.Form["email"];
            var Response = Request.Form["Response"];
            var ActionName = Request.Form["ActionName"];
            var StatusApproval = Request.Form["StatusApproval"];
            var Reason = Request.Form["Reason"];
            var Data = new EmailApprovals();
            var Login = new Login();

            Login = GetLoginByEmail(email);
            if (Login == null)
            {
                Data.Status = "Failed";
                Data.Messages = "User Not Found";
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var CsfDto = _csfBLL.GetCsfById(Convert.ToInt32(id));
                if (CsfDto == null)
                {
                    Data.Status = "Failed";
                    Data.Messages = "Document Not Found";
                    return Json(Data, JsonRequestBehavior.AllowGet);
                }

                if (Response == "1")
                {
                    if (StatusApproval == "5")
                    {
                        try
                        {
                            CsfWorkflow(CsfDto.TRA_CSF_ID, Enums.ActionType.Approve, null, Login);
                        }
                        catch (Exception ex)
                        {
                            Data.Status = "Failed";
                            Data.Messages = ex.Message;
                        }
                    }
                    else if (Response == "6")
                    {
                        try
                        {
                            var csfData = _csfBLL.GetCsfById(CsfDto.TRA_CSF_ID);

                            if (string.IsNullOrEmpty(csfData.VENDOR_NAME))
                            {
                                //get vendor name
                                var vendorName = string.Empty;

                                var dataAllPricelist = _pricelistBLL.GetPriceList().Where(x => x.IsActive).ToList();

                                var allVendor = _vendorBLL.GetVendor().Where(x => x.IsActive).ToList();

                                var zonePriceList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive && x.Basetown == csfData.LOCATION_CITY)
                                                                                                    .OrderByDescending(x => x.ValidFrom).FirstOrDefault();

                                var zonePriceListByUserCsf = zonePriceList == null ? string.Empty : zonePriceList.ZonePriceList;

                                //select vendor from pricelist
                                var dataVendor = dataAllPricelist.Where(x => (x.Manufacture == null ? "" : x.Manufacture.ToLower()) == (csfData.MANUFACTURER == null ? "" : csfData.MANUFACTURER.ToLower())
                                                                        && (x.Model == null ? "" : x.Model.ToLower()) == (csfData.MODEL == null ? "" : csfData.MODEL.ToLower())
                                                                        && (x.Series == null ? "" : x.Series.ToLower()) == (csfData.SERIES == null ? "" : csfData.SERIES.ToLower())
                                                                        && x.Year == csfData.CREATED_DATE.Year
                                                                        && (x.ZonePriceList == null ? "" : x.ZonePriceList.ToLower()) == zonePriceListByUserCsf.ToLower()).FirstOrDefault();

                                if (_settingBLL.GetByID(Convert.ToInt32(csfData.VEHICLE_TYPE)).SettingValue.ToLower() == "benefit")
                                {
                                    dataVendor = dataAllPricelist.Where(x => (x.Manufacture == null ? "" : x.Manufacture.ToLower()) == (csfData.MANUFACTURER == null ? "" : csfData.MANUFACTURER.ToLower())
                                                                        && (x.Model == null ? "" : x.Model.ToLower()) == (csfData.MODEL == null ? "" : csfData.MODEL.ToLower())
                                                                        && (x.Series == null ? "" : x.Series.ToLower()) == (csfData.SERIES == null ? "" : csfData.SERIES.ToLower())
                                                                        && x.Year == csfData.CREATED_DATE.Year).FirstOrDefault();
                                }

                                var vendorId = dataVendor == null ? 0 : dataVendor.Vendor;

                                var dataVendorDetail = allVendor.Where(x => x.MstVendorId == vendorId).FirstOrDefault();

                                vendorName = dataVendor == null ? string.Empty : (dataVendorDetail == null ? string.Empty : dataVendorDetail.ShortName);

                                csfData.VENDOR_NAME = vendorName;
                                var saveResult = _csfBLL.Save(csfData, Login);
                            }

                            CsfWorkflow(CsfDto.TRA_CSF_ID, Enums.ActionType.Approve, null,Login);
                        }
                        catch (Exception ex)
                        {
                            Data.Status = "Failed";
                            Data.Messages = ex.Message;
                        }
                    }
                    Data.Status = "Success";
                    Data.Messages = string.Empty;

                }
                else if (Response == "0")
                {
                    try
                    {
                        CsfWorkflow(CsfDto.TRA_CSF_ID, Enums.ActionType.Reject, Convert.ToInt32(Reason), Login);
                    }
                    catch (Exception ex)
                    {
                        Data.Status = "Failed";
                        Data.Messages = ex.Message;
                    }
                    Data.Status = "Success";
                    Data.Messages = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Data.Status = "Failed";
                Data.Messages = ex.Message;
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CRF()
        {
            var id = Request.Form["id"];
            var email = Request.Form["email"];
            var Response = Request.Form["Response"];
            var ActionName = Request.Form["ActionName"];
            var StatusApproval = Request.Form["StatusApproval"];
            var Reason = Request.Form["Reason"];
            var Data = new EmailApprovals();
            var Login = new Login();

            Login = GetLoginByEmail(email);
            if (Login == null)
            {
                Data.Status = "Failed";
                Data.Messages = "User Not Found";
                return Json(Data, JsonRequestBehavior.AllowGet);
            }

            var CrfDto = _crfBLL.GetDataById(Convert.ToInt32(id));
            if (CrfDto == null)
            {
                Data.Status = "Failed";
                Data.Messages = "Document Not Found";
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            try
            {
                if (Response == "1")
                {
                    _crfBLL.Approve(CrfDto.TRA_CRF_ID, Login);
                }
                else if (Response == "0")
                {
                    _crfBLL.Reject(CrfDto.TRA_CRF_ID, Convert.ToInt32(Response), Login);
                }
                Data.Status = "Success";
                Data.Messages = string.Empty;
            }
            catch (Exception ex)
            {
                Data.Status = "Failed";
                Data.Messages = ex.Message;
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Temporary()
        {
            var id = Request.Form["id"];
            var email = Request.Form["email"];
            var Response = Request.Form["Response"];
            var ActionName = Request.Form["ActionName"];
            var StatusApproval = Request.Form["StatusApproval"];
            var Reason = Request.Form["Reason"];
            var Data = new EmailApprovals();
            var Login = new Login();

            Login = GetLoginByEmail(email);
            if (Login == null)
            {
                Data.Status = "Failed";
                Data.Messages = "User Not Found";
                return Json(Data, JsonRequestBehavior.AllowGet);
            }

            var TempDto = _tempBLL.GetTempById(Convert.ToInt32(id));
            if (TempDto == null)
            {
                Data.Status = "Failed";
                Data.Messages = "Document Not Found";
                return Json(Data, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if(Response == "1")
                {
                    try
                    {
                        CsfWorkflow(TempDto.TRA_TEMPORARY_ID, Enums.ActionType.Approve, null, Login);
                    }
                    catch (Exception ex)
                    {
                        Data.Status = "Failed";
                        Data.Messages = ex.Message;
                    }               
                }
                else if(Response =="0")
                {
                    try
                    {
                        TempWorkflow(TempDto.TRA_TEMPORARY_ID, Enums.ActionType.Reject, Convert.ToInt32(Reason), Login);
                    }
                    catch (Exception ex)
                    {
                        Data.Status = "Failed";
                        Data.Messages = ex.Message;
                    }
                }
                Data.Status = "Success";
                Data.Messages = string.Empty;
            }
            catch (Exception ex)
            {
                Data.Status = "Failed";
                Data.Messages = ex.Message;
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        public Login GetLoginByEmail(string EmailUser)
        {
            var GetLogin = new Login();
            var GetRoleList = GetRoleUsers();
            IRoleBLL _userBll = MvcApplication.GetInstance<RoleBLL>();

            try
            {
                var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
                var typeEnv = ConfigurationManager.AppSettings["Environment"];
                var serverIntranet = ConfigurationManager.AppSettings["ServerIntranet"];

                var LoginQuery = "SELECT FULL_NAME AS USER_LOGIN, ID AS EMPLOYEE_ID, INTERNAL_EMAIL AS NAME " + serverIntranet + ".[dbo].[tbl_ADSI_User] WHERE EMAIL = '" + EmailUser + "'";

                if (typeEnv == "VTI")
                {
                    LoginQuery = "SELECT FULL_NAME as USER_LOGIN, ID AS EMPLOYEE_ID, FULL_NAME AS NAME  FROM EMAIL_FOR_VTI WHERE EMAIL = '" + EmailUser + "'";
                }

                EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
                string connectionString = e.ProviderConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                con.Open();

                //----------GetLogin------------//  
                SqlCommand query = new SqlCommand(LoginQuery, con);
                SqlDataReader reader = query.ExecuteReader();
                while (reader.Read())
                {

                    GetLogin.USER_ID = reader["NAME"].ToString();
                    var UserLogin = reader["USER_LOGIN"].ToString();
                    if (UserLogin != "")
                    {
                        UserLogin = reader["USER_LOGIN"].ToString().Split('\\')[reader["USER_LOGIN"].ToString().Split('\\').Length - 1];
                        
                    }
                    GetLogin.USERNAME = UserLogin;
                    var employeeId = reader["EMPLOYEE_ID"].ToString();
                    if (employeeId != "")
                    {
                        employeeId = employeeId.Replace("ID", "");
                        employeeId = Convert.ToInt32(employeeId).ToString("00000000");
                    }
                    GetLogin.EMPLOYEE_ID = employeeId;
                    var RoleName = GetRoleList.Where(x => (string.IsNullOrEmpty(x.Login) ? "" : x.Login.ToUpper()) == (string.IsNullOrEmpty(GetLogin.USERNAME) ? "" : GetLogin.USERNAME.ToUpper())).FirstOrDefault();
                    if(RoleName != null)
                    {
                        GetLogin.UserRole = _userBll.GetUserRole(RoleName.RoleName);
                    }
                }
                reader.Close();

                con.Close();
            }
            catch (Exception ex)
            {
                throw new Exception( ex.Message);
            }
           
            return GetLogin;
        }

        public List<LdapDto> GetRoleUsers()
        {
            IRoleBLL _userBll = MvcApplication.GetInstance<RoleBLL>();
            EntityConnectionStringBuilder e = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FMSEntities"].ConnectionString);
            string connectionString = e.ProviderConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            var list = new List<String>();
            var typeEnv = ConfigurationManager.AppSettings["Environment"];
            var getrole = new List<LdapDto>();

            SqlCommand query =
                    new SqlCommand("SELECT SETTING_VALUE FROM MST_SETTING WHERE SETTING_GROUP = 'USER_ROLE'", con);
            SqlDataReader reader = query.ExecuteReader();
            while (reader.Read())
            {
                var roleName = reader[0].ToString();
                list.Add(roleName);
            }
            reader.Close();

            if (typeEnv == "VTI")
            {
                query =
                        new SqlCommand("SELECT AD_GROUP, EMPLOYEE_ID, LOGIN,DISPLAY_NAME, EMAIL from LOGIN_FOR_VTI",
                            con);

                reader = query.ExecuteReader();
                while (reader.Read())
                {
                    var data = new LdapDto();
                    data.ADGroup = reader[0].ToString();
                    data.EmployeeId = reader[1].ToString();
                    data.Login = reader[2].ToString();
                    data.DisplayName = reader[3].ToString();
                    data.RoleName = "USER";
                    var arsplit = new List<string>();
                    if (!string.IsNullOrEmpty(data.ADGroup))
                    {
                        arsplit = data.ADGroup.Split(' ').ToList();
                        arsplit.RemoveAt(arsplit.Count - 1);
                        arsplit.RemoveAt(arsplit.Count - 1);
                        data.RoleName = string.Join(" ", arsplit.ToArray());
                        data.RoleName = data.RoleName.Substring(23);
                        getrole.Add(data);
                    }


                }
                reader.Close();
            }
            else
            {
                foreach (var item in list)
                {
                    query =
                        new SqlCommand(
                            "SELECT ADGroup = '" + item +
                            "', employeeID, login = sAMAccountName, displayName FROM OPENQUERY(ADSI, 'SELECT employeeID, sAMAccountName, displayName, name, givenName, whenCreated, whenChanged, SN, manager, distinguishedName, info FROM ''LDAP://DC=PMINTL,DC=NET'' WHERE memberOf = ''CN = " +
                            item +
                            ", OU = ID, OU = Security, OU = IMDL Managed Groups, OU = Global, OU = Users & Workstations, DC = PMINTL, DC = NET''') ",
                            con);
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
            }
            return getrole;
        }

        #region ---------------------- Workflow ------------------------
        private void CtfWorkflow(long id, Enums.ActionType actionType, int? comment, bool Endrent, bool isBenefit, string DocumentNumber, Login CurrentUser)
        {
            var input = new CtfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                UserRole = CurrentUser.UserRole,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                ActionType = actionType,
                Comment = comment,
                EndRent = Endrent,
                isBenefit = isBenefit,
                DocumentNumber = DocumentNumber
            };

            _ctfBLL.CtfWorkflow(input);
        }

        private void CsfWorkflow(long id, Enums.ActionType actionType, int? comment,Login CurrentUser)
        {
            var attachmentsList = new List<string>();

            //if fleet approve and send csv to vendor
            if (CurrentUser.UserRole == Enums.UserRole.Fleet && actionType == Enums.ActionType.Approve)
            {
                var docForVendor = CreateExcelCSFForVendor(id);
                attachmentsList.Add(docForVendor);
            }

            var input = new CsfWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                Attachments = attachmentsList
            };

            _csfBLL.CsfWorkflow(input);
        }

        private void TempWorkflow(long id, Enums.ActionType actionType, int? comment, Login CurrentUser)
        {
            var attachmentsList = new List<string>();

            //if fleet approve and send csv to vendor
            if (CurrentUser.UserRole == Enums.UserRole.Fleet &&
                                        (actionType == Enums.ActionType.Approve || actionType == Enums.ActionType.Submit))
            {
                var docForVendor = CreateExcelTemporaryForVendor(id);
                attachmentsList.Add(docForVendor);
            }

            var input = new TempWorkflowDocumentInput
            {
                DocumentId = id,
                UserId = CurrentUser.USER_ID,
                EmployeeId = CurrentUser.EMPLOYEE_ID,
                UserRole = CurrentUser.UserRole,
                ActionType = actionType,
                Comment = comment,
                Attachments = attachmentsList
            };

            _tempBLL.TempWorkflow(input);
        }
        #endregion

        #region ----------------- Export To Excel -----------------

        #region --- CSF Excel ---
        private string CreateExcelCSFForVendor(long id)
        {
            //get data
            var csfData = _csfBLL.GetCsfById(id);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(2, 2, "System");
            slDocument.MergeWorksheetCells(2, 2, 2, 4);

            slDocument.SetCellValue(2, 5, "Vendor");
            slDocument.MergeWorksheetCells(2, 5, 2, 10);

            slDocument.SetCellValue(2, 11, "User");
            slDocument.MergeWorksheetCells(2, 11, 2, 17);

            slDocument.SetCellValue(2, 18, "Fleet");
            slDocument.MergeWorksheetCells(2, 18, 2, 26);

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Font.FontSize = 11;
            slDocument.SetCellStyle(2, 2, 2, 26, valueStyle);

            //create header
            slDocument = CreateHeaderExcelCSFForVendor(slDocument);

            //create data
            slDocument = CreateDataExcelCSFForVendor(slDocument, csfData);

            var fileName = "ExcelForVendor_CSF_" + csfData.TRA_CSF_ID + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelCSFForVendor(SLDocument slDocument)
        {
            int iRow = 3;

            slDocument.SetCellValue(iRow, 2, "Request Number");
            slDocument.SetCellValue(iRow, 3, "Employee Name");
            slDocument.SetCellValue(iRow, 4, "Vendor");
            slDocument.SetCellValue(iRow, 5, "Police Number");
            slDocument.SetCellValue(iRow, 6, "Chasis Number");
            slDocument.SetCellValue(iRow, 7, "Engine Number");
            slDocument.SetCellValue(iRow, 8, "Contract Start Date");
            slDocument.SetCellValue(iRow, 9, "Contract End Date");
            slDocument.SetCellValue(iRow, 10, "AirBag");
            slDocument.SetCellValue(iRow, 11, "Make");
            slDocument.SetCellValue(iRow, 12, "Model");
            slDocument.SetCellValue(iRow, 13, "Series");
            slDocument.SetCellValue(iRow, 14, "Transmission");
            slDocument.SetCellValue(iRow, 15, "Color");
            slDocument.SetCellValue(iRow, 16, "Body type");
            slDocument.SetCellValue(iRow, 17, "Location");
            slDocument.SetCellValue(iRow, 18, "Branding");
            slDocument.SetCellValue(iRow, 19, "Purpose");
            slDocument.SetCellValue(iRow, 20, "Request Year");
            slDocument.SetCellValue(iRow, 21, "PO");
            slDocument.SetCellValue(iRow, 22, "PO Line");
            slDocument.SetCellValue(iRow, 23, "Vat");
            slDocument.SetCellValue(iRow, 24, "Restitution");
            slDocument.SetCellValue(iRow, 25, "Price");
            slDocument.SetCellValue(iRow, 26, "Comments");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(iRow, 2, iRow, 26, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelCSFForVendor(SLDocument slDocument, TraCsfDto csfData)
        {
            int iRow = 4; //starting row data

            var vSpecListData = _vehicleBLL.GetVehicleSpect().Where(x => x.Year == csfData.CREATED_DATE.Year
                                                                        && x.Manufacturer != null
                                                                        && x.Models != null
                                                                        && x.Series != null
                                                                        && x.BodyType != null
                                                                        && x.IsActive).ToList();

            var vSpecList = vSpecListData.Where(x => x.Manufacturer.ToUpper() == csfData.MANUFACTURER.ToUpper()
                                                    && x.Models.ToUpper() == csfData.MODEL.ToUpper()
                                                    && x.Series.ToUpper() == csfData.SERIES.ToUpper()
                                                    && x.BodyType.ToUpper() == csfData.BODY_TYPE.ToUpper()).FirstOrDefault();

            var transmissionData = vSpecList == null ? string.Empty : vSpecList.Transmission;

            var policeNumberCfmIdle = string.Empty;
            var chasCfmIdle = string.Empty;
            var engCfmIdle = string.Empty;
            if (csfData.CFM_IDLE_ID != null)
            {
                var cfmData = _fleetBLL.GetFleetById((int)csfData.CFM_IDLE_ID);
                if (cfmData != null)
                {
                    policeNumberCfmIdle = cfmData.PoliceNumber == null ? string.Empty : cfmData.PoliceNumber;
                    chasCfmIdle = cfmData.ChasisNumber == null ? string.Empty : cfmData.ChasisNumber;
                    engCfmIdle = cfmData.EngineNumber == null ? string.Empty : cfmData.EngineNumber;
                    transmissionData = cfmData.Transmission == null ? string.Empty : cfmData.Transmission;
                }
            }

            slDocument.SetCellValue(iRow, 2, csfData.DOCUMENT_NUMBER);
            slDocument.SetCellValue(iRow, 3, csfData.EMPLOYEE_NAME);
            slDocument.SetCellValue(iRow, 4, csfData.VENDOR_NAME);
            slDocument.SetCellValue(iRow, 5, policeNumberCfmIdle);
            slDocument.SetCellValue(iRow, 6, chasCfmIdle);
            slDocument.SetCellValue(iRow, 7, engCfmIdle);
            slDocument.SetCellValue(iRow, 8, csfData.EFFECTIVE_DATE.ToOADate());
            slDocument.SetCellValue(iRow, 9, string.Empty);
            slDocument.SetCellValue(iRow, 10, "YES");
            slDocument.SetCellValue(iRow, 11, csfData.MANUFACTURER);
            slDocument.SetCellValue(iRow, 12, csfData.MODEL);
            slDocument.SetCellValue(iRow, 13, csfData.SERIES);
            slDocument.SetCellValue(iRow, 14, transmissionData);
            slDocument.SetCellValue(iRow, 15, csfData.COLOUR);
            slDocument.SetCellValue(iRow, 16, csfData.BODY_TYPE);
            slDocument.SetCellValue(iRow, 17, csfData.LOCATION_CITY);
            slDocument.SetCellValue(iRow, 18, string.Empty);
            slDocument.SetCellValue(iRow, 19, string.Empty);
            slDocument.SetCellValue(iRow, 20, csfData.CREATED_DATE.Year.ToString());
            slDocument.SetCellValue(iRow, 21, string.Empty);
            slDocument.SetCellValue(iRow, 22, string.Empty);
            slDocument.SetCellValue(iRow, 23, 0);
            slDocument.SetCellValue(iRow, 24, "NO");
            slDocument.SetCellValue(iRow, 25, 0);
            slDocument.SetCellValue(iRow, 26, string.Empty);

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(2, 26);
            slDocument.SetCellStyle(iRow, 2, iRow, 26, valueStyle);

            SLStyle dateStyle = slDocument.CreateStyle();
            dateStyle.FormatCode = "dd/MM/yyyy";

            slDocument.SetCellStyle(iRow, 8, iRow, 8, dateStyle);

            return slDocument;
        }
        #endregion

        #region --- Temporary Excel ---
        #region --------- Add Attachment File For Vendor --------------

        private string CreateExcelTemporaryForVendor(long id)
        {
            //get data
            var tempData = _tempBLL.GetTempById(id);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(2, 2, "System");
            slDocument.MergeWorksheetCells(2, 2, 2, 4);

            slDocument.SetCellValue(2, 5, "Vendor");
            slDocument.MergeWorksheetCells(2, 5, 2, 10);

            slDocument.SetCellValue(2, 11, "User");
            slDocument.MergeWorksheetCells(2, 11, 2, 17);

            slDocument.SetCellValue(2, 18, "Fleet");
            slDocument.MergeWorksheetCells(2, 18, 2, 26);

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Font.FontSize = 11;
            slDocument.SetCellStyle(2, 2, 2, 26, valueStyle);

            //create header
            slDocument = CreateHeaderExcelTemporaryForVendor(slDocument);

            //create data
            slDocument = CreateDataExcelTemporaryForVendor(slDocument, tempData);

            var fileName = "ExcelForVendor_TMP_" + tempData.TRA_TEMPORARY_ID + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelTemporaryForVendor(SLDocument slDocument)
        {
            int iRow = 3;

            slDocument.SetCellValue(iRow, 2, "Request Number");
            slDocument.SetCellValue(iRow, 3, "Employee Name");
            slDocument.SetCellValue(iRow, 4, "Vendor");
            slDocument.SetCellValue(iRow, 5, "Police Number");
            slDocument.SetCellValue(iRow, 6, "Chasis Number");
            slDocument.SetCellValue(iRow, 7, "Engine Number");
            slDocument.SetCellValue(iRow, 8, "Contract Start Date");
            slDocument.SetCellValue(iRow, 9, "Contract End Date");
            slDocument.SetCellValue(iRow, 10, "AirBag");
            slDocument.SetCellValue(iRow, 11, "Make");
            slDocument.SetCellValue(iRow, 12, "Model");
            slDocument.SetCellValue(iRow, 13, "Series");
            slDocument.SetCellValue(iRow, 14, "Transmission");
            slDocument.SetCellValue(iRow, 15, "Color");
            slDocument.SetCellValue(iRow, 16, "Body type");
            slDocument.SetCellValue(iRow, 17, "Location");
            slDocument.SetCellValue(iRow, 18, "Branding");
            slDocument.SetCellValue(iRow, 19, "Purpose");
            slDocument.SetCellValue(iRow, 20, "Request Year");
            slDocument.SetCellValue(iRow, 21, "PO");
            slDocument.SetCellValue(iRow, 22, "PO Line");
            slDocument.SetCellValue(iRow, 23, "Vat");
            slDocument.SetCellValue(iRow, 24, "Restitution");
            slDocument.SetCellValue(iRow, 25, "Price");
            slDocument.SetCellValue(iRow, 26, "Comments");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(iRow, 2, iRow, 26, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelTemporaryForVendor(SLDocument slDocument, TemporaryDto tempData)
        {
            int iRow = 4; //starting row data

            var vSpecListData = _vehicleBLL.GetVehicleSpect().Where(x => x.Year == tempData.CREATED_DATE.Year
                                                                        && x.Manufacturer != null
                                                                        && x.Models != null
                                                                        && x.Series != null
                                                                        && x.BodyType != null
                                                                        && x.IsActive).ToList();

            var vSpecList = vSpecListData.Where(x => x.Manufacturer.ToUpper() == tempData.MANUFACTURER.ToUpper()
                                                    && x.Models.ToUpper() == tempData.MODEL.ToUpper()
                                                    && x.Series.ToUpper() == tempData.SERIES.ToUpper()
                                                    && x.BodyType.ToUpper() == tempData.BODY_TYPE.ToUpper()).FirstOrDefault();

            var transmissionData = vSpecList == null ? string.Empty : vSpecList.Transmission;

            var policeNumberCfmIdle = string.Empty;
            var chasCfmIdle = string.Empty;
            var engCfmIdle = string.Empty;
            if (tempData.CFM_IDLE_ID != null)
            {
                var cfmData = _fleetBLL.GetFleetById((int)tempData.CFM_IDLE_ID);
                if (cfmData != null)
                {
                    policeNumberCfmIdle = cfmData.PoliceNumber == null ? string.Empty : cfmData.PoliceNumber;
                    chasCfmIdle = cfmData.ChasisNumber == null ? string.Empty : cfmData.ChasisNumber;
                    engCfmIdle = cfmData.EngineNumber == null ? string.Empty : cfmData.EngineNumber;
                    transmissionData = cfmData.Transmission == null ? string.Empty : cfmData.Transmission;
                }
            }

            slDocument.SetCellValue(iRow, 2, tempData.DOCUMENT_NUMBER_TEMP);
            slDocument.SetCellValue(iRow, 3, tempData.EMPLOYEE_NAME);
            slDocument.SetCellValue(iRow, 4, tempData.VENDOR_NAME);
            slDocument.SetCellValue(iRow, 5, policeNumberCfmIdle);
            slDocument.SetCellValue(iRow, 6, chasCfmIdle);
            slDocument.SetCellValue(iRow, 7, engCfmIdle);
            slDocument.SetCellValue(iRow, 8, tempData.START_DATE.Value.ToOADate());
            slDocument.SetCellValue(iRow, 9, tempData.END_DATE.Value.ToOADate());
            slDocument.SetCellValue(iRow, 10, "YES");
            slDocument.SetCellValue(iRow, 11, tempData.MANUFACTURER);
            slDocument.SetCellValue(iRow, 12, tempData.MODEL);
            slDocument.SetCellValue(iRow, 13, tempData.SERIES);
            slDocument.SetCellValue(iRow, 14, transmissionData);
            slDocument.SetCellValue(iRow, 15, tempData.COLOR);
            slDocument.SetCellValue(iRow, 16, tempData.BODY_TYPE);
            slDocument.SetCellValue(iRow, 17, tempData.LOCATION_CITY);
            slDocument.SetCellValue(iRow, 18, string.Empty);
            slDocument.SetCellValue(iRow, 19, string.Empty);
            slDocument.SetCellValue(iRow, 20, tempData.CREATED_DATE.Year.ToString());
            slDocument.SetCellValue(iRow, 21, string.Empty);
            slDocument.SetCellValue(iRow, 22, string.Empty);
            slDocument.SetCellValue(iRow, 23, 0);
            slDocument.SetCellValue(iRow, 24, "NO");
            slDocument.SetCellValue(iRow, 25, 0);
            slDocument.SetCellValue(iRow, 26, string.Empty);

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(2, 26);
            slDocument.SetCellStyle(iRow, 2, iRow, 26, valueStyle);

            SLStyle dateStyle = slDocument.CreateStyle();
            dateStyle.FormatCode = "dd/MM/yyyy";

            slDocument.SetCellStyle(iRow, 8, iRow, 9, dateStyle);

            return slDocument;
        }

        #endregion

        #endregion

        #endregion
    }
}
