using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Naver.Compass.Common.Helper
{
    [TestFixture]
    public class GlobalDataTest
    {
        [Test]
        public void SetLibrarisExpandedTest()
        {
            var current = GlobalData.LibrariesExpanded;

            string expanded = "1111|1000000000000000000|1" ;
           
            GlobalData.LibrariesExpanded = expanded;
            Assert.That(Equals(GlobalData.LibrariesExpanded, expanded));

            GlobalData.LibrariesExpanded = current;
        }

        //[TestCase("en-US")]
        //[TestCase("ja-JP")]
        //[TestCase("ko-KR")]
        //[TestCase("zh-CN")]
        //public void FindResource(string culture)
        //{
        //    string curCulture = GlobalData.Culture;

        //    GlobalData.Culture = culture;

        //    string resource = GlobalData.FindResource("Menu_File_OpenRecent");
        //    switch(GlobalData.Culture)
        //    {
        //        case "en-US":
        //            Assert.AreEqual("Open Recent", resource);
        //            break;
        //        case "ja-JP":
        //            Assert.AreEqual("最近使用したドキュメント", resource);
        //            break;
        //        case "ko-KR":
        //            Assert.AreEqual("최근 문서 열기", resource);
        //            break;
        //        case "zh-CN":
        //            Assert.AreEqual("打开最近文件", resource);
        //            break;
        //    }

        //    GlobalData.Culture = curCulture;

        //}
    }
}
