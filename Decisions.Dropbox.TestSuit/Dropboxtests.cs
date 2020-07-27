using DecisionsFramework.ServiceLayer.Services.Document;
using DecisionsFramework.ServiceLayer.Services.Folder;
using Dropbox.Api.Files;
using DropboxWebClientAPI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decicions.Dropbox.TestSuit
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
                md = DropBoxWebClientAPI.GetMetadata(TestData.AccessToken, TestData.TestFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (md == null)
                DropBoxWebClientAPI.CreateFolder(TestData.AccessToken, TestData.TestFolder);
        }

        [TestCleanup]
        public void CleanupTests()
        {
            DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, TestData.TestFolder);
        }

        [TestMethod]
        public void CreateFolder()
        {
            string testFolderPath = TestData.TestFolder + TestData.TestFolder;

            var entity = DropBoxWebClientAPI.CreateFolder(TestData.AccessToken, testFolderPath);

            Assert.IsTrue(entity.IsFolder);

        }

        [TestMethod]
        public void GetEntityList()
        {
            var fileList = DropBoxWebClientAPI.GetFilesArray(TestData.AccessToken, "/");
            Assert.IsTrue(fileList.Length > 0);

            var folderList = DropBoxWebClientAPI.GetFoldersArray(TestData.AccessToken, TestData.TestFolder);
            Assert.IsTrue(folderList.Length == 0);

            var folderList2 = DropBoxWebClientAPI.GetFoldersArray(TestData.AccessToken, "/");
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
                    DropBoxWebClientAPI.CreateFolder(TestData.AccessToken, testFolderPath + (i));

                var folderList = DropBoxWebClientAPI.GetFoldersArray(TestData.AccessToken, TestData.TestFolder, 3);
                Assert.IsTrue(folderList.Length == maxFolderCount);
            }
            finally
            {
                for (int i = 0; i < maxFolderCount; i++)
                {
                    try
                    {
                        DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, testFolderPath + i);
                    }
                    catch { break; }
                }
            }
        }

        [TestMethod]
        public void GetMetadata()
        {
            DropBoxWebClientAPI.UploadFile(TestData.AccessToken, TestData.LocalTestFile, TestData.TestFolder);
            string remoteFile = TestData.TestFolder + "/" + Path.GetFileName(TestData.LocalTestFile);
            try
            {
                var metadata = DropBoxWebClientAPI.GetMetadata(TestData.AccessToken, TestData.TestFolder);
                Assert.IsTrue(metadata.IsFolder);

                var metadataFile = DropBoxWebClientAPI.GetMetadata(TestData.AccessToken, remoteFile);
                Assert.IsTrue(metadataFile.IsFile);

                var metadata2 = DropBoxWebClientAPI.GetMetadata(TestData.AccessToken, "/unexisted");
                Assert.IsNull(metadata2);
            }
            finally
            {
                DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, remoteFile);
            }
        }

        [TestMethod]
        public void UploadAndDownloadFile()
        {
            string remoteFile = TestData.TestFolder + "/" + Path.GetFileName(TestData.LocalTestFile);
            try
            {
                var filemeta = DropBoxWebClientAPI.UploadFile(TestData.AccessToken, TestData.LocalTestFile, "");
                Assert.IsTrue(filemeta.IsFile);
                DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, filemeta.PathDisplay);

                var filemeta2 = DropBoxWebClientAPI.UploadFile(TestData.AccessToken, TestData.LocalTestFile, TestData.TestFolder);
                Assert.IsTrue(filemeta2.IsFile);
                DropBoxWebClientAPI.DownloadFile(TestData.AccessToken, remoteFile, TestData.LocalDownloadTestDirectory);
                Assert.IsTrue(System.IO.File.Exists(TestData.LocalDownloadTestFile));
            }
            finally
            {
                
            }
        }


        [TestMethod]
        public void DeleteEntity()
        {
            string remoteFile = TestData.TestFolder + "/" + Path.GetFileName(TestData.LocalTestFile);
            DropBoxWebClientAPI.UploadFile(TestData.AccessToken, TestData.LocalTestFile, TestData.TestFolder);

            string remoteFolder = TestData.TestFolder + TestData.TestFolder;
            var folderEntity = DropBoxWebClientAPI.CreateFolder(TestData.AccessToken, remoteFolder);

            try
            {
                DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, remoteFile);
                DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, remoteFolder);
            }
            finally
            {
                try
                {
                    DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, remoteFile);
                }
                catch { }
                try
                {
                    DropBoxWebClientAPI.DeleteFile(TestData.AccessToken, remoteFolder);
                }
                catch { }
            }
        }

        [TestMethod]
        public void ShareFolderTest()
        {
            try
            {
                DropBoxWebClientAPI.ShareFolder(TestData.AccessToken, TestData.TestFolder);

                DropBoxWebClientAPI.UnshareFolder(TestData.AccessToken, TestData.TestFolder);
            }
            finally
            {
                var sharedFolderList = DropBoxWebClientAPI.GetFoldersSharingSettings(TestData.AccessToken);
                foreach (var folder in sharedFolderList)
                {
                    if (TestData.TestFolder.Contains(folder.Name))
                        try
                        {
                            DropBoxWebClientAPI.UnshareFolder(TestData.AccessToken, folder.SharedFolderId);
                        }
                        catch { }
                }
            }
        }


    }

}

