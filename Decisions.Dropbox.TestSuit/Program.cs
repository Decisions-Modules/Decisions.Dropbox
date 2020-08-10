using System;
using System.Linq;
using Dropbox.Api;
using Dropbox.Api.Sharing;
using Dropbox.Api.TeamLog;

namespace Test
{
    internal class Program
    {
        const string TestEmail = "kovalchuk_i_v@mail.ru";
        const string TestFolder = "/TEMPFOLDER/";
        const string LocalTestFile = "c:/data/tmp/ttt.txt";
        const string RemoteTestFile = TestFolder+"ttt.txt";

        public static void Main(string[] args)
        {
          /*  string token = "tB-tWqjJhvAAAAAAAAAAGr2AlPqx9Y2jUGXTReo5fTBML7XOkXUCiRKFsAK1UVRC";

            var a = DropBoxWebClientAPI.GetFilesArray(token,"/");
            var aa = DropBoxWebClientAPI.GetFoldersArray(token,"/");

            var sharingRes = DropBoxWebClientAPI.ShareFolder(token, TestFolder);

            var sharedFolders = DropBoxWebClientAPI.GetFoldersSharingSettings(token);

            SharedFolderMetadata sharedFolderMeta = sharedFolders.Where(it => TestFolder.Contains(it.Name)).First();

            SharedFolderMetadata sharedFolderMeta2 = DropBoxWebClientAPI.GetFolderSharingSettings(token, sharedFolderMeta.SharedFolderId);

            var addMemberRes = DropBoxWebClientAPI.AddMembersToFolder(token, sharedFolderMeta.SharedFolderId, TestEmail);
            var folderMemberRes = DropBoxWebClientAPI.FolderMembersArray(token, sharedFolderMeta.SharedFolderId);
            var removeMemberRes = DropBoxWebClientAPI.RemoveMemberFromFolder(token, sharedFolderMeta.SharedFolderId, TestEmail);
            var folderMemberRes2 = DropBoxWebClientAPI.FolderMembersArray(token, sharedFolderMeta.SharedFolderId);

            var s = DropBoxWebClientAPI.UploadFile(token, LocalTestFile, TestFolder);

            //var b = DropBoxWebClientAPI.DownloadFile(token, RemoteTestFile);

            var link = DropBoxWebClientAPI.CreateSharedLink(token, RemoteTestFile);
            var addMamberRes = DropBoxWebClientAPI.AddMembersToFile(token, RemoteTestFile, TestEmail);
            var fileMemberRes = DropBoxWebClientAPI.FileMembersArray(token, RemoteTestFile);
            var fileSharing = DropBoxWebClientAPI.GetFileSharingSettings(token, RemoteTestFile);
            var removeMamberRes = DropBoxWebClientAPI.RemoveMemberFromFile(token, RemoteTestFile, TestEmail);
            var fileMemberRes2 = DropBoxWebClientAPI.FileMembersArray(token, RemoteTestFile);
            var unshareRes = DropBoxWebClientAPI.UnshareFile(token, RemoteTestFile);

            var delete = DropBoxWebClientAPI.DeleteFile(token, RemoteTestFile);          

            var swww = DropBoxWebClientAPI.GetFolderSharingSettings(token, sharedFolderMeta.SharedFolderId);

            var unShareResult = DropBoxWebClientAPI.UnshareFolder(token, sharedFolderMeta.SharedFolderId);
            foreach (var sharedMeta in sharedFolders.Entries)
            {
                var unshareFolderRes = DropBoxWebClientAPI.UnshareFolder(token, sharedMeta.SharedFolderId);
            }*/

        }
    }
}