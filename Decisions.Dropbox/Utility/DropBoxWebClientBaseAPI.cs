using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decisions.DropboxApi
{
    public static partial class DropBoxWebClientAPI
    {
        static void CorrectDropboxPath(ref string path)
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

        /// <summary>
        ///     Get sharing settings for ALL folders
        /// </summary>
        public static SharedFolderMetadata[] GetAllFoldersSharingSettings(string token, int limit = 1000)
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

        public static void UnshareFolderById(string token, string sharedFolderId)
        {
            using (var client = new DropboxClient(token))
            {
                client.Sharing.UnshareFolderAsync(sharedFolderId).GetAwaiter().GetResult();
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

        private static void WaitForJob(DropboxClient client, string jobId, int millisecondsTimeout)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

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
    }

}
