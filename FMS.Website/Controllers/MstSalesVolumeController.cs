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
using System.Globalization;


namespace FMS.Website.Controllers
{
    public class MstSalesVolumeController : BaseController
    {
        private ISalesVolumeBLL _SalesVolumeBLL;
        private Enums.MenuList _mainMenu;
        private IPageBLL _pageBLL;

        public MstSalesVolumeController(IPageBLL PageBll, ISalesVolumeBLL SalesVolumeBLL) : base(PageBll, Enums.MenuList.MasterSalesVolume)
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
            foreach(SalesVolumeItem item in model.Details)
            {
                item.MonthS = this.SetMonthToString(item.Month);
            }
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Detail(int MstSalesVolumeId)
        {
            var data = _SalesVolumeBLL.GetSalesVolumeById(MstSalesVolumeId);
            var model = Mapper.Map<SalesVolumeItem>(data);
            model.MonthS = this.SetMonthToString(model.Month);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
            slDocument.MergeWorksheetCells(1, 1, 1, 10);
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

            var fileName = "Master_Data_Sales_Volume" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterSalesVolume(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Type");
            slDocument.SetCellValue(iRow, 2, "Region");
            slDocument.SetCellValue(iRow, 3, "Month");
            slDocument.SetCellValue(iRow, 4, "Year");
            slDocument.SetCellValue(iRow, 5, "Value");
            slDocument.SetCellValue(iRow, 6, "Created By");
            slDocument.SetCellValue(iRow, 7, "Created Date");
            slDocument.SetCellValue(iRow, 8, "Modified By");
            slDocument.SetCellValue(iRow, 9, "Modified Date");
            slDocument.SetCellValue(iRow, 10, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 10, headerStyle);

            return slDocument;

        }

        public string SetMonthToString(int Month)
        {
            if(Month == 0)
            {
                return "Month 0 is not exist";
            }
            else if (Month == 1)
            {
                return "Jan";
            }
            else if(Month == 2)
            {
                return "Feb";
            }
            else if (Month == 3)
            {
                return "Mar";
            }
            else if (Month == 4)
            {
                return "Apr";
            }
            else if (Month == 5)
            {
                return "May";
            }
            else if (Month == 6)
            {
                return "Jun";
            }
            else if (Month == 7)
            {
                return "Jul";
            }
            else if (Month == 8)
            {
                return "Aug";
            }
            else if (Month == 9)
            {
                return "Sep";
            }
            else if (Month == 10)
            {
                return "Nov";
            }
            else if (Month == 11)
            {
                return "Oct";
            }
            else if (Month == 12)
            {
                return "Dec";
            }

            return "An Error Occurred";
        }

        private SLDocument CreateDataExcelMasterSalesVolume(SLDocument slDocument, List<SalesVolumeItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstSalesVolumeId);
                slDocument.SetCellValue(iRow, 2, data.Type);
                slDocument.SetCellValue(iRow, 3, data.Region);
                data.MonthS = this.SetMonthToString(data.Month);
                slDocument.SetCellValue(iRow, 4, data.MonthS);
                slDocument.SetCellValue(iRow, 5, data.Year);
                slDocument.SetCellValue(iRow, 6, data.Value);
                slDocument.SetCellValue(iRow, 7, data.CreatedBy);
                slDocument.SetCellValue(iRow, 8, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 9, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 10, data == null ? "" : data.ModifiedDate.Value.ToString("dd-MM-yyyy HH:mm:ss"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 10, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 10, "InActive");
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
            slDocument.SetCellStyle(3, 1, iRow - 1, 10, valueStyle);

            return slDocument;
        }
        #endregion


        #region ImportXLS
        public ActionResult Upload()
        {
            var model = new SalesVolumeModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
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
                        data.CreatedBy = CurrentUser.USERNAME;
                        data.ModifiedBy = null;
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        
                        var dto = Mapper.Map<SalesVolumeDto>(data);

                        _SalesVolumeBLL.CheckSalesVolume(data.Type, data.Region, data.Month, data.Year, CurrentUser.USERNAME);

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
            var model = new List<SalesVolumeUpload>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "" || dataRow[0] == "Type")
                    {
                        continue;
                    }
                    var item = new SalesVolumeUpload();
                    item.Type = dataRow[0].ToString();
                    item.Region = dataRow[1].ToString();
                    item.Month = dataRow[2].ToString();
                    item.Year = dataRow[3].ToString();
                    item.Value = dataRow[4].ToString();
                    model.Add(item);
                }
            }
            return Json(model);
        }
        #endregion ImportXLS
    }
}
