using System.Runtime.Serialization;

namespace DropboxWebClientAPI.Models
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