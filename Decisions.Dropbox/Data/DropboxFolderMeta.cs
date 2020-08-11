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
    public class DropboxFolderMeta
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public FolderPolicy Policy { get;  set; }

        [DataMember]
        public string PreviewUrl { get; set; }

        [DataMember]
        public string SharedFolderId { get; set; }

        [DataMember]
        public DateTime TimeInvited { get; set; }

        [DataMember]
        public SharedContentLinkMetadata LinkMetadata { get; set; }

        [DataMember]
        public FolderPermission[] Permissions { get; set; }

        [DataMember]
        public AccessInheritance AccessInheritance { get; set; }

        public AccessLevel AccessType { get; set; }
        
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
