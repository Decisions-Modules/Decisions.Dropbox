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

        internal static DropboxErrorInfo FromException(Exception ex)
        {
            return new DropboxErrorInfo()
            {
                ErrorMessage = (ex.Message ?? ex.ToString()),
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
