using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Utils;
using FMS.Website.Models;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using SpreadsheetLight;

namespace FMS.Website.Controllers
{
    public class RptFuelController : BaseController
    {
        #region --------- Field and Constructor --------------

        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;
        private IRptFuelBLL _rptFuelBLL;
        private ISettingBLL _settingBLL;
        private IFleetBLL _fleetBLL;
        private IEmployeeBLL _employeeBLL;
        private ILocationMappingBLL _locationMappingBLL;

        public RptFuelController(IPageBLL pageBll, IRptFuelBLL rptFuelBLL, ISettingBLL SettingBLL, IFleetBLL fleetBLL, IEmployeeBLL employeeBLL, ILocationMappingBLL locationMappingBLL) 
            : base(pageBll, Core.Enums.MenuList.RptFuel)
        {
            _pageBLL = pageBll;
            _rptFuelBLL = rptFuelBLL;
            _settingBLL = SettingBLL;
            _fleetBLL = fleetBLL;
            _employeeBLL = employeeBLL;
            _locationMappingBLL = locationMappingBLL;
            _mainMenu = Enums.MenuList.RptExecutiveSummary;
        }

        #endregion

        public ActionResult Index()
        {
            try
            {
                var model = new RptFuelModel();
                var input = Mapper.Map<RptFuelByParamInput>(model.SearchView);
                var data = _rptFuelBLL.GetRptFuel(input);
                model.MainMenu = _mainMenu;
                model.TitleForm = "Fuel Report";
                model.TitleExport = "ExportFuel";
                model.CurrentLogin = CurrentUser;
                var settingData = _settingBLL.GetSetting();
                var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
                var costCenter = _fleetBLL.GetFleet().Select(x=> new {x.CostCenter}).ToList().Distinct().OrderBy(x => x.CostCenter);
                var function = _employeeBLL.GetEmployee().Select(x=> new {x.DIRECTORATE}).ToList().Distinct().OrderBy(x => x.DIRECTORATE);
                var locationMapping = _locationMappingBLL.GetLocationMapping().Select(x=> new {x.Region}).ToList().Distinct().OrderBy(x => x.Region);

                model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
                model.SearchView.CostCenterList = new SelectList(costCenter, "CostCenter", "CostCenter");
                model.SearchView.FunctionList = new SelectList(function, "DIRECTORATE", "DIRECTORATE");
                model.SearchView.RegionalList = new SelectList(locationMapping, "Region", "Region");
                model.RptFuelItem = Mapper.Map<List<RptFuelItem>>(data);

                var dataFuel = _rptFuelBLL.GetRptFuel(input);

                foreach (var item in model.RptFuelItem)
                {
                    if (input.MonthFrom == 1)
                    {
                        input.MonthFrom = 12;
                        input.YearFrom = input.YearFrom - 1;
                    }
                    else
                    {
                        input.MonthFrom = input.MonthFrom - 1;
                    }

                    if (item.Odometer != 0)
                    {
                        var data_temp = dataFuel.Where(x => x.PoliceNumber == item.PoliceNumber).Select(x => x.Odometer).FirstOrDefault();
                        if (data_temp == 0)
                        {
                            item.Usage = data_temp;
                            item.kmlt = item.Usage;
                        }
                        else
                        {
                            item.Usage = item.Odometer - data_temp;
                            item.kmlt = Math.Round(item.Usage / item.Liter, 2);
                        }
                    }
                }

                return View(model);
            }
            catch (Exception exception)
            {
                var model = new RptFuelModel();
                var input = Mapper.Map<RptFuelByParamInput>(model.SearchView);
                var data = _rptFuelBLL.GetRptFuel(input);
                model.MainMenu = _mainMenu;
                model.TitleForm = "Fuel Report";
                model.TitleExport = "ExportFuel";
                model.CurrentLogin = CurrentUser;
                var settingData = _settingBLL.GetSetting();
                var listVehType = settingData.Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType) && x.IsActive).Select(x => new { x.SettingValue }).ToList();
                var costCenter = _fleetBLL.GetFleet().Select(x => new { x.CostCenter }).ToList().Distinct().OrderBy(x => x.CostCenter);
                var function = _employeeBLL.GetEmployee().Select(x => new { x.DIRECTORATE }).ToList().Distinct().OrderBy(x => x.DIRECTORATE);
                var locationMapping = _locationMappingBLL.GetLocationMapping().Select(x => new { x.Region }).ToList().Distinct().OrderBy(x => x.Region);

                model.SearchView.VehicleTypeList = new SelectList(listVehType, "SettingValue", "SettingValue");
                model.SearchView.CostCenterList = new SelectList(costCenter, "CostCenter", "CostCenter");
                model.SearchView.FunctionList = new SelectList(function, "DIRECTORATE", "DIRECTORATE");
                model.SearchView.RegionalList = new SelectList(locationMapping, "Region", "Region");
                model.RptFuelItem = Mapper.Map<List<RptFuelItem>>(data);
                
                model.ErrorMessage = exception.Message;
                return View(model);
            }
        }

        [HttpPost]
        public PartialViewResult FilterFuel(RptFuelModel model)
        {
            model.RptFuelItem = GetFuelData(model.SearchView);
            var input = Mapper.Map<RptFuelByParamInput>(model.SearchView);

            var dataFuel = _rptFuelBLL.GetRptFuel(input);

            foreach (var item in model.RptFuelItem)
            {
                if (input.MonthFrom == 1)
                {
                    input.MonthFrom = 12;
                    input.YearFrom = input.YearFrom - 1;
                }
                else
                {
                    input.MonthFrom = input.MonthFrom - 1;
                }

                if (item.Odometer != 0)
                {
                    var data_temp = dataFuel.Where(x => x.PoliceNumber == item.PoliceNumber).Select(x => x.Odometer).FirstOrDefault();
                    if (data_temp == 0)
                    {
                        item.Usage = data_temp;
                        item.kmlt = item.Usage;
                    }
                    else
                    {
                        item.Usage = item.Odometer - data_temp;
                        item.kmlt = Math.Round(item.Usage / item.Liter, 2);
                    }
                }
            }
            return PartialView("_ListFuel", model);
        }

        private List<RptFuelItem> GetFuelData(RptFuelSearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _rptFuelBLL.GetRptFuel(new RptFuelByParamInput());
                return Mapper.Map<List<RptFuelItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<RptFuelByParamInput>(filter);

            var dbData = _rptFuelBLL.GetRptFuel(input);
            return Mapper.Map<List<RptFuelItem>>(dbData);
        }

        #region --------- Export --------------

        public void ExportFuel(RptFuelModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<RptFuelByParamInput>(model.SearchViewExport);
            pathFile = CreateXlsRptFuel(input);

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

        private string CreateXlsRptFuel(RptFuelByParamInput input)
        {
            //get data
            List<RptFuelDto> data = _rptFuelBLL.GetRptFuel(input);
            var listData = Mapper.Map<List<RptFuelItem>>(data);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Fuel Report Data");
            slDocument.MergeWorksheetCells(1, 1, 1, 19);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelDashboard(slDocument);

            //create data
            slDocument = CreateDataExcelDashboard(slDocument, listData, input);

            var fileName = "RptFuel" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelDashboard(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Liter");
            slDocument.SetCellValue(iRow, 3, "Odometer");
            slDocument.SetCellValue(iRow, 4, "Usage");
            slDocument.SetCellValue(iRow, 5, "KM/Lt");
            slDocument.SetCellValue(iRow, 6, "Cost");
            slDocument.SetCellValue(iRow, 7, "Full Type");
            slDocument.SetCellValue(iRow, 8, "Cost Center");
            slDocument.SetCellValue(iRow, 9, "Function");
            slDocument.SetCellValue(iRow, 10, "Manufacturer");
            slDocument.SetCellValue(iRow, 11, "Models");
            slDocument.SetCellValue(iRow, 12, "Series");
            slDocument.SetCellValue(iRow, 13, "Body Type");
            slDocument.SetCellValue(iRow, 14, "Vehicle Type");
            slDocument.SetCellValue(iRow, 15, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 16, "Location");
            slDocument.SetCellValue(iRow, 17, "Regional");
            slDocument.SetCellValue(iRow, 18, "Report Month");
            slDocument.SetCellValue(iRow, 19, "Report Year");

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

        private SLDocument CreateDataExcelDashboard(SLDocument slDocument, List<RptFuelItem> listData, RptFuelByParamInput input)
        {
            int iRow = 3; //starting row data
            var dataFuel = _rptFuelBLL.GetRptFuel(input);
            foreach (var data in listData)
            {
                if (input.MonthFrom == 1)
                {
                    input.MonthFrom = 12;
                    input.YearFrom = input.YearFrom - 1;
                }
                else
                {
                    input.MonthFrom = input.MonthFrom - 1;
                }

                if (data.Odometer != 0)
                {
                    var data_temp = dataFuel.Where(x => x.PoliceNumber == data.PoliceNumber).Select(x => x.Odometer).FirstOrDefault();
                    if (data_temp == 0)
                    {
                        data.Usage = data_temp;
                        data.kmlt = data.Usage;
                    }
                    else
                    {
                        data.Usage = data.Odometer - data_temp;
                        data.kmlt = Math.Round(data.Usage / data.Liter, 2);
                    }
                }
                slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 2, data.Liter);
                slDocument.SetCellValue(iRow, 3, data.Odometer);
                slDocument.SetCellValue(iRow, 4, data.Usage);
                slDocument.SetCellValue(iRow, 5, data.kmlt);
                slDocument.SetCellValue(iRow, 6, data.Cost);
                slDocument.SetCellValue(iRow, 7, data.FuelType);
                slDocument.SetCellValue(iRow, 8, data.CostCenter);
                slDocument.SetCellValue(iRow, 9, data.Function);
                slDocument.SetCellValue(iRow, 10, data.Manufacturer);
                slDocument.SetCellValue(iRow, 11, data.Models);
                slDocument.SetCellValue(iRow, 12, data.Series);
                slDocument.SetCellValue(iRow, 13, data.BodyType);
                slDocument.SetCellValue(iRow, 14, data.VehicleType);
                slDocument.SetCellValue(iRow, 15, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 16, data.Location);
                slDocument.SetCellValue(iRow, 17, data.Regional);
                slDocument.SetCellValue(iRow, 18, data.Month);
                slDocument.SetCellValue(iRow, 19, data.ReportYear);

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 19);
            slDocument.SetCellStyle(3, 1, iRow - 1,19, valueStyle);

            return slDocument;
        }

        #endregion

    }
}
