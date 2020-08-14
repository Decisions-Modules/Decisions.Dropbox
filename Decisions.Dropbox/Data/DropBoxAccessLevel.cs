using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.DropboxApi
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DropBoxAccessLevel { owner, editor, viewer, viewer_no_comment }
}
