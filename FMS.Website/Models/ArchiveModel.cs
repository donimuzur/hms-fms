using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FMS.Website.Models
{
    public class ArchiveModel : BaseModel
    {
        public ArchiveModel()
        {
            Details = new List<ArchiveItem>();
            SearchView = new ArchiveSearchView();
        }
        public List<ArchiveItem> Details { get; set; }
        public ArchiveSearchView SearchView { get; set; }
        public string Message { get; set; }
    }
    public class ArchiveItem
    {
        public string Table { get; set; }
        public string Type { set; get; }
        public bool Checklist { get; set; }
        public long Id { get; set; }
    }
    public class ArchiveSearchView
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Operator { get; set; }
        public SelectList OperatorList { get; set; }
        public string VehicleType { get; set; }
        public SelectList VehicleTypeList { get; set; }
    }

}