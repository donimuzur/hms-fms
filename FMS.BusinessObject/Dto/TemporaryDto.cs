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
        public string ACTUAL_GROUP { get; set; }
        public string SUPPLY_METHOD { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string POLICE_NUMBER { get; set; }
        public string MANUFACTURER { get; set; }
        public string MODEL { get; set; }
        public string SERIES { get; set; }
        public string BODY_TYPE { get; set; }
        public string COLOR { get; set; }
        public string LOCATION_CITY { get; set; }
        public string LOCATION_ADDRESS { get; set; }
        public Nullable<System.DateTime> START_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string EMPLOYEE_ID_CREATOR { get; set; }
        public string EMPLOYEE_ID_FLEET_APPROVAL { get; set; }
    }
}
