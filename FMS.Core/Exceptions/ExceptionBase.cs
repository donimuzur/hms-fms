using System;
using System.Runtime.Serialization;

namespace FMS.Core.Exceptions
{
    public class ExceptionBase : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionBase"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <exception cref="System.ArgumentNullException">description</exception>
        public ExceptionBase(string description)
            : base(description)
        {
            if (description == null) throw new ArgumentNullException("description");
            Code = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionBase"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="inner">The inner.</param>
        /// <exception cref="System.ArgumentNullException">
        /// description
        /// or
        /// inner
        /// </exception>
        public ExceptionBase(string description, Exception inner)
            : base(description, inner)
        {
            // avoid devs t let throw exception without details
            if (description == null) throw new ArgumentNullException("description");
            if (inner == null) throw new ArgumentNullException("inner");
            Code = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        public ExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Code = ExceptionCodes.BaseExceptions.unhandled_exception.ToString();
        }


        /// <summary>
        /// Gets or sets the error code (will be used to link to localization resource)
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        ///// <summary>
        ///// Gets or sets the message objects.
        ///// objects will expose value that can be merged in in the error message by the error handler
        ///// </summary>
        ///// <value>The message objects.</value>
        //public object[] MessageObjects
        //{
        //    get;
        //    set;
        //}
    }
}
