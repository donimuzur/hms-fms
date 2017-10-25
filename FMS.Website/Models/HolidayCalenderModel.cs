using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class HolidayCalenderModel : BaseModel
    {
        public HolidayCalenderModel()
        {
            Details = new List<HolidayCalenderItem>();
        }

        public List<HolidayCalenderItem> Details { get; set; }
    }

    public class HolidayCalenderItem : BaseModel
    {
        public DateTime MstHolidayDate { get; set; }
        public String Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class HolidayCalenderUpload : BaseModel
    {
        public String MstHolidayDate { get; set; }
        public String Description { get; set; }
    }
    }