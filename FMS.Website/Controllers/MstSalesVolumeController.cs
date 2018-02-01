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
using FMS.BusinessObject.Inputs;
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
            _pageBLL = PageBll;
        }

        //
        // GET: /SalesVolume/

        public ActionResult Index()
        {
            var model = new SalesVolumeModel();
            var input = Mapper.Map<SalesVolumeParamInput>(model.SearchView);
            var data = _SalesVolumeBLL.GetSalesVolume(input);
            var allData = _SalesVolumeBLL.GetAllSalesVolume();

            model.Details = Mapper.Map<List<SalesVolumeItem>>(data);
            model.SearchView.TypeList = new SelectList(allData.Select(x => new { x.Type }).Distinct().ToList(), "Type", "Type");
            model.SearchView.RegionalList = new SelectList(allData.Select(x => new { x.Region }).Distinct().ToList(), "Region", "Region");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return View(model);
        }

        public ActionResult Detail(int MstSalesVolumeId)
        {
            var data = _SalesVolumeBLL.GetSalesVolumeById(MstSalesVolumeId);
            var model = Mapper.Map<SalesVolumeItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public PartialViewResult FilterData(SalesVolumeModel model)
        {
            model.Details = GetData(model.SearchView);
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return PartialView("_ListData", model);
        }

        private List<SalesVolumeItem> GetData(SearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _SalesVolumeBLL.GetSalesVolume(new SalesVolumeParamInput());
                return Mapper.Map<List<SalesVolumeItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<SalesVolumeParamInput>(filter);

            var dbData = _SalesVolumeBLL.GetSalesVolume(input);
            return Mapper.Map<List<SalesVolumeItem>>(dbData);
        }

        #region ExportExcel
        public string ExportMasterSalesVolume(SalesVolumeModel model)
        {
            string pathFile = "";

            var input = Mapper.Map<SalesVolumeParamInput>(model.SearchViewExport);
            pathFile = CreateXlsMasterSalesVolume(input);
            return pathFile;
        }

        public void GetExcelFile(string pathFile)
        {
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
        private string CreateXlsMasterSalesVolume(SalesVolumeParamInput input)
        {
            //get data

            List<SalesVolumeDto> SalesVolume = _SalesVolumeBLL.GetSalesVolume(input);
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

        private SLDocument CreateDataExcelMasterSalesVolume(SLDocument slDocument, List<SalesVolumeItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Type);
                slDocument.SetCellValue(iRow, 2, data.Region);
                slDocument.SetCellValue(iRow, 3, data.MonthS);
                slDocument.SetCellValue(iRow, 4, data.Year);
                slDocument.SetCellValue(iRow, 5, string.Format("{0:N0}", data.Value));
                slDocument.SetCellValue(iRow, 6, data.CreatedBy);
                slDocument.SetCellValue(iRow, 7, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 8, data.ModifiedBy == null ? data.CreatedBy : data.ModifiedBy);
                slDocument.SetCellValue(iRow, 9, data == null ? "" : data.ModifiedDate.Value.ToString("dd-MM-yyyy HH:mm:ss"));
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
            var model = new List<SalesVolumeItem>();
            if (data != null)
            {
                try
                {
                    foreach (var dataRow in data.DataRows)
                    {
                        try
                        {
                            if (dataRow[0] == "" || dataRow[0] == "Type")
                            {
                                continue;
                            }
                            var item = new SalesVolumeItem();
                            item.Type = dataRow[0].ToString();
                            item.Region = dataRow[1].ToString();
                            item.Month = Convert.ToInt32(dataRow[2].ToString());
                            item.Year = Convert.ToInt32(dataRow[3].ToString());
                            item.Value = decimal.Parse(dataRow[4].ToString());
                            item.IsActive = dataRow[5].ToString() == "Active" ? true : false;
                            item.IsActiveS = dataRow[5].ToString();
                            model.Add(item);
                        }
                        catch(Exception)
                        {
                            
                        }
                    }
                }
                catch(Exception)
                {

                }
            }
            return Json(model);
        }
        #endregion ImportXLS
    }
}
