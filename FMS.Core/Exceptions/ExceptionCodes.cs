using System.ComponentModel;

namespace FMS.Core.Exceptions
{
    public abstract partial class ExceptionCodes
    {
        public enum BaseExceptions
        {
            [Description("An unknown error occured")]
            unhandled_exception,

            InvalidDateFormat
        }

        public enum BLLExceptions
        {
            [Description("An unknown error occured")]
            unhandled_exception,

            [Description("Invalid access rights for this action")]
            InvalidAccessRight,

            [Description("Operation not allowed")]
            OperationNotAllowed,

            [Description("The data received is not valid")]
            InvalidData,

            [Description("The data could not found")]
            DataNotFound,

            [Description("There is a duplicate in the entities")]
            DuplicateEntity,

            [Description("The login is not valid")]
            LoginNotMatch,

            [Description("The end date should be greater than start date")]
            StartDateGreaterThanEndDate,
        }

        /// <summary>
        /// Security Exceptions for wcf responses
        /// </summary>
        public enum SecurityExceptions
        {
            access_denied,
            authorization_denied,
            authentication_failure
        }

    }
}
