using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Dto
{
    public class NoVehicleDto
    {
        public int ID { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string SUPPLY_METHOD { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public int? NO_OF_VEHICLE { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT { get; set; }
        public int? NO_OF_VEHICLE_WTC { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class NoVehicleWtcDto
    {
        public int ID { get; set; }
        public string REGIONAL { get; set; }
        public string FUNCTION { get; set; }
        public int? NO_OF_VEHICLE { get; set; }
        public int? NO_OF_VEHICLE_SALES { get; set; }
        public int? NO_OF_VEHICLE_MARKETING { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class NoVehicleMakeDto
    {
        public int ID { get; set; }
        public string MANUFACTURER { get; set; }
        public string BODY_TYPE { get; set; }
        public int? NO_OF_VEHICLE { get; set; }
        public int? NO_OF_VEHICLE_FIRST { get; set; }
        public int? NO_OF_VEHICLE_ELSE { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class OdometerDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public decimal? TOTAL_KM { get; set; }
        public decimal? TOTAL_KM_BENEFIT { get; set; }
        public decimal? TOTAL_KM_WTC { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class LiterByFunctionDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public decimal? TOTAL_LITER { get; set; }
        public decimal? TOTAL_LITER_BENEFIT { get; set; }
        public decimal? TOTAL_LITER_WTC { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class FuelCostByFunctionDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public decimal? TOTAL_FUEL_COST { get; set; }
        public decimal? TOTAL_FUEL_COST_BENEFIT { get; set; }
        public decimal? TOTAL_FUEL_COST_WTC { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class LeaseCostByFunctionDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public decimal? TOTAL_LEASE_COST { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class SalesByRegionDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public decimal? TOTAL_KM { get; set; }
        public decimal? TOTAL_COST { get; set; }
        public decimal? STICK { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class AccidentDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public int? ACCIDENT_COUNT { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class AcVsObDto
    {
        public int ID { get; set; }
        public string FUNCTION { get; set; }
        public decimal? ACTUAL_COST { get; set; }
        public decimal? COST_OB { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class SumPtdByFunctionDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public int? TOTAL_VEHICLE { get; set; }
        public decimal? TOTAL_VEHICLE_COST { get; set; }
        public decimal? TOTAL_FUEL_AMOUNT { get; set; }
        public int? TOTAL_FUEL_COST { get; set; }
        public decimal? TOTAL_KM { get; set; }
        public decimal? TOTAL_OPERATIONAL_COST { get; set; }
        public int? ACCIDENT_COUNT { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}
