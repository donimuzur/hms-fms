using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;

namespace FMS.BusinessObject.Inputs
{
    public class CsfWorkflowDocumentInput
    {
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        public string EmployeeId { get; set; }
        public Enums.UserRole UserRole { get; set; }
        public int? Comment { get; set; }
        public Enums.ActionType ActionType { get; set; }
        public string DocumentNumber { get; set; }
    }

    public class VehicleFromVendorUpload
    {
        public long TraTemporaryId { get; set; }
        public string TemporaryNumber { get; set; }
        public Enums.DocumentStatus TemporaryStatus { get; set; }
        public string TemporaryStatusName { get; set; }
        public string CsfNumber { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string CostCenter { get; set; }
        public string PoNumber { get; set; }
        public string PoliceNumber { get; set; }
        public string ChasisNumber { get; set; }
        public string EngineNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public string VendorName { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public string StartPeriodName { get; set; }
        public string EndPeriodName { get; set; }
        public string StartPeriodValue { get; set; }
        public string EndPeriodValue { get; set; }
        public bool IsAirBag { get; set; }
        public bool IsVat { get; set; }
        public bool IsRestitution { get; set; }
        public string Transmission { get; set; }
        public string Branding { get; set; }
        public string Purpose { get; set; }
        public int VehicleYear { get; set; }
        public string PoLine { get; set; }

        public int ReasonIdTemp { get; set; }
        public string ReasonTemp { get; set; }
        public string UrlTemp { get; set; }
        public string MessageError { get; set; }
        public string MessageErrorStopper { get; set; }
    }

    public class VehicleFromUserUpload
    {
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string BodyType { get; set; }
        public string Color { get; set; }
        public string Vendor { get; set; }
        public string MessageError { get; set; }
    }
}
