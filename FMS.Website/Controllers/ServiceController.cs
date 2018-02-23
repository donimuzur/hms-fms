using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BLL.Crf;
using FMS.BLL.Csf;
using FMS.BLL.Ctf;
using FMS.BLL.Temporary;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.DAL;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;
using FMS.BusinessObject.Dto;
using FMS.Core;
using FMS.BusinessObject.Business;
using FMS.BLL.Fleet;
using FMS.BLL.Setting;
using FMS.BLL.VehicleSpect;
using System.Configuration;
using FMS.BLL.CtfExtend;
using FMS.BLL.Ccf;

namespace FMS.Website.Controllers
{
    public class ServiceController : Controller
    {
        private IUnitOfWork uow;
        private ITraCrfBLL crfBll;
        private ITraCtfBLL ctfBLL;
        private ICtfExtendBLL ctfExtendBLL;
        private ITraCsfBLL csfBll;
        private ITraTemporaryBLL tempBll;
        private IFleetBLL fleetBLL;
        private ISettingBLL settingBLL;
        private IVehicleSpectBLL vehicleSpectBLL;
        private ITraCcfBLL ccfBLL;
        // GET: /Service/
        public ServiceController()
        {
            uow = new SqlUnitOfWork();
            crfBll = new CrfBLL(uow);
            ctfBLL = new CtfBLL(uow);
            csfBll = new CsfBLL(uow);
            tempBll = new TemporaryBLL(uow);
            fleetBLL = new FleetBLL(uow);
            settingBLL = new SettingBLL(uow);
            vehicleSpectBLL = new VehicleSpectBLL(uow);
            ctfExtendBLL = new CtfExtendBLL(uow);
            ccfBLL = new CcfBLL(uow);
        }

        public ActionResult CompleteTransaction()
        {
           

            //CRF Complete
            var errorMessage = crfBll.CompleteAllDocument();
            
            //CTF Complete
            ctfBLL.CheckCtfInProgress();

            //CSF Complete
            csfBll.CheckCsfInProgress();

            //Temporary Complete
            tempBll.CheckTempInProgress();

            ccfBLL.NotifEmail();

            return View();
        }

        public ActionResult BatchEmail()
        {
            var Scheduler = ConfigurationManager.AppSettings["Scheduler"];
            var ListScheduler = Scheduler.Split(',').ToList();
            var GetHour = DateTime.Now.Hour;
            var OnScheduler = ListScheduler.Contains(GetHour.ToString());
            if(OnScheduler)
            {
                GetListCtfInProgress();
                GetListCrfInProgress();
                GetListCsfInProgress();
                GetListTempInProgress();
            }
            return View();
        }

        #region  ------- Batch Email Vendor CTF--------
        public void GetListCtfInProgress()
        {
            try
            {
                var ListCtf = ctfBLL.GetCtf().Where(x => (x.DocumentStatus == Enums.DocumentStatus.InProgress || x.DocumentStatus == Enums.DocumentStatus.Extended) && x.DateSendVendor == null).ToList();
                var ListCtfDto = new List<TraCtfDto>();
                var Vendor = new List<String>();
                bool IsSend = false;
                foreach (var CtfData in ListCtf)
                {
                    var vehicle = fleetBLL.GetFleet().Where(x => x.IsActive && x.PoliceNumber == CtfData.PoliceNumber && x.EmployeeID == CtfData.EmployeeId).FirstOrDefault();
                    if (vehicle != null)
                    {
                        CtfData.Vendor = (vehicle.VendorName == null ? "" : vehicle.VendorName.ToUpper());
                        CtfData.ChasisNumber = vehicle.ChasisNumber;
                        CtfData.EngineNumber = vehicle.EngineNumber;
                        CtfData.Manufacture = vehicle.Manufacturer;
                        CtfData.Models = vehicle.Models;
                        CtfData.Series = vehicle.Series;
                        CtfData.Basetown = vehicle.City;
                    }
                    ListCtfDto.Add(CtfData);
                }

                Vendor = ListCtfDto.Where(x => !string.IsNullOrEmpty(x.Vendor)).Select(x => x.Vendor).Distinct().ToList();

                foreach (var VendorItem in Vendor)
                {

                    var reListCtfDto = ListCtfDto.Where(x => (x.Vendor == null ? "" : x.Vendor.ToUpper()) == VendorItem).ToList();

                    var WtcListCtf = reListCtfDto.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "WTC").ToList();

                    var BenefitListCtf = reListCtfDto.Where(x => (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == "BENEFIT").ToList();

                    string AttacthmentWtc = null;
                    string AttacthmentBenefit = null;

                    if (WtcListCtf.Count > 0)
                    {
                        AttacthmentWtc = CreateExcelForVendorCTF(WtcListCtf, "WTC");
                    }

                    if (BenefitListCtf.Count > 0)
                    {
                        AttacthmentBenefit = CreateExcelForVendorCTF(BenefitListCtf, "BENEFIT");
                    }
                    reListCtfDto = reListCtfDto.OrderBy(x => x.VehicleType).ToList();
                    IsSend = ctfBLL.BatchEmailCtf(reListCtfDto, VendorItem, AttacthmentWtc, AttacthmentBenefit);

                    if (IsSend)
                    {
                        foreach (var Ctf in reListCtfDto)
                        {
                            Ctf.DateSendVendor = DateTime.Now;

                            var login = new Login();
                            login.USER_ID = "SYSTEM";
                            ctfBLL.Save(Ctf, login);
                        }
                    }

                }
            }
            catch (Exception exp)
            {
                ctfBLL.SendEmailForErrorBatch(exp.Message);
            }
          
        }
        private string CreateExcelForVendorCTF(List<TraCtfDto> CtfDto, string VehicleType)
        {

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(2, 2, "Detail Vehicle");
            slDocument.MergeWorksheetCells(2, 2, 2, 15);

            slDocument.SetCellValue(2, 16, "Detail Withdrawal");
            slDocument.MergeWorksheetCells(2, 16, 2, 20);

            slDocument.SetCellValue(2, 21, "Detail Extend");
            slDocument.MergeWorksheetCells(2, 21, 2, 24);


            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Font.FontSize = 11;
            slDocument.SetCellStyle(2, 2, 2, 24, valueStyle);

            //create header
            slDocument = CreateHeaderExcelForVendorCTF(slDocument);

            //create data
            slDocument = CreateDataExcelForVendorCTF(slDocument, CtfDto);

            var fileName = "Attachment_CTF_" + VehicleType + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelForVendorCTF(SLDocument slDocument)
        {
            int iRow = 3;

            slDocument.SetCellValue(iRow, 2, "Request Number");
            slDocument.SetCellValue(iRow, 3, "Employee Name");
            slDocument.SetCellValue(iRow, 4, "Vendor");
            slDocument.SetCellValue(iRow, 5, "Police Number");
            slDocument.SetCellValue(iRow, 6, "Chasis Number");
            slDocument.SetCellValue(iRow, 7, "Engine Number");
            slDocument.SetCellValue(iRow, 8, "Manufacture");
            slDocument.SetCellValue(iRow, 9, "Model");
            slDocument.SetCellValue(iRow, 10, "Series");
            slDocument.SetCellValue(iRow, 11, "VehicleUsage");
            slDocument.SetCellValue(iRow, 12, "Basetown");
            slDocument.SetCellValue(iRow, 13, "Transfer to CFM Idle");
            slDocument.SetCellValue(iRow, 14, "Contract End Date");
            slDocument.SetCellValue(iRow, 15, "Termination Date");
            slDocument.SetCellValue(iRow, 16, "PIC");
            slDocument.SetCellValue(iRow, 17, "Date & Time");
            slDocument.SetCellValue(iRow, 18, "Phone Number");
            slDocument.SetCellValue(iRow, 19, "City");
            slDocument.SetCellValue(iRow, 20, "Address");
            slDocument.SetCellValue(iRow, 21, "New PO Number");
            slDocument.SetCellValue(iRow, 22, "New PO Line");
            slDocument.SetCellValue(iRow, 23, "New Contract End Date");
            slDocument.SetCellValue(iRow, 24, "New Police Number");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(iRow, 2, iRow, 24, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelForVendorCTF(SLDocument slDocument, List<TraCtfDto> ctfData)
        {
            int iRow = 4; //starting row data
            var extendList = ctfExtendBLL.GetCtfExtend();
            foreach (var data in ctfData)
            {
                var GetExtendCtf = extendList.Where(x => x.TraCtfId == data.TraCtfId).FirstOrDefault();

                slDocument.SetCellValue(iRow, 2, data.DocumentNumber);
                slDocument.SetCellValue(iRow, 3, data.EmployeeName);
                slDocument.SetCellValue(iRow, 4, data.Vendor);
                slDocument.SetCellValue(iRow, 5, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 6, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 7, data.EngineNumber);
                slDocument.SetCellValue(iRow, 8, data.Manufacture);
                slDocument.SetCellValue(iRow, 9, data.Models);
                slDocument.SetCellValue(iRow, 10, data.Series);
                slDocument.SetCellValue(iRow, 11, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 12, data.Basetown);
                slDocument.SetCellValue(iRow, 13, data.IsTransferToIdle == true ? "Yes" : "No");
                if (data.EndRendDate.HasValue) slDocument.SetCellValue(iRow, 14, data.EndRendDate.Value.ToOADate());
                if (data.EffectiveDate.HasValue) slDocument.SetCellValue(iRow, 15, data.EffectiveDate.Value.ToOADate());
                slDocument.SetCellValue(iRow, 16, data.WithdPic);
                if (data.WithdDate.HasValue) slDocument.SetCellValue(iRow, 17, data.WithdDate.Value.ToOADate());
                slDocument.SetCellValue(iRow, 18, data.WithdPhone);
                slDocument.SetCellValue(iRow, 19, data.WithdCity);
                slDocument.SetCellValue(iRow, 20, data.WithdAddress);
                
                slDocument.SetCellValue(iRow, 21, GetExtendCtf == null ? "" : GetExtendCtf.ExtendPoNumber);
                slDocument.SetCellValue(iRow, 22, GetExtendCtf == null ? "" : GetExtendCtf.ExtedPoLine);
                if ((GetExtendCtf == null ? false : GetExtendCtf.NewProposedDate.HasValue) == true) slDocument.SetCellValue(iRow, 23, GetExtendCtf.NewProposedDate.Value.ToOADate());
                slDocument.SetCellValue(iRow, 24, GetExtendCtf == null ? "" : GetExtendCtf.ExtendPoliceNumber);

                SLStyle dateStyle = slDocument.CreateStyle();
                dateStyle.FormatCode = "dd-MMM-yyyy";

                slDocument.SetCellStyle(iRow, 14, iRow, 14, dateStyle);
                slDocument.SetCellStyle(iRow, 15, iRow, 15, dateStyle);
                slDocument.SetCellStyle(iRow, 23, iRow, 23, dateStyle);

                dateStyle = slDocument.CreateStyle();
                dateStyle.FormatCode = "dd-MMM-yyyy HH:mm";
                slDocument.SetCellStyle(iRow, 17, iRow, 17, dateStyle);
                SLStyle valueStyle = slDocument.CreateStyle();
                valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

                slDocument.AutoFitColumn(2, 24);
                slDocument.SetCellStyle(iRow, 2, iRow, 24, valueStyle);

                iRow++;

            }


            //create style
            return slDocument;
        }

        #endregion
        
        #region  ------- Batch Email Vendor CRF--------
        public void GetListCrfInProgress()
        {
            try
            {
                var ListCrf = crfBll.GetList().Where(x => x.DOCUMENT_STATUS == (int)Enums.DocumentStatus.InProgress && x.DATE_SEND_VENDOR == null).ToList();
                var ListCrfDto = new List<TraCrfDto>();
                var Vendor = new List<String>();
                bool IsSend = false;
                foreach (var CrfData in ListCrf)
                {
                    var vehicle = fleetBLL.GetFleet().Where(x => x.IsActive && x.PoliceNumber == CrfData.POLICE_NUMBER && x.EmployeeID == CrfData.EMPLOYEE_ID).FirstOrDefault();
                    if (vehicle != null)
                    {
                        CrfData.CHASIS_NUMBER = vehicle.ChasisNumber;
                        CrfData.ENGINE_NUMBER = vehicle.EngineNumber;
                    }
                }

                Vendor = ListCrf.Where(x => x.VENDOR_NAME != null).Select(x => x.VENDOR_NAME).Distinct().ToList();

                foreach (var VendorItem in Vendor)
                {

                    var reListCrfDto = ListCrf.Where(x => (x.VENDOR_NAME == null ? "" : x.VENDOR_NAME.ToUpper()) == VendorItem).ToList();

                    var WtcListCrf = reListCrfDto.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "WTC").ToList();

                    var BenefitListCrf = reListCrfDto.Where(x => (x.VEHICLE_TYPE == null ? "" : x.VEHICLE_TYPE.ToUpper()) == "BENEFIT").ToList();

                    string AttacthmentWtc = null;
                    string AttacthmentBenefit = null;

                    if (WtcListCrf.Count > 0)
                    {
                        AttacthmentWtc = CreateExcelForVendorCRF(WtcListCrf, "WTC");
                    }

                    if (BenefitListCrf.Count > 0)
                    {
                        AttacthmentBenefit = CreateExcelForVendorCRF(BenefitListCrf, "BENEFIT");
                    }

                    reListCrfDto = reListCrfDto.OrderBy(x => x.VEHICLE_TYPE).ToList();
                    IsSend = crfBll.BatchEmailCrf(reListCrfDto, VendorItem, AttacthmentWtc, AttacthmentBenefit);

                    if (IsSend)
                    {
                        foreach (var Crf in reListCrfDto)
                        {
                            Crf.DATE_SEND_VENDOR = DateTime.Now;

                            var login = new BusinessObject.Business.Login();
                            login.USER_ID = "SYSTEM";
                            crfBll.SaveCrf(Crf);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                crfBll.SendEmailForErrorBatch(exp.Message);
            }
            
        }
        private string CreateExcelForVendorCRF(List<TraCrfDto> CrfDto, string VehicleType)
        {

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(2, 2, "Detail Vehicle");
            slDocument.MergeWorksheetCells(2, 2, 2, 16);

            slDocument.SetCellValue(2, 17, "Detail Withdrawal");
            slDocument.MergeWorksheetCells(2, 17, 2, 21);

            slDocument.SetCellValue(2, 22, "Detail Deliverable");
            slDocument.MergeWorksheetCells(2, 22, 2, 25);

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Font.FontSize = 11;
            slDocument.SetCellStyle(2, 2, 2, 25, valueStyle);

            //create header
            slDocument = CreateHeaderExcelForVendorCRF(slDocument);

            //create data
            slDocument = CreateDataExcelForVendorCRF(slDocument, CrfDto);

            var fileName = "Attachment_CRF_" + VehicleType + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }
        private SLDocument CreateHeaderExcelForVendorCRF(SLDocument slDocument)
        {
            int iRow = 3;

            slDocument.SetCellValue(iRow, 2, "Request Number");
            slDocument.SetCellValue(iRow, 3, "Employee Name");
            slDocument.SetCellValue(iRow, 4, "Vendor");
            slDocument.SetCellValue(iRow, 5, "Police Number");
            slDocument.SetCellValue(iRow, 6, "Chasis Number");
            slDocument.SetCellValue(iRow, 7, "Engine Number");
            slDocument.SetCellValue(iRow, 8, "Manufacture");
            slDocument.SetCellValue(iRow, 9, "Model");
            slDocument.SetCellValue(iRow, 10, "Series");
            slDocument.SetCellValue(iRow, 11, "VehicleUsage");
            slDocument.SetCellValue(iRow, 12, "Current Basetown");
            slDocument.SetCellValue(iRow, 13, "New Basetown");
            slDocument.SetCellValue(iRow, 14, "Expected Delivery Date");
            slDocument.SetCellValue(iRow, 15, "Change Unit");
            slDocument.SetCellValue(iRow, 16, "Change Police Number");
            slDocument.SetCellValue(iRow, 17, "PIC Name");
            slDocument.SetCellValue(iRow, 18, "Date & Time");
            slDocument.SetCellValue(iRow, 19, "Phone Number");
            slDocument.SetCellValue(iRow, 20, "City");
            slDocument.SetCellValue(iRow, 21, "Address");
            slDocument.SetCellValue(iRow, 22, "PIC Name");
            slDocument.SetCellValue(iRow, 23, "Phone Number");
            slDocument.SetCellValue(iRow, 24, "City");
            slDocument.SetCellValue(iRow, 25, "Address");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.SetCellStyle(iRow, 2, iRow, 25, headerStyle);

            return slDocument;

        }
        private SLDocument CreateDataExcelForVendorCRF(SLDocument slDocument, List<TraCrfDto> CrfDto)
        {
            int iRow = 4; //starting row data
            foreach (var data in CrfDto)
            {
                slDocument.SetCellValue(iRow, 2, data.DOCUMENT_NUMBER);
                slDocument.SetCellValue(iRow, 3, data.EMPLOYEE_NAME);
                slDocument.SetCellValue(iRow, 4, data.VENDOR_NAME);
                slDocument.SetCellValue(iRow, 5, data.POLICE_NUMBER);
                slDocument.SetCellValue(iRow, 6, data.CHASIS_NUMBER);
                slDocument.SetCellValue(iRow, 7, data.ENGINE_NUMBER);
                slDocument.SetCellValue(iRow, 8, data.MANUFACTURER);
                slDocument.SetCellValue(iRow, 9, data.MODEL);
                slDocument.SetCellValue(iRow, 10, data.SERIES);
                slDocument.SetCellValue(iRow, 11, data.VEHICLE_USAGE);
                slDocument.SetCellValue(iRow, 12, data.LOCATION_OFFICE);
                slDocument.SetCellValue(iRow, 13, data.LOCATION_OFFICE_NEW);
                if (data.EXPECTED_DATE.HasValue) slDocument.SetCellValue(iRow, 14, data.EXPECTED_DATE.Value.ToOADate());
                slDocument.SetCellValue(iRow, 15, ((data.RelocationType == null ? "" : data.RelocationType.ToUpper()) == "CHANGE UNIT" ? "Yes" : "No"));
                slDocument.SetCellValue(iRow, 16, data.CHANGE_POLICE_NUMBER == true ? "Yes" : "No");
                slDocument.SetCellValue(iRow, 17, data.WITHD_PIC);
                if (data.WITHD_DATETIME.HasValue) slDocument.SetCellValue(iRow, 18, data.WITHD_DATETIME.Value.ToOADate());
                slDocument.SetCellValue(iRow, 19, data.WITHD_PHONE);
                slDocument.SetCellValue(iRow, 20, data.WITHD_CITY);
                slDocument.SetCellValue(iRow, 21, data.WITHD_ADDRESS);
                slDocument.SetCellValue(iRow, 22, data.DELIV_PIC);
                slDocument.SetCellValue(iRow, 23, data.DELIV_PHONE);
                slDocument.SetCellValue(iRow, 24, data.DELIV_CITY);
                slDocument.SetCellValue(iRow, 25, data.DELIV_ADDRESS);

                SLStyle dateStyle = slDocument.CreateStyle();
                dateStyle.FormatCode = "dd-MMM-yyyy";
                slDocument.SetCellStyle(iRow, 14, iRow, 14, dateStyle);

                dateStyle = slDocument.CreateStyle();
                dateStyle.FormatCode = "dd-MMM-yyyy HH:mm";
                slDocument.SetCellStyle(iRow, 18, iRow, 18, dateStyle);

                SLStyle valueStyle = slDocument.CreateStyle();
                valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

                slDocument.AutoFitColumn(2, 25);
                slDocument.SetCellStyle(iRow, 2, iRow, 25, valueStyle);

                iRow++;

            }


            //create style
            return slDocument;
        }

        #endregion

        #region ------- Batch Email Vendor CSF--------

        public void GetListCsfInProgress()
        {
            try
            {
                var ListCsf = csfBll.GetList().Where(x => x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress &&
                                                        !x.DATE_SEND_VENDOR.HasValue && x.VENDOR_NAME != null).ToList();

                var wtcType = settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "WTC").FirstOrDefault().MstSettingId.ToString();
                var benefitType = settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId.ToString();

                var Vendor = new List<String>();
                bool IsSend = false;

                Vendor = ListCsf.Select(x => x.VENDOR_NAME).Distinct().ToList();

                foreach (var VendorItem in Vendor)
                {
                    var reListCsfDto = ListCsf.Where(x => x.VENDOR_NAME.ToUpper() == VendorItem.ToUpper()).ToList();

                    var WtcListCsf = reListCsfDto.Where(x => x.VEHICLE_TYPE == wtcType).ToList();

                    var BenefitListCsf = reListCsfDto.Where(x => x.VEHICLE_TYPE == benefitType).ToList();

                    string AttacthmentWtc = null;
                    string AttacthmentBenefit = null;

                    if (WtcListCsf.Count > 0)
                    {
                        AttacthmentWtc = CreateAttachmentForVendorCSF(WtcListCsf, "WTC");
                    }

                    if (BenefitListCsf.Count > 0)
                    {
                        AttacthmentBenefit = CreateAttachmentForVendorCSF(BenefitListCsf, "BENEFIT");
                    }

                    reListCsfDto = reListCsfDto.OrderBy(x => x.VEHICLE_TYPE).ToList();
                    IsSend = csfBll.BatchEmailCsf(reListCsfDto, VendorItem, AttacthmentWtc, AttacthmentBenefit);

                    if (IsSend)
                    {
                        foreach (var Csf in reListCsfDto)
                        {
                            Csf.DATE_SEND_VENDOR = DateTime.Now;

                            var login = new Login();
                            login.USER_ID = "SYSTEM";
                            csfBll.Save(Csf, login);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                csfBll.SendEmailForErrorBatch(exception.Message);
            }
        }

        private string CreateAttachmentForVendorCSF(List<TraCsfDto> listData, string vehicleType)
        {
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
            slDocument = CreateHeaderAttachmentForVendorCSF(slDocument);

            //create data
            slDocument = CreateDataAttachmentForVendorCSF(slDocument, listData);

            var fileName = "Attachment_CSF_" + vehicleType + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderAttachmentForVendorCSF(SLDocument slDocument)
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

        private SLDocument CreateDataAttachmentForVendorCSF(SLDocument slDocument, List<TraCsfDto> listData)
        {
            int iRow = 4; //starting row data

            var vSpecListData = vehicleSpectBLL.GetVehicleSpect().Where(x => x.Manufacturer != null
                                                                        && x.Models != null
                                                                        && x.Series != null
                                                                        && x.BodyType != null
                                                                        && x.IsActive).ToList();

            foreach (var csfData in listData)
            {
                var vSpecList = vSpecListData.Where(x => x.Year == csfData.CREATED_DATE.Year
                                                        && x.Manufacturer.ToUpper() == csfData.MANUFACTURER.ToUpper()
                                                        && x.Models.ToUpper() == csfData.MODEL.ToUpper()
                                                        && x.Series.ToUpper() == csfData.SERIES.ToUpper()
                                                        && x.BodyType.ToUpper() == csfData.BODY_TYPE.ToUpper()).FirstOrDefault();

                var transmissionData = vSpecList == null ? string.Empty : vSpecList.Transmission;

                var policeNumberCfmIdle = string.Empty;
                var chasCfmIdle = string.Empty;
                var engCfmIdle = string.Empty;
                if (csfData.CFM_IDLE_ID != null)
                {
                    var cfmData = fleetBLL.GetFleetById((int)csfData.CFM_IDLE_ID);
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

                iRow++;
            }

            return slDocument;
        }

        #endregion

        #region ------- Batch Email Vendor TEMPORARY--------
        public void GetListTempInProgress()
        {
            try
            {
                var ListTemp = tempBll.GetList().Where(x => x.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress &&
                                                        !x.DATE_SEND_VENDOR.HasValue && x.VENDOR_NAME != null).ToList();

                var wtcType = settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "WTC").FirstOrDefault().MstSettingId.ToString();
                var benefitType = settingBLL.GetSetting().Where(x => x.SettingGroup == "VEHICLE_TYPE" && x.SettingName == "BENEFIT").FirstOrDefault().MstSettingId.ToString();

                var Vendor = new List<String>();
                bool IsSend = false;

                Vendor = ListTemp.Select(x => x.VENDOR_NAME).Distinct().ToList();

                foreach (var VendorItem in Vendor)
                {
                    var reListTempDto = ListTemp.Where(x => x.VENDOR_NAME.ToUpper() == VendorItem.ToUpper()).ToList();

                    var WtcListTemp = reListTempDto.Where(x => x.VEHICLE_TYPE == wtcType).ToList();

                    var BenefitListTemp = reListTempDto.Where(x => x.VEHICLE_TYPE == benefitType).ToList();

                    string AttacthmentWtc = null;
                    string AttacthmentBenefit = null;

                    if (WtcListTemp.Count > 0)
                    {
                        AttacthmentWtc = CreateAttachmentForVendor(WtcListTemp, "WTC");
                    }

                    if (BenefitListTemp.Count > 0)
                    {
                        AttacthmentBenefit = CreateAttachmentForVendor(BenefitListTemp, "BENEFIT");
                    }

                    reListTempDto = reListTempDto.OrderBy(x => x.VEHICLE_TYPE).ToList();
                    IsSend = tempBll.BatchEmailTemp(reListTempDto, VendorItem, AttacthmentWtc, AttacthmentBenefit);

                    if (IsSend)
                    {
                        foreach (var Temp in reListTempDto)
                        {
                            Temp.DATE_SEND_VENDOR = DateTime.Now;

                            var login = new Login();
                            login.USER_ID = "SYSTEM";
                            tempBll.Save(Temp, login);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                tempBll.SendEmailForErrorBatch(exception.Message);
            }
        }

        private string CreateAttachmentForVendor(List<TemporaryDto> listData, string vehicleType)
        {
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
            slDocument = CreateHeaderAttachmentForVendor(slDocument);

            //create data
            slDocument = CreateDataAttachmentForVendor(slDocument, listData);

            var fileName = "Attachment_TEMP_" + vehicleType + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderAttachmentForVendor(SLDocument slDocument)
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

        private SLDocument CreateDataAttachmentForVendor(SLDocument slDocument, List<TemporaryDto> listData)
        {
            int iRow = 4; //starting row data
            
            var vSpecListData = vehicleSpectBLL.GetVehicleSpect().Where(x => x.Manufacturer != null
                                                                        && x.Models != null
                                                                        && x.Series != null
                                                                        && x.BodyType != null
                                                                        && x.IsActive).ToList();

            foreach (var tempData in listData)
            {
                var vSpecList = vSpecListData.Where(x => x.Year == tempData.CREATED_DATE.Year
                                                        && x.Manufacturer.ToUpper() == tempData.MANUFACTURER.ToUpper()
                                                        && x.Models.ToUpper() == tempData.MODEL.ToUpper()
                                                        && x.Series.ToUpper() == tempData.SERIES.ToUpper()
                                                        && x.BodyType.ToUpper() == tempData.BODY_TYPE.ToUpper()).FirstOrDefault();

                var transmissionData = vSpecList == null ? string.Empty : vSpecList.Transmission;

                var policeNumberCfmIdle = string.Empty;
                var chasCfmIdle = string.Empty;
                var engCfmIdle = string.Empty;
                if (tempData.CFM_IDLE_ID != null)
                {
                    var cfmData = fleetBLL.GetFleetById((int)tempData.CFM_IDLE_ID);
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

                iRow++;
            }

            return slDocument;
        }

        #endregion
    }
}
