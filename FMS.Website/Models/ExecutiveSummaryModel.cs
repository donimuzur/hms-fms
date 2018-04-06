using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    #region --------- Summary All --------------

    public class SummaryAllModel : BaseModel
    {
        public SummaryAllModel()
        {
            NoVehicleList = new List<NoVehicleData>();
            NoVehicleWtcList = new List<NoVehicleWtcData>();
            NoVehicleMakeList = new List<NoVehicleMakeData>();
            OdometerDataList = new List<OdometerData>();
            LiterByFuncDataList = new List<LiterByFunctionData>();
            FuelCostByFuncDataList = new List<FuelCostByFunctionData>();
            LeaseCostByFuncDataList = new List<LeaseCostByFunctionData>();
            SalesByRegionDataList = new List<SalesByRegionData>();
            AccidentDataList = new List<AccidentData>();
            AcVsObDataList = new List<AcVsObData>();
            SearchViewExport = new SummarySearchViewExport();
            SearchView = new SummarySearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
        }

        public bool IsByRegion { get; set; }
        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<NoVehicleData> NoVehicleList { get; set; }
        public List<NoVehicleWtcData> NoVehicleWtcList { get; set; }
        public List<NoVehicleMakeData> NoVehicleMakeList { get; set; }
        public List<OdometerData> OdometerDataList { get; set; }
        public List<LiterByFunctionData> LiterByFuncDataList { get; set; }
        public List<FuelCostByFunctionData> FuelCostByFuncDataList { get; set; }
        public List<LeaseCostByFunctionData> LeaseCostByFuncDataList { get; set; }
        public List<SalesByRegionData> SalesByRegionDataList { get; set; }
        public List<AccidentData> AccidentDataList { get; set; }
        public List<AcVsObData> AcVsObDataList { get; set; }
        public SummarySearchView SearchView { get; set; }
        public SummarySearchViewExport SearchViewExport { get; set; }
    }

    public class SummarySearchView
    {
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }
    }

    public class SummarySearchViewExport
    {
        public int MonthFrom { get; set; }
        public int YearFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearTo { get; set; }
    }

    #endregion

    #region --------- Number Of Vehicle --------------

    public class ExecutiveSummaryModel : BaseModel
    {
        public ExecutiveSummaryModel()
        {
            NoVehicleList = new List<NoVehicleData>();
            SearchViewExport = new VehicleSearchViewExport();
            SearchView = new VehicleSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<NoVehicleData> NoVehicleList { get; set; }
        public VehicleSearchView SearchView { get; set; }
        public VehicleSearchViewExport SearchViewExport { get; set; }
    }

    public class NoVehicleData
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string Regional { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }
        public int? NoOfVehicle { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class VehicleSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Regional { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList Functions { get; set; }
        public MultiSelectList ZoneList { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList VehicleTypeList { get; set; }
        public SelectList SupplyMethodList { get; set; }
        public SelectList RegionalList { get; set; }
    }

    public class VehicleSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Regional { get; set; }
        public string SupplyMethod { get; set; }
        public string Function { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- Number Of Vehicle WTC --------------

    public class NumberVehicleWtcModel : BaseModel
    {
        public NumberVehicleWtcModel()
        {
            NoVehicleWtcList = new List<NoVehicleWtcData>();
            SearchViewExport = new VehicleSearchViewExportWtc();
            SearchView = new VehicleSearchViewWtc();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<NoVehicleWtcData> NoVehicleWtcList { get; set; }
        public VehicleSearchViewWtc SearchView { get; set; }
        public VehicleSearchViewExportWtc SearchViewExport { get; set; }
    }

    public class NoVehicleWtcData
    {
        public int Id { get; set; }
        public string Regional { get; set; }
        public string Function { get; set; }
        public int? NoOfVehicle { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class VehicleSearchViewWtc
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Regional { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList ZoneList { get; set; }
        public MultiSelectList Functions { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
    }

    public class VehicleSearchViewExportWtc
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Regional { get; set; }
        public string Function { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- Number Of Vehicle Make --------------

    public class NumberVehicleMakeModel : BaseModel
    {
        public NumberVehicleMakeModel()
        {
            NoVehicleMakeList = new List<NoVehicleMakeData>();
            SearchViewExport = new VehicleSearchViewExportMake();
            SearchView = new VehicleSearchViewMake();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<NoVehicleMakeData> NoVehicleMakeList { get; set; }
        public VehicleSearchViewMake SearchView { get; set; }
        public VehicleSearchViewExportMake SearchViewExport { get; set; }
    }

    public class NoVehicleMakeData
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string BodyType { get; set; }
        public int? NoOfVehicle { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class VehicleSearchViewMake
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Manufacturer { get; set; }
        public string BodyType { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }
    }

    public class VehicleSearchViewExportMake
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Manufacturer { get; set; }
        public string BodyType { get; set; }
    }

    #endregion

    #region --------- Odometer --------------

    public class OdometerModel : BaseModel
    {
        public OdometerModel()
        {
            OdometerDataList = new List<OdometerData>();
            SearchViewExport = new OdometerSearchViewExport();
            SearchView = new OdometerSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<OdometerData> OdometerDataList { get; set; }
        public OdometerSearchView SearchView { get; set; }
        public OdometerSearchViewExport SearchViewExport { get; set; }
    }

    public class OdometerData
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public decimal? TotalKm { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class OdometerSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList Functions { get; set; }
        public MultiSelectList ZoneList { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

    public class OdometerSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- Liter By Function --------------

    public class LiterByFunctionModel : BaseModel
    {
        public LiterByFunctionModel()
        {
            LiterByFuncDataList = new List<LiterByFunctionData>();
            SearchViewExport = new LiterByFuncSearchViewExport();
            SearchView = new LiterByFuncSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<LiterByFunctionData> LiterByFuncDataList { get; set; }
        public LiterByFuncSearchView SearchView { get; set; }
        public LiterByFuncSearchViewExport SearchViewExport { get; set; }
    }

    public class LiterByFunctionData
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public decimal? TotalLiter { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class LiterByFuncSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList Functions { get; set; }
        public MultiSelectList ZoneList { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

    public class LiterByFuncSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- Fuel Cost By Function --------------

    public class FuelCostByFunctionModel : BaseModel
    {
        public FuelCostByFunctionModel()
        {
            FuelCostByFuncDataList = new List<FuelCostByFunctionData>();
            SearchViewExport = new FuelCostByFuncSearchViewExport();
            SearchView = new FuelCostByFuncSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<FuelCostByFunctionData> FuelCostByFuncDataList { get; set; }
        public FuelCostByFuncSearchView SearchView { get; set; }
        public FuelCostByFuncSearchViewExport SearchViewExport { get; set; }
    }

    public class FuelCostByFunctionData
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public decimal? TotalFuelCost { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class FuelCostByFuncSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList Functions { get; set; }
        public MultiSelectList ZoneList { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

    public class FuelCostByFuncSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- Lease Cost By Function --------------

    public class LeaseCostByFunctionModel : BaseModel
    {
        public LeaseCostByFunctionModel()
        {
            LeaseCostByFuncDataList = new List<LeaseCostByFunctionData>();
            SearchViewExport = new LeaseCostByFuncSearchViewExport();
            SearchView = new LeaseCostByFuncSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<LeaseCostByFunctionData> LeaseCostByFuncDataList { get; set; }
        public LeaseCostByFuncSearchView SearchView { get; set; }
        public LeaseCostByFuncSearchViewExport SearchViewExport { get; set; }
    }

    public class LeaseCostByFunctionData
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public decimal? TotalLeaseCost { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class LeaseCostByFuncSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList Functions { get; set; }
        public MultiSelectList ZoneList { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
    }

    public class LeaseCostByFuncSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- Sales By Region --------------

    public class SalesByRegionModel : BaseModel
    {
        public SalesByRegionModel()
        {
            SalesByRegionDataList = new List<SalesByRegionData>();
            SearchViewExport = new SalesByRegionSearchViewExport();
            SearchView = new SalesByRegionSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<SalesByRegionData> SalesByRegionDataList { get; set; }
        public SalesByRegionSearchView SearchView { get; set; }
        public SalesByRegionSearchViewExport SearchViewExport { get; set; }
    }

    public class SalesByRegionData
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public decimal? TotalKm { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? Stick { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class SalesByRegionSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList ZoneList { get; set; }
        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
    }

    public class SalesByRegionSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Region { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- Accident --------------

    public class AccidentModel : BaseModel
    {
        public AccidentModel()
        {
            AccidentDataList = new List<AccidentData>();
            SearchViewExport = new AccidentSearchViewExport();
            SearchView = new AccidentSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<AccidentData> AccidentDataList { get; set; }
        public AccidentSearchView SearchView { get; set; }
        public AccidentSearchViewExport SearchViewExport { get; set; }
    }

    public class AccidentData
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public int? AccidentCount { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AccidentSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }
        public string Zone { get; set; }
        public string ZoneId { get; set; }

        public MultiSelectList Functions { get; set; }
        public MultiSelectList ZoneList { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

    public class AccidentSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string ZoneId { get; set; }
    }

    #endregion

    #region --------- AC Vs OB --------------

    public class AcVsObModel : BaseModel
    {
        public AcVsObModel()
        {
            AcVsObDataList = new List<AcVsObData>();
            SearchViewExport = new AcVsObSearchViewExport();
            SearchView = new AcVsObSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<AcVsObData> AcVsObDataList { get; set; }
        public AcVsObSearchView SearchView { get; set; }
        public AcVsObSearchViewExport SearchViewExport { get; set; }
    }

    public class AcVsObData
    {
        public int Id { get; set; }
        public string Function { get; set; }
        public string VehicleType { get; set; }
        public decimal? ActualCost { get; set; }
        public decimal? CostOb { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public int? Unit { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AcVsObSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }

        public MultiSelectList Functions { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    //new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList VehicleTypeList { get; set; }
    }

    public class AcVsObSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Function { get; set; }
        public string VehicleType { get; set; }
    }

    #endregion

    #region --------- Sum PTD By Function --------------

    public class SumPtdByFunctionModel : BaseModel
    {
        public SumPtdByFunctionModel()
        {
            SumPtdByFuncDataList = new List<SumPtdByFunctionData>();
            SearchViewExport = new SumPtdByFuncSearchViewExport();
            SearchView = new SumPtdByFuncSearchView();
            SearchView.MonthFrom = DateTime.Now.Month;
            SearchView.MonthTo = DateTime.Now.Month;
        }

        public string TitleForm { get; set; }
        public string TitleExport { get; set; }
        public List<SumPtdByFunctionData> SumPtdByFuncDataList { get; set; }
        public SumPtdByFuncSearchView SearchView { get; set; }
        public SumPtdByFuncSearchViewExport SearchViewExport { get; set; }
    }

    public class SumPtdByFunctionData
    {
        public int Id { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public int? TotalVehicle { get; set; }
        public decimal? TotalVehicleCost { get; set; }
        public decimal? TotalFuelAmount { get; set; }
        public int? TotalFuelCost { get; set; }
        public decimal? TotalKm { get; set; }
        public decimal? TotalOperationalCost { get; set; }
        public int? AccidentCount { get; set; }
        public int? ReportMonth { get; set; }
        public string Month { get; set; }
        public int? ReportYear { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class SumPtdByFuncSearchView
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
        public string FunctionId { get; set; }

        public MultiSelectList Functions { get; set; }

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "", Value = "0" },
                    new SelectListItem() {Text = "January", Value = "1" },
                    new SelectListItem() {Text = "February", Value = "2" },
                    new SelectListItem() {Text = "March", Value = "3" },
                    new SelectListItem() {Text = "April", Value = "4" },
                    new SelectListItem() {Text = "May", Value = "5" },
                    new SelectListItem() {Text = "June", Value = "6" },
                    new SelectListItem() {Text = "July", Value = "7" },
                    new SelectListItem() {Text = "August", Value = "8" },
                    new SelectListItem() {Text = "September", Value = "9" },
                    new SelectListItem() {Text = "October", Value = "10" },
                    new SelectListItem() {Text = "November", Value = "11" },
                    new SelectListItem() {Text = "December", Value = "12" }
                };
                return new SelectList(items, "Value", "Text");
            }

        }

        public SelectList RegionalList { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

    public class SumPtdByFuncSearchViewExport
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string VehicleType { get; set; }
        public string Region { get; set; }
        public string Function { get; set; }
    }

    #endregion
}