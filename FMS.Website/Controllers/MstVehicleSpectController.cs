using System.IO.Compression;
using System.IO.Packaging;
using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Dto;
using FMS.Core;
using FMS.Utils;
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
using System.Configuration;

namespace FMS.Website.Controllers
{
    public class MstVehicleSpectController : BaseController
    {
        private IVehicleSpectBLL _VehicleSpectBLL;
        private ISettingBLL _settingBLL;
        private Enums.MenuList _mainMenu;
        private const string pathUpload = "/files_upload/VehicleSpect/";

        public MstVehicleSpectController(IPageBLL PageBll, IVehicleSpectBLL VehicleSpectBLL, ISettingBLL SettingBll) : base(PageBll, Enums.MenuList.MasterVehicleSpect)
        {
            _VehicleSpectBLL = VehicleSpectBLL;
            _settingBLL = SettingBll;
            _mainMenu = Enums.MenuList.MasterData;
        }

        //
        // GET: /VehicleSpect/

        public ActionResult Index()
        {
            if (!System.IO.Directory.Exists(Server.MapPath("~" + pathUpload)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath("~" + pathUpload));
            }

            var data = _VehicleSpectBLL.GetVehicleSpect();
            var model = new VehicleSpectModel();
            model.Details = Mapper.Map<List<VehicleSpectItem>>(data);
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

        public VehicleSpectItem initCreate()
        {
            var model = new VehicleSpectItem();
            //var list1 = new List<SelectListItem>
            //{
            //    new SelectListItem { Text = "MPV", Value = "MPV" },
            //    new SelectListItem { Text = "SUV", Value = "SUV" },
            //    new SelectListItem { Text = "Forklift", Value = "Forklift" },
            //    new SelectListItem { Text = "Motorcycle", Value = "Motorcycle" },
            //    new SelectListItem { Text = "Truck", Value = "Truck" }
            //};

            var list1 = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.BodyType) && x.IsActive).ToList();
            model.BodyTypeList = new SelectList(list1, "SettingValue", "SettingValue");

            var list2 = new List<SelectListItem>
            {
                new SelectListItem { Text = "0", Value = "0" },
                new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" }
            };
            model.GroupLevelList = new SelectList(list2, "Value", "Text");

            var list3 =
                _settingBLL.GetSetting()
                    .Where(
                        x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.Transmission) && x.IsActive)
                    .ToList();

            model.TransmissionList = new SelectList(list3, "SettingValue", "SettingValue");

            var list4 = _settingBLL.GetSetting()
                    .Where(
                        x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.FuelType) && x.IsActive)
                    .ToList();
            model.FuelTypeList = new SelectList(list4, "SettingValue", "SettingValue");

            return model;
        }

        public ActionResult Create()
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Index");
            }
            var model = initCreate();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(VehicleSpectItem Model, HttpPostedFileBase image)
        {
            //try
            //{
                if (ModelState.IsValid)
                {
                    var dto = Mapper.Map<VehicleSpectDto>(Model);
                    dto.CreatedDate = DateTime.Now;
                    dto.CreatedBy = CurrentUser.USERNAME; 
                    dto.ModifiedDate = null;

                    if (Model.ImageFile != null)
                    {
                        string imagename = System.IO.Path.GetFileName(Model.ImageFile.FileName);
                        Model.ImageFile.SaveAs(Server.MapPath("~" + pathUpload + imagename));
                        string filepathtosave = pathUpload + imagename;
                        dto.Image = imagename;
                    }
                    else
                    {
                        dto.Image = "noimage.jpeg";
                    }

                    _VehicleSpectBLL.Save(dto);
                }
            //}
            //catch (Exception)
            //{
            //    var model = initCreate();
            //    return View(model);
            //}

            return RedirectToAction("Index", "MstVehicleSpect");
        }


        public VehicleSpectItem initEdit(VehicleSpectItem model)
        {
            
            var list1 = _settingBLL.GetSetting().Where(x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.BodyType) && x.IsActive).ToList();
            model.BodyTypeList = new SelectList(list1, "SettingValue", "SettingValue",model.BodyType);

            var list2 = new List<SelectListItem>
            {
                new SelectListItem { Text = "0", Value = "0" },
                new SelectListItem { Text = "1", Value = "1" },
                new SelectListItem { Text = "2", Value = "2" },
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "4", Value = "4" }
            };
            model.GroupLevelList = new SelectList(list2, "Value", "Text", model.GroupLevel);

            var list3 =
                _settingBLL.GetSetting()
                    .Where(
                        x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.Transmission) && x.IsActive)
                    .ToList();
            
            model.TransmissionList = new SelectList(list3, "SettingValue", "SettingValue", model.Transmission);

            var list4 = _settingBLL.GetSetting()
                    .Where(
                        x => x.SettingGroup == EnumHelper.GetDescription(Enums.SettingGroup.FuelType) && x.IsActive)
                    .ToList();
            model.FuelTypeList = new SelectList(list4, "SettingValue", "SettingValue", model.FuelTypeSpect);

            return model;
        }

        public ActionResult View(int MstVehicleSpectId)
        {
            var data = _VehicleSpectBLL.GetVehicleSpectById(MstVehicleSpectId);
            var model = new VehicleSpectItem();
            model = Mapper.Map<VehicleSpectItem>(data);
            model.MainMenu = _mainMenu;
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            model.ImageS = model.Image;
            model.Image = webRootUrl + pathUpload + model.Image;
            model = initEdit(model);
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterVehicleSpect, MstVehicleSpectId);
            return View(model);
        }

        public ActionResult Edit(int MstVehicleSpectId)
        {
            if (CurrentUser.UserRole == Enums.UserRole.Viewer)
            {
                return RedirectToAction("Index");
            }
            var data = _VehicleSpectBLL.GetVehicleSpectById(MstVehicleSpectId);
            var model = new VehicleSpectItem();
            model = Mapper.Map<VehicleSpectItem>(data);
            model.MainMenu = _mainMenu;
            model = initEdit(model);
            var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
            model.ImageS = model.Image;
            model.Image = webRootUrl + pathUpload + model.Image;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterVehicleSpect, MstVehicleSpectId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(VehicleSpectItem model, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var data = Mapper.Map<VehicleSpectDto>(model);
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = CurrentUser.USERNAME;
                if (model.ImageFile != null)
                {
                    string imagename = System.IO.Path.GetFileName(model.ImageFile.FileName);
                    //Uri.
                    model.ImageFile.SaveAs(Server.MapPath("~" + pathUpload + imagename));
                    string filepathtosave = VirtualPathUtility.ToAbsolute(pathUpload) + imagename;
                    data.Image = imagename;
                }
                else
                {
                    data.Image = model.ImageS;
                }
                try
                {
                    _VehicleSpectBLL.Save(data, CurrentUser);
                    AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                }
                catch (Exception exception)
                {
                    AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                    model.MainMenu = _mainMenu;
                    model = initEdit(model);
                    var webRootUrl = ConfigurationManager.AppSettings["WebRootUrl"];
                    model.Image = webRootUrl + pathUpload + model.Image;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstVehicleSpect");
        }

        public ActionResult Upload()
        {
            var model = new VehicleSpectModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }


        [HttpPost]
        public ActionResult Upload(VehicleSpectModel Model)
        {
            if (ModelState.IsValid)
            {
                foreach (VehicleSpectItem data in Model.Details)
                {
                    try
                    {
                        string imagename = "";
                        if (data.ImageFile != null)
                        {
                            imagename = System.IO.Path.GetFileName(data.ImageFile.FileName);
                            data.Image = imagename;
                            data.ImageFile.SaveAs(Server.MapPath("~" + pathUpload + imagename));
                            //string filepathtosave = VirtualPathUtility.ToAbsolute(pathUpload) + imagename;
                            data.Image = imagename;
                        }
                        //Stream imageStream = System.IO.File.Open(data.Image, FileMode.Open);
                        //byte[] imageBytes;
                        //using (BinaryReader br = new BinaryReader(imageStream))
                        //{
                        //    imageBytes = br.ReadBytes(500000);
                        //    br.Close();
                        //}
                        //System.IO.File.WriteAllBytes(Server.MapPath("~/files_upload/" + imagename), imageBytes);
                        //data.Image = imagename;

                        data.CreatedDate = DateTime.Now;
                        data.CreatedBy = CurrentUser.USERNAME;
                        data.ModifiedDate = null;
                        data.IsActive = true;
                        var dto = Mapper.Map<VehicleSpectDto>(data);
                        
                        _VehicleSpectBLL.Save(dto);
                        AddMessageInfo(Constans.SubmitMessage.Saved, Enums.MessageInfoType.Success);
                    }
                    catch (Exception exception)
                    {
                        Model.MainMenu = _mainMenu;
                        Model.CurrentLogin = CurrentUser;
                        Model.ErrorMessage = exception.Message;
                        AddMessageInfo(exception.Message, Enums.MessageInfoType.Error);
                        return View(Model);
                    }
                }
            }
            return RedirectToAction("Index", "MstVehicleSpect");
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var qtyPacked = string.Empty;
            var qty = string.Empty;

            var data = (new ExcelReader()).ReadExcel(upload);
            var model = new List<VehicleSpectItem>();
            if (data != null)
            {
                foreach (var dataRow in data.DataRows)
                {
                    if (dataRow[0] == "")
                    {
                        continue;
                    }
                    if(dataRow[0] == "Manufacturer")
                    {
                        continue;
                    }
                    var item = new VehicleSpectItem();
                    item.Manufacturer = dataRow[0].ToString();
                    item.Models = dataRow[1].ToString();
                    item.Series = dataRow[2].ToString();
                    item.Transmission = dataRow[3].ToString();
                    item.FuelTypeSpect = dataRow[4].ToString();
                    item.BodyType = dataRow[5].ToString();
                    item.Year = Convert.ToInt32(dataRow[6].ToString());
                    item.Colour = dataRow[7].ToString();
                    item.GroupLevel = Convert.ToInt32(dataRow[8].ToString());
                    item.FlexPoint = Convert.ToInt32(dataRow[9].ToString());
                    item.Image = dataRow[10].ToString();
                    item.IsActive = dataRow[15].ToString() == "Active"? true : false;
                    item.IsActiveS = dataRow[15].ToString();
                    
                    string message = "";
                    var dto = Mapper.Map<VehicleSpectDto>(item);
                    _VehicleSpectBLL.ValidateSpect(dto, out message);
                    item.Message = message;
                    model.Add(item);
                }
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult UploadImageBatch(HttpPostedFileBase file)
        {
            var model = new VehicleSpectModel();
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            
            if (file != null)
            {



                if (file.ContentType != "application/zip")
                {


                    model.ErrorMessage = "Only zip file allowed (.zip)";
                    
                }
                else
                {
                    try
                    {
                        using (ZipArchive archive = new ZipArchive(file.InputStream, ZipArchiveMode.Read))
                        {
                            foreach (var entry in archive.Entries)
                            {
                                var destPath = Server.MapPath("~" + pathUpload + entry.FullName);
                                var destStream = new FileStream(destPath, FileMode.Create);

                                entry.Open().CopyTo(destStream);
                                destStream.Close();

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        model.ErrorMessage = ex.Message;
                    }
                    
                    
                    
                    
                }
            }
            else
            {
                model.ErrorMessage = "Please upload a file.";
            }


            
            
            return View("Upload", model);
        }

        public string ExportMasterVehicleSpect()
        {
            string pathFile = "";
            pathFile = CreateXlsMasterVehicleSpect();
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
        private string CreateXlsMasterVehicleSpect()
        {
            //get data
            List<VehicleSpectDto> vendor = _VehicleSpectBLL.GetVehicleSpect();
            var listData = Mapper.Map<List<VehicleSpectItem>>(vendor);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master Vehicle Spect");
            slDocument.MergeWorksheetCells(1, 1, 1, 16);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterVehicleSpect(slDocument);

            //create data
            slDocument = CreateDataExcelMasterVehicleSpect(slDocument, listData);

            var fileName = "Master_Data_Vehicle_Spect" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterVehicleSpect(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Manufacturer");
            slDocument.SetCellValue(iRow, 2, "Model");
            slDocument.SetCellValue(iRow, 3, "Series");
            slDocument.SetCellValue(iRow, 4, "Transmission");
            slDocument.SetCellValue(iRow, 5, "Fuel Type");
            slDocument.SetCellValue(iRow, 6, "Body Type");
            slDocument.SetCellValue(iRow, 7, "Request Year");
            slDocument.SetCellValue(iRow, 8, "Colour");
            slDocument.SetCellValue(iRow, 9, "Group Level");
            slDocument.SetCellValue(iRow, 10, "Flex Point");
            slDocument.SetCellValue(iRow, 11, "Image Name");
            slDocument.SetCellValue(iRow, 12, "Created Date");
            slDocument.SetCellValue(iRow, 13, "Created By");
            slDocument.SetCellValue(iRow, 14, "Modified Date");
            slDocument.SetCellValue(iRow, 15, "Modified By");
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

        private SLDocument CreateDataExcelMasterVehicleSpect(SLDocument slDocument, List<VehicleSpectItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.Manufacturer);
                slDocument.SetCellValue(iRow, 2, data.Models);
                slDocument.SetCellValue(iRow, 3, data.Series);
                slDocument.SetCellValue(iRow, 4, data.Transmission);
                slDocument.SetCellValue(iRow, 5, data.FuelTypeSpect);
                slDocument.SetCellValue(iRow, 6, data.BodyType);
                slDocument.SetCellValue(iRow, 7, data.Year);
                slDocument.SetCellValue(iRow, 8, data.Colour);
                slDocument.SetCellValue(iRow, 9, data.GroupLevel);
                slDocument.SetCellValue(iRow, 10, data.FlexPoint);
                slDocument.SetCellValue(iRow, 11, data.Image);
                slDocument.SetCellValue(iRow, 12, data.CreatedDate.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 13, data.CreatedBy);
                slDocument.SetCellValue(iRow, 14, data == null ? "" : data.ModifiedDate.Value.ToString("dd-MMM-yyyy HH:mm:ss"));
                slDocument.SetCellValue(iRow, 15, data.ModifiedBy);
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

            slDocument.AutoFitColumn(1, 8);
            slDocument.SetCellStyle(3, 1, iRow - 1, 16, valueStyle);

            return slDocument;
        }

    }
}
