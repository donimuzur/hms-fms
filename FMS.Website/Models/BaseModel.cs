using System.Collections.Generic;
using FMS.BusinessObject;
using FMS.Core;

namespace FMS.Website.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            //ChangesHistoryList = new List<ChangesHistoryItemModel>();
            
            
        }
        public Enums.MenuList MainMenu { get; set; }
        public SysMenu CurrentMenu { get; set; }
        //public string ErrorMessage { get; set; }

        //public List<ChangesHistoryItemModel> ChangesHistoryList { get; set; }
        
        
        
        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }

        public string MessageTitle { get; set; }
        public List<string> MessageBody { get; set; }

        public bool IsShowNewButton { get; set; }
        public bool IsNotViewer { get; set; }
    }
}