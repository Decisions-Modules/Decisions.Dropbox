using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using System.Linq;

namespace Decisions.DropboxApi
{
    internal class Mapper
    {
        internal static DropboxResource Map(Metadata obj)
        {
            return new DropboxResource
            {
                Name = obj.Name,
                IsDeleted = obj.IsDeleted,
                ResourceType = obj.IsFolder ? DropboxResourceType.Folder : DropboxResourceType.File,
                PathDisplay = obj.PathDisplay,
                PathLower = obj.PathLower,
                ParentSharedFolderId = obj.ParentSharedFolderId
            };
        }

        internal static DropboxUser Map(UserInfo obj)
        {
            return new DropboxUser
            {
                Email = obj.Email,
                DisplayedName = obj.DisplayName
            };
        }
        
        internal static DropboxUser Map(InviteeInfo invitee)
        {
            return new DropboxUser
            {
                Email = invitee?.AsEmail?.Value,
                DisplayedName = ""
            };
        }

        internal static DropboxFileMeta Map(SharedFileMetadata obj)
        {
            if (obj == null) return null;
            return new DropboxFileMeta
            {
                Id = obj.Id,
                Name = obj.Name,
                Policy = obj.Policy,
                PreviewUrl = obj.PreviewUrl,
                AccessType = Map(obj.AccessType),
                ExpectedLinkMetadata = obj.ExpectedLinkMetadata,
                LinkMetadata = obj.LinkMetadata,
                OwnerDisplayNames = obj.OwnerDisplayNames?.ToArray(),
                OwnerTeamId = obj.OwnerTeam?.Id,
                OwnerTeamName = obj.OwnerTeam?.Name,
                ParentSharedFolderId = obj.ParentSharedFolderId,
                PathDisplay = obj.PathDisplay,
                PathLower = obj.PathLower,
                Permissions = obj.Permissions?.ToArray(),
                TimeInvited = obj.TimeInvited
            };
        }

        internal static DropBoxAccessLevel Map(AccessLevel al)
        {
            if(al.IsEditor) return DropBoxAccessLevel.editor;
            if(al.IsOwner) return DropBoxAccessLevel.owner;
            if(al.IsViewerNoComment) return DropBoxAccessLevel.viewer_no_comment;
            return DropBoxAccessLevel.viewer;
        }

        internal static DropboxFolderMeta Map(SharedFolderMetadata obj)
        {
            if (obj == null) return null;
            return new DropboxFolderMeta()
            {
                Name = obj.Name,
                Policy = obj.Policy,
                PreviewUrl = obj.PreviewUrl,
                SharedFolderId = obj.SharedFolderId,
                TimeInvited = obj.TimeInvited,
                LinkMetadata = obj.LinkMetadata,
                Permissions = obj.Permissions?.ToArray(),
                AccessInheritance = obj.AccessInheritance,
                AccessType = obj.AccessType,
                IsInsideTeamFolder = obj.IsInsideTeamFolder,
                IsTeamFolder = obj.IsTeamFolder,
                OwnerDisplayNames = obj.OwnerDisplayNames?.ToArray(),
                OwnerTeamId = obj.OwnerTeam?.Id,
                OwnerTeamName = obj.OwnerTeam?.Name,
                ParentSharedFolderId = obj.ParentSharedFolderId,
                PathLower = obj.PathLower,
                ParentFolderName = obj.ParentFolderName,
            };

        }
    }
}