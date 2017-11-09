using System;
using FMS.Core;

namespace FMS.BusinessObject.Dto
{
    public class WorkflowHistoryDto
    {
        public long WORKFLOW_HISTORY_ID { get; set; }
        public Enums.MenuList MODUL_ID { get; set; }
        public long FORM_ID { get; set; }
        public Enums.ActionType ACTION { get; set; }
        public string ACTION_BY { get; set; }
        public DateTime? ACTION_DATE { get; set; }
        public int REMARK { get; set; }
    }
}
