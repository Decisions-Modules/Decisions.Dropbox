using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using DropboxWebClientAPI.Models;

namespace DropboxWebClientAPI
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

        internal static FileMeta Map(SharedFileMetadata obj)
        {
            return new FileMeta
            {
                Id = obj.Id,
                Name = obj.Name,
                Policy = obj.Policy,
                PreviewUrl = obj.PreviewUrl,
                AccessType = obj.AccessType,
                ExpectedLinkMetadata = obj.ExpectedLinkMetadata,
                LinkMetadata = obj.LinkMetadata,
                OwnerDisplayNames = obj.OwnerDisplayNames,
                OwnerTeam = obj.OwnerTeam,
                ParentSharedFolderId = obj.ParentSharedFolderId,
                PathDisplay = obj.PathDisplay,
                PathLower = obj.PathLower,
                Permissions = obj.Permissions,
                TimeInvited = obj.TimeInvited
            };
        }

        /*internal static FolderMeta Map(SharedFolderMetadata obj)
        {
            return new FileMeta
            {
                Id = obj.Id,
                Name = obj.Name,
                Policy = obj.Policy,
                PreviewUrl = obj.PreviewUrl,
                AccessType = obj.AccessType,
                ExpectedLinkMetadata = obj.ExpectedLinkMetadata,
                LinkMetadata = obj.LinkMetadata,
                OwnerDisplayNames = obj.OwnerDisplayNames,
                OwnerTeam = obj.OwnerTeam,
                ParentSharedFolderId = obj.ParentSharedFolderId,
                PathDisplay = obj.PathDisplay,
                PathLower = obj.PathLower,
                Permissions = obj.Permissions,
                TimeInvited = obj.TimeInvited
            };
        }*/

    }
}