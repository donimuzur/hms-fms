using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.Core;

namespace FMS.Website.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            ChangesLogs = new List<ChangesLogs>();
        }
        public Enums.MenuList MainMenu { get; set; }
        
        //public string ErrorMessage { get; set; }


        public Login CurrentLogin { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }

        public string MessageTitle { get; set; }
        public List<string> MessageBody { get; set; }

        public bool IsShowNewButton { get; set; }
        public bool IsNotViewer { get; set; }

        public List<ChangesLogs> ChangesLogs { get; set; }
        public List<WorkflowLogs> WorkflowLogs { get; set; }

        public RoleDto CurrentPageAccess
        {
            get; set;
        }

        public int WriteAccess { get; set; }
        public int ReadAccess { get; set; }
    }


    public class ChangesLogs
    {
        public string UserName { get; set; }
        public string UserId { get; set; }

        public string Role { get; set; }
        public string Action { get; set; }

        public DateTime ActionDate { get; set; }
    }

    public class WorkflowLogs
    {
        public string UserName { get; set; }
        public string UserId { get; set; }

        public string RoleName { get; set; }

        public string Action { get; set; }

        public string Remark { get; set; }
        public DateTime ActionDate { get; set; }
    }
}