using System.Runtime.Serialization;

namespace Decisions.DropboxApi.Data
{
    [DataContract]
    public class Entity
    {
        [DataMember]
        public bool IsFile { get; set; }

        [DataMember]
        public bool IsFolder { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PathLower { get; set; }

        [DataMember]
        public string PathDisplay { get; set; }

        [DataMember]
        public string ParentSharedFolderId { get; set; }
    }
}