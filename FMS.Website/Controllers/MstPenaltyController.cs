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
    public class MstPenaltyController : BaseController
    {
        private IPenaltyBLL _penaltyBLL;
        private IPenaltyLogicBLL _penaltyLogicBLL;
        private Enums.MenuList _mainMenu;
        private IVendorBLL _vendorBLL;
        //
        // GET: /MstPenalty/

        public MstPenaltyController(IPageBLL pageBll, IPenaltyBLL penaltyBLL, IPenaltyLogicBLL penaltyLogicBLL, IVendorBLL vendorBLL) : base(pageBll, Enums.MenuList.MasterPenalty)
        {
            _penaltyBLL = penaltyBLL;
            _mainMenu = Enums.MenuList.MasterData;
            _penaltyLogicBLL = penaltyLogicBLL;
            _vendorBLL = vendorBLL;
        }
        public ActionResult Index()
        {
            var data = _penaltyBLL.GetPenalty();
            var model = new PenaltyModel();
            model.Details = Mapper.Map<List<PenaltyItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                model.IsShowNewButton = false;
                model.IsNotViewer = false;
            }
            else
            {
                model.IsShowNewButton = true;
                model.IsNotViewer = true;
            }
            return View(model);
        }

        public PenaltyItem listdata(PenaltyItem model)
        {
            var Vehiclelist = new List<SelectListItem>
            {
                new SelectListItem { Text = "BENEFIT", Value = "BENEFIT" },
                new SelectListItem { Text = "WTC", Value = "WTC"}
            };
            model.VehicleList = new SelectList(Vehiclelist, "Value", "Text");

            var Restitutionlist = new List<SelectListItem>
            {
                new SelectListItem { Text = "YES", Value = "true"},
                new SelectListItem { Text = "NO", Value = "false"}
            };
            model.RestitutionList = new SelectList(Restitutionlist, "Value", "Text");

            var PenaltyList = new List<SelectListItem>();
            List<PenaltyLogicDto> PenaltyLogicDataList = _penaltyLogicBLL.GetPenaltyLogic();
            
            foreach (PenaltyLogicDto item in PenaltyLogicDataList)
            {
                var temp = new SelectListItem();
                temp.Text = item.MstPenaltyLogicId.ToString();
                temp.Value = item.MstPenaltyLogicId.ToString();
                PenaltyList.Add(temp);
            }

            model.PenaltyList = new SelectList(PenaltyList, "Value", "Text");

            var VendorList = new List<SelectListItem>();
            List<VendorDto> VendorDataList = _vendorBLL.GetVendor();

            foreach(VendorDto item in VendorDataList)
            {
                var temp = new SelectListItem();
                temp.Text = item.VendorName.ToString();
                temp.Value = item.MstVendorId.ToString();
                PenaltyList.Add(temp);
            }
            model.VendorList = new SelectList(VendorDataList, "Value", "Text");
            return model;
        }

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Index");
            }
            var model = new PenaltyItem();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model = listdata(model);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(PenaltyItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<PenaltyDto>(model);
                data.CreatedBy = CurrentUser.USERNAME;
                data.CreatedDate = DateTime.Today;
                data.ModifiedDate = null;
                data.IsActive = true;
                _penaltyBLL.Save(data);
            }
            return RedirectToAction("Index", "MstPenalty");
        }

        public ActionResult View(int MstPenaltyId)
        {
            var data = _penaltyBLL.GetByID(MstPenaltyId);
            var model = new PenaltyItem();
            model = Mapper.Map<PenaltyItem>(data);
            model.VendorName = _vendorBLL.GetByID(model.Vendor) == null ? "" : _vendorBLL.GetByID(model.Vendor).VendorName;
            model.PenaltyLogic = this.GetPenaltyLogicById(MstPenaltyId).Data.ToString();
            model = listdata(model);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterPenalty, MstPenaltyId);
            return View(model);
        }

        public ActionResult Edit(int MstPenaltyId)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Index");
            }
            var data = _penaltyBLL.GetByID(MstPenaltyId);
            var model = new PenaltyItem();
            model = Mapper.Map<PenaltyItem>(data);
            model.VendorName = _vendorBLL.GetByID(model.Vendor) == null ? "" : _vendorBLL.GetByID(model.Vendor).VendorName;
            model.PenaltyLogic = this.GetPenaltyLogicById(MstPenaltyId).Data.ToString();
            model = listdata(model);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterPenalty, MstPenaltyId);
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                model.IsNotViewer = false;
            }
            else
            {
                model.IsNotViewer = true;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PenaltyItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<PenaltyDto>(model);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USERNAME;

                _penaltyBLL.Save(data, CurrentUser);
            }
            return RedirectToAction("Index", "MstPenalty");
        }

        public JsonResult GetPenaltyLogicById(int penaltyLogicId)
        {
            string PenaltyLogic = "";
            PenaltyLogic = _penaltyLogicBLL.GetPenaltyLogicById(penaltyLogicId) == null ? "" : _penaltyLogicBLL.GetPenaltyLogicById(penaltyLogicId).PenaltyLogic;

            return Json(PenaltyLogic, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Upload()
        {
            var model = new PenaltyModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(PenaltyModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (PenaltyItem data in Model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME; ;
                        data.ModifiedDate = null;
                        data.IsActive = true;

                        var dto = Mapper.Map<PenaltyDto>(data);
                        _penaltyBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstPenalty");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<PenaltyItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new PenaltyItem();
                    var vendorName = dataRow[0].ToString();
                    int vendorId = _vendorBLL.GetExist(vendorName) == null? 0 : _vendorBLL.GetExist(vendorName).MstVendorId;
                    item.VendorName = vendorName;
                    item.Vendor = vendorId;
                    item.Manufacturer = dataRow[1].ToString();
                    item.Models = dataRow[2].ToString();
                    item.Series = dataRow[3].ToString();
                    item.Year = Convert.ToInt32(dataRow[4].ToString());
                    item.MonthStart = Convert.ToInt32(dataRow[5].ToString());
                    item.MonthEnd = Convert.ToInt32(dataRow[6].ToString());
                    item.VehicleType = dataRow[7].ToString();
                    item.Penalty = Convert.ToInt32(dataRow[8].ToString());
                    item.Restitution = Convert.ToBoolean(Convert.ToInt32(dataRow[9]));
                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterPenalty()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterPenalty();

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
        private string CreateXlsMasterPenalty()
        {
            //get data
            List<PenaltyDto> penalty = _penaltyBLL.GetPenalty();
            var listData = Mapper.Map<List<PenaltyItem>>(penalty);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Penalty");
            slDocument.MergeWorksheetCells(1, 1, 1, 16);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterPenalty(slDocument);

            //create data
            slDocument = CreateDataExcelMasterPenalty(slDocument, listData);

            var fileName = "Master_Data_Penalty" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterPenalty(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "ID Penalty");
            slDocument.SetCellValue(iRow, 2, "Vendor");
            slDocument.SetCellValue(iRow, 3, "Manufacturer");
            slDocument.SetCellValue(iRow, 4, "Model");
            slDocument.SetCellValue(iRow, 5, "Series");
            slDocument.SetCellValue(iRow, 6, "Year");
            slDocument.SetCellValue(iRow, 7, "Month Start");
            slDocument.SetCellValue(iRow, 8, "Month End");
            slDocument.SetCellValue(iRow, 9, "Vehicle Type");
            slDocument.SetCellValue(iRow, 10, "Formula");
            slDocument.SetCellValue(iRow, 11, "Restitution");
            slDocument.SetCellValue(iRow, 12, "Created By");
            slDocument.SetCellValue(iRow, 13, "Created Date");
            slDocument.SetCellValue(iRow, 14, "Modified By");
            slDocument.SetCellValue(iRow, 15, "Modified Date");
            slDocument.SetCellValue(iRow, 16, "Status");
        
            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 16, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterPenalty(SLDocument slDocument, List<PenaltyItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.MstPenaltyId);
                slDocument.SetCellValue(iRow, 2, data.VendorName);
                slDocument.SetCellValue(iRow, 3, data.Manufacturer);
                slDocument.SetCellValue(iRow, 4, data.Models);
                slDocument.SetCellValue(iRow, 5, data.Series);
                slDocument.SetCellValue(iRow, 6, data.Year);
                slDocument.SetCellValue(iRow, 7, data.MonthStart);
                slDocument.SetCellValue(iRow, 8, data.MonthEnd);
                slDocument.SetCellValue(iRow, 9, data.VehicleType);
                slDocument.SetCellValue(iRow, 10, data.Penalty);
                slDocument.SetCellValue(iRow, 11, data.Restitution == false? "No" : "Yes");
                slDocument.SetCellValue(iRow, 12, data.CreatedBy);
                slDocument.SetCellValue(iRow, 13, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 14, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 15, data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                if (data.IsActive)
                {
                    slDocument.SetCellValue(iRow, 16, "Active");
                }
                else
                {
                    slDocument.SetCellValue(iRow, 16, "InActive");
                }

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 15);
            slDocument.SetCellStyle(3, 1, iRow - 1, 16, valueStyle);

            return slDocument;
        }

        #endregion

    }
}
