using System;

namespace FMS.Logger.Models
{
    public class ExceptionInfo
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string ExceptionType { get; set; }
        public string StackTrace { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateTime { get; set; }
    }
}
