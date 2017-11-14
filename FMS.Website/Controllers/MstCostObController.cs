
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using FMS.Website.Models;
using FMS.Contract.BLL;
using FMS.Core;
using AutoMapper;
using FMS.BusinessObject.Dto;
using System.Web;
using System.IO;
using ExcelDataReader;
using System.Data;
using FMS.Website.Utility;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class MstCostObController : BaseController
    {
        private ICostObBLL _costObBLL;
        private IPageBLL _pageBLL;
        private IRemarkBLL _remarkBLL;
        private ILocationMappingBLL _locationMappingBLL;
        private IVehicleSpectBLL _vehicleSpectBll;
        private Enums.MenuList _mainMenu;

        public MstCostObController(IPageBLL PageBll, ICostObBLL CostObBLL,IRemarkBLL RemarkBLL, ILocationMappingBLL LocationMappingBLL, IVehicleSpectBLL VehicleSpectBll)
            : base(PageBll, Enums.MenuList.MasterCostOB)
        {
            _costObBLL = CostObBLL;
            _remarkBLL = RemarkBLL;
            _locationMappingBLL = LocationMappingBLL;
            _pageBLL = PageBll;
            _vehicleSpectBll = VehicleSpectBll;
            _mainMenu = Enums.MenuList.MasterData;
  
        }

        public CostObItem InitialModel(CostObItem model)
        {
            var RemarkList = _remarkBLL.GetRemark().Where(x => x.IsActive == true).ToList();
            model.RemarkList = new SelectList(RemarkList, "Remark", "Remark");
            var ZoneList = _locationMappingBLL.GetLocationMapping().Where(x => x.IsActive == true).ToList();
            model.ZoneList = new SelectList(ZoneList, "ZonePriceList", "ZonePriceList");
            var ModelList = _vehicleSpectBll.GetVehicleSpect().Where(x => x.IsActive == true).ToList();
            model.ModelList = new SelectList(ModelList, "ModelList", "ModelList");

            return model;
        }
        public ActionResult Index()
        {
            var data = _costObBLL.GetCostOb();
            var model = new CostObModel();
            model.Details = Mapper.Map<List<CostObItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

       
        public ActionResult Create()
        {
            var model = new CostObItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            return View(model);
        }

        
        [HttpPost]
        public ActionResult Create(CostObItem item)
        {
            string year = Request.Params["Year"];
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<CostObDto>(item);
                data.CreatedBy = CurrentUser.USER_ID;
                data.CreatedDate = DateTime.Now;
                data.IsActive = true;
                try
                {
                    _costObBLL.Save(data);
                }
                catch (Exception ex)
                {

                    item.ErrorMessage = ex.Message;
                    item.MainMenu = _mainMenu;
                    item.CurrentLogin = CurrentUser;
                    item = InitialModel(item);
                    return View(item);
                }

            }
            return RedirectToAction("Index", "MstCostOb");
        }

        public ActionResult Edit(int? MstCostObid)
        {
            if (!MstCostObid.HasValue)
            {
                return HttpNotFound();
            }

            var data = _costObBLL.GetByID(MstCostObid.Value);
            var model = new CostObItem();
            model = Mapper.Map<CostObItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = InitialModel(model);
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterCostOB, MstCostObid.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CostObItem item)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<CostObDto>(item);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy =CurrentUser.USER_ID;

                try
                {
                    _costObBLL.Save(data,CurrentUser);
                }
                catch (Exception ex)
                {
                    item.ErrorMessage = ex.Message;
                    item.MainMenu = _mainMenu;
                    item.CurrentLogin = CurrentUser;
                    item = InitialModel(item);
                }
            }
            return RedirectToAction("Index", "MstCostOb");
        }

        public ActionResult Upload()
        {
            var model = new CostObModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(CostObModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (CostObItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USER_ID;
                        data.IsActive = true;

                        if (data.ErrorMessage == "" | data.ErrorMessage == null)
                        {
                            var dto = Mapper.Map<CostObDto>(data);

                            _costObBLL.Save(dto);
                        }

                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstCostOb");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<CostObItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new CostObItem();
                    try
                    {

                        item.Year = Int32.Parse(dataRow[0].ToString());
                        item.Zone = dataRow[1].ToString();
                        item.Model = dataRow[2].ToString();
                        item.Type = dataRow[3].ToString();
                        item.ObCost = Int32.Parse(dataRow[4].ToString());
                        item.Remark = dataRow[5].ToString();

                        model.Add(item);
                    }
                    catch (Exception ex)
                    {
                        var a = ex.Message;
                    }
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterCostOb()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterCostOb();

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

        private string CreateXlsMasterCostOb()
        {
            //get data
            List<CostObDto> costOb = _costObBLL.GetCostOb();
            var listData = Mapper.Map<List<CostObItem>>(costOb);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Cost OB");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterCostOb(slDocument);

            //create data
            slDocument = CreateDataExcelMasterCostOb(slDocument, listData);

            var fileName = "Master_Data_CostOb" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterCostOb(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Year");
            slDocument.SetCellValue(iRow, 2, "Zone");
            slDocument.SetCellValue(iRow, 3, "Model");
            slDocument.SetCellValue(iRow, 4, "Type");
            slDocument.SetCellValue(iRow, 5, "Cost OB");
            slDocument.SetCellValue(iRow, 6, "Remark");
            slDocument.SetCellValue(iRow, 7, "Created Date");
            slDocument.SetCellValue(iRow, 8, "Created By");
            slDocument.SetCellValue(iRow, 9, "Modified Date");
            slDocument.SetCellValue(iRow, 10, "Modified By");
            slDocument.SetCellValue(iRow, 11, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 11, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterCostOb(SLDocument slDocument, List<CostObItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Year);
                slDocument.SetCellValue(iRow, 2, data.Zone);
                slDocument.SetCellValue(iRow, 3, data.Model);
                slDocument.SetCellValue(iRow, 4, data.Type);
                slDocument.SetCellValue(iRow, 5, data.ObCost);
                slDocument.SetCellValue(iRow, 6, data.Remark);
                slDocument.SetCellValue(iRow, 7, data.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 8, data.CreatedBy);
                slDocument.SetCellValue(iRow, 9, data.ModifiedDate == null ? "": data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 10, data.ModifiedBy);
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 11, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 11, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 12);
            slDocument.SetCellStyle(3, 1, iRow - 1, 11, valueStyle);

            return slDocument;
        }

        #endregion
    }
      

}