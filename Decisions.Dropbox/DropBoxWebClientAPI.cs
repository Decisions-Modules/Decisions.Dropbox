using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Decisions.DropboxApi.Data;
using Dropbox.Api;
using Dropbox.Api.Async;
using Dropbox.Api.CloudDocs;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;

namespace Decisions.DropboxApi
{
    public static class DropBoxWebClientAPI
    {
        public static Entity CreateFolder(string token, string path)
        {
            using (var client = new DropboxClient(token))
            {
                CreateFolderResult res = client.Files.CreateFolderV2Async(path).GetAwaiter().GetResult();
                return Mapper.Map(res.Metadata);
            }
        }

        private static void CorrectDropboxPath(ref string path)
        {
            if (path == null) path = "";
            path = path.Replace(Path.DirectorySeparatorChar, '/');
        }

        private static List<Metadata> GetFolderContext(string token, string path, uint? limit = null)
        {
            CorrectDropboxPath(ref path);

            if (path == "/") // DropBox wants the empty string for the root directory
                path = "";

            var res = new List<Metadata>();
            using (var client = new DropboxClient(token))
            {
                ListFolderResult allItems = client.Files.ListFolderAsync(path, limit: limit).Result;
                res.AddRange(allItems.Entries);

                while (allItems.HasMore)
                {
                    allItems = client.Files.ListFolderContinueAsync(new ListFolderContinueArg(allItems.Cursor)).Result;
                    res.AddRange(allItems.Entries);
                }
            }
            return res;
        }

        public static Entity[] GetFoldersArray(string token, string path, uint? limit = null)
        {
            var folderContext = GetFolderContext(token, path, limit);

            List<Metadata> listOfFolders = folderContext.Where(x => x.IsFolder).ToList();

            return listOfFolders.Select(Mapper.Map).ToArray();
        }

        public static Entity[] GetFilesArray(string token, string path, uint? limit = null)
        {
            var folderContext = GetFolderContext(token, path, limit);

            List<Metadata> listOfFiles = folderContext.Where(x => x.IsFile).ToList();

            return listOfFiles.Select(Mapper.Map).ToArray();
        }

        public static Metadata GetMetadata(string token, string pathToFile)
        {
            CorrectDropboxPath(ref pathToFile);
            using (var client = new DropboxClient(token))
            {
                try
                {
                    Metadata result = client.Files.GetMetadataAsync(pathToFile).Result;
                    return result;
                }
                catch (Exception e)
                {
                    Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError> target = null;
                    if (e is System.AggregateException)
                    {
                        var exceptions = (e as System.AggregateException).InnerExceptions;
                        foreach (var it in exceptions)
                        {
                            if (it is Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError>)
                                target = (Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError>)it;
                        }
                    }
                    else
                        if (e is Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError>)
                        target = (Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError>)e;

                    if (target != null && target.ErrorResponse.IsPath && target.ErrorResponse.AsPath.Value.IsNotFound)
                        return null;
                    throw;
                }

            }
        }

        public static Entity UploadFile(string token, string fileForUploadingFullPathWithExtension, string dropboxPath)
        {
            CorrectDropboxPath(ref dropboxPath);
            if (string.IsNullOrEmpty(dropboxPath) || dropboxPath.Last() != '/')
                dropboxPath += '/';

            using (var client = new DropboxClient(token))
            {
                var fileName = Path.GetFileName(fileForUploadingFullPathWithExtension);

                FileMetadata file = client.Files.UploadAsync($"{dropboxPath}{fileName}",
                        new WriteMode().AsOverwrite,
                        false, DateTime.Now, false, null, false,
                        new StreamReader(fileForUploadingFullPathWithExtension).BaseStream)
                    .GetAwaiter().GetResult();

                return Mapper.Map(file);
            }
        }

        public static void DownloadFile(string token, string pathToDropboxFile, string localFolder)
        {
            CorrectDropboxPath(ref pathToDropboxFile);
            using (var client = new DropboxClient(token))
            {
                var downloadFile = client.Files.DownloadAsync(pathToDropboxFile).GetAwaiter().GetResult();
                string localFile = localFolder.TrimEnd('/', '\\') + Path.DirectorySeparatorChar + Path.GetFileName(pathToDropboxFile);
                using (var file = File.Create(localFile))
                {
                    var stream = downloadFile.GetContentAsStreamAsync().GetAwaiter().GetResult();
                    stream.CopyTo(file);
                }
            }
        }

        public static void DeleteResource(string token, string filePath)
        {
            CorrectDropboxPath(ref filePath);
            using (var client = new DropboxClient(token))
            {
                DeleteResult result = client.Files.DeleteV2Async(filePath).GetAwaiter().GetResult();
            }
        }

        public static FileMeta GetFileSharingSettings(string token, string filePath)
        {
            CorrectDropboxPath(ref filePath);
            using (var client = new DropboxClient(token))
            {

                SharedFileMetadata result = client.Sharing.GetFileMetadataAsync(filePath).GetAwaiter().GetResult();
                return Mapper.Map(result);
            }
        }

        /// <summary>
        ///     Get sharing settings for ALL folders
        /// </summary>
        public static SharedFolderMetadata[] GetFoldersSharingSettings(string token, int limit = 1000)
        {
            using (var client = new DropboxClient(token))
            {
                var resultList = new List<SharedFolderMetadata>();

                ListFoldersResult result;
                do
                {
                    result = client.Sharing.ListFoldersAsync().GetAwaiter().GetResult();
                    resultList.AddRange(result.Entries);
                }
                while (result.Cursor != null && resultList.Count < limit);

                return resultList.ToArray();
            }
        }

        /// <summary>
        ///     Get sharing settings for SPECIFIC folder by it's id
        /// </summary>
        public static FolderMeta GetFolderSharingSettings(string token, string sharedFolder)
        {
            CorrectDropboxPath(ref sharedFolder);
            using (var client = new DropboxClient(token))
            {
                string sharedFolderId = GetSharedFolderId(client, sharedFolder);
                SharedFolderMetadata result = client.Sharing.GetFolderMetadataAsync(sharedFolderId).Result;
                return Mapper.Map(result);
            }
        }

        public static FolderMeta ShareFolder(string token, string folder, int millisecondsTimeout = 10000, bool forceAsync = false)
        {
            CorrectDropboxPath(ref folder);

            using (var client = new DropboxClient(token))
            {
                var shareFolderLaunch = client.Sharing.ShareFolderAsync(folder, forceAsync: forceAsync).Result;

                SharedFolderMetadata sfm = shareFolderLaunch.AsComplete?.Value;
                if (shareFolderLaunch.IsAsyncJobId)
                {
                    string jobId = shareFolderLaunch.AsAsyncJobId.Value;
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();

                    ShareFolderJobStatus jobStatus = client.Sharing.CheckShareJobStatusAsync(jobId).Result;

                    while (jobStatus.IsInProgress && stopWatch.ElapsedMilliseconds < millisecondsTimeout)
                    {
                        Thread.Sleep(1000);
                        jobStatus = client.Sharing.CheckShareJobStatusAsync(jobId).Result;
                    }

                    if (jobStatus.IsFailed)
                    {
                        var str = jobStatus.AsFailed.Value.ToString();
                        throw new DropBoxException(str);
                    }

                    sfm = jobStatus.AsComplete?.Value;
                }

                return Mapper.Map(sfm);
            }
        }

        public static void UnshareFolder(string token, string sharedFolder, int millisecondsTimeout = 10000)
        {
            using (var client = new DropboxClient(token))
            {
                string sharedFolderId = GetSharedFolderId(client, sharedFolder);

                LaunchEmptyResult unshareFolderLaunch = client.Sharing.UnshareFolderAsync(sharedFolderId).GetAwaiter().GetResult();

                if (!unshareFolderLaunch.IsComplete)
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    string jobId = unshareFolderLaunch.AsAsyncJobId.Value;
                    JobStatus jobStatus = client.Sharing.CheckJobStatusAsync(jobId).Result;
                    while (jobStatus.IsInProgress && stopWatch.ElapsedMilliseconds < millisecondsTimeout)
                    {
                        Thread.Sleep(1000);
                        jobStatus = client.Sharing.CheckJobStatusAsync(jobId).Result;
                    }

                    if (jobStatus.IsFailed)
                    {
                        var str = jobStatus.AsFailed.Value.ToString();
                        throw new DropBoxException(str);
                    }
                }
            }
        }

        public static void UnshareFolderById(string token, string sharedFolderId)
        {
            using (var client = new DropboxClient(token))
            {
                client.Sharing.UnshareFolderAsync(sharedFolderId).GetAwaiter().GetResult();
            }
        }

        public static string CreateSharedLink(string token, string path)
        {
            CorrectDropboxPath(ref path);
            using (var client = new DropboxClient(token))
            {
                SharedLinkMetadata result;

                result = client.Sharing.CreateSharedLinkWithSettingsAsync(path).GetAwaiter().GetResult();

                return result?.Url;
            }
        }

        public static void RevokeSharedLink(string token, string url)
        {
            using (var client = new DropboxClient(token))
            {
                client.Sharing.RevokeSharedLinkAsync(url).Wait();
            };

        }

        private static AccessLevel GetAccessLevel(DropBoxAccessLevel? dropBoxAccessLevel)
        {
            AccessLevel accessLevel = AccessLevel.Viewer.Instance;

            if (dropBoxAccessLevel != null)
                switch (dropBoxAccessLevel)
                {
                    case DropBoxAccessLevel.editor:
                        accessLevel = AccessLevel.Editor.Instance;
                        break;
                    case DropBoxAccessLevel.owner:
                        accessLevel = AccessLevel.Owner.Instance;
                        break;
                    case DropBoxAccessLevel.viewer:
                        accessLevel = AccessLevel.Viewer.Instance;
                        break;
                    case DropBoxAccessLevel.viewer_no_comment:
                        accessLevel = AccessLevel.ViewerNoComment.Instance;
                        break;
                }
            return accessLevel;
        }

        public static void AddMembersToFile(string token, string pathToFile, DropBoxAccessLevel? dropBoxAccessLevel, params string[] emails)
        {
            CorrectDropboxPath(ref pathToFile);
            AccessLevel accessLevel = GetAccessLevel(dropBoxAccessLevel);

            List<MemberSelector> membersEmails = new List<MemberSelector>();
            foreach (string email in emails)
            {
                membersEmails.Add(new MemberSelector.Email(email));
            }

            List<FileMemberActionResult> res;
            using (var client = new DropboxClient(token))
            {
                res = client.Sharing.AddFileMemberAsync(pathToFile, membersEmails, accessLevel: accessLevel).Result;
            }

        }

        public static void RemoveMemberFromFile(string token, string pathToFile, string memberEmail)
        {
            CorrectDropboxPath(ref pathToFile);
            using (var client = new DropboxClient(token))
            {
                client.Sharing.RemoveFileMember2Async(pathToFile, new MemberSelector.Email(memberEmail)).GetAwaiter()
                    .GetResult();

            }
        }

        public static User[] FileMembersArray(string token, string path, uint limit = 100)
        {
            var users = new List<UserFileMembershipInfo>();
            var invitees = new List<InviteeMembershipInfo>();

            using (var client = new DropboxClient(token))
            {
                SharedFileMembers answer = client.Sharing.ListFileMembersAsync(path, limit: limit).Result;
                users.AddRange(answer.Users);
                invitees.AddRange(answer.Invitees);

                while (answer?.Cursor != null)
                {
                    answer = client.Sharing.ListFileMembersContinueAsync(answer?.Cursor).Result;
                    users.AddRange(answer.Users);
                    invitees.AddRange(answer.Invitees);
                }

                var result = users.Select(it => Mapper.Map(it.User));
                result = result.Concat(invitees.Select(it => Mapper.Map(it.Invitee)));

                return result.ToArray();
            }
        }

        public static void UnshareFile(string token, string path)
        {
            using (var client = new DropboxClient(token))
            {
                client.Sharing.UnshareFileAsync(path).Wait();
            }
        }

        public static void AddMembersToFolder(string token, string sharedFolder, DropBoxAccessLevel? dropBoxAccessLevel, params string[] emails)
        {
            CorrectDropboxPath(ref sharedFolder);

            AccessLevel accessLevel = GetAccessLevel(dropBoxAccessLevel);
            List<AddMember> membersEmails = new List<AddMember>();
            foreach (string email in emails)
            {
                membersEmails.Add(new AddMember(new MemberSelector.Email(email), accessLevel));
            }

            using (var client = new DropboxClient(token))
            {
                string sharedFolderId = GetSharedFolderId(client, sharedFolder);
                client.Sharing.AddFolderMemberAsync(sharedFolderId, membersEmails).GetAwaiter().GetResult();
            }

        }

        public static void RemoveMemberFromFolder(string token, string sharedFolder, string memberEmail)
        {
            CorrectDropboxPath(ref sharedFolder);

            using (var client = new DropboxClient(token))
            {
                string sharedFolderId = GetSharedFolderId(client, sharedFolder);
                client.Sharing.RemoveFolderMemberAsync(sharedFolderId, new MemberSelector.Email(memberEmail),
                    false).GetAwaiter().GetResult();
            }
        }

        public static User[] FolderMembersArray(string token, string sharedFolder, uint limit = 100)
        {
            using (var client = new DropboxClient(token))
            {
                var users = new List<UserMembershipInfo>();
                var invitees = new List<InviteeMembershipInfo>();

                string sharedFolderId = GetSharedFolderIdOrNull(client, sharedFolder);

                if (sharedFolderId != null)
                {
                    SharedFolderMembers answer = client.Sharing.ListFolderMembersAsync(sharedFolderId).Result;
                    users.AddRange(answer.Users);
                    invitees.AddRange(answer.Invitees);
                    while (answer.Cursor != null)
                    {
                        answer = client.Sharing.ListFolderMembersContinueAsync(answer.Cursor).Result;
                        users.AddRange(answer.Users);
                        invitees.AddRange(answer.Invitees);
                    };
                }

                var result = users.Select(x => Mapper.Map(x.User));
                result = result.Concat(invitees.Select(x => Mapper.Map(x.Invitee)));

                return result.ToArray();
            }
        }

        private static string GetSharedFolderIdOrNull(DropboxClient client, string sharedFolder)
        {
            Metadata metadata = client.Files.GetMetadataAsync(sharedFolder).Result;
            if (metadata.AsFolder == null)
                throw new DropBoxException($"{sharedFolder} is not a folder.");

            return metadata.AsFolder.SharedFolderId;
        }

        private static string GetSharedFolderId(DropboxClient client, string sharedFolder)
        {
            string sharedFolderId = GetSharedFolderIdOrNull(client, sharedFolder);
            if (sharedFolderId == null)
                throw new DropBoxException($"{sharedFolder} is not shared.");

            return sharedFolderId;
        }

    }
}