using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dropbox.Api;
using Dropbox.Api.CloudDocs;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using DropboxWebClientAPI.Models;

namespace DropboxWebClientAPI
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
            if (path == "/" || path == "\\")
                path = "";

            CorrectDropboxPath(ref path);

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
                            if(it is Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError>)
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

        public static void DeleteFile(string token, string filePath)
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
        public static SharedFolderMetadata GetFolderSharingSettings(string token, string folder)
        {
            CorrectDropboxPath(ref folder);
            using (var client = new DropboxClient(token))
            {
                Metadata metadata = client.Files.GetMetadataAsync(folder).Result;

                var result = client.Sharing.GetFolderMetadataAsync(metadata.AsFolder.SharedFolderId).GetAwaiter().GetResult();
                return result;
            }
        }

        public static bool AddMembersToFile(string token, string pathToFile, params string[] emails)
        {
            CorrectDropboxPath(ref pathToFile);
            List<MemberSelector> membersEmails = new List<MemberSelector>();
            foreach (string email in emails)
            {
                membersEmails.Add(new MemberSelector.Email(email));
            }

            using (var client = new DropboxClient(token))
            {
                try
                {
                    client.Sharing.AddFileMemberAsync(pathToFile, membersEmails).GetAwaiter().GetResult();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }


        public static void AddMembersToFolder(string token, string sharedFolder, params string[] emails)
        {
            CorrectDropboxPath(ref sharedFolder);

            List<AddMember> membersEmails = new List<AddMember>();
            foreach (string email in emails)
            {
                membersEmails.Add(new AddMember(new MemberSelector.Email(email)));
            }

            using (var client = new DropboxClient(token))
            {

                Metadata metadata = client.Files.GetMetadataAsync(sharedFolder).Result;
                client.Sharing.AddFolderMemberAsync(metadata.AsFolder.SharedFolderId, membersEmails).GetAwaiter().GetResult();
            }

        }


        public static void RemoveMemberFromFolder(string token, string sharedFolder, string memberEmail)
        {
            CorrectDropboxPath(ref sharedFolder);

            using (var client = new DropboxClient(token))
            {
                Metadata metadata = client.Files.GetMetadataAsync(sharedFolder).Result;
                client.Sharing.RemoveFolderMemberAsync(metadata.AsFolder.SharedFolderId, new MemberSelector.Email(memberEmail),
                    false).GetAwaiter().GetResult();
            }

        }


        public static bool RemoveMemberFromFile(string token, string pathToFile, string memberEmail)
        {
            CorrectDropboxPath(ref pathToFile);
            using (var client = new DropboxClient(token))
            {
                try
                {
                    client.Sharing.RemoveFileMember2Async(pathToFile, new MemberSelector.Email(memberEmail)).GetAwaiter()
                        .GetResult();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }


        public static void ShareFolder(string token, string folder)
        {
            CorrectDropboxPath(ref folder);

            using (var client = new DropboxClient(token))
            {
                var shareFolderLaunch = client.Sharing.ShareFolderAsync(folder).GetAwaiter().GetResult();
                var complete = shareFolderLaunch.AsComplete;

                SharedFolderMetadata res = complete.Value;
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


        public static User[] FileMembersArray(string token, string path)
        {
            using (var client = new DropboxClient(token))
            {
                IList<UserFileMembershipInfo> result = null;

                result = client.Sharing.ListFileMembersAsync(path).GetAwaiter().GetResult().Users;

                return result.Select(x => Mapper.Map(x.User)).ToArray();
            }
        }

        public static User[] FolderMembersArray(string token, string sharedFolder)
        {
            using (var client = new DropboxClient(token))
            {
                IList<UserMembershipInfo> result = null;
                Metadata metadata = client.Files.GetMetadataAsync(sharedFolder).Result;
                result = client.Sharing.ListFolderMembersAsync(metadata.AsFolder.SharedFolderId).GetAwaiter().GetResult().Users;

                return result.Select(x => Mapper.Map(x.User)).ToArray();
            }
        }

        public static bool UnshareFile(string token, string path)
        {
            using (var client = new DropboxClient(token))
            {
                try
                {
                    client.Sharing.UnshareFileAsync(path).GetAwaiter().GetResult();
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        public static void UnshareFolder(string token, string sharedFolder)
        {
            using (var client = new DropboxClient(token))
            {
                Metadata metadata = client.Files.GetMetadataAsync(sharedFolder).Result;
                client.Sharing.UnshareFolderAsync(metadata.AsFolder.SharedFolderId).GetAwaiter().GetResult();
            }
        }
    }
}