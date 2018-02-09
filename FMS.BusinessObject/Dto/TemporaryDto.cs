using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;

namespace FMS.BusinessObject.Dto
{
    public class TemporaryDto
    {
        public long TRA_TEMPORARY_ID { get; set; }
        public string DOCUMENT_NUMBER_TEMP { get; set; }
        public Enums.DocumentStatus DOCUMENT_STATUS { get; set; }
        public string DOCUMENT_NUMBER_RELATED { get; set; }
        public int REASON_ID { get; set; }
        public string REASON_NAME { get; set; }
        public string EMPLOYEE_ID { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string COST_CENTER { get; set; }
        public Nullable<int> GROUP_LEVEL { get; set; }
        public Nullable<int> CAR_GROUP_LEVEL { get; set; }
        public Nullable<long> CFM_IDLE_ID { get; set; }
        public string ACTUAL_GROUP { get; set; }
        public string SUPPLY_METHOD { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string VEHICLE_USAGE { get; set; }
        public string VEHICLE_TYPE_NAME { get; set; }
        public string REGIONAL { get; set; }
        public string POLICE_NUMBER { get; set; }
        public string PROJECT_NAME { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public string BODY_TYPE { get; set; }
        public string COLOR { get; set; }
        public string LOCATION_CITY { get; set; }
        public string LOCATION_ADDRESS { get; set; }
        public string VENDOR_NAME { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string VENDOR_POLICE_NUMBER { get; set; }
        public string VENDOR_CHASIS_NUMBER { get; set; }
        public string VENDOR_ENGINE_NUMBER { get; set; }
        public Nullable<System.DateTime> VENDOR_CONTRACT_START_DATE { get; set; }
        public Nullable<System.DateTime> VENDOR_CONTRACT_END_DATE { get; set; }
        public string VENDOR_MANUFACTURER { get; set; }
        public string VENDOR_MODEL { get; set; }
        public string VENDOR_SERIES { get; set; }
        public string VENDOR_TRANSMISSION { get; set; }
        public string VENDOR_COLOUR { get; set; }
        public string VENDOR_BODY_TYPE { get; set; }
        public Nullable<bool> VENDOR_AIR_BAG { get; set; }
        public Nullable<bool> VENDOR_ABS { get; set; }
        public string VENDOR_BRANDING { get; set; }
        public string VENDOR_PURPOSE { get; set; }
        public string VENDOR_PO_NUMBER { get; set; }
        public string VENDOR_PO_LINE { get; set; }
        public Nullable<bool> VENDOR_VAT { get; set; }
        public Nullable<bool> VENDOR_RESTITUTION { get; set; }
        public string VENDOR_VENDOR { get; set; }
        public string EMPLOYEE_ID_CREATOR { get; set; }
        public string EMPLOYEE_ID_FLEET_APPROVAL { get; set; }
        public string ASSIGNED_TO { get; set; }
        public string COMMENTS { get; set; }
        public decimal PRICE { get; set; }
        public decimal VAT_DECIMAL { get; set; }
        public DateTime? DATE_SEND_VENDOR { get; set; }
    }
}
