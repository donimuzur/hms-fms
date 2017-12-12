using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
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
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
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

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ALL", Value = "0" },
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
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
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

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ALL", Value = "0" },
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
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
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
                    new SelectListItem() {Text = "ALL", Value = "0" },
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
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
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

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ALL", Value = "0" },
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
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
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

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ALL", Value = "0" },
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
            SearchView.YearFrom = DateTime.Now.Year;
            SearchView.YearTo = DateTime.Now.Year;
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

        public SelectList MonthList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "ALL", Value = "0" },
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
    }

    #endregion
}