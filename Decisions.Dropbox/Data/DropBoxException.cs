using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.DropboxApi
{
    [DataContract]
    public class DropboxErrorInfo
    {
        [DataMember]
        public string ErrorMessage { get; set; }

        internal static DropboxErrorInfo FromException(Exception exception)
        {
            var messages = new StringBuilder();

            if (exception is System.AggregateException)
            {
                var exceptions = (exception as System.AggregateException).InnerExceptions;
                foreach (var ex in exceptions)
                {
                    if (messages.Length > 0) messages.AppendLine();
                    messages.Append(ex.Message ?? ex.ToString());
                }
            }
            else
                messages.Append(exception.Message ?? exception.ToString());

            return new DropboxErrorInfo()
            {
                ErrorMessage = (messages.ToString()),
            };
        }

        override public String ToString()
        {
            return ErrorMessage;
        }
    }

    public class DropBoxException:Exception
    {
        public DropBoxException() : base()
        { }
        public DropBoxException(string message) : base(message)
        { }
    }


}
