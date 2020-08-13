using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using System.Linq;

namespace Decisions.DropboxApi
{
    internal class Mapper
    {


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

        internal static DropboxSharedFileMetadata Map(SharedFileMetadata obj)
        {
            if (obj == null) return null;
            return new DropboxSharedFileMetadata
            {
                Id = obj.Id,
                Name = obj.Name,
                PreviewUrl = obj.PreviewUrl,
                AccessType = Map(obj.AccessType),
                OwnerDisplayNames = obj.OwnerDisplayNames?.ToArray(),
                OwnerTeamId = obj.OwnerTeam?.Id,
                OwnerTeamName = obj.OwnerTeam?.Name,
                ParentSharedFolderId = obj.ParentSharedFolderId,
                PathDisplay = obj.PathDisplay,
                PathLower = obj.PathLower,
                TimeInvited = obj.TimeInvited,
                Url = obj.LinkMetadata?.Url
            };
        }

        internal static DropBoxAccessLevel Map(AccessLevel al)
        {
            if (al.IsEditor) return DropBoxAccessLevel.editor;
            if (al.IsOwner) return DropBoxAccessLevel.owner;
            if (al.IsViewerNoComment) return DropBoxAccessLevel.viewer_no_comment;
            return DropBoxAccessLevel.viewer;
        }

        internal static DropboxSharedFolderMetadata Map(SharedFolderMetadata obj)
        {
            if (obj == null) return null;
            return new DropboxSharedFolderMetadata()
            {
                Name = obj.Name,
                PreviewUrl = obj.PreviewUrl,
                SharedFolderId = obj.SharedFolderId,
                TimeInvited = obj.TimeInvited,
                AccessType = Map(obj.AccessType),
                IsInsideTeamFolder = obj.IsInsideTeamFolder,
                IsTeamFolder = obj.IsTeamFolder,
                OwnerDisplayNames = obj.OwnerDisplayNames?.ToArray(),
                OwnerTeamId = obj.OwnerTeam?.Id,
                OwnerTeamName = obj.OwnerTeam?.Name,
                ParentSharedFolderId = obj.ParentSharedFolderId,
                PathLower = obj.PathLower,
                ParentFolderName = obj.ParentFolderName,
                Url = obj.LinkMetadata?.Url
            };

        }

        private static void FillDropboxResource(DropboxResource resource, Metadata obj)
        {
            resource.Name = obj.Name;
            resource.IsDeleted = obj.IsDeleted;
            resource.ResourceType = obj.IsFolder ? DropboxResourceType.Folder : DropboxResourceType.File;
            resource.PathDisplay = obj.PathDisplay;
            resource.PathLower = obj.PathLower;
            resource.ParentSharedFolderId = obj.ParentSharedFolderId;
        }

        internal static DropboxFile MapFile(Metadata data)
        {
            var res = new DropboxFile();
            FillDropboxResource(res, data);

            var fileMeta = data.AsFile;

            res.Id = fileMeta.Id;
            res.ClientModified = fileMeta.ClientModified;
            res.ServerModified = fileMeta.ServerModified;
            res.Rev = fileMeta.Rev;
            res.Size = fileMeta.Size;
            res.IsDownloadable = fileMeta.IsDownloadable;

            return res;
        }

        internal static DropboxFolder MapFolder(Metadata data)
        {
            var res = new DropboxFolder();
            FillDropboxResource(res, data);

            var folderMeta = data.AsFolder;
            res.Id = folderMeta.Id;
            res.SharedFolderId = folderMeta.SharedFolderId;

            return res;
        }
    }
}