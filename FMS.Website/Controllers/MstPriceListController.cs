
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
    public class MstPriceListController : BaseController
    {
        private IPriceListBLL _priceListBLL;
        private IPageBLL _pageBLL;
        private Enums.MenuList _mainMenu;

        public MstPriceListController(IPageBLL PageBll, IPriceListBLL PriceListBLL) : base(PageBll, Enums.MenuList.MasterPriceList)
        {
            _priceListBLL = PriceListBLL;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
  
        }
        public ActionResult Index()
        {
            var data = _priceListBLL.GetPriceList();
            var model = new PriceListModel();
            model.Details = Mapper.Map<List<PriceListItem>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

       
        public ActionResult Create()
        {
            var model = new PriceListItem();
            model.MainMenu = _mainMenu;
            return View(model);
        }

        
        [HttpPost]
        public ActionResult Create(PriceListItem item)
        {
            string year = Request.Params["Year"];
            if (ModelState.IsValid)
            {
                var dataexist = _priceListBLL.GetExist(item.Model);
                if (dataexist != null)
                {
                    AddMessageInfo("Data Already Exist", Enums.MessageInfoType.Warning);
                    return View(item);
                }
                else
                {
                    var data = Mapper.Map<PriceListDto>(item);
                    data.CreatedBy = "Hardcode User";
                    data.CreatedDate = DateTime.Today;
                    data.ModifiedDate = null;
                    try
                    {
                        _priceListBLL.Save(data);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                    }

                }
            }
            return RedirectToAction("Index", "MstPriceList");
        }

        public ActionResult Edit(int? MstPriceListid)
        {
            if (!MstPriceListid.HasValue)
            {
                return HttpNotFound();
            }

            var data = _priceListBLL.GetByID(MstPriceListid.Value);
            var model = new PriceListItem();
            model = Mapper.Map<PriceListItem>(data);
            model.MainMenu = _mainMenu;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PriceListItem item)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<PriceListDto>(item);

                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = "Hardcode User";

                try
                {
                    _priceListBLL.Save(data);
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }
            return RedirectToAction("Index", "MstPriceList");
        }

        public ActionResult Upload()
        {
            var model = new PriceListModel();
            model.MainMenu = _mainMenu;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(PriceListModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (PriceListItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = "Hardcode User";
                        data.ModifiedDate = null;
                        data.IsActive = true;

                        var dto = Mapper.Map<PriceListDto>(data);
                        _priceListBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstPriceList");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<PriceListItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new PriceListItem();
                    item.Manufacture = dataRow[0].ToString();
                    item.Model = dataRow[1].ToString();
                    item.Series = dataRow[2].ToString();
                    item.Year = Int32.Parse(dataRow[3].ToString());
                    item.Price = Int32.Parse(dataRow[4].ToString());
                    item.InstallmenHMS = Int32.Parse(dataRow[5].ToString());
                    item.InstallmenEMP = Int32.Parse(dataRow[6].ToString());    
                    item.CreatedBy = "Hardcode User";
                    item.CreatedDate = DateTime.Now;
                    if (dataRow[9].ToString() == "Yes" | dataRow[9].ToString() == "YES" | dataRow[9].ToString() == "true" | dataRow[9].ToString() == "TRUE" | dataRow[9].ToString() == "1")
                    {
                        item.IsActive = true;
                    }
                    else if (dataRow[9].ToString() == "No" | dataRow[9].ToString() == "NO" | dataRow[9].ToString() == "False" | dataRow[9].ToString() == "FALSE" | dataRow[9].ToString() == "0")
                    {
                        item.IsActive = false;
                    }
                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterPriceList()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterPriceList();

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

        private string CreateXlsMasterPriceList()
        {
            //get data
            List<PriceListDto> priceList = _priceListBLL.GetPriceList();
            var listData = Mapper.Map<List<PriceListItem>>(priceList);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Price List");
            slDocument.MergeWorksheetCells(1, 1, 1, 8);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterPriceList(slDocument);

            //create data
            slDocument = CreateDataExcelMasterPriceList(slDocument, listData);

            var fileName = "Master_Data_PriceList" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterPriceList(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Manufacture");
            slDocument.SetCellValue(iRow, 2, "Model");
            slDocument.SetCellValue(iRow, 3, "Series");
            slDocument.SetCellValue(iRow, 4, "Vehicle Year");
            slDocument.SetCellValue(iRow, 5, "Price");
            slDocument.SetCellValue(iRow, 6, "Installment HMS");
            slDocument.SetCellValue(iRow, 7, "Installment EMP");
            slDocument.SetCellValue(iRow, 8, "Created Date");
            slDocument.SetCellValue(iRow, 9, "Created By");
            slDocument.SetCellValue(iRow, 10, "Modified Date");
            slDocument.SetCellValue(iRow, 11, "Modified By");
            slDocument.SetCellValue(iRow, 12, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 12, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterPriceList(SLDocument slDocument, List<PriceListItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Manufacture);
                slDocument.SetCellValue(iRow, 2, data.Model);
                slDocument.SetCellValue(iRow, 3, data.Series);
                slDocument.SetCellValue(iRow, 4, data.Year);
                slDocument.SetCellValue(iRow, 5, data.Price);
                slDocument.SetCellValue(iRow, 6, data.InstallmenHMS);
                slDocument.SetCellValue(iRow, 7, data.InstallmenEMP);
                slDocument.SetCellValue(iRow, 8, data.CreatedDate.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 9, data.CreatedBy);
                slDocument.SetCellValue(iRow, 10, data.ModifiedDate.Value.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 11, data.ModifiedBy);
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 12, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 12, "InActive");
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
            slDocument.SetCellStyle(3, 1, iRow - 1, 12, valueStyle);

            return slDocument;
        }

        #endregion
    }
      

}