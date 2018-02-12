using System;
namespace FMS.Logger.Models
{
    public class ExceptionModel
    {
        public ExceptionModel() { }
        public ExceptionModel(string methodName, 
            string className, 
            string nameSpace, 
            string errorMessage,
            string stackTrace,
            string exceptionType,
            DateTime dateTime)
        {
            this.MethodName = methodName;
            this.ClassName = className;
            this.NameSpace = nameSpace;
            this.ErrorMessage = errorMessage;
            this.StackTrace = stackTrace;
            this.ExceptionType = exceptionType;
            this.DateTime = dateTime;
        }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public string NameSpace { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionType { get; set; }
        public DateTime DateTime { get; set; }
    }
}
