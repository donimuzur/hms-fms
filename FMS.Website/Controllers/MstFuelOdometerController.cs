using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Dto;
using FMS.Core;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.Website.Utility;

namespace FMS.Website.Controllers
{
    public class MstFuelOdometerController : BaseController
    {
        private IFuelOdometerBLL _fuelodometerBLL;
        private Enums.MenuList _mainMenu;

        public MstFuelOdometerController(IPageBLL PageBll, IFuelOdometerBLL FuelOdometerBLL) : base(PageBll, Enums.MenuList.MasterFuelOdoMeter)
        {
            _fuelodometerBLL = FuelOdometerBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /MstFuelOdometer/

        public ActionResult Index()
        {
            var data = _fuelodometerBLL.GetFuelOdometer();
            var model = new FuelOdometerModel();
            model.Details = Mapper.Map<List<FuelOdometerItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public ActionResult Detail()
        {
            var data = _fuelodometerBLL.GetFuelOdometer();
            var model = new FuelOdometerModel();
            model.Details = Mapper.Map<List<FuelOdometerItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #region ExportExcel
        public void ExportMasterFuelOdometer()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterFuelOdometer();

            var newFile = new FileInfo(pathFile);

            var fileName = Path.GetFileName(pathFile);

            string attachment = string.Format("attachment; filename={0}", fileName);
            Response.Clear();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(newFile.FullName);
            Response.Flush();
            newFile.Delete();
            Response.End();
        }

        private string CreateXlsMasterFuelOdometer()
        {
            //get data
            List<FuelOdometerDto> vendor = _fuelodometerBLL.GetFuelOdometer();
            var listData = Mapper.Map<List<FuelOdometerItem>>(vendor);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Fuel & Odometer");
            slDocument.MergeWorksheetCells(1, 1, 1, 19);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterFuelOdometer(slDocument);

            //create data
            slDocument = CreateDataExcelMasterFuelOdometer(slDocument, listData);

            var fileName = "Master Data Fuel and Odometer " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterFuelOdometer(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Vehicle Type");
            slDocument.SetCellValue(iRow, 2, "Police Number");
            slDocument.SetCellValue(iRow, 3, "Employee ID");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "ECS RMB Trans ID");
            slDocument.SetCellValue(iRow, 6, "SEQ Number");
            slDocument.SetCellValue(iRow, 7, "Claim Type");
            slDocument.SetCellValue(iRow, 8, "Date Of Cost");
            slDocument.SetCellValue(iRow, 9, "Cost Center");
            slDocument.SetCellValue(iRow, 10, "Fuel Amount");
            slDocument.SetCellValue(iRow, 11, "Last KM");
            slDocument.SetCellValue(iRow, 12, "Cost");
            slDocument.SetCellValue(iRow, 13, "Claim Comment");
            slDocument.SetCellValue(iRow, 14, "Posted Time");
            slDocument.SetCellValue(iRow, 15, "Created By");
            slDocument.SetCellValue(iRow, 16, "Created Date");
            slDocument.SetCellValue(iRow, 17, "Modified By");
            slDocument.SetCellValue(iRow, 18, "Modified Date");
            slDocument.SetCellValue(iRow, 19, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 19, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterFuelOdometer(SLDocument slDocument, List<FuelOdometerItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.VehicleType);
                slDocument.SetCellValue(iRow, 2, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.EcsRmbTransId);
                slDocument.SetCellValue(iRow, 6, data.SeqNumber);
                slDocument.SetCellValue(iRow, 7, data.ClaimType);
                slDocument.SetCellValue(iRow, 8, data.DateOfCost);
                slDocument.SetCellValue(iRow, 9, data.CostCenter);
                slDocument.SetCellValue(iRow, 10, data.FuelAmount);
                slDocument.SetCellValue(iRow, 11, data.LastKM);
                slDocument.SetCellValue(iRow, 12, data.Cost);
                slDocument.SetCellValue(iRow, 13, data.ClaimComment);
                slDocument.SetCellValue(iRow, 14, data.PostedTime.ToString("dd/MM/yyyy"));
                slDocument.SetCellValue(iRow, 15, data.CreatedBy);
                slDocument.SetCellValue(iRow, 16, data.CreatedDate.ToString("dd/MM/yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 17, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 18, data == null ? "" : data.ModifiedDate.Value.ToString("dd/MM/yyyy hh: mm"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 19, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 19, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 8);
            slDocument.SetCellStyle(3, 1, iRow - 1, 19, valueStyle);

            return slDocument;
        }
        #endregion

    }
}
