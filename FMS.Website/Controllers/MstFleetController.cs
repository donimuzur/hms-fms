using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using FMS.BLL.Vendor;
using FMS.BusinessObject.Dto;
using FMS.Contract.BLL;
using FMS.Core;
using FMS.Website.Models;
using FMS.Website.Utility;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Controllers
{
    public class MstFleetController : BaseController
    {
        private IFleetBLL _fleetBLL;
        private IVendorBLL _vendorBLL;
        private IPageBLL _pageBLL;
        private Enums.MenuList _mainMenu;

        public MstFleetController(IPageBLL PageBll, IFleetBLL  FleetBLL, IVendorBLL VendorBLL) : base(PageBll, Enums.MenuList.MasterFleet )
        {
            _fleetBLL = FleetBLL;
            _vendorBLL = VendorBLL;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }
        // GET: /MstFleet/
        public ActionResult Index()
        {
            var data = _fleetBLL.GetFleet();
            var model = new FleetModel();
            model.Details=Mapper.Map<List<FleetItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        #region Create
        public FleetItem initCreate()
        {
            var data = _vendorBLL.GetVendor().Where(x => x.IsActive == true);
            var model = new FleetItem();

            model.VendorList = new SelectList(data, "VendorName", "VendorName");

            var list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Extend", Value = "Extend" },
                new SelectListItem { Text = "Temporary", Value = "Temporary" },
                new SelectListItem { Text = "Lease", Value = "Lease" },
                new SelectListItem { Text = "Services", Value = "Services" }
            };
            model.SupplyMethodList = new SelectList(list1, "Value", "Text");

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Automatic", Value = "Automatic" },
                new SelectListItem { Text = "Manual", Value = "Manual" }
            };
            model.TransmissionList = new SelectList(list1, "Value", "Text");

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "MPV", Value = "MPV" },
                new SelectListItem { Text = "SUV", Value = "SUV" },
                new SelectListItem { Text = "Forklift", Value = "Forklift" },
                new SelectListItem { Text = "Motorcycle", Value = "Motorcycle" },
                new SelectListItem { Text = "Truck", Value = "Truck" }
            };
            model.BodyTypeList = new SelectList(list1, "Value", "Text");

            return model;
        }

        //public ActionResult Create()
        //{
        //    var model = initCreate();
        //    model.MainMenu = _mainMenu;
            
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Create(FleetItem Model)
        //{
        //    try
        //    {
        //        if(ModelState.IsValid)
        //        {
        //            var dto = Mapper.Map<FleetDto>(Model);
        //            dto.CreatedDate = DateTime.Now;
        //            dto.CreatedBy = "Doni";
        //            dto.IsActive = true;
        //            _fleetBLL.Save(dto);
        //        }
        //    }
        //    catch (Exception )
        //    {
        //        var model = initCreate();
        //        return View(model);
        //    }

        //    return RedirectToAction("Index","MstFleet"); 
        //}
        #endregion
        
        public FleetItem initEdit(FleetItem model)
        {
            var data = _vendorBLL.GetVendor().Where(x => x.IsActive == true);

            model.VendorList = new SelectList(data, "VendorName", "VendorName", model.VendorName);

            var list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Extend", Value = "Extend" },
                new SelectListItem { Text = "Temporary", Value = "Temporary" },
                new SelectListItem { Text = "Lease", Value = "Lease" },
                new SelectListItem { Text = "Services", Value = "Services" }
            };
            model.SupplyMethodList = new SelectList(list1, "Value", "Text",model.SupplyMethod);

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "Automatic", Value = "Automatic" },
                new SelectListItem { Text = "Manual", Value = "Manual" }
            };
            model.TransmissionList = new SelectList(list1, "Value", "Text", model.Transmission);

            list1 = new List<SelectListItem>
            {
                new SelectListItem { Text = "MPV", Value = "MPV" },
                new SelectListItem { Text = "SUV", Value = "SUV" },
                new SelectListItem { Text = "Forklift", Value = "Forklift" },
                new SelectListItem { Text = "Motorcycle", Value = "Motorcycle" },
                new SelectListItem { Text = "Truck", Value = "Truck" }
            };
            model.BodyTypeList = new SelectList(list1, "Value", "Text",model.BodyType);

            return model;
        }

        public ActionResult Edit(int? MstFleetId)
        {
            if (!MstFleetId.HasValue)
            {
                return HttpNotFound();
            }
            var data = _fleetBLL.GetFleetById(MstFleetId.Value);
            var model = Mapper.Map<FleetItem>(data);
            model = initEdit(model);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterFleet, MstFleetId.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FleetItem model)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<FleetDto>(model);
                data.ModifiedBy = CurrentUser.USERNAME;
                data.ModifiedDate = DateTime.Now;

                _fleetBLL.Save(data, CurrentUser);
            }
            return RedirectToAction("Index","MstFleet");
        }


        public ActionResult Upload()
        {
            var model = new FleetModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(FleetModel model)
        {
            if (ModelState.IsValid)
            {
                foreach(var data in model.Details)
                {
                    try
                    {
                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USER_ID;
                        data.IsActive = true;
                        if (data.EmployeeID == "null" ||  data.EmployeeID == "NULL" || data.EmployeeID == null)
                        {
                            data.EmployeeID = null;
                        }
                        if (data.StartDates != "" & data.StartDates != "null" & data.StartDates != "NULL" & data.StartDates != null)
                        {
                            data.StartDate = Convert.ToDateTime(data.StartDates);
                        }
                        else { data.StartDate = null; }
                        if(data.EndDates != null & data.EndDates != "null" & data.EndDates != "NULL" & data.EndDates !="")
                        {
                            data.EndDate = Convert.ToDateTime(data.EndDates);
                        }
                        else { data.EndDate = null; }
                        if (data.StartContracts != "" & data.StartContracts != "null" & data.StartContracts != "NULL" & data.StartContracts !=null)
                        {
                            data.StartContract = Convert.ToDateTime(data.StartContracts);
                        }
                        else { data.StartContract = null; }
                        if (data.EndContracts != "" & data.EndContracts != "null" & data.EndContracts != "NULL" & data.EndContracts != null)
                        {
                            data.EndContract = Convert.ToDateTime(data.EndContracts);
                        }
                        else { data.EndContract = null; }
                        if (data.TerminationDates != "" & data.TerminationDates != "null" & data.TerminationDates != "NULL" & data.TerminationDates!= null)
                        {
                            data.TerminationDate = Convert.ToDateTime(data.TerminationDates);
                        }
                        else { data.TerminationDate = null; }

                        var dto = Mapper.Map<FleetDto>(data);
                        _fleetBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        return View(model);
                        
                    }
                }
            }
            return RedirectToAction("Index", "MstFleet");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;
            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<FleetItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    var item = new FleetItem();
                    item.PoliceNumber  = dataRow[0];
                    item.ChasisNumber = dataRow[1];
                    item.EngineNumber = dataRow[2];
                    if (dataRow[3] != "NULL" & dataRow[29] != "")
                    {
                        item.EmployeeID = dataRow[3];
                    }
                    item.EmployeeName  = dataRow[4];
                    item.GroupLevel = Convert.ToInt32(dataRow[5]);
                    item.ActualGroup  = dataRow[6];
                    item.AssignedTo  = dataRow[7];
                    item.CostCenter = dataRow[8];
                    item.VendorName = dataRow[9];
                    item.Manufacturer = dataRow[10];
                    item.Models = dataRow[11];
                    item.Series = dataRow[12];
                    item.BodyType = dataRow[13];
                    item.Color = dataRow[14];
                    item.Transmission = dataRow[15];
                    item.CarGroupLevel = Convert.ToInt32(dataRow[16]);
                    item.FuelType = dataRow[17];
                    item.Branding = dataRow[18];
                    item.Airbag = Convert.ToBoolean (Convert.ToInt32(dataRow[19]));
                    item.VehicleYear = Convert.ToInt32(dataRow[20]);
                    item.VehicleType = dataRow[21];
                    item.VehicleUsage = dataRow[22];
                    item.SupplyMethod  = dataRow[23];
                    item.City = dataRow[24];
                    item.Address = dataRow[25];
                    item.Purpose = dataRow[26];
                    item.Vat  = Convert.ToBoolean(Convert.ToInt32 (dataRow[27]));
                    item.Restitution =Convert.ToBoolean ( Convert.ToInt32(dataRow[28]));

                    if (dataRow[29] != "NULL" && dataRow[29] != "")
                    {
                        item.StartDates = dataRow[29];
                    }
                    if (dataRow[30] != "NULL" && dataRow[30] != "")
                    {
                        item.EndDates =dataRow[30];
                    }
                    if (dataRow[31] != "NULL" && dataRow[31] != "")
                    {
                        item.TerminationDates = dataRow[31];
                    }
                    
                    item.PoNumber = dataRow[32];
                    item.PoLine  = dataRow[33];
                    if (dataRow[34] != "NULL" && dataRow[34] != "")
                    {
                        item.StartContracts = dataRow[34];
                    }
                    if (dataRow[35] != "NULL" && dataRow[35] != "")
                    {
                        item.EndContracts = dataRow[35];
                    }
                        
                    item.Price= Convert.ToInt32 (dataRow[36]);
                    item.VehicleStatus  = dataRow[37];
                    model.Add(item);
                }
            }
            return Json(model);
        }

        #region export xls
        public void ExportMasterFleet()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterFleet();

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

        private string CreateXlsMasterFleet()
        {
            //get data
            List<FleetDto> fleet = _fleetBLL.GetFleet();
            var listData = Mapper.Map<List<FleetItem>>(fleet);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Fleet");
            slDocument.MergeWorksheetCells(1, 1, 1,46);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterFleet(slDocument);

            //create data
            slDocument = CreateDataExcelMasterFleet(slDocument, listData);

            var fileName = "Master_Data_Fleet" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterFleet(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Police Number");
            slDocument.SetCellValue(iRow, 2, "Chasis Number");
            slDocument.SetCellValue(iRow, 3, "Engine Number");
            slDocument.SetCellValue(iRow, 4, "Employee ID");
            slDocument.SetCellValue(iRow, 5, "Employee Name");
            slDocument.SetCellValue(iRow, 6, "Group Level");
            slDocument.SetCellValue(iRow, 7, "Actual Group");
            slDocument.SetCellValue(iRow, 8, "Assigned To");
            slDocument.SetCellValue(iRow, 9, "Cost Center");
            slDocument.SetCellValue(iRow, 10, "Vendor Name");
            slDocument.SetCellValue(iRow, 11, "Manufacturer");
            slDocument.SetCellValue(iRow, 12, "Models");
            slDocument.SetCellValue(iRow, 13, "Series");
            slDocument.SetCellValue(iRow, 14, "Body Type");
            slDocument.SetCellValue(iRow, 15, "Color");
            slDocument.SetCellValue(iRow, 16, "Transmission");
            slDocument.SetCellValue(iRow, 17, "Car Group Level");
            slDocument.SetCellValue(iRow, 18, "Fuel Type");
            slDocument.SetCellValue(iRow, 19, "Branding");
            slDocument.SetCellValue(iRow, 20, "Airbag");
            slDocument.SetCellValue(iRow, 21, "Vehicle Year");
            slDocument.SetCellValue(iRow, 22, "Vehicle Type");
            slDocument.SetCellValue(iRow, 23, "Vehicle Usage");
            slDocument.SetCellValue(iRow, 24, "Supply Method");
            slDocument.SetCellValue(iRow, 25, "City");
            slDocument.SetCellValue(iRow, 26, "Address");
            slDocument.SetCellValue(iRow, 27, "Purpose");
            slDocument.SetCellValue(iRow, 28, "Vat");
            slDocument.SetCellValue(iRow, 29, "Restitution");
            slDocument.SetCellValue(iRow, 30, "Star tDate");
            slDocument.SetCellValue(iRow, 31, "End Date");
            slDocument.SetCellValue(iRow, 32, "Termination Date");
            slDocument.SetCellValue(iRow, 33, "PO Number");
            slDocument.SetCellValue(iRow, 34, "PO Line");
            slDocument.SetCellValue(iRow, 35, "Start Contract");
            slDocument.SetCellValue(iRow, 36, "End Contract");
            slDocument.SetCellValue(iRow, 37, "Price");
            slDocument.SetCellValue(iRow, 38, "Vehicle Status");
            slDocument.SetCellValue(iRow, 39, "Is Taken");
            slDocument.SetCellValue(iRow, 40, "GR Left Qty");
            slDocument.SetCellValue(iRow, 41, "Created By");
            slDocument.SetCellValue(iRow, 42, "Created Date");
            slDocument.SetCellValue(iRow, 43, "Modified By");
            slDocument.SetCellValue(iRow, 44, "Modified Date");
            slDocument.SetCellValue(iRow, 45, "Modified By");
            slDocument.SetCellValue(iRow, 46, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 46, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterFleet(SLDocument slDocument, List<FleetItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PoliceNumber);
                slDocument.SetCellValue(iRow, 2, data.ChasisNumber);
                slDocument.SetCellValue(iRow, 3, data.EngineNumber);
                slDocument.SetCellValue(iRow, 4, data.EmployeeID);
                slDocument.SetCellValue(iRow, 5, data.EmployeeName );
                slDocument.SetCellValue(iRow, 6, data.GroupLevel);
                slDocument.SetCellValue(iRow, 7, data.ActualGroup);
                slDocument.SetCellValue(iRow, 8, data.AssignedTo);
                slDocument.SetCellValue(iRow, 9, data.CostCenter);
                slDocument.SetCellValue(iRow, 10, data.VendorName);
                slDocument.SetCellValue(iRow, 11, data.Manufacturer);
                slDocument.SetCellValue(iRow, 12, data.Models);
                slDocument.SetCellValue(iRow, 13, data.Series);
                slDocument.SetCellValue(iRow, 14, data.BodyType);
                slDocument.SetCellValue(iRow, 15, data.Color);
                slDocument.SetCellValue(iRow, 16, data.Transmission);
                slDocument.SetCellValue(iRow, 17, data.CarGroupLevel);
                slDocument.SetCellValue(iRow, 18, data.FuelType);
                slDocument.SetCellValue(iRow, 19, data.Branding);
                slDocument.SetCellValue(iRow, 20, data.Airbag);
                slDocument.SetCellValue(iRow, 21, data.VehicleYear);
                slDocument.SetCellValue(iRow, 22, data.VehicleType);
                slDocument.SetCellValue(iRow, 23, data.VehicleUsage);
                slDocument.SetCellValue(iRow, 24, data.SupplyMethod);
                slDocument.SetCellValue(iRow, 25, data.City);
                slDocument.SetCellValue(iRow, 26, data.Address);
                slDocument.SetCellValue(iRow, 27, data.Purpose);
                slDocument.SetCellValue(iRow, 28, data.Vat);
                slDocument.SetCellValue(iRow, 29, data.Restitution);
                slDocument.SetCellValue(iRow, 30, data.StartDate == null ? "" : data.StartDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 31, data.EndDate == null ? "" :  data.EndDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 32, data.TerminationDate == null ? "" : data.TerminationDate.Value.ToString("dd-MMM-yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 33, data.PoNumber);
                slDocument.SetCellValue(iRow, 34, data.PoLine);
                slDocument.SetCellValue(iRow, 35, data.StartContract== null ? "" : data.StartContract.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 36, data.EndContract == null ? "" : data.EndContract.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 37, data.Price);
                slDocument.SetCellValue(iRow, 38, data.VehicleStatus);
                slDocument.SetCellValue(iRow, 39, data.IsTaken);
                slDocument.SetCellValue(iRow, 40, data.GrLeftQty);
                slDocument.SetCellValue(iRow, 41, data.CreatedBy);
                slDocument.SetCellValue(iRow, 42, data.CreatedDate.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 43, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 44, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy hh:mm:ss"));
                slDocument.SetCellValue(iRow, 45, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 46, data.IsActive == true ? "Active" : "InActive");
          

                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 46);
            slDocument.SetCellStyle(3, 1, iRow - 1, 46, valueStyle);

            return slDocument;
        }

        #endregion

    }
}
