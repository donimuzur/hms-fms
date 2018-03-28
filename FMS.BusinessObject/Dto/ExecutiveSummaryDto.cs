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
        public int? NO_OF_VEHICLE_BENEFIT_1 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_2 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_3 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_4 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_5 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_6 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_7 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_8 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_9 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_10 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_11 { get; set; }
        public int? NO_OF_VEHICLE_BENEFIT_12 { get; set; }
        public int? NO_OF_VEHICLE_WTC_1 { get; set; }
        public int? NO_OF_VEHICLE_WTC_2 { get; set; }
        public int? NO_OF_VEHICLE_WTC_3 { get; set; }
        public int? NO_OF_VEHICLE_WTC_4 { get; set; }
        public int? NO_OF_VEHICLE_WTC_5 { get; set; }
        public int? NO_OF_VEHICLE_WTC_6 { get; set; }
        public int? NO_OF_VEHICLE_WTC_7 { get; set; }
        public int? NO_OF_VEHICLE_WTC_8 { get; set; }
        public int? NO_OF_VEHICLE_WTC_9 { get; set; }
        public int? NO_OF_VEHICLE_WTC_10 { get; set; }
        public int? NO_OF_VEHICLE_WTC_11 { get; set; }
        public int? NO_OF_VEHICLE_WTC_12 { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
        public string LABEL3 { get; set; }
        public string LABEL4 { get; set; }
        public string LABEL5 { get; set; }
        public string LABEL6 { get; set; }
        public string LABEL7 { get; set; }
        public string LABEL8 { get; set; }
        public string LABEL9 { get; set; }
        public string LABEL10 { get; set; }
        public string LABEL11 { get; set; }
        public string LABEL12 { get; set; }
        public string LABEL13 { get; set; }
        public string LABEL14 { get; set; }
        public string LABEL15 { get; set; }
        public string LABEL16 { get; set; }
        public string LABEL17 { get; set; }
        public string LABEL18 { get; set; }
        public string LABEL19 { get; set; }
        public string LABEL20 { get; set; }
        public string LABEL21 { get; set; }
        public string LABEL22 { get; set; }
        public string LABEL23 { get; set; }
        public string LABEL24 { get; set; }
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

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
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

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
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
        public decimal? TOTAL_KM_SALES { get; set; }
        public decimal? TOTAL_KM_MARKETING { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
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
        public decimal? TOTAL_LITER_SALES { get; set; }
        public decimal? TOTAL_LITER_MARKETING { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
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
        public decimal? TOTAL_FUEL_COST_SALES { get; set; }
        public decimal? TOTAL_FUEL_COST_MARKETING { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
    }

    public class LeaseCostByFunctionDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public decimal? TOTAL_LEASE_COST { get; set; }
        public decimal? TOTAL_LEASE_COST_JAVA { get; set; }
        public decimal? TOTAL_LEASE_COST_ELSE { get; set; }
        public decimal? TOTAL_LEASE_COST_SALES { get; set; }
        public decimal? TOTAL_LEASE_COST_MARKETING { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
    }

    public class FuelLeaseDto
    {
        public string FUNCTION { get; set; }
        public decimal? TOTAL_FUEL_COST { get; set; }
        public decimal? TOTAL_LEASE_COST { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
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

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
    }

    public class AccidentDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public int? ACCIDENT_COUNT { get; set; }
        public int? ACCIDENT_COUNT_BENEFIT { get; set; }
        public int? ACCIDENT_COUNT_WTC { get; set; }
        public int? ACCIDENT_COUNT_SALES { get; set; }
        public int? ACCIDENT_COUNT_MARKETING { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
    }

    public class AcVsObDto
    {
        public int ID { get; set; }
        public string FUNCTION { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public decimal? ACTUAL_COST { get; set; }
        public decimal? ACTUAL_COST_BENEFIT { get; set; }
        public decimal? ACTUAL_COST_WTC { get; set; }
        public decimal? COST_OB { get; set; }
        public decimal? COST_OB_BENEFIT { get; set; }
        public decimal? COST_OB_WTC { get; set; }
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public int? UNIT { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string LABEL1 { get; set; }
        public string LABEL2 { get; set; }
        public string LABEL3 { get; set; }
        public string LABEL4 { get; set; }
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
