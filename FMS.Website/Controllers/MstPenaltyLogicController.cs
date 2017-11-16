using FMS.Contract.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FMS.Core;
using FMS.Website.Models;
using System.Data;
using FMS.BusinessObject.Dto;
using AutoMapper;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace FMS.Website.Controllers
{
    public class MstPenaltyLogicController : BaseController
    {
        private IPenaltyLogicBLL _penaltyLogicBLL;
        private IPageBLL _pageBLL;
        private IVendorBLL _vendorBLL;
        private Enums.MenuList _mainMenu;
        public MstPenaltyLogicController(IPageBLL PageBll, IPenaltyLogicBLL PenaltyLogicBLL, IVendorBLL VendorBLL) : base(PageBll, Enums.MenuList.MasterComplaintCategory)
        {
            _penaltyLogicBLL = PenaltyLogicBLL;
            _vendorBLL = VendorBLL;
            _pageBLL = PageBll;
            _mainMenu = Enums.MenuList.MasterData;
        }
        public ActionResult Index()
        {
            var data = _penaltyLogicBLL.GetPenaltyLogic();
            var model = new PenaltyLogicModel();
            model.Details = Mapper.Map<List<PenaltyLogicItem>>(data);
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }
        public ActionResult Create()
        {
            var model = new PenaltyLogicItem();
            var Kolomlist = new List<SelectListItem>
            {
                new SelectListItem { Text = "Penalty", Value = "MST_PENALTY.PENALTY"},
                new SelectListItem { Text = "HMS Price", Value = "MST_PRICELIST.INSTALLMEN_HMS" },
                new SelectListItem { Text = "EMP Price", Value = "MST_PRICELIST.INSTALLMEN_EMP" },
                new SelectListItem { Text = "VPrice", Value = "MST_PRICELIST.PRICE" }
            };
            model.KolomList = new SelectList(Kolomlist, "Value", "Text");
            var Operatorlist = new List<SelectListItem>
            {
                new SelectListItem { Text = "+", Value = "+"},
                new SelectListItem { Text = "-", Value = "-" },
                new SelectListItem { Text = "/", Value = "/" },
                new SelectListItem { Text = "*", Value = "*" },
                new SelectListItem { Text = "(", Value = "(" },
                new SelectListItem { Text = ")", Value = ")" }
            };
            model.OperatorList = new SelectList(Operatorlist, "Value", "Text");

            var VendorList = _vendorBLL.GetVendor();
            model.VendorList = new SelectList(VendorList, "MstVendorId", "VendorName");

            model.KolomList = new SelectList(Kolomlist, "Value", "Text");
            var VehicleTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "WTC", Value = "BENEFIT"},
                new SelectListItem { Text = "BENEFIT", Value = "BENEFIT" }
            };
            model.VehicleTypeList = new SelectList(VehicleTypeList, "Value", "Text");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(PenaltyLogicItem model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var cek = new DataTable().Compute(model.CekRumus, null).ToString();
                    var Dto = Mapper.Map<PenaltyLogicDto>(model);
                    Dto.CreatedBy = CurrentUser.USERNAME; ;
                    Dto.CreatedDate = DateTime.Now;
                    Dto.IsActive = true;

                    _penaltyLogicBLL.Save(Dto);
                }
                catch (Exception exp)
                {
                    var msg = exp.Message;
                    var Kolomlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Penalty", Value = "MST_PENALTY.PENALTY"},
                        new SelectListItem { Text = "HMS Price", Value = "MST_PRICELIST.INSTALLMEN_HMS" },
                        new SelectListItem { Text = "EMP Price", Value = "MST_PRICELIST.INSTALLMEN_EMP" },
                        new SelectListItem { Text = "VPrice", Value = "MST_PRICELIST.PRICE" }
                    };
                    model.KolomList = new SelectList(Kolomlist, "Value", "Text");
                    var Operatorlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "+", Value = "+"},
                        new SelectListItem { Text = "-", Value = "-" },
                        new SelectListItem { Text = "/", Value = "/" },
                        new SelectListItem { Text = "*", Value = "*" },
                        new SelectListItem { Text = "(", Value = "(" },
                        new SelectListItem { Text = ")", Value = ")" }
                    };
                    model.OperatorList = new SelectList(Operatorlist, "Value", "Text");
                    var VendorList = _vendorBLL.GetVendor();
                    model.VendorList = new SelectList(VendorList, "MstVendorId", "VendorName");

                    model.KolomList = new SelectList(Kolomlist, "Value", "Text");
                    var VehicleTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "WTC", Value = "BENEFIT"},
                new SelectListItem { Text = "BENEFIT", Value = "BENEFIT" }
            };
                    model.VehicleTypeList = new SelectList(VehicleTypeList, "Value", "Text");
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
                     
            }
            return RedirectToAction("Index", "MstPenaltyLogic");
        }

        public ActionResult Edit(int MstPenaltyLogicId)
        {
            var data = _penaltyLogicBLL.GetPenaltyLogicById(MstPenaltyLogicId);
            var model = Mapper.Map<PenaltyLogicItem>(data);
            var Kolomlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Penalty", Value = "MST_PENALTY.PENALTY"},
                        new SelectListItem { Text = "HMS Price", Value = "MST_PRICELIST.INSTALLMEN_HMS" },
                        new SelectListItem { Text = "EMP Price", Value = "MST_PRICELIST.INSTALLMEN_EMP" },
                        new SelectListItem { Text = "VPrice", Value = "MST_PRICELIST.PRICE" }
                    };
            model.KolomList = new SelectList(Kolomlist, "Value", "Text");
            var Operatorlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "+", Value = "+"},
                        new SelectListItem { Text = "-", Value = "-" },
                        new SelectListItem { Text = "/", Value = "/" },
                        new SelectListItem { Text = "*", Value = "*" },
                        new SelectListItem { Text = "(", Value = "(" },
                        new SelectListItem { Text = ")", Value = ")" }
                    };
            model.OperatorList = new SelectList(Operatorlist, "Value", "Text");
            var VendorList = _vendorBLL.GetVendor();
            model.VendorList = new SelectList(VendorList, "MstVendorId", "VendorName");

            model.KolomList = new SelectList(Kolomlist, "Value", "Text");
            var VehicleTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "WTC", Value = "BENEFIT"},
                new SelectListItem { Text = "BENEFIT", Value = "BENEFIT" }
            };
            model.VehicleTypeList = new SelectList(VehicleTypeList, "Value", "Text");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterPenaltyLogic, MstPenaltyLogicId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(PenaltyLogicItem model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var cek = new DataTable().Compute(model.CekRumus, null).ToString();
                    var Dto = Mapper.Map<PenaltyLogicDto>(model);
                    Dto.ModifiedBy = CurrentUser.USERNAME; ;
                    Dto.ModifiedDate = DateTime.Now;

                    _penaltyLogicBLL.Save(Dto, CurrentUser);
                }
                catch (Exception)
                {
                    var Kolomlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Penalty", Value = "MST_PENALTY.PENALTY"},
                        new SelectListItem { Text = "HMS Price", Value = "MST_PRICELIST.INSTALLMEN_HMS" },
                        new SelectListItem { Text = "EMP Price", Value = "MST_PRICELIST.INSTALLMEN_EMP" },
                        new SelectListItem { Text = "VPrice", Value = "MST_PRICELIST.PRICE" }
                    };
                    model.KolomList = new SelectList(Kolomlist, "Value", "Text");
                    var Operatorlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "+", Value = "+"},
                        new SelectListItem { Text = "-", Value = "-" },
                        new SelectListItem { Text = "/", Value = "/" },
                        new SelectListItem { Text = "*", Value = "*" },
                        new SelectListItem { Text = "(", Value = "(" },
                        new SelectListItem { Text = ")", Value = ")" }
                    };
                    model.OperatorList = new SelectList(Operatorlist, "Value", "Text");
                    model.MainMenu = _mainMenu;
                    model.CurrentLogin = CurrentUser;
                    return View(model);
                }
            }
            return RedirectToAction("Index", "MstPenaltyLogic");
        }
        
        public ActionResult Detail(int MstPenaltyLogicId)
        {
            var data = _penaltyLogicBLL.GetPenaltyLogicById(MstPenaltyLogicId);
            var model = Mapper.Map<PenaltyLogicItem>(data);
            var Kolomlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Penalty", Value = "MST_PENALTY.PENALTY"},
                        new SelectListItem { Text = "HMS Price", Value = "MST_PRICELIST.INSTALLMEN_HMS" },
                        new SelectListItem { Text = "EMP Price", Value = "MST_PRICELIST.INSTALLMEN_EMP" },
                        new SelectListItem { Text = "VPrice", Value = "MST_PRICELIST.PRICE" }
                    };
            model.KolomList = new SelectList(Kolomlist, "Value", "Text");
            var Operatorlist = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "+", Value = "+"},
                        new SelectListItem { Text = "-", Value = "-" },
                        new SelectListItem { Text = "/", Value = "/" },
                        new SelectListItem { Text = "*", Value = "*" },
                        new SelectListItem { Text = "(", Value = "(" },
                        new SelectListItem { Text = ")", Value = ")" }
                    };
            model.OperatorList = new SelectList(Operatorlist, "Value", "Text");
            var VendorList = _vendorBLL.GetVendor();
            model.VendorList = new SelectList(VendorList, "MstVendorId", "VendorName");

            model.KolomList = new SelectList(Kolomlist, "Value", "Text");
            var VehicleTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "WTC", Value = "BENEFIT"},
                new SelectListItem { Text = "BENEFIT", Value = "BENEFIT" }
            };
            model.VehicleTypeList = new SelectList(VehicleTypeList, "Value", "Text");
            model.MainMenu = _mainMenu;
            model.CurrentLogin = CurrentUser;
            model.ChangesLogs = GetChangesHistory((int)Enums.MenuList.MasterPenaltyLogic, MstPenaltyLogicId);
            return View(model);
        }
        #region export xls
        public void ExportMasterPenaltyLogic()
        {
            string pathFile = "";

            pathFile = CreateXlsMasterPenaltyLogic();

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

        private string CreateXlsMasterPenaltyLogic()
        {
            //get data
            List<PenaltyLogicDto> PenaltyLogic = _penaltyLogicBLL.GetPenaltyLogic();
            var listData = Mapper.Map<List<PenaltyLogicItem>>(PenaltyLogic);

            var slDocument = new SLDocument();

            //title
            slDocument.SetCellValue(1, 1, "Master PenaltyLogic");
            slDocument.MergeWorksheetCells(1, 1, 1, 7);
            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.SetHorizontalAlignment(HorizontalAlignmentValues.Center);
            valueStyle.Font.Bold = true;
            valueStyle.Font.FontSize = 18;
            slDocument.SetCellStyle(1, 1, valueStyle);

            //create header
            slDocument = CreateHeaderExcelMasterPenaltyLogic(slDocument);

            //create data
            slDocument = CreateDataExcelMasterPenaltyLogic(slDocument, listData);

            var fileName = "Master_Data_PenaltyLogic" + DateTime.Now.ToString("_yyyyMMddHHmmss") + ".xlsx";
            var path = Path.Combine(Server.MapPath(Constans.UploadPath), fileName);

            slDocument.SaveAs(path);

            return path;

        }

        private SLDocument CreateHeaderExcelMasterPenaltyLogic(SLDocument slDocument)
        {
            int iRow = 2;

            slDocument.SetCellValue(iRow, 1, "Funtion Name");
            slDocument.SetCellValue(iRow, 2, "Cost Center");
            slDocument.SetCellValue(iRow, 3, "Created Date");
            slDocument.SetCellValue(iRow, 4, "Created By");
            slDocument.SetCellValue(iRow, 5, "Modified Date");
            slDocument.SetCellValue(iRow, 6, "Modified By");
            slDocument.SetCellValue(iRow, 7, "Status");

            SLStyle headerStyle = slDocument.CreateStyle();
            headerStyle.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            headerStyle.Font.Bold = true;
            headerStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            headerStyle.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);

            slDocument.SetCellStyle(iRow, 1, iRow, 7, headerStyle);

            return slDocument;

        }

        private SLDocument CreateDataExcelMasterPenaltyLogic(SLDocument slDocument, List<PenaltyLogicItem> listData)
        {
            int iRow = 3; //starting row data

            foreach (var data in listData)
            {
                slDocument.SetCellValue(iRow, 1, data.PenaltyLogic);
                slDocument.SetCellValue(iRow, 2, data.Year);
                slDocument.SetCellValue(iRow, 3, data.CreatedDate.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 4, data.CreatedBy);
                slDocument.SetCellValue(iRow, 5, data.ModifiedDate == null ? "" : data.ModifiedDate.Value.ToString("dd - MM - yyyy hh: mm"));
                slDocument.SetCellValue(iRow, 6, data.ModifiedBy);
                slDocument.SetCellValue(iRow, 7, data.IsActive == true ? "Active" : "InActive");
                iRow++;
            }

            //create style
            SLStyle valueStyle = slDocument.CreateStyle();
            valueStyle.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
            valueStyle.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;

            slDocument.AutoFitColumn(1, 7);
            slDocument.SetCellStyle(3, 1, iRow - 1, 7, valueStyle);

            return slDocument;
        }

        #endregion
    }
}
