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
    public class MstEpafController : BaseController
    {
        private IEpafBLL _epafBLL;
        private Enums.MenuList _mainMenu;
        public MstEpafController(IPageBLL PageBll, IEpafBLL EpafBLL) : base(PageBll, Enums.MenuList.MasterEpaf)
        {
            _epafBLL = EpafBLL;
            _mainMenu = Enums.MenuList.MasterData;
        }
        //
        // GET: /MstEpaf/
        public ActionResult Index()
        {
            var data = _epafBLL.GetEpaf();
            var model = new EpafModel();
            model.Details = Mapper.Map<List<EpafItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        public ActionResult Detail(int MstEpafId)
        {
            var data = _epafBLL.GetEpafById(MstEpafId);
            var model = new EpafItem();
            model = Mapper.Map<EpafItem>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #region ExportExcel
        public void ExportMasterEpaf()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterEpaf();

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

        private string CreateXlsMasterEpaf()
        {
            //get data
            List<EpafDto> vendor = _epafBLL.GetEpaf();
            var listData = Mapper.Map<List<EpafItem>>(vendor);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master ePaf");
            slDocument.MergeWorksheetCells(1, 1, 1, 17);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterEpaf(slDocument);

            //create data
            slDocument = CreateDataExcelMasterEpaf(slDocument, listData);

            var fileName = "Master Data Epaf " + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterEpaf(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Document Type");
            slDocument.SetCellValue(iRow, 2, "Epaf Action");
            slDocument.SetCellValue(iRow, 3, "Employee ID");
            slDocument.SetCellValue(iRow, 4, "Employee Name");
            slDocument.SetCellValue(iRow, 5, "Cost Center");
            slDocument.SetCellValue(iRow, 6, "Efective Date");
            slDocument.SetCellValue(iRow, 7, "Group Level");
            slDocument.SetCellValue(iRow, 8, "City");
            slDocument.SetCellValue(iRow, 9, "Base Town");
            slDocument.SetCellValue(iRow, 10, "Expat");
            slDocument.SetCellValue(iRow, 11, "Letter Send");
            slDocument.SetCellValue(iRow, 12, "Last Update");
            slDocument.SetCellValue(iRow, 13, "Created By");
            slDocument.SetCellValue(iRow, 14, "Created Date");
            slDocument.SetCellValue(iRow, 15, "Modified By");
            slDocument.SetCellValue(iRow, 16, "Modified Date");
            slDocument.SetCellValue(iRow, 17, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 17, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterEpaf(SLDocument slDocument, List<EpafItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.DocumentType);
                slDocument.SetCellValue(iRow, 2, data.EpafAction);
                slDocument.SetCellValue(iRow, 3, data.EmployeeId);
                slDocument.SetCellValue(iRow, 4, data.EmployeeName);
                slDocument.SetCellValue(iRow, 5, data.CostCenter);
                slDocument.SetCellValue(iRow, 6, data == null ? "" : data.EfectiveDate.Value.ToString("dd/MM/yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 7, data.GroupLevel);
                slDocument.SetCellValue(iRow, 8, data.City);
                slDocument.SetCellValue(iRow, 9, data.BaseTown);
                slDocument.SetCellValue(iRow, 10, data.Expat);
                slDocument.SetCellValue(iRow, 11, data.LetterSend);
                slDocument.SetCellValue(iRow, 12, data.LastUpdate);
                slDocument.SetCellValue(iRow, 13, data.CreatedBy);
                slDocument.SetCellValue(iRow, 14, data.CreatedDate.ToString("dd/MM/yyyy hh:mm"));
                slDocument.SetCellValue(iRow, 15, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 16, data == null ? "" : data.ModifiedDate.Value.ToString("dd/MM/yyyy hh: mm"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 17, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 17, "InActive");
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
            slDocument.SetCellStyle(3, 1, iRow - 1, 17, valueStyle);

            return slDocument;
        }
        #endregion

    }
}
