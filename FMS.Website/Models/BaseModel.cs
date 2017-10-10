using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMS.BusinessObject.Business;
using FMS.Core;

namespace FMS.Website.Models
{
    public class BaseModel
    {
        
        public Enums.MenuList MainMenu { get; set; }
        
        //public string ErrorMessage { get; set; }


        public Login CurrentLogin { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }

        public string MessageTitle { get; set; }
        public List<string> MessageBody { get; set; }

        public bool IsShowNewButton { get; set; }
        public bool IsNotViewer { get; set; }
    }
}