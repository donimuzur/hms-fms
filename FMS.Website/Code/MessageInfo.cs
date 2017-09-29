using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMS.Core;

namespace FMS.Website.Code
{
    public class MessageInfo
    {
        public List<string> MessageText { get; set; }
        public Enums.MessageInfoType MessageInfoType { get; set; }

        public MessageInfo()
        {
        }

        public MessageInfo(List<string> messagetext, Enums.MessageInfoType messageinfotype)
        {
            MessageText = messagetext;
            MessageInfoType = messageinfotype;
        }
    }
}