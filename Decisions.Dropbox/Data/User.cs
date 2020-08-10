using System.Runtime.Serialization;

namespace Decisions.DropboxApi.Data
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string DisplayedName { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}