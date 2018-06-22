﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BusinessObject.Inputs
{
    public class SalesVolumeParamInput
    {
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public string Type { get; set; }
        public string Regional { get; set; }
        public string Table { get; set; }
    }

    public class CostObParamInput
    {
        public bool? Status { get; set; }
        public string VehicleType { get; set; }
        public string Function { get; set; }
        public string Regional { get; set; }
        public int? Year { get; set; }
        public string Table { get; set; }
    }
    public class EpafParamInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EpafAction { get; set; }
        public string DocumentType { get; set; }
        public string Table { get; set; }
    }

    public class LocationMappingParamInput
    {
        public string Location { get; set; }
        public string Address { get; set; }
        public string Basetown { get; set; }
        public string Region { get; set; }
        public string ZoneSales { get; set; }
        public string ZonePriceList { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
