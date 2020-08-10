using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decisions.DropboxApi.TestSuit
{
    static class TestData
    {
        public static string TestFolder = @"/DELETE_ME";

        public static string LocalTestDirectory = "c:/data/tmp";
        public static string LocalDownloadTestDirectory = "c:/data/tmp/download";

        public static string LocalTestFile = LocalTestDirectory +"/ttt.txt";
        public static string LocalDownloadTestFile = LocalDownloadTestDirectory+ "/ttt.txt";

        public static string RemoteTestFile = TestFolder + @"/tttt.txt";
        public static string RemoteNonExistentFile = "/nonexistent";

        public static string[] TestEmails = { "kovalchuk.i.v.1976@gmail.com", "chuk1976@yandex.ru" };
    }
}
