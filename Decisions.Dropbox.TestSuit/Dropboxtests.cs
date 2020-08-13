using DecisionsFramework.ServiceLayer.Services.Document;
using DecisionsFramework.ServiceLayer.Services.Folder;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decisions.DropboxApi.TestSuit
{
    [TestClass]
    public class Dropboxtests
    {
        [TestInitialize]
        public void InitTests()
        {
            Metadata md = null;
            try
            {
                md = DropBoxWebClientAPI.GetMetadata(Token.AccessToken, TestData.TestFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (md == null)
                DropBoxWebClientAPI.CreateFolder(Token.AccessToken, TestData.TestFolder);
        }

        [TestCleanup]
        public void CleanupTests()
        {
            DropBoxWebClientAPI.DeleteResource(Token.AccessToken, TestData.TestFolder);
        }

        private string UploadTestFile()
        {
            string remoteFile = TestData.TestFolder + "/" + Path.GetFileName(TestData.LocalTestFile);
            DropBoxWebClientAPI.UploadFile(Token.AccessToken, TestData.LocalTestFile, TestData.TestFolder);
            return remoteFile;
        }

        [TestMethod]
        public void CreateFolder()
        {
            string testFolderPath = TestData.TestFolder + TestData.TestFolder;

            var entity = DropBoxWebClientAPI.CreateFolder(Token.AccessToken, testFolderPath);

            Assert.IsTrue(entity.ResourceType == DropboxResourceType.Folder);

        }

        [TestMethod]
        public void GetEntityList()
        {
            var fileList = DropBoxWebClientAPI.GetFilesArray(Token.AccessToken, "/");
            Assert.IsTrue(fileList.Length > 0);

            var folderList = DropBoxWebClientAPI.GetFoldersArray(Token.AccessToken, TestData.TestFolder);
            Assert.IsTrue(folderList.Length == 0);

            var folderList2 = DropBoxWebClientAPI.GetFoldersArray(Token.AccessToken, "/");
            Assert.IsTrue(folderList2.Length > 0);
        }

        [TestMethod]
        public void GetLongFolderList()
        {
            int maxFolderCount = 15;
            string testFolderPath = TestData.TestFolder + TestData.TestFolder;
            try
            {
                for (int i = 0; i < maxFolderCount; i++)
                    DropBoxWebClientAPI.CreateFolder(Token.AccessToken, testFolderPath + (i));

                var folderList = DropBoxWebClientAPI.GetFoldersArray(Token.AccessToken, TestData.TestFolder, 3);
                Assert.IsTrue(folderList.Length == maxFolderCount);
            }
            finally
            {
                for (int i = 0; i < maxFolderCount; i++)
                {
                    try
                    {
                        DropBoxWebClientAPI.DeleteResource(Token.AccessToken, testFolderPath + i);
                    }
                    catch { break; }
                }
            }
        }

        [TestMethod]
        public void GetMetadata()
        {
            string remoteFile = UploadTestFile();
            try
            {
                var metadata = DropBoxWebClientAPI.GetMetadata(Token.AccessToken, TestData.TestFolder);
                Assert.IsTrue(metadata.IsFolder);

                var metadataFile = DropBoxWebClientAPI.GetMetadata(Token.AccessToken, remoteFile);
                Assert.IsTrue(metadataFile.IsFile);

                var metadata2 = DropBoxWebClientAPI.GetMetadata(Token.AccessToken, TestData.RemoteNonExistentFile);
                Assert.IsNull(metadata2);
            }
            finally
            {
                DropBoxWebClientAPI.DeleteResource(Token.AccessToken, remoteFile);
            }
        }

        [TestMethod]
        public void UploadAndDownloadFile()
        {
            string remoteFile = TestData.TestFolder + "/" + Path.GetFileName(TestData.LocalTestFile);
            try
            {
                var filemeta = DropBoxWebClientAPI.UploadFile(Token.AccessToken, TestData.LocalTestFile, "");
                Assert.IsTrue(filemeta.ResourceType == DropboxResourceType.File);
                DropBoxWebClientAPI.DeleteResource(Token.AccessToken, filemeta.PathDisplay);

                var filemeta2 = DropBoxWebClientAPI.UploadFile(Token.AccessToken, TestData.LocalTestFile, TestData.TestFolder);
                Assert.IsTrue(filemeta2.ResourceType == DropboxResourceType.File);
                DropBoxWebClientAPI.DownloadFile(Token.AccessToken, remoteFile, TestData.LocalDownloadTestDirectory);
                Assert.IsTrue(System.IO.File.Exists(TestData.LocalDownloadTestFile));
            }
            finally
            {

            }
        }

        [TestMethod]
        public void DeleteResource()
        {
            string remoteFile = UploadTestFile();

            string remoteFolder = TestData.TestFolder + TestData.TestFolder;
            var folderEntity = DropBoxWebClientAPI.CreateFolder(Token.AccessToken, remoteFolder);

            try
            {
                DropBoxWebClientAPI.DeleteResource(Token.AccessToken, remoteFile);
                DropBoxWebClientAPI.DeleteResource(Token.AccessToken, remoteFolder);

                bool deleteError = false;
                try
                {
                    DropBoxWebClientAPI.DeleteResource(Token.AccessToken, TestData.RemoteNonExistentFile);
                }
                catch (Dropbox.Api.ApiException<Dropbox.Api.Files.DeleteError>)
                {
                    deleteError = true;
                }
                Assert.IsTrue(deleteError);
            }
            finally
            {
                try
                {
                    DropBoxWebClientAPI.DeleteResource(Token.AccessToken, remoteFile);
                }
                catch { }
                try
                {
                    DropBoxWebClientAPI.DeleteResource(Token.AccessToken, remoteFolder);
                }
                catch { }
            }
        }

        [TestMethod]
        public void ShareFolderTest()
        {
            DropboxSharedFolderMetadata sharedMeta1 = null, sharedMetaAgain1, sharedMeta2 = null;

            try
            {
                sharedMeta1 = DropBoxWebClientAPI.ShareFolder(Token.AccessToken, TestData.TestFolder, 10000, false);
                sharedMetaAgain1 = DropBoxWebClientAPI.GetSharedFolderMetadata(Token.AccessToken, TestData.TestFolder);

                DropBoxWebClientAPI.UnshareFolder(Token.AccessToken, TestData.TestFolder);

                sharedMeta2 = DropBoxWebClientAPI.ShareFolder(Token.AccessToken, TestData.TestFolder, 10000, true);
                DropBoxWebClientAPI.UnshareFolder(Token.AccessToken, TestData.TestFolder);
            }
            finally
            {
                var sharedFolderList = DropBoxWebClientAPI.GetAllFoldersSharingSettings(Token.AccessToken);
                foreach (var folder in sharedFolderList)
                {
                    if (TestData.TestFolder.Contains(folder.Name))
                        try
                        {
                            DropBoxWebClientAPI.UnshareFolderById(Token.AccessToken, folder.SharedFolderId);
                        }
                        catch { }
                }
            }

            Assert.IsNotNull(sharedMeta1);
            Assert.IsNotNull(sharedMeta2);
        }

        [TestMethod]
        public void SharedLinkTest()
        {
            string folderUrl = DropBoxWebClientAPI.CreateSharedLink(Token.AccessToken, TestData.TestFolder);
            Assert.IsNotNull(folderUrl);
            DropBoxWebClientAPI.RevokeSharedLink(Token.AccessToken, folderUrl);

            string remoteFile = UploadTestFile();
            string fileUrl = DropBoxWebClientAPI.CreateSharedLink(Token.AccessToken, remoteFile);
            Assert.IsNotNull(fileUrl);

            DropboxSharedFileMetadata fileMeta = DropBoxWebClientAPI.GetSharedFileMetadata(Token.AccessToken, remoteFile);
            DropBoxWebClientAPI.RevokeSharedLink(Token.AccessToken, fileUrl);

            bool revokeError = false;
            try
            {
                DropBoxWebClientAPI.RevokeSharedLink(Token.AccessToken, fileUrl);
            }
            catch
            {
                revokeError = true;
            }
            Assert.IsTrue(revokeError);
        }

        [TestMethod]
        public void FileMemberTest()
        {
            string remoteFile = UploadTestFile();
            DropboxUser[] users0 = DropBoxWebClientAPI.FileMembersArray(Token.AccessToken, remoteFile, 1);

            DropBoxWebClientAPI.AddMembersToFile(Token.AccessToken, remoteFile, DropBoxAccessLevel.viewer, TestData.TestEmails);
            DropboxUser[] users2 = DropBoxWebClientAPI.FileMembersArray(Token.AccessToken, remoteFile, 1);
            Assert.IsTrue(users2.Length >= TestData.TestEmails.Length);
            foreach (var email in TestData.TestEmails)
            {
                var foundEmail = users2.FirstOrDefault((it) => { return it.Email == email; });
                Assert.IsNotNull(foundEmail);
            }

            DropBoxWebClientAPI.RemoveMemberFromFile(Token.AccessToken, remoteFile, TestData.TestEmails[0]);
            DropboxUser[] users3 = DropBoxWebClientAPI.FileMembersArray(Token.AccessToken, remoteFile);
            Assert.AreEqual(users2.Length, users3.Length + 1);

            DropBoxWebClientAPI.UnshareFile(Token.AccessToken, remoteFile);
            DropboxUser[] noUsers = DropBoxWebClientAPI.FileMembersArray(Token.AccessToken, remoteFile);
            foreach (var email in TestData.TestEmails)
            {
                var foundEmail = noUsers.FirstOrDefault((it) => { return it.Email == email; });
                Assert.IsNull(foundEmail);
            }
        }

        [TestMethod]
        public void FolderMemberTest()
        {
            DropBoxWebClientAPI.ShareFolder(Token.AccessToken, TestData.TestFolder, 10000, false);
            try
            {
                DropboxUser[] users0 = DropBoxWebClientAPI.FolderMembersArray(Token.AccessToken, TestData.TestFolder, 1);

                DropBoxWebClientAPI.AddMembersToFolder(Token.AccessToken, TestData.TestFolder, DropBoxAccessLevel.viewer, TestData.TestEmails);
                DropboxUser[] users2 = DropBoxWebClientAPI.FolderMembersArray(Token.AccessToken, TestData.TestFolder, 1);
                Assert.IsTrue(users2.Length >= TestData.TestEmails.Length);
                foreach (var email in TestData.TestEmails)
                {
                    var foundEmail = users2.FirstOrDefault((it) => { return it.Email == email; });
                    Assert.IsNotNull(foundEmail);
                }

                DropBoxWebClientAPI.RemoveMemberFromFolder(Token.AccessToken, TestData.TestFolder, TestData.TestEmails[0]);
                DropboxUser[] users3 = DropBoxWebClientAPI.FolderMembersArray(Token.AccessToken, TestData.TestFolder);
                Assert.AreEqual(users2.Length-1, users3.Length);

                DropBoxWebClientAPI.UnshareFolder(Token.AccessToken, TestData.TestFolder);
                DropboxUser[] noUsers = DropBoxWebClientAPI.FolderMembersArray(Token.AccessToken, TestData.TestFolder);
                foreach (var email in TestData.TestEmails)
                {
                    var foundEmail = noUsers.FirstOrDefault((it) => { return it.Email == email; });
                    Assert.IsNull(foundEmail);
                }
            }
            finally
            {
                try
                {
                    DropBoxWebClientAPI.UnshareFolder(Token.AccessToken, TestData.TestFolder);
                }
                catch { }
            }
        }

    }

}

