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
using FMS.BusinessObject.Inputs;

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
            foreach (var item in model.Details)
            {
                var Vendor = _vendorBLL.GetByID(item.Vendor);
                if (Vendor != null)
                {
                    item.VendorName = Vendor.VendorName;
                }
            }
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            model.SearchView = Initial(model);
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

        [HttpPost]
        public PartialViewResult ListPenalty(PenaltyModel model)
        {
            model.Details = new List<PenaltyItem>();
            model.Details = GetPenalty(model.SearchView);
            foreach (var item in model.Details)
            {
                var Vendor = _vendorBLL.GetByID(item.Vendor);
                if (Vendor != null)
                {
                    item.VendorName = Vendor.VendorName;
                }
            }
            model.SearchView = Initial(model);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.CurrentPageAccess = CurrentPageAccess;
            return PartialView("_ListPenalty", model);
        }

        public PenaltySearchView Initial(PenaltyModel model)
        {
            model.SearchView.BodyTypeList = new SelectList(model.Details.Select(x => new { x.BodyType }).Distinct().ToList(), "BodyType", "BodyType");
            model.SearchView.ManufacturerList = new SelectList(model.Details.Select(x => new { x.Manufacturer }).Distinct().ToList(), "Manufacturer", "Manufacturer");
            model.SearchView.ModelList = new SelectList(model.Details.Select(x => new { x.Models }).Distinct().ToList(), "Models", "Models");
            model.SearchView.RequestYearList = new SelectList(model.Details.Select(x => new { x.Year }).Distinct().ToList(), "Year", "Year");
            model.SearchView.SeriesList = new SelectList(model.Details.Select(x => new { x.Series }).Distinct().ToList(), "Series", "Series");
            model.SearchView.VehicleTypeList = new SelectList(model.Details.Select(x => new { x.VehicleType }).Distinct().ToList(), "VehicleType", "VehicleType");
            model.SearchView.VendorList = new SelectList(model.Details.Select(x => new { x.Vendor, x.VendorName }).Distinct().ToList(), "Vendor", "VendorName");
            return model.SearchView;
        }

        private List<PenaltyItem> GetPenalty(PenaltySearchView filter = null)
        {
            if (filter == null)
            {
                //Get All
                var data = _penaltyBLL.GetPenalty(new PenaltyParamInput());
                return Mapper.Map<List<PenaltyItem>>(data);
            }

            //getbyparams
            var input = Mapper.Map<PenaltyParamInput>(filter);

            var dbData = _penaltyBLL.GetPenalty(input);
            return Mapper.Map<List<PenaltyItem>>(dbData);
        }

        public PenaltyItem listdata(PenaltyItem model)
        {
            var Vehiclelist = new List<SelectListItem>
            {
                new SelectListItem { Text = "BENEFIT", Value = "BENEFIT" },
                new SelectListItem { Text = "WTC", Value = "WTC"}
            };
            model.VehicleList = new SelectList(Vehiclelist, "Value", "Text");

            var PenaltyLogicDataList = _penaltyLogicBLL.GetPenaltyLogic().Where(x => x.IsActive).Select(x => new { x.MstPenaltyLogicId }).Distinct().ToList();

            model.PenaltyList = new SelectList(PenaltyLogicDataList, "MstPenaltyLogicId", "MstPenaltyLogicId");

            var VendorDataList = _vendorBLL.GetVendor().Where(x => x.IsActive).Select(x => new { x.VendorName, x.MstVendorId }).Distinct().ToList();

            model.VendorList = new SelectList(VendorDataList, "MstVendorId", "VendorName");
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
                try
                {
                    var Exist = _penaltyBLL.GetPenalty().Where(x => (x.BodyType == null ? "" : x.BodyType.ToUpper()) == (model.BodyType == null ? "" : model.BodyType.ToUpper())
                                && (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (model.Manufacturer == null ? "" : model.Manufacturer.ToUpper())
                                && (x.Model == null ? "" : x.Model.ToUpper()) == (model.Models == null ? "" : model.Models.ToUpper())
                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (model.VehicleType == null ? "" : model.VehicleType.ToUpper())
                                && (x.BodyType == null ? "" : x.BodyType.ToUpper()) == (model.BodyType == null ? "" : model.BodyType.ToUpper())
                                && (x.Series == null ? "" : x.Series.ToUpper()) == (model.Series == null ? "" : model.Series.ToUpper())
                                && x.Year == model.Year && x.Vendor == model.Vendor && x.Penalty == model.Penalty && x.MonthEnd == model.MonthEnd && x.MonthStart == model.MonthStart && x.IsActive).FirstOrDefault();

                    if (Exist != null)
                    {
                        model.ErrorMessage = "Data Already Exist in Master Penalty";
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        model.PenaltyLogic = this.GetPenaltyLogicById(model.Penalty).Data.ToString();
                        model = listdata(model);
                        return View(model);
                    }

                    var data = Mapper.Map<PenaltyDto>(model);
                    data.CreatedBy = CurrentUser.USER_ID;
                    data.CreatedDate = DateTime.Now;
                    data.ModifiedDate = null;
                    data.IsActive = true;

                    _penaltyBLL.Save(data);
                    _penaltyBLL.SaveChanges();
                }
                catch (Exception exp)
                {
                    model.ErrorMessage = exp.Message;
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    model = listdata(model);
                    return View(model);
                }

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
                try
                {
                    var Exist = _penaltyBLL.GetPenalty().Where(x => (x.BodyType == null ? "" : x.BodyType.ToUpper()) == (model.BodyType == null ? "" : model.BodyType.ToUpper())
                               && (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (model.Manufacturer == null ? "" : model.Manufacturer.ToUpper())
                               && (x.Model == null ? "" : x.Model.ToUpper()) == (model.Models == null ? "" : model.Models.ToUpper())
                               && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (model.VehicleType == null ? "" : model.VehicleType.ToUpper())
                               && (x.BodyType == null ? "" : x.BodyType.ToUpper()) == (model.BodyType == null ? "" : model.BodyType.ToUpper())
                               && (x.Series == null ? "" : x.Series.ToUpper()) == (model.Series == null ? "" : model.Series.ToUpper())
                               && x.Year == model.Year && x.Vendor == model.Vendor && x.Penalty == model.Penalty && x.MonthEnd == model.MonthEnd && x.MonthStart == model.MonthStart && x.IsActive && x.MstPenaltyId != model.MstPenaltyId).FirstOrDefault();

                    if (Exist != null)
                    {
                        model.ErrorMessage = "Data Already Exist in Master Penalty";
                        model.MainMenu = _mainMenu;
                        model.CurrentLogin = CurrentUser;
                        model = listdata(model);
                        model.PenaltyLogic = this.GetPenaltyLogicById(model.Penalty).Data.ToString();
                        return View(model);
                    }

                    var data = Mapper.Map<PenaltyDto>(model);
                    data.ModifiedDate = DateTime.Now;
                    data.ModifiedBy = CurrentUser.USER_ID;

                    _penaltyBLL.Save(data, CurrentUser);
                    _penaltyBLL.SaveChanges();
                }
                catch (Exception EXP)
                {
                    model.ErrorMessage = EXP.Message;
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    model = listdata(model);
                    return View(model);
                }
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
                        var Exist = _penaltyBLL.GetPenalty().Where(x => (x.BodyType == null ? "" : x.BodyType.ToUpper()) == (data.BodyType == null ? "" : data.BodyType.ToUpper())
                                && (x.Manufacturer == null ? "" : x.Manufacturer.ToUpper()) == (data.Manufacturer == null ? "" : data.Manufacturer.ToUpper())
                                && (x.Model == null ? "" : x.Model.ToUpper()) == (data.Models == null ? "" : data.Models.ToUpper())
                                && (x.VehicleType == null ? "" : x.VehicleType.ToUpper()) == (data.VehicleType == null ? "" : data.VehicleType.ToUpper())
                                && (x.BodyType == null ? "" : x.BodyType.ToUpper()) == (data.BodyType == null ? "" : data.BodyType.ToUpper())
                                && (x.Series == null ? "" : x.Series.ToUpper()) == (data.Series == null ? "" : data.Series.ToUpper())
                                && x.Year == data.Year && x.Vendor == data.Vendor && x.Penalty == data.Penalty && x.MonthEnd == data.MonthEnd && x.MonthStart == data.MonthStart && x.IsActive).FirstOrDefault();

                        if (Exist != null)
                        {
                            Exist.IsActive = false;
                            Exist.ModifiedBy = "SYSTEM";
                            Exist.ModifiedDate = DateTime.Now;
                            _penaltyBLL.Save(Exist);
                        }

                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USER_ID; ;
                        data.ModifiedDate = null;
                        data.IsActive = true;

                        var dto = Mapper.Map<PenaltyDto>(data);
                        _penaltyBLL.Save(dto);

                    }
                    catch (Exception exception)
                    {
                        Model.ErrorMessage = exception.Message;
                        Model.MainMenu = _mainMenu;
                        Model.CurrentLogin = CurrentUser;
                        return View(Model);
                    }
                }
                try
                {
                    _penaltyBLL.SaveChanges();
                }
                catch (Exception EXP)
                {
                    Model.ErrorMessage = EXP.Message;
                    Model.MainMenu = _mainMenu;
                    Model.CurrentLogin = CurrentUser;
                    return View(Model);
                }
            }
            return RedirectToAction("Index", "MstPenalty");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<PenaltyItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow.Count <= 0)
                    {
                        continue;
                    }
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new PenaltyItem();
                    item.ErrorMessage = "";

                    var vendorName = dataRow[0].ToString();
                    if (vendorName == "")
                    {
                        item.ErrorMessage = "Vendor Name Can't be Empty";
                    }
                    else
                    {
                        item.VendorName = vendorName;
                        var vendor = _vendorBLL.GetVendor().Where(x => x.IsActive && (x.VendorName == null ? "" : x.VendorName.ToUpper()) == (vendorName == null ? "" : vendorName.ToUpper())).FirstOrDefault();
                        if (vendor == null)
                        {
                            item.ErrorMessage = "Vendor is not in the Master Vendor";
                        }
                        else
                        {
                            item.Vendor = vendor.MstVendorId;
                        }
                    }

                    if (dataRow[1] == "")
                    {
                        item.ErrorMessage = "Request Year Can't be Empty";
                    }
                    else
                    {
                        try
                        {
                            item.Year = Convert.ToInt32(dataRow[1].ToString());
                        }
                        catch (Exception)
                        {
                            item.ErrorMessage = "Request year must be number";
                        }
                    }

                    if (dataRow[2] == "")
                    {
                        item.ErrorMessage = "Minimum lease Term Can't be Empty";
                    }
                    else
                    {
                        try
                        {
                            item.MonthStart = Convert.ToInt32(dataRow[2].ToString());
                        }
                        catch (Exception)
                        {
                            item.ErrorMessage = "Minimum lease Term must be number";
                        }
                    }

                    if (dataRow[3] == "")
                    {
                        item.ErrorMessage = "Maximum lease Term Can't be Empty";
                    }
                    else
                    {
                        try
                        {
                            item.MonthEnd = Convert.ToInt32(dataRow[3].ToString());
                        }
                        catch (Exception)
                        {
                            item.ErrorMessage = "Maximum lease must be number";
                        }
                    }

                    item.Manufacturer = dataRow[4].ToString();
                    item.Models = dataRow[5].ToString();
                    item.Series = dataRow[6].ToString();
                    item.BodyType = dataRow[7].ToString();

                    item.VehicleType = (dataRow[8] == null ? "" : dataRow[8].ToUpper());
                    if (item.VehicleType == "")
                    {
                        item.ErrorMessage = "Vehicle Type can't be empty";
                    }

                    if (dataRow[9] == "")
                    {
                        item.ErrorMessage = "Penalty Id Can't be empty";
                    }
                    else
                    {
                        try
                        {
                            item.Penalty = Convert.ToInt32(dataRow[9].ToString());
                            var PenaltyLogic = _penaltyLogicBLL.GetPenaltyLogicById(item.Penalty);
                            if (PenaltyLogic == null)
                            {
                                item.ErrorMessage = "This Penalty Id is not in master penalty Logic";
                            }
                        }
                        catch (Exception)
                        {

                            item.ErrorMessage = "Penalty Id must be number";
                        }
                    }
                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region export xls
        public string ExportMasterPenalty(PenaltyModel model = null)
        {
            string pathFile = "";
            pathFile = CreateXlsMasterPenalty(model.SearchView);
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
        private string CreateXlsMasterPenalty(PenaltySearchView filter)
        {
            //get data

            var input = Mapper.Map<PenaltyParamInput>(filter);
            List<PenaltyDto> penalty = _penaltyBLL.GetPenalty(input);
            var listData = Mapper.Map<List<PenaltyItem>>(penalty);
            foreach (var item in listData)
            {
                var Vendor = _vendorBLL.GetByID(item.Vendor);
                if (Vendor != null)
                {
                    item.VendorName = Vendor.VendorName;
                }
            }
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
            slDocument.SetCellValue(iRow, 3, "Request Year");
            slDocument.SetCellValue(iRow, 4, "Month Start");
            slDocument.SetCellValue(iRow, 5, "Month End");
            slDocument.SetCellValue(iRow, 6, "Manufacturer");
            slDocument.SetCellValue(iRow, 7, "Model");
            slDocument.SetCellValue(iRow, 8, "Series");
            slDocument.SetCellValue(iRow, 9, "Body Type");
            slDocument.SetCellValue(iRow, 10, "Vehicle Type");
            slDocument.SetCellValue(iRow, 11, "Penalty Logic Id");
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
                slDocument.SetCellValue(iRow, 3, data.Year.Value);
                slDocument.SetCellValue(iRow, 4, data.MonthStart);
                slDocument.SetCellValue(iRow, 5, data.MonthEnd);
                slDocument.SetCellValue(iRow, 6, data.Manufacturer);
                slDocument.SetCellValue(iRow, 7, data.Models);
                slDocument.SetCellValue(iRow, 8, data.Series);
                slDocument.SetCellValue(iRow, 9, data.BodyType);
                slDocument.SetCellValue(iRow, 10, data.VehicleType);
                slDocument.SetCellValue(iRow, 11, data.Penalty);
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
