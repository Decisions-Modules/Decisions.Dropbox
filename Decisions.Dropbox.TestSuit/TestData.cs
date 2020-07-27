using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decicions.Dropbox.TestSuit
{
    static class TestData
    {
        public static string AccessToken = "tB-tWqjJhvAAAAAAAAAAGr2AlPqx9Y2jUGXTReo5fTBML7XOkXUCiRKFsAK1UVRC";

        public static string TestFolder = @"/DELETE_ME";
        public static string LocalTestDirectory = "c:/data/tmp";
        public static string LocalDownloadTestDirectory = "c:/data/tmp/download";

        public static string LocalTestFile = LocalTestDirectory +"/ttt.txt";
        public static string LocalDownloadTestFile = LocalDownloadTestDirectory+ "/ttt.txt";

        public static string RemoteTestFile = TestFolder + @"/ttt.txt";
    }
}
