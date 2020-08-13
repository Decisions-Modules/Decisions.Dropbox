using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace Decisions.DropboxApi
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DropboxResourceType { Unavailable = 0, File = 1, Folder = 2 }

    [DataContract]
    public class DropboxResource
    {
        [DataMember]
        public DropboxResourceType ResourceType { get; set; }

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

    [DataContract]
    public class DropboxFile : DropboxResource
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public DateTime ClientModified { get; set; }

        [DataMember]
        public DateTime ServerModified { get; set; }

        [DataMember]
        public string Rev { get; set; }

        [DataMember]
        public ulong Size { get; set; }

        [DataMember]
        public bool IsDownloadable { get; set; }
    }

    [DataContract]
    public class DropboxFolder : DropboxResource
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string SharedFolderId { get; set; }
    }

}
