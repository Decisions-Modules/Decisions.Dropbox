using Dropbox.Api.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.DropboxApi
{
    [DataContract]
    public class DropboxSharedFolderMetadata
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string PreviewUrl { get; set; }

        [DataMember]
        public string SharedFolderId { get; set; }

        [DataMember]
        public DateTime TimeInvited { get; set; }

        public DropBoxAccessLevel AccessType { get; set; }
        
        [DataMember]
        public bool IsInsideTeamFolder { get; set; }
        
        [DataMember]
        public bool IsTeamFolder { get; set; }


        [DataMember]
        public string[] OwnerDisplayNames { get; set; }

        [DataMember]
        public string OwnerTeamId { get; set; }

        [DataMember]
        public string OwnerTeamName { get; set; }

        [DataMember]
        public string ParentSharedFolderId { get; set; }

        [DataMember]
        public string PathLower { get; set; }

        [DataMember]
        public string ParentFolderName { get; set; }

    }

}
