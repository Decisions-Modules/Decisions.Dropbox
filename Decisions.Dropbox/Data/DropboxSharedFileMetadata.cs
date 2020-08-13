using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dropbox.Api.Sharing;

namespace Decisions.DropboxApi
{
    [DataContract]
    public class DropboxSharedFileMetadata
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PreviewUrl { get; set; }

        [DataMember]
        public DropBoxAccessLevel AccessType { get; set; }

        [DataMember]
        public string[] OwnerDisplayNames { get; set; }

        [DataMember]
        public string OwnerTeamId { get; set; }

        [DataMember]
        public string OwnerTeamName { get; set; }

        [DataMember]
        public string ParentSharedFolderId { get; set; }

        [DataMember]
        public string PathDisplay { get; set; }

        [DataMember]
        public string PathLower { get; set; }

        [DataMember]
        public DateTime? TimeInvited { get; set; }

    }



}