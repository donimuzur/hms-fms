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
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class OdometerDto
    {
        public int ID { get; set; }
        public string REGION { get; set; }
        public string FUNCTION { get; set; }
        public string VEHCILE_TYPE { get; set; }
        public decimal? TOTAL_KM { get; set; }
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
        public int? REPORT_MONTH { get; set; }
        public int? REPORT_YEAR { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}
