using FMS.Utils;

namespace FMS.Core.Exceptions
{
    public class BLLException : ExceptionBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="BLLException"/> class from being created.
        /// </summary>
        private BLLException() : base("") { }

        /// <summary>
        /// Initializes a new instance of the BLLException with an exception code (enum)
        /// </summary>
        /// <param name="code">The code.</param>
        public BLLException(ExceptionCodes.BLLExceptions code)
            : base(EnumHelper.GetDescription(code))
        {
            Code = code.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the BLLException with an exception code (enum) and a message
        /// </summary>
        /// <param name="code">The code.</param>
        public BLLException(ExceptionCodes.BLLExceptions code, string message)
            : base(message)
        {
            Code = code.ToString();
        }
    }
}