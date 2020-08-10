using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.DropboxApi.Data
{
    public class DropBoxException:Exception
    {
        public DropBoxException() : base()
        { }
        public DropBoxException(string message) : base(message)
        { }
    }
}
