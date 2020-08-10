using Decisions.DropboxApi.Data;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using System.Linq;

namespace Decisions.DropboxApi
{
    internal class Mapper
    {
        internal static Entity Map(Metadata obj)
        {
            return new Entity
            {
                Name = obj.Name,
                IsDeleted = obj.IsDeleted,
                IsFile = obj.IsFile,
                IsFolder = obj.IsFolder,
                PathDisplay = obj.PathDisplay,
                PathLower = obj.PathLower,
                ParentSharedFolderId = obj.ParentSharedFolderId
            };
        }

        internal static User Map(UserInfo obj)
        {
            return new User
            {
                Email = obj.Email,
                DisplayedName = obj.DisplayName
            };
        }
        
        internal static User Map(InviteeInfo invitee)
        {
            return new User
            {
                Email = invitee?.AsEmail?.Value,
                DisplayedName = ""
            };
        }

        internal static FileMeta Map(SharedFileMetadata obj)
        {
            if (obj == null) return null;
            return new FileMeta
            {
                Id = obj.Id,
                Name = obj.Name,
                Policy = obj.Policy,
                PreviewUrl = obj.PreviewUrl,
                AccessType = obj.AccessType,
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

        internal static FolderMeta Map(SharedFolderMetadata obj)
        {
            if (obj == null) return null;
            return new FolderMeta()
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