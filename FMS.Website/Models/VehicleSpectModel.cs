using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class VehicleSpect
    {
        public long VehicleSpectDocID { get; set; }
        public int VehicleSpectId { get; set; }
        public string File_Name { get; set; }
        public string File_Path { get; set; }
    }
    public class VehicleSpectModel : BaseModel
    {
        public VehicleSpectModel()
        {
            Details = new List<VehicleSpectItem>();
        }

        public List<VehicleSpectItem> Details { get; set; }
    }
    public class VehicleSpectItem : BaseModel
    {
        public int MstVehicleSpectId { get; set; }
        public string Manufacturer { get; set; }
        public string Models { get; set; }
        public string Series { get; set; }
        public string Transmission { get; set; }
        public string BodyType { get; set; }
        public int Year { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }
        public int GroupLevel { get; set; }
        public int FlexPoint { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public SelectList BodyTypeList { get; set; }
        public SelectList GroupLevelList { get; set; }
        public SelectList TransmissionList { get; set; }
    }
}