using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FMS.Website.Models
{
    public class SettingModel : BaseModel
    {
        public SettingModel()
        {
            Details = new List<SettingItem>();
        }

        public List<SettingItem> Details { get; set; }
    }

    public class SettingItem : BaseModel
    {
        public int MstSettingId { get; set; }
        public string SettingGroup { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }

    }

} 