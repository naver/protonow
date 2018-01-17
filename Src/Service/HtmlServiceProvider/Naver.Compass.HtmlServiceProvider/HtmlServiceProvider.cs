using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Win32;
using Naver.Compass.Service.Document;
using System.Diagnostics;

namespace Naver.Compass.Service.Html
{
    /*
     * [Output] + [data] + ["page_guid"] + page_data.js // var page_data={} / var master_data={} 
     *          |        |
     *          |        |
     *          |        + document_data.js // var htDocumentData={}
     *          |
     *          + [images]
     *          |
     *          + [resources] // Viewer 
     *          |
     *          + index.html // Viewer
     *          |
     *          + m_viewer.html // Viewer
     *          |
     *          + viewer.html // Viewer
     * 
     * */
    public class HtmlServiceProvider : IHtmlServiceProvider
    {
        public HtmlServiceProvider()
        {
            _bByMD5 = false;
        }

        #region IHtmlServiceProvider
        public void RenderDifferInfo(List<IDocument> Docs, string outputFolder = null)
        {
            if (Docs == null || Docs.Count<1)
            {
                throw new ArgumentException("Documents is null!");
            }

            if (!String.IsNullOrEmpty(outputFolder))
            {
                InitializeOutputFolder(outputFolder);
            }
            else
            {
                InitializeOutputFolder(_doc.GeneratorConfigurationSet.DefaultHtmlConfiguration.OutputFolder);
            }

            CopyDifferTemplateFilesToOutputFolder();

            // Generate document_data.js
            JsDifferInfo jsDoc = new JsDifferInfo(this, Docs);
            jsDoc.SaveToJsFile();


        }
        public void Render(IDocument doc, string outputFolder, bool ByMD5 = false)
        {
            if (doc == null)
            {
                throw new ArgumentException("Document is null!");
            }
            _doc = doc;
            _bByMD5 = ByMD5;

            if (!String.IsNullOrEmpty(outputFolder))
            {
                InitializeOutputFolder(outputFolder);
            }
            else
            {
                InitializeOutputFolder(_doc.GeneratorConfigurationSet.DefaultHtmlConfiguration.OutputFolder);
            }

            if(ByMD5==false)
            {
                #region create html without md5
                // Set the image output directory based on output folder if it is invalid path.
                if (!Directory.Exists(_imagesStreamManager.WorkingDirectory))
                {
                    _imagesStreamManager.WorkingDirectory = Path.Combine(_outputFolder, "images");
                }

                // Set the start html file.
                _startHtmlFile = _outputFolder + START_PAGE_NAME;
                CopyTemplateFilesToOutputFolder();

                // Generate document_data.js
                JsDocument jsDoc = new JsDocument(this);
                jsDoc.SaveToJsFile();

                Guid currentPageGuid = _doc.GeneratorConfigurationSet.DefaultHtmlConfiguration.CurrentPage;
                if (currentPageGuid != Guid.Empty)
                {
                    // Only generate current page.
                    IPage currentPage = _doc.Pages[currentPageGuid];
                    if (currentPage != null)
                    {
                        bool isClosedPage = false;
                        if (!currentPage.IsOpened)
                        {
                            isClosedPage = true;
                            currentPage.Open();
                        }

                        JsPage jsPage = new JsPage(this, currentPage);
                        jsPage.SaveToJsFile();

                        if (isClosedPage)
                        {
                            currentPage.Close();
                        }
                    }
                }
                else
                {
                    // Generate all pages.
                    foreach (IPage page in _doc.Pages)
                    {
                        bool isClosedPage = false;
                        if (!page.IsOpened)
                        {
                            isClosedPage = true;
                            page.Open();
                        }

                        JsPage jsPage = new JsPage(this, page);
                        jsPage.SaveToJsFile();

                        if (isClosedPage)
                        {
                            page.Close();
                        }
                    }
                }

                // Generate all master pages.
                foreach (IPage page in _doc.MasterPages)
                {
                    bool isClosedPage = false;
                    if (!page.IsOpened)
                    {
                        isClosedPage = true;
                        page.Open();
                    }

                    JsPage jsPage = new JsPage(this, page);
                    jsPage.SaveToJsFile();

                    if (isClosedPage)
                    {
                        page.Close();
                    }
                }
                #endregion
            }
            else
            {

                // Set the start Html file.
                _startHtmlFile = _outputFolder + START_PAGE_NAME;

                // Generate all pages.
                foreach (IPage page in _doc.Pages)
                {
                    //this is just for debug test, delete it later       
                    Debug.WriteLine("===========================page md5 hash===========================");
                    string szhash = MD5HashManager.GetHash(page, true);
                    Debug.WriteLine("====page md5:" + szhash + "  ===========================");


                    bool isClosedPage = false;
                    if (!page.IsOpened)
                    {
                        isClosedPage = true;
                        page.Open();
                    }

                    JsPage jsPage = new JsPage(this, page, _bByMD5);
                    jsPage.SaveToJsFile();

                    if (isClosedPage)
                    {
                        page.Close();
                    }
                }  

                // Generate all master pages.
                foreach (IPage page in _doc.MasterPages)
                {
                    bool isClosedPage = false;
                    if (!page.IsOpened)
                    {
                        isClosedPage = true;
                        page.Open();
                    }

                    //this is just for debug test, delete it later       
                    Debug.WriteLine("===========================page md5 hash===========================");
                    string szhash = MD5HashManager.GetHash(page, true);
                    Debug.WriteLine("====page md5:" + szhash + "  ===========================");


                    JsPage jsPage = new JsPage(this, page,true);
                    jsPage.SaveToJsFile();

                    if (isClosedPage)
                    {
                        page.Close();
                    }
                }

            }           

            if(ByMD5==true)
            {
                _doc.ImagesStreamManager.ClearCachedStream();

            }    
            // Don't hold the document any more.
            _doc = null;

            // Reset image working directory.
           // _imagesStreamManager.WorkingDirectory = String.Empty;
        }
        public string StartHtmlFile
        {
            get { return _startHtmlFile; }
        }
        public string StartPageName
        {
            get { return START_PAGE_NAME; }
        }
        public IHtmlHashStreamManager ImagesStreamManager
        {
            get { return _imagesStreamManager; }
        }
        #endregion

        #region Pulbic Function and Property
        public IDocument RenderingDocument
        {
            get { return _doc; }
            set { _doc = value; }
        }
        public string OutputFolder
        {
            get { return _outputFolder; }
        }
        public string ProductVersionInfo
        {
            get
            {
                try
                {
                    // js version format is x.x.x
                    RegistryKey compassKey = Registry.LocalMachine.OpenSubKey(@"Software\Design Studio");

                    if (compassKey != null)
                    {
                        string productVersion = compassKey.GetValue("CurrentVersion").ToString();
                        if (String.IsNullOrEmpty(productVersion) == false)
                        {
                            productVersion = productVersion.Substring(0, productVersion.LastIndexOf("."));
                            return "  " + productVersion;
                        }
                    }
                }
                catch
                {
                }

                return String.Empty;
            }
        }

        public bool IsHtmlGenerating { get; set; }
        #endregion

        #region Helper Methods
        private void InitializeOutputFolder(string outputFolder)
        {
            _outputFolder = outputFolder;
            DirectoryInfo dirInfo = new DirectoryInfo(_outputFolder);
            if (!dirInfo.Exists)
            {
                dirInfo = Directory.CreateDirectory(_outputFolder);
            }
 
            // Process.Start will throw exception if the _startHtmlFile contains redundant backslash (\)
            // Remove redundant backslash via DirectoryInfo FullName property.
            _outputFolder = dirInfo.FullName;

            if (_outputFolder.LastIndexOf(@"\") != _outputFolder.Length - 1)
            {
                _outputFolder += @"\";
            }
        }
        private void CopyTemplateFilesToOutputFolder()
        {
            // Copy index.html
            SaveResourceToFile("Naver.Compass.Service.Html.resources.index.html", _startHtmlFile);
            
            //Copy viewer.html
            SaveResourceToFile("Naver.Compass.Service.Html.resources.viewer.html", _outputFolder + "viewer.html");

            if(_doc.GeneratorConfigurationSet.DefaultHtmlConfiguration.GenerateMobileFiles)
            {
                SaveResourceToFile("Naver.Compass.Service.Html.resources.m_viewer.html", _outputFolder + "m_viewer.html");
            }

            // Copy css 
            string cssFolder = _outputFolder + "resources" + @"\" + "css";
            Directory.CreateDirectory(cssFolder);
            cssFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.css.libs.min.css", cssFolder + "libs.min.css");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.css.protoNow_frame_mobile.min.css", cssFolder + "protoNow_frame_mobile.min.css");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.css.protoNow_frame_pc.min.css", cssFolder + "protoNow_frame_pc.min.css");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.css.protoNow_page.min.css", cssFolder + "protoNow_page.min.css");

            string cssMapFolder = cssFolder += @"\map";
            Directory.CreateDirectory(cssMapFolder);
            cssMapFolder += @"\";

            SaveResourceToFile("Naver.Compass.Service.Html.resources.css.map.protoNow_frame_mobile.css.map", cssMapFolder + "protoNow_frame_mobile.css.map");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.css.map.protoNow_frame_pc.css.map", cssMapFolder + "protoNow_frame_pc.css.map");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.css.map.protoNow_page.css.map", cssMapFolder + "protoNow_page.css.map");

            // Copy images
            string imagesFolder = _outputFolder + "resources" + @"\" + "images";
            Directory.CreateDirectory(imagesFolder);
            imagesFolder += @"\";

            // Copy m folder
            string mFolder = imagesFolder + "m";
            Directory.CreateDirectory(mFolder);
            mFolder += @"\";

            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.m.sp_protoNow_m.png", mFolder + "sp_protoNow_m.png");

            // Copy widget folder
            string widgetFolder = imagesFolder + "widget";
            Directory.CreateDirectory(widgetFolder);
            widgetFolder += @"\";

            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.widget.btn_close_widget.png", widgetFolder + "btn_close_widget.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.widget.flickPage.png", widgetFolder + "flickPage.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.widget.flickPage_on.png", widgetFolder + "flickPage_on.png");

            // a
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ajax-loader.gif", imagesFolder + "ajax-loader.gif");

            // b
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.blank.png", imagesFolder + "blank.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.btm_bnr.png", imagesFolder + "btm_bnr.png");

            // f
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_16.ico", imagesFolder + "favicon_16.ico");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_24.ico", imagesFolder + "favicon_24.ico");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_protoNow_w114.png", imagesFolder + "favicon_protoNow_w114.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_protoNow_w57.png", imagesFolder + "favicon_protoNow_w57.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.folder.png", imagesFolder + "folder.png");

            // i
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ico_pageNode.svg", imagesFolder + "ico_pageNode.svg");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ico_unfd.gif", imagesFolder + "ico_unfd.gif");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ico_unfd.png", imagesFolder + "ico_unfd.png");

            // l
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.logo_mobileViewer.svg", imagesFolder + "logo_mobileViewer.svg");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.logo_protoNow.png", imagesFolder + "logo_protoNow.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.logo_protoNow_x2.png", imagesFolder + "logo_protoNow_x2.png");

            // m
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.minus.gif", imagesFolder + "minus.gif");

            // p
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.plus.gif", imagesFolder + "plus.gif");

            // s
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_mobile_device.png", imagesFolder + "sp_mobile_device.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_mobile_device2.png", imagesFolder + "sp_mobile_device2.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_protoNow.png", imagesFolder + "sp_protoNow.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_protoNow_x2.png", imagesFolder + "sp_protoNow_x2.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_protonow2.png", imagesFolder + "sp_protonow2.png");

            // t
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.transparent.gif", imagesFolder + "transparent.gif");

            // w
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hlb.png", imagesFolder + "watch_hlb.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hlm.png", imagesFolder + "watch_hlm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hlt.png", imagesFolder + "watch_hlt.png");

            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hrb.png", imagesFolder + "watch_hrb.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hrm.png", imagesFolder + "watch_hrm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hrt.png", imagesFolder + "watch_hrt.png");

            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vbl.png", imagesFolder + "watch_vbl.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vbm.png", imagesFolder + "watch_vbm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vbr.png", imagesFolder + "watch_vbr.png");

            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vtl.png", imagesFolder + "watch_vtl.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vtm.png", imagesFolder + "watch_vtm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vtr.png", imagesFolder + "watch_vtr.png");

            // Copy js 
            string scriptsFolder = _outputFolder + "resources" + @"\" + "js";
            Directory.CreateDirectory(scriptsFolder);
            scriptsFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.js.frame.min.js", scriptsFolder + "frame.min.js");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.js.page.min.js", scriptsFolder + "page.min.js");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.js.protoNowStarter.min.js", scriptsFolder + "protoNowStarter.min.js");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.js.vendor.min.js", scriptsFolder + "vendor.min.js");

            // Copy locales
            string localesFolder = _outputFolder + "resources" + @"\" + "locales";
            Directory.CreateDirectory(localesFolder);
            
            // zh
            string zhFolder = localesFolder +  @"\zh";
            Directory.CreateDirectory(zhFolder);
            zhFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.zh.translation.json", zhFolder + "translation.json");

            // en
            string enFolder = localesFolder + @"\en";
            Directory.CreateDirectory(enFolder);
            enFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.en.translation.json", enFolder + "translation.json");

            // ja
            string jaFolder = localesFolder + @"\ja";
            Directory.CreateDirectory(jaFolder);
            jaFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.ja.translation.json", jaFolder + "translation.json");

            // ko
            string koFolder = localesFolder + @"\ko";
            Directory.CreateDirectory(koFolder);
            koFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.ko.translation.json", koFolder + "translation.json");
        }
        private void CopyDifferTemplateFilesToOutputFolder()
        {
            string targetFolder = _outputFolder.Substring(0, _outputFolder.LastIndexOf(@"data"));

            // Copy index.html
            SaveResourceToFile("Naver.Compass.Service.Html.resources.differ.index.html", targetFolder + "index.html");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.differ.viewer.html", targetFolder + "viewer.html");


            // Copy css 
            string cssFolder = targetFolder + "css";
            Directory.CreateDirectory(cssFolder);
            cssFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.differ.css.index.css", cssFolder + "index.css");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.differ.css.viewer.css", cssFolder + "viewer.css");

            // Copy js 
            string scriptsFolder = targetFolder + "js";
            Directory.CreateDirectory(scriptsFolder);
            scriptsFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.differ.js.index.js", scriptsFolder + "index.js");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.differ.js.viewer.js",scriptsFolder + "viewer.js");


            // Copy images
            string imagesFolder = targetFolder + "css" + @"\" + "images";
            Directory.CreateDirectory(imagesFolder);
            imagesFolder += @"\";

            // Copy diff folder
            string diffFolder = imagesFolder + "diff";
            Directory.CreateDirectory(diffFolder);
            diffFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.btn-diff-active.png", diffFolder + "btn-diff-active.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.btn-diff-normal.png", diffFolder + "btn-diff-normal.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-both-s-c.png", diffFolder + "bullet-both-s-c.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-both-s-w.png", diffFolder + "bullet-both-s-w.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-circle-l.png", diffFolder + "bullet-circle-l.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-circle-s.png", diffFolder + "bullet-circle-s.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-triangle-l.png", diffFolder + "bullet-triangle-l.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-triangle-s.png", diffFolder + "bullet-triangle-s.png");

            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-triangle-s-w.png", diffFolder + "bullet-triangle-s-w.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.diff.bullet-circle-s-w.png", diffFolder + "bullet-circle-s-w.png");

            // Copy m folder
            string mFolder = imagesFolder + "m";
            Directory.CreateDirectory(mFolder);
            mFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.m.sp_protoNow_m.png", mFolder + "sp_protoNow_m.png");

            // Copy widget folder
            string widgetFolder = imagesFolder + "widget";
            Directory.CreateDirectory(widgetFolder);
            widgetFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.widget.btn_close_widget.png", widgetFolder + "btn_close_widget.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.widget.flickPage.png", widgetFolder + "flickPage.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.widget.flickPage_on.png", widgetFolder + "flickPage_on.png");
            // a
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ajax-loader.gif", imagesFolder + "ajax-loader.gif");
            // b
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.blank.png", imagesFolder + "blank.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.btm_bnr.png", imagesFolder + "btm_bnr.png");
            // f
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_16.ico", imagesFolder + "favicon_16.ico");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_24.ico", imagesFolder + "favicon_24.ico");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_protoNow_w114.png", imagesFolder + "favicon_protoNow_w114.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.favicon_protoNow_w57.png", imagesFolder + "favicon_protoNow_w57.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.folder.png", imagesFolder + "folder.png");
            // i
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ico_pageNode.svg", imagesFolder + "ico_pageNode.svg");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ico_unfd.gif", imagesFolder + "ico_unfd.gif");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.ico_unfd.png", imagesFolder + "ico_unfd.png");
            // l
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.logo_mobileViewer.svg", imagesFolder + "logo_mobileViewer.svg");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.logo_protoNow.png", imagesFolder + "logo_protoNow.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.logo_protoNow_x2.png", imagesFolder + "logo_protoNow_x2.png");
            // m
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.minus.gif", imagesFolder + "minus.gif");
            // p
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.plus.gif", imagesFolder + "plus.gif");
            // s
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_mobile_device.png", imagesFolder + "sp_mobile_device.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_mobile_device2.png", imagesFolder + "sp_mobile_device2.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_protoNow.png", imagesFolder + "sp_protoNow.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_protoNow_x2.png", imagesFolder + "sp_protoNow_x2.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.sp_protonow2.png", imagesFolder + "sp_protonow2.png");
            // t
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.transparent.gif", imagesFolder + "transparent.gif");
            // w
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hlb.png", imagesFolder + "watch_hlb.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hlm.png", imagesFolder + "watch_hlm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hlt.png", imagesFolder + "watch_hlt.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hrb.png", imagesFolder + "watch_hrb.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hrm.png", imagesFolder + "watch_hrm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_hrt.png", imagesFolder + "watch_hrt.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vbl.png", imagesFolder + "watch_vbl.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vbm.png", imagesFolder + "watch_vbm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vbr.png", imagesFolder + "watch_vbr.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vtl.png", imagesFolder + "watch_vtl.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vtm.png", imagesFolder + "watch_vtm.png");
            SaveResourceToFile("Naver.Compass.Service.Html.resources.images.watch_vtr.png", imagesFolder + "watch_vtr.png");

            // Copy locales
            string localesFolder = targetFolder + "resources" + @"\" + "locales";
            Directory.CreateDirectory(localesFolder);

            // zh
            string zhFolder = localesFolder + @"\zh";
            Directory.CreateDirectory(zhFolder);
            zhFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.zh.translation.json", zhFolder + "translation.json");

            // en
            string enFolder = localesFolder + @"\en";
            Directory.CreateDirectory(enFolder);
            enFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.en.translation.json", enFolder + "translation.json");

            // ja
            string jaFolder = localesFolder + @"\ja";
            Directory.CreateDirectory(jaFolder);
            jaFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.ja.translation.json", jaFolder + "translation.json");

            // ko
            string koFolder = localesFolder + @"\ko";
            Directory.CreateDirectory(koFolder);
            koFolder += @"\";
            SaveResourceToFile("Naver.Compass.Service.Html.resources.locales.ko.translation.json", koFolder + "translation.json");

        }

        public void SaveResourceToFile(string resource, string filePath)
        {
            using (Stream stream = _assembly.GetManifestResourceStream(resource))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
        #endregion

        Assembly _assembly = Assembly.GetExecutingAssembly();
        private IDocument _doc;
        private string _startHtmlFile;
        private string _outputFolder;
        private readonly string START_PAGE_NAME = @"index.html";
        private HtmlHashStreamManager _imagesStreamManager = new HtmlHashStreamManager();
        private bool _bByMD5;
    }
}
