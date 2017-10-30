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
    public class MstSalesVolumeController : BaseController
    {
        private ISalesVolumeBLL _SalesVolumeBLL;
        private Enums.MenuList _mainMenu;

        public MstSalesVolumeController(IPageBLL PageBll, ISalesVolumeBLL SalesVolumeBLL) : base(PageBll, Enums.MenuList.MasterEpaf)
        {
            _SalesVolumeBLL = SalesVolumeBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /SalesVolume/

        public ActionResult Index()
        {
            var data = _SalesVolumeBLL.GetSalesVolume();
            var model = new SalesVolumeModel();
            model.Details = Mapper.Map<List<SalesVolumeItem>>(data);
            model.MainMenu = _mainMenu;
            return View(model);
        }

        #region ExportExcel
        public void ExportMasterSalesVolume()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterSalesVolume();

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

        private string CreateXlsMasterSalesVolume()
        {
            //get data
            List<SalesVolumeDto> SalesVolume = _SalesVolumeBLL.GetSalesVolume();
            var listData = Mapper.Map<List<SalesVolumeItem>>(SalesVolume);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Sales Volume");
            slDocument.MergeWorksheetCells(1, 1, 1, 11);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterSalesVolume(slDocument);

            //create data
            slDocument = CreateDataExcelMasterSalesVolume(slDocument, listData);

            var fileName = "Master Data Sales Volume " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterSalesVolume(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Mst Sales Volume ID");
            slDocument.SetCellValue(iRow, 2, "Type");
            slDocument.SetCellValue(iRow, 3, "Region");
            slDocument.SetCellValue(iRow, 4, "Month");
            slDocument.SetCellValue(iRow, 5, "Year");
            slDocument.SetCellValue(iRow, 6, "Value");
            slDocument.SetCellValue(iRow, 7, "Created By");
            slDocument.SetCellValue(iRow, 8, "Created Date");
            slDocument.SetCellValue(iRow, 9, "Modified By");
            slDocument.SetCellValue(iRow, 10, "Modified Date");
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

        private SLDocument CreateDataExcelMasterSalesVolume(SLDocument slDocument, List<SalesVolumeItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstSalesVolumeId);
                slDocument.SetCellValue(iRow, 2, data.Type);
                slDocument.SetCellValue(iRow, 3, data.Region);
                slDocument.SetCellValue(iRow, 4, data.Month);
                slDocument.SetCellValue(iRow, 5, data.Year);
                slDocument.SetCellValue(iRow, 6, data.Value);
                slDocument.SetCellValue(iRow, 7, data.CreatedBy);
                slDocument.SetCellValue(iRow, 8, data.CreatedDate.ToString("dd/MM/yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 9, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 10, data == null ? "" : data.ModifiedDate.Value.ToString("dd/MM/yyyy hh: mm"));
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

            slDocument.AutoFitColumn(1, 8);
            slDocument.SetCellStyle(3, 1, iRow - 1, 11, valueStyle);

            return slDocument;
        }
        #endregion


        #region ImportXLS
        public ActionResult Upload()
        {
            var model = new SalesVolumeModel();
            model.MainMenu = _mainMenu;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(SalesVolumeModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (SalesVolumeItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = "User";
                        data.ModifiedDate = null;
                        data.IsActive = true;

                        var dto = Mapper.Map<SalesVolumeDto>(data);
                        _SalesVolumeBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstSalesVolume");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<SalesVolumeItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new SalesVolumeItem();
                    item.Type = dataRow[0].ToString();
                    item.Region = dataRow[1].ToString();
                    item.Month = Convert.ToInt32(dataRow[2].ToString());
                    item.Year = Convert.ToInt32(dataRow[3].ToString());
                    item.Value = Convert.ToDecimal(dataRow[4].ToString());
                    model.Add(item);
                }
            }
            return Json(model);
        }
        #endregion ImportXLS
    }
}
