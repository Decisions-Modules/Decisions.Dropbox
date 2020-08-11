using System.Runtime.Serialization;

namespace Decisions.DropboxApi
{
    [DataContract]
    public class DropboxUser
    {
        [DataMember]
        public string DisplayedName { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}