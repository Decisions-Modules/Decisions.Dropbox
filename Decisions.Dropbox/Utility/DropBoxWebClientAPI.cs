using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Dropbox.Api;
using Dropbox.Api.Async;
using Dropbox.Api.CloudDocs;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;

namespace Decisions.DropboxApi
{
    public static partial class DropBoxWebClientAPI
    {
        public static DropboxFolder CreateFolder(string token, string path)
        {
            using (var client = new DropboxClient(token))
            {
                CreateFolderResult res = client.Files.CreateFolderV2Async(path).GetAwaiter().GetResult();
                return Mapper.MapFolder(res.Metadata);
            }
        }

        public static DropboxFolder[] GetFoldersArray(string token, string path, uint? limit = null)
        {
            var folderContext = GetFolderContext(token, path, limit);

            List<Metadata> listOfFolders = folderContext.Where(x => x.IsFolder).ToList();

            return listOfFolders.Select(Mapper.MapFolder).ToArray();
        }

        public static DropboxFile[] GetFilesArray(string token, string path, uint? limit = null)
        {
            var folderContext = GetFolderContext(token, path, limit);

            List<Metadata> listOfFiles = folderContext.Where(x => x.IsFile).ToList();

            return listOfFiles.Select(Mapper.MapFile).ToArray();
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

        public static DropboxFile UploadFile(string token, string fileForUploadingFullPathWithExtension, string dropboxFolderPath)
        {
            CorrectDropboxPath(ref dropboxFolderPath);
            if (string.IsNullOrEmpty(dropboxFolderPath) || dropboxFolderPath.Last() != '/')
                dropboxFolderPath += '/';

            using (var client = new DropboxClient(token))
            {
                var fileName = Path.GetFileName(fileForUploadingFullPathWithExtension);

                FileMetadata file = client.Files.UploadAsync($"{dropboxFolderPath}{fileName}",
                        new WriteMode().AsOverwrite,
                        false, DateTime.Now, false, null, false,
                        new StreamReader(fileForUploadingFullPathWithExtension).BaseStream)
                    .GetAwaiter().GetResult();

                return Mapper.MapFile(file);
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

        public static DropboxSharedFileMetadata GetSharedFileMetadata(string token, string filePath)
        {
            CorrectDropboxPath(ref filePath);
            using (var client = new DropboxClient(token))
            {
                SharedFileMetadata result = client.Sharing.GetFileMetadataAsync(filePath).GetAwaiter().GetResult();
                return Mapper.Map(result);
            }
        }

        /// <summary>
        ///     Get sharing settings for SPECIFIC folder by it's id
        /// </summary>
        public static DropboxSharedFolderMetadata GetSharedFolderMetadata(string token, string sharedFolder)
        {
            CorrectDropboxPath(ref sharedFolder);
            using (var client = new DropboxClient(token))
            {
                string sharedFolderId = GetSharedFolderId(client, sharedFolder);
                SharedFolderMetadata result = client.Sharing.GetFolderMetadataAsync(sharedFolderId).Result;
                return Mapper.Map(result);
            }
        }

        public static DropboxSharedFolderMetadata ShareFolder(string token, string folder, int millisecondsTimeout = 30000, bool forceAsync = false)
        {
            CorrectDropboxPath(ref folder);

            using (var client = new DropboxClient(token))
            {
                ShareFolderLaunch shareFolderLaunch = client.Sharing.ShareFolderAsync(folder, forceAsync: forceAsync).Result;

                SharedFolderMetadata sfm = shareFolderLaunch.AsComplete?.Value;
                if (shareFolderLaunch.IsAsyncJobId)
                {
                    string jobId = shareFolderLaunch.AsAsyncJobId.Value;
                    WaitForJob(client, jobId, millisecondsTimeout);
                    ShareFolderJobStatus jobStatus = client.Sharing.CheckShareJobStatusAsync(jobId).Result;
                    sfm = jobStatus.AsComplete?.Value;
                }

                return Mapper.Map(sfm);
            }
        }

        public static void UnshareFolder(string token, string sharedFolder, int millisecondsTimeout = 30000)
        {
            using (var client = new DropboxClient(token))
            {
                string sharedFolderId = GetSharedFolderId(client, sharedFolder);
                LaunchEmptyResult unshareFolderLaunch = client.Sharing.UnshareFolderAsync(sharedFolderId).GetAwaiter().GetResult();

                if (!unshareFolderLaunch.IsComplete)
                {
                    string jobId = unshareFolderLaunch.AsAsyncJobId.Value;
                    WaitForJob(client, jobId, millisecondsTimeout);
                }
            }
        }

        public static void UnshareFile(string token, string path)
        {
            using (var client = new DropboxClient(token))
            {
                client.Sharing.UnshareFileAsync(path).Wait();
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
                client.Sharing.RemoveFileMember2Async(pathToFile, new MemberSelector.Email(memberEmail)).Wait();
            }
        }

        public static DropboxUser[] FileMembersArray(string token, string path, uint limit = 100)
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

        public static void RemoveMemberFromFolder(string token, string sharedFolder, string memberEmail, int millisecondsTimeout = 30000 )
        {
            CorrectDropboxPath(ref sharedFolder);

            using (var client = new DropboxClient(token))
            {
                string sharedFolderId = GetSharedFolderId(client, sharedFolder);
                LaunchResultBase removeLaunch = client.Sharing.RemoveFolderMemberAsync(sharedFolderId, new MemberSelector.Email(memberEmail), false).Result;

                if (removeLaunch.IsAsyncJobId)
                {
                    string jobId = removeLaunch.AsAsyncJobId.Value;
                    WaitForJob(client, jobId, millisecondsTimeout);
                }
            }
        }

        public static DropboxUser[] FolderMembersArray(string token, string sharedFolder, uint limit = 100)
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
    }
}