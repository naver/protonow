using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using Naver.Compass.Common.Helper;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Threading;
using Naver.Compass.Service.Html;
using System.Diagnostics;
using Naver.Compass.Service;
using Naver.Compass.WidgetLibrary;
using Naver.Compass.Service.WebServer;


namespace Naver.Compass.Module.PreviewModule
{
    partial class PagePreViewModel : ViewModelBase
    {
        #region Constructor
        public PagePreViewModel()
        {
            if (IsInDesignMode)
                return;

            _pageGID = Guid.Empty;
            _busyIndicator = ServiceLocator.Current.GetInstance<BusyIndicatorContext>();
            _htmlService = ServiceLocator.Current.GetInstance<IHtmlServiceProvider>();
            _ListEventAggregator.GetEvent<PagePreviewEvent>().Subscribe(PagePreviewEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<GenerateHTMLEvent>().Subscribe(GenerateHTMLEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<GenerateMD5HTMLEvent>().Subscribe(GenerateMD5HTMLEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<PublishHTMLEvent>().Subscribe(UploadHtmlEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<PublishMD5HTMLEvent>().Subscribe(UploadMD5HtmlEventHandler, ThreadOption.UIThread);
            _ListEventAggregator.GetEvent<SynUploadEvent>().Subscribe(OnSynUpload, ThreadOption.UIThread);

        }
        #endregion

        #region Private Member
        Guid _pageGID;  //this is a loading page tag;
        string _outputFolder;        
        private BusyIndicatorContext _busyIndicator;
        public List<WidgetPreViewModeBase> _currentItems;
        Guid _currentPageGID = Guid.Empty;
        Guid _waitingPageGID = Guid.Empty;
        IHtmlServiceProvider _htmlService;
        #endregion Private member


        #region Private Function
        private void ConvetToImage(Guid widgetID, Guid viewID, Visual visual, int width, int height)
        {
            try
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(visual);


                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                using (Stream stream = new MemoryStream())
                {
                    encoder.Save(stream);

                    _htmlService.ImagesStreamManager.SetConsumerStream(widgetID, viewID, stream, "png");
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine("ConvetToImage() raised exception : " + exp.Message);
            }
        }
        //this is for preview
        private void CreateAllObjs(IDocument document)  
        {
            Debug.WriteLine("----->Preview----->Enter Create VM AllObjs() ---");

            IPage CurrentPage = document.Pages.GetPage(_pageGID);
            if (CurrentPage == null)
            {
                return;
            }

            if (_currentItems != null)
            {
                _currentItems.Clear();
            }
            _currentItems = new List<WidgetPreViewModeBase>();
            List<IRegion> objs = GetAllObjects(CurrentPage);
            foreach (IRegion wdg in objs)
            {
                WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, true);
                if (preItem == null)
                {
                    continue;
                }
                _currentItems.Add(preItem);
            }
            Debug.WriteLine("----->Preview----->Exit Create VM AllObjs() ---");
        }        
        //this is for image convert
        private void CreateImageObjs(IDocument document)
        {
            Debug.WriteLine("--- Enter CreateImageObjs() ---");
            IPage CurrentPage = document.Pages.GetPage(_pageGID);
            if (CurrentPage == null)
            {
                CurrentPage = document.MasterPages.GetPage(_pageGID);
                if (CurrentPage == null)
                    return;
            }

            //initialize  the style property with base-adaptive
            Guid BaseViewID = document.AdaptiveViewSet.Base.Guid;
            IPageView pageView = CurrentPage.PageViews.GetPageView(BaseViewID);       
            
            //Load
            if (_currentItems != null)
            {
                _currentItems.Clear();
            }
            _currentItems = new List<WidgetPreViewModeBase>();

            if (document.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType == ExportType.ImageFile)
            {
                List<IRegion> objs = GetAllObjects(CurrentPage);
                foreach (IRegion wdg in objs)
                {
                    if (wdg is IShape || wdg is ILine)
                    {
                        WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, true);
                        if (preItem == null)
                        {
                            continue;
                        }
                        preItem.ChangeCurrentPageView(pageView);
                        preItem.IsHiddenInvalid = true;
                        _currentItems.Add(preItem);
                    }
                }
            }
            else
            {

                List<IRegion> objs = GetAllObjects(CurrentPage);
                foreach (IRegion wdg in objs)
                {
                    if (wdg is ILine)
                    {
                        WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, true);
                        if (preItem == null)
                        {
                            continue;
                        }
                        preItem.ChangeCurrentPageView(pageView);
                        preItem.IsHiddenInvalid = true;
                        _currentItems.Add(preItem);
                    }
                }
            }

            
            Debug.WriteLine("--- Exit CreateImageObjs() ---");
        }
        private void CreateInnerImageObjs(IPage ChildPage, IDocument document)
        {
            if (ChildPage == null)
                return;

            //initialize  the style property with base-adaptive
            Guid BaseViewID = document.AdaptiveViewSet.Base.Guid;
            IPageView pageView = ChildPage.PageViews.GetPageView(BaseViewID);     

            if (_currentItems != null)
            {
                _currentItems.Clear();
            }
            else
            {
                _currentItems = new List<WidgetPreViewModeBase>();
            }

            if (document.GeneratorConfigurationSet.DefaultHtmlConfiguration.ExportType == ExportType.ImageFile)
            {
                List<IRegion> objs = GetAllObjects(ChildPage);
                foreach (IRegion wdg in objs)
                {
                    if ( wdg is IShape || wdg is ILine)
                    {
                        WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, true);
                        if (preItem == null)
                        {
                            continue;
                        }

                        preItem.ChangeCurrentPageView(pageView);
                        preItem.IsHiddenInvalid = true;
                        _currentItems.Add(preItem);
                    }
                }
            }
            else
            {
                List<IRegion> objs = GetAllObjects(ChildPage);
                foreach (IRegion wdg in objs)
                {
                    if (wdg is ILine)
                    {
                        WidgetPreViewModeBase preItem = ReadOnlyWidgetFactory.CreateWidget(wdg, true);
                        if (preItem == null)
                        {
                            continue;
                        }

                        preItem.ChangeCurrentPageView(pageView);
                        preItem.IsHiddenInvalid = true;
                        _currentItems.Add(preItem);
                    }
                }
            }
            Debug.WriteLine("--- Exit CreateImageObjs() ---");
        }        
        private bool GenerateHtml()
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc == null || doc.Document == null)
            {
                return false;
            }
                        
            try
            {
                _htmlService.Render(doc.Document, _outputFolder);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        
        #region Help Function
        private void GetAllChildrenPage(List<IPage> Chidlren, IPage ParentPage)
        {
            List<IRegion> objs = GetAllObjects(ParentPage);            
            foreach (IRegion item in objs)
            {
                if(item is IDynamicPanel)
                {
                    foreach (IPanelStatePage statpage in (item as IDynamicPanel).PanelStatePages)
                    {
                        Chidlren.Add(statpage);
                        GetAllChildrenPage(Chidlren,statpage);
                    }
                            
                }
                else if(item is IHamburgerMenu)
                {
                    IPage statpage = (item as IHamburgerMenu).MenuPage;
                    Chidlren.Add(statpage);
                    GetAllChildrenPage(Chidlren, statpage);
                }
                else if(item is IToast)
                {
                    IPage statpage = (item as IToast).ToastPage;
                    Chidlren.Add(statpage);
                    GetAllChildrenPage(Chidlren, statpage);
                }  
                //else if(item is IMaster)
                //{
                //    IPage statpage = (item as IMaster).MasterPage;
                //    Chidlren.Add(statpage);
                //    GetAllChildrenPage(Chidlren, statpage);
                //}
                continue;
            }

        }
        private bool IsPageOpen()
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            IPage CurrentPage = doc.Document.Pages.GetPage(_pageGID);
            if (CurrentPage == null)
            {
                return false;
            }
            return CurrentPage.IsOpened;
        }
        private List<IRegion> GetAllObjects(IPage page)
        {
            List<IRegion> all = new List<IRegion>();
            foreach (IRegion wdg in page.Widgets)
            {
                all.Add(wdg);
            }
            foreach (IRegion master in page.Masters)
            {
                all.Add(master);
            }
            return all;
        }
        #endregion Private Function


        #region Binding Property
        private Stream _imageStream = null;
        private Visibility _nailIconShow = Visibility.Collapsed;
        public ObservableCollection<WidgetPreViewModeBase> items;
        public ObservableCollection<WidgetPreViewModeBase> Items
        {
            get
            {
                if (items == null)
                {
                    items = new ObservableCollection<WidgetPreViewModeBase>();
                }//if    
                return items;
            }
        }
        public ImageSource NailImgSource
        {
            get
            {
                try
                {
                    if (_imageStream == null)
                    {
                        return null;
                    }

                    //Create the Nail image if size is too large
                    using (System.Drawing.Image drawingImage = System.Drawing.Image.FromStream(_imageStream))
                    {
                        if (drawingImage.Width > 99 || drawingImage.Height > 99)
                        {
                            using (System.Drawing.Image thumbImage =
                                drawingImage.GetThumbnailImage((int)(drawingImage.Width * 1.2), (int)(drawingImage.Height * 1.2), () => { return true; }, IntPtr.Zero))
                            {

                                MemoryStream ms = new MemoryStream();
                                thumbImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                //_imageStream = ms;
                            }
                        }
                    }


                    //new image loading solution for improved memory release
                    _imageStream.Seek(0, SeekOrigin.Begin);
                    ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                    return imageSourceConverter.ConvertFrom(_imageStream) as BitmapFrame;
                }
                catch (System.Exception ex)
                {
                    return null;
                }
            }
            set
            {
                FirePropertyChanged("NailImgSource");
            }
        }
        public Visibility NailIconShow
        {
            get { return _nailIconShow; }
            set
            {
                if (_nailIconShow != value)
                {
                    _nailIconShow = value;
                    if(value==Visibility.Visible)
                    {
                        NailIconShowRevert = Visibility.Collapsed;
                    }
                    else
                    {
                        NailIconShowRevert = Visibility.Visible;                    
                    }
                    FirePropertyChanged("NailIconShow");
                    FirePropertyChanged("NailIconShowRevert");
                }
            }
        }
        public Visibility NailIconShowRevert
        { get; set; }
        #endregion

        #region Public Property for Preview
        public Canvas PreCanvas
        {
            get
            {
                PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
                return pre.PreviviewCanvas;
            }
            set
            {
                PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
                pre.PreviviewCanvas = value;
            }
        }
        Visual _preBorder;
        public Visual PreBorder
        {
            get
            {
                PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
                return pre.PreviviewBox;
            }
            set
            {
                PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
                pre.PreviviewBox = value;
                //_preBorder = value;
            }
        }
        #endregion
        
        #region Message Handler
        public void PagePreviewEventHandler(Guid pageGID)
        {
            if (_htmlService.IsHtmlGenerating == true)
            {
                Debug.WriteLine("----->Preview----->Invalid,So Exit:" + _currentPageGID.ToString());
                return;
            }

            lock (this)
            {
                if (pageGID == Guid.Empty)
                {
                    //if (_pageGID == Guid.Empty)
                    //{
                    //    Items.Clear();
                    //    GC.Collect();
                    //    GC.WaitForPendingFinalizers();
                    //    GC.Collect();

                    //}
                    _currentPageGID = Guid.Empty;
                    _waitingPageGID = Guid.Empty;
                }
                else
                {
                    if (_currentPageGID == Guid.Empty)
                    {
                        _currentPageGID = pageGID;
                        _waitingPageGID = Guid.Empty;
                        Debug.WriteLine("----->Preview----->Set Page 1st:" + _currentPageGID.ToString());
                    }
                    else
                    {
                        _waitingPageGID = pageGID;
                        Debug.WriteLine("----->Preview----->Set Page 2nd:" + _waitingPageGID.ToString());
                    }
                }
                
            }
            if (_waitingPageGID != Guid.Empty || _currentPageGID == Guid.Empty || _pageGID != Guid.Empty)
            {
                return;
            }
            Debug.WriteLine("----->Preview----->Start Page:" + _currentPageGID.ToString());
            UpdatePreviewPage(_currentPageGID);

            
        }
        public async void GenerateHTMLEventHandler(object data)
        {
            PreviewParameter para = data as PreviewParameter;
            string filePath = para.SavePath;
            // Make textbox update source data.
            ISelectionService _selectionService = ServiceLocator.Current.GetInstance<SelectionServiceProvider>();
            IPagePropertyData page = _selectionService.GetCurrentPage();

            if (para.IsPreviewCurrentPage==true && page == null)
            {
                return;
            }

            if (page != null && page.EditorCanvas != null)
            {
                page.EditorCanvas.Focus();
            }

            if ((_currentPageGID != Guid.Empty))
            {
                return;
            }

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if(string.IsNullOrEmpty(filePath))
            {
                return;
            }
            if (doc == null || doc.Document == null)
            {
                return;
            }

            //Get Image save path;
            //doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.OutputFolder = @"D:\pic";
            _outputFolder = filePath;
            string imgPath = filePath+@"\images\";
            try
            {
                if (Directory.Exists(imgPath) == false)
                {
                    Directory.CreateDirectory(imgPath);
                }
                _htmlService.ImagesStreamManager.WorkingDirectory = imgPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(GlobalData.FindResource("Error_Generate_Html") + ex.Message);
                return;
            }

            if (para.IsPreviewCurrentPage)
            {
                Guid currentPageGuid = page.PageGID;
                if (page.ActivePage is IEmbeddedPage)
                {
                    IEmbeddedPage embeddedPage = page.ActivePage as IEmbeddedPage;
                    currentPageGuid = embeddedPage.ParentWidget.ParentPage.Guid;
                }

                doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.CurrentPage = currentPageGuid;
            }
            else
            {
                doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.CurrentPage = Guid.Empty;
            }

        
            Debug.WriteLine("----->HtmlGen----->Start All Convert");    
            //Async operation to Generate Html and open it
            await AsyncGenerateHtml(para.IsBrowerOpen);

            // Reset image working directory.
            _htmlService.ImagesStreamManager.WorkingDirectory = string.Empty;
        }

        public async void GenerateMD5HTMLEventHandler(object data)
        {
            DiffGeneratorParameter para = data as DiffGeneratorParameter;
            string filePath = para.SavePath;
            if(string.IsNullOrEmpty(filePath)||para.Docs==null|| para.Docs.Count<1)
            {
                return;
            }

            //Get Image save path;
            //doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.OutputFolder = @"D:\pic";
            _outputFolder = filePath+ @"\data";
            string imgPath = filePath+ @"\images";
            try
            {
                if (Directory.Exists(imgPath) == false)
                {
                    Directory.CreateDirectory(imgPath);
                }
                if (Directory.Exists(_outputFolder) == false)
                {
                    Directory.CreateDirectory(_outputFolder);
                }

                _htmlService.ImagesStreamManager.WorkingDirectory = imgPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(GlobalData.FindResource("Error_Generate_Html") + ex.Message);
                return;
            }

            await AsyncGenerateMD5Html(para);

            // Reset image working directory.
            _htmlService.ImagesStreamManager.WorkingDirectory = string.Empty;
        }
        #endregion

        #region MD5 HTML Generator
        private async Task AsyncGenerateMD5Html(DiffGeneratorParameter para)
        {

            //Application.Current.MainWindow.IsEnabled = false;
            _busyIndicator.Progress = 3;
            _busyIndicator.CanStop = true;
            _busyIndicator.IsShow = true;

            //load page data and create all images,ignore it now
            //foreach (IDocumentService doc in para.Docs)
            //{
            //    await AsyncConvertAllPages(doc.Document);
            //}

            //generate the html files.  
            //if (_busyIndicator.IsContinue == false)
            //{
            //    _busyIndicator.IsShow = false;
            //    IsHtmlGenerating = false;
            //    return;
            //}
            _busyIndicator.Progress = 30;
            //_busyIndicator.Content=@"Generate the HTML Page......";


            //Create All Page Htmls with MD5 hash
            int i = 0;
            foreach(IDocumentService doc in para.Docs)
            {
                await AsyncConvertAllPages(doc.Document);

                string szLocation=_outputFolder+ @"\"+i++;
                bool bIsSuccessful = await Task.Factory.StartNew<bool>(() => GenerateMD5Html(doc, szLocation));
                if (bIsSuccessful == false)
                {
                    _busyIndicator.IsShow = false;
                    _htmlService.IsHtmlGenerating = false;
                    MessageBox.Show(GlobalData.FindResource("Error_Generate_Html_Access"));
                    return;
                }
                _busyIndicator.Progress = _busyIndicator.Progress+15;

            }

            //Create Differ project Json Date
            bool bSuccessful = await Task.Factory.StartNew<bool>(() => GenerateMD5DifferInfo(para.Docs, _outputFolder));
            if (bSuccessful == false)
            {
                _busyIndicator.IsShow = false;
                _htmlService.IsHtmlGenerating = false;
                MessageBox.Show(GlobalData.FindResource("Error_Generate_Html_Access"));
                return;
            }

            //Browser to open page
            if (_busyIndicator.IsContinue == false)
            {
                _busyIndicator.IsShow = false;
                _htmlService.IsHtmlGenerating = false;
                return;
            }
            _busyIndicator.Progress = 98;
            _busyIndicator.Content=@"HTML Page is ready now, Open the Browser..";
          
            //open the web browser
            IWebServer httpServer = ServiceLocator.Current.GetInstance<IWebServer>();
            if (httpServer != null)
            {
                try
                {
                    Process.Start("explorer.exe", httpServer.GetWebUrl());
                }
                catch
                {
                    NLogger.Error("Preview:Open Browser failed!");
                }                    
            }
            else
            {
                NLogger.Error("httpServer is null when open browner!");
            }


            //end progress show
            _busyIndicator.Progress = 100;
            _busyIndicator.IsShow = false;
            _htmlService.IsHtmlGenerating = false;
        }
        private bool GenerateMD5Html(IDocumentService doc, string szLocation)
        {
            if (doc == null || doc.Document == null)
            {
                return false;
            }

            try
            {
                _htmlService.Render(doc.Document, szLocation, true);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool GenerateMD5DifferInfo(List<IDocumentService> Docs, string szLocation)
        {
            if (Docs == null || Docs.Count<2)
            {
                return false;
            }

            try
            {
                List<IDocument> all = new List<IDocument>();
                foreach(IDocumentService it in Docs)
                {
                    if(it.Document!=null)
                    {
                        all.Add(it.Document);
                    }
                }
                _htmlService.RenderDifferInfo(all, szLocation);
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Private Async Function
        private async Task AsyncGenerateHtml(bool IsOpenBrower)
        {
            if (_htmlService.IsHtmlGenerating == false)
            {
                _htmlService.IsHtmlGenerating = true;
            }
            else
            {
                return;
            }

            //Application.Current.MainWindow.IsEnabled = false;
            _busyIndicator.Progress = 3;
            _busyIndicator.CanStop = true;
            _busyIndicator.IsShow = true;

            //load page data and create all images
            await AsyncConvertAllPages();
            Debug.WriteLine("----->HtmlGen----->End All Convert");

            //generate the html files.  
            if (_busyIndicator.IsContinue == false)
            {
                _busyIndicator.IsShow = false;
                _htmlService.IsHtmlGenerating = false;
                return;
            }
            _busyIndicator.Progress = 90;
            _busyIndicator.Content=@"Generate the HTML Page......";
            bool bIsSuccessful = await Task.Factory.StartNew<bool>(GenerateHtml);
            if (bIsSuccessful==false)
            {
                _busyIndicator.IsShow = false;
                _htmlService.IsHtmlGenerating = false;
                MessageBox.Show(GlobalData.FindResource("Error_Generate_Html_Access"));
                return;

            }

            //Browser to open page
            if (_busyIndicator.IsContinue == false)
            {
                _busyIndicator.IsShow = false;
                _htmlService.IsHtmlGenerating = false;
                return;
            }
            _busyIndicator.Progress = 98;
            _busyIndicator.Content=@"HTML Page is ready now, Open the Browser..";
          
            if (IsOpenBrower)
            {
                IWebServer httpServer = ServiceLocator.Current.GetInstance<IWebServer>();
                if (httpServer != null)
                {
                    try
                    {
                        Process.Start("explorer.exe", httpServer.GetWebUrl());
                    }
                    catch
                    {
                        NLogger.Error("Preview:Open Browser failed!");
                    }
                    
                }
                else
                {
                    NLogger.Error("httpServer is null when open browner!");
                }
            }

            _busyIndicator.Progress = 100;
            _busyIndicator.IsShow = false;
            _htmlService.IsHtmlGenerating = false;
        }
        private async Task AsyncConvertAllPages(IDocument document=null)
        {
            if(document==null)
            {
                #region Normal Page
                IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
                if (doc == null || doc.Document == null || doc.Document.Pages.Count <= 0)
                {
                    return;
                }

                int nDiv = 90 / doc.Document.Pages.Count;
                int nProgress = nDiv;
                Debug.WriteLine("----->HtmlGen----->Enter Async Convert All Pages()");


                //IPage CurrentPage = doc.Document.Pages.GetPage(_pageGID);
                Guid SelPageGID = doc.Document.GeneratorConfigurationSet.DefaultHtmlConfiguration.CurrentPage;
                if (SelPageGID == Guid.Empty)
                {
                    List<IPage> TopPages = new List<IPage>();
                    TopPages.AddRange(doc.Document.Pages);
                    TopPages.AddRange(doc.Document.MasterPages);
                    foreach (IPage CurrentPage in TopPages)
                    {
                        if (_busyIndicator.IsContinue == false)
                            break;
                        _busyIndicator.Progress = nProgress;
                        _busyIndicator.Content = @"Convert the Image from Page:" + "\n" + CurrentPage.Name;
                        nProgress += nDiv;

                        bool isClosedPage = false;
                        if (!CurrentPage.IsOpened)
                        {
                            isClosedPage = true;
                            CurrentPage.Open();
                        }

                        //Convert Current Page Self
                        if (CurrentPage is IMasterPage)
                        {
                            await AsyncConvertNormalPage(CurrentPage.Guid, doc.Document, true);

                            List<IPage> Chidlren = new List<IPage>();
                            GetAllChildrenPage(Chidlren, CurrentPage);
                            foreach (IPage ChildPage in Chidlren)
                            {
                                await AsyncConvertChildlPage(ChildPage, doc.Document);
                            }
                        }
                        else
                        {
                            await AsyncConvertNormalPage(CurrentPage.Guid, doc.Document, false);

                            //Convert All Children Pages(Dynamic/Hamburg)
                            List<IPage> Chidlren = new List<IPage>();
                            GetAllChildrenPage(Chidlren, CurrentPage);
                            foreach (IPage ChildPage in Chidlren)
                            {
                                await AsyncConvertChildlPage(ChildPage, doc.Document);
                            }
                        }

                        if (isClosedPage)
                        {
                            CurrentPage.Close();
                        }
                    }
                }
                else
                {
                    IPage CurrentPage = doc.Document.Pages.GetPage(SelPageGID);
                    if (CurrentPage == null)
                    {
                        return;
                    }

                    bool isClosedPage = false;
                    if (!CurrentPage.IsOpened)
                    {
                        isClosedPage = true;
                        CurrentPage.Open();
                    }

                    _busyIndicator.Progress = 90;
                    _busyIndicator.Content = @"Convert the Image from Page:" + "\n" + CurrentPage.Name;

                    //Convert Current Page Self
                    await AsyncConvertNormalPage(CurrentPage.Guid, doc.Document,false);

                    //Convert All Children Pages(Dynamic/Hamburg)
                    List<IPage> Chidlren = new List<IPage>();
                    GetAllChildrenPage(Chidlren, CurrentPage);
                    foreach (IPage ChildPage in Chidlren)
                    {
                        await AsyncConvertChildlPage(ChildPage,doc.Document);
                    }

                    if (isClosedPage)
                    {
                        CurrentPage.Close();
                    }
                }

                Debug.WriteLine("----->HtmlGen----->Exit Async Convert All Pages()");
                #endregion
            }
            else
            {
                #region MD5 Page
                if(document.IsOpened==false)
                {
                    return;
                }

                List<IPage> TopPages = new List<IPage>();
                TopPages.AddRange(document.Pages);
                TopPages.AddRange(document.MasterPages);
                foreach (IPage CurrentPage in TopPages)
                {
                    if (_busyIndicator.IsContinue == false)
                        break;
                    CurrentPage.Open();


                    //Convert Current Page Self
                    if (CurrentPage is IMasterPage)
                    {
                        await AsyncConvertNormalPage(CurrentPage.Guid, document, true);

                        List<IPage> Chidlren = new List<IPage>();
                        GetAllChildrenPage(Chidlren, CurrentPage);
                        foreach (IPage ChildPage in Chidlren)
                        {
                            await AsyncConvertChildlPage(ChildPage, document);
                        }
                    }
                    else
                    {
                        await AsyncConvertNormalPage(CurrentPage.Guid, document,false);

                        //Convert All Children Pages(Dynamic/Hamburg)
                        List<IPage> Chidlren = new List<IPage>();
                        GetAllChildrenPage(Chidlren, CurrentPage);
                        foreach (IPage ChildPage in Chidlren)
                        {
                            await AsyncConvertChildlPage(ChildPage, document);
                        }
                    }
                }
                #endregion

            }
            
        }
        private async Task AsyncConvertNormalPage(Guid pGID,IDocument document,bool isMasterPage=false)
        {
            Debug.WriteLine("----->HtmlGen----->Start Loading Normal Page:" + pGID);
            await UpdateHtmlPreviewPage(pGID, document); //load widgets            
            
            IPage CurrentPage = null;           
            if(isMasterPage==false)
            {
                CurrentPage = document.Pages.GetPage(pGID);
            }
            else
            {
                CurrentPage = document.MasterPages.GetPage(pGID);
            }
            if(CurrentPage==null)
            {
                return;
            }
            
            Guid BaseViewID = document.AdaptiveViewSet.Base.Guid;
            foreach (IPageView pageView in CurrentPage.PageViews)
            {
                Guid CurViewID = pageView.Guid;
                if (CurViewID == BaseViewID)
                {
                    CurViewID = Guid.Empty;
                }
                else
                {
                    foreach (var wdg in items)
                    {
                        wdg.ChangeCurrentPageView(pageView);
                    }
                }

                await Task.Factory.StartNew(ConvertPage); //update layout
                await Task.Factory.StartNew(() => CreateImg(CurViewID)); //create image
            }
            Debug.WriteLine("----->HtmlGen----->Exit Loading Normal Page:" + pGID);
        }
        private async Task AsyncConvertChildlPage(IPage ChildPage,IDocument document)
        {
            bool isClosedPage = false;
            if (!ChildPage.IsOpened)
            {
                isClosedPage = true;
                ChildPage.Open();
            }

            Debug.WriteLine("----->HtmlGen----->Start Loading Child Page");
            await UpdateChildPage(ChildPage); //load widgets
            await Task.Factory.StartNew(ConvertPage); //update layout

            Guid BaseViewID = document.AdaptiveViewSet.Base.Guid;
            foreach (IPageView pageView in ChildPage.PageViews)
            {
                Guid CurViewID = pageView.Guid;
                if (CurViewID == BaseViewID)
                {
                    CurViewID = Guid.Empty;
                }
                else
                {
                    foreach (var wdg in items)
                    {
                        wdg.ChangeCurrentPageView(pageView);
                    }
                }
                await Task.Factory.StartNew(ConvertPage); //update layout
                await Task.Factory.StartNew(() => CreateImg(CurViewID)); 
            }

            if (isClosedPage)
            {
                ChildPage.Close();
            }
            Debug.WriteLine("----->HtmlGen----->Exit Loading Child Page");
        }
        //Dispatcher operation to update canvas' layout for create images.
        private void ConvertPage()
        {           
            PreCanvas.Dispatcher.Invoke(DispatcherPriority.Background,(Action)(() =>
                {
                    Debug.WriteLine("--- PreCanvas.UpdateLayout(); ---");
                    PreCanvas.UpdateLayout();
                }));
        }
        //Dispatcher operation to create image files according to current all widgets.
        private void CreateImg(Guid viewID)
        {
            Debug.WriteLine("----->HtmlGen----->Enter Create Current View Images()");

            int nNumber=0;
            PreCanvas.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() =>
            {
                nNumber= PreCanvas.Children.Count;
                if(nNumber == 0)
                {
                    return;
                }

                IHtmlServiceProvider htmlService = ServiceLocator.Current.GetInstance<IHtmlServiceProvider>();

                for (int i = 0; i < nNumber; i++)
                {  
                    ContentPresenter it = PreCanvas.Children[i] as ContentPresenter;
                    if (it == null || it.Visibility != Visibility.Visible)
                    {
                        continue;
                    }

                    try
                    {
                        Border br = VisualTreeHelper.GetChild(it, 0) as Border;
                        //item.Measure(new Size(100, 100));
                        //item.Arrange(new Rect(new Size(100, 100)));
                        Guid widgetGuid = Guid.Empty;
                        if (!Guid.TryParse(br.Tag as String, out widgetGuid))
                        {
                            return;
                        }

                        //Start to create image file
                        ConvetToImage(widgetGuid, viewID, br.Child, Convert.ToInt32(br.ActualWidth), Convert.ToInt32(br.ActualHeight));
                        Debug.WriteLine("----->HtmlGen----->Create the No:"+i+" Img file In All:"+ nNumber);
                    }
                    catch
                    {
                        NLogger.Error("Create Image File failed! Index:" + i + " Img file In All:" + nNumber);
                        continue;
                    }
                        
                }

            }));
            Debug.WriteLine("----->HtmlGen----->Exit Create Current View Images()");
        }
        #endregion Private Async Function

        #region  Preview
        private async void UpdatePreviewPage(Guid pageGID)
        {

            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc == null || doc.Document == null)
            {
                return;
            }

            //Create and show all widgets by async-operation to avoid the UI block
            _pageGID = pageGID;
            while (_currentPageGID != Guid.Empty)
            {
                Items.Clear();
                if (IsPageOpen()==true)
                {
                    //_imageStream = null;
                    //NailImgSource = null;

                    NailIconShow = Visibility.Collapsed;
                    //PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
                    //pre.PreviviewBox = pre.PreviviewCanvas;
                    try
                    {
                        await AsyncLoadWidgets(_currentPageGID);
                    }
                    catch
                    {
                        _currentPageGID = Guid.Empty;
                        _waitingPageGID = Guid.Empty;
                        Debug.WriteLine("----->Preview----->Exception and Exit:" + _currentPageGID.ToString());
                        return;                
                    }
                    
                }
                else 
                {
                    NailIconShow = Visibility.Visible;
                    //PreviewModel pre = ServiceLocator.Current.GetInstance<PreviewModel>();
                    //pre.PreviviewBox = _preBorder;
                    await AsyncLoadNailIcon(_currentPageGID);                
                }               

            }

            //if(_currentPageGID == Guid.Empty && _waitingPageGID == Guid.Empty)
            //{
            //    Items.Clear();
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //    GC.Collect();

            //}
            _pageGID = Guid.Empty;
            Debug.WriteLine("----->Preview----->End Page:" + _currentPageGID.ToString());
        }
        private async Task AsyncLoadWidgets(Guid pGID)
        {     
            //Async operation to create all widget VM object for efficiency
            _pageGID = pGID;
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            await Task.Factory.StartNew(()=>CreateAllObjs(doc.Document));

            //initialize  the style property with base-adaptive
            IPage CurrentPage = doc.Document.Pages.GetPage(pGID);
            if(CurrentPage==null)
            {
                return;
            }
            Guid BaseViewID = doc.Document.AdaptiveViewSet.Base.Guid;
            IPageView pageView = CurrentPage.PageViews.GetPageView(BaseViewID);     

            //Async operation to load single widget to canvas and update layout
            foreach (WidgetPreViewModeBase wdg in _currentItems)
            {
                if (_waitingPageGID != Guid.Empty||_currentPageGID==Guid.Empty|| _htmlService.IsHtmlGenerating == true)
                {                    
                    break;
                }
                wdg.ChangeCurrentPageView(pageView);

                //Sync loading
                Debug.WriteLine("----->Preview----->Add Widget 2 Canvas:"+wdg.WidgetID);
                items.Add(wdg);

                //Async loading        
                //PreCanvas.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                //{
                //    Debug.WriteLine("----->Preview----->Add Widget 2 Canvas:" + wdg.WidgetID);
                //    items.Add(wdg);
                //}));
            }

            lock (this)
            {
                if (_waitingPageGID != Guid.Empty)
                {

                    _currentPageGID = _waitingPageGID;
                    _waitingPageGID = Guid.Empty;
                    Debug.WriteLine("----->Preview----->Second Page:" + _currentPageGID.ToString());
                }
                else
                {
                    _currentPageGID = Guid.Empty;
                    _waitingPageGID = Guid.Empty;
                }                
            }
            Debug.WriteLine("----->Preview----->AsyncLoadWidgets is Done");
        }
        private async Task AsyncLoadNailIcon(Guid pGID)
        {     
            //Async operation to create all widget VM object for efficiency
            _pageGID = pGID;
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            IPage CurrentPage = doc.Document.Pages.GetPage(pGID);
            if(CurrentPage==null)
            {
                return;
            }

            //Async operation to load thumbnail
             await Task.Factory.StartNew(() => 
            { 
                _imageStream = CurrentPage.Thumbnail;
                FirePropertyChanged("NailImgSource");
            });   
            
            lock (this)
            {
                if (_waitingPageGID != Guid.Empty)
                {

                    _currentPageGID = _waitingPageGID;
                    _waitingPageGID = Guid.Empty;
                    Debug.WriteLine("Second Page:" + _currentPageGID.ToString());
                }
                else
                {
                    _currentPageGID = Guid.Empty;
                    _waitingPageGID = Guid.Empty;
                }                
            }

        }
        #endregion

        #region  Html  
        //Load child page's widgets
        private async Task UpdateChildPage(IPage ChildPage) 
        {
            //Initialize the data
            _pageGID = ChildPage.Guid;
            //Create and show all widgets by async-operation to avoid the UI block
            Items.Clear();

            await AsyncLoadWidgets(ChildPage);
            _pageGID = Guid.Empty;
        }

        private async Task AsyncLoadWidgets(IPage ChildPage)
        {
            Debug.WriteLine("----->HtmlGen----->Start Async Loading All Images()");
            //Async operation to create all widget VM object for efficiency
            await Task.Factory.StartNew(() => CreateInnerImageObjs(ChildPage, ChildPage.ParentDocument));
            Debug.WriteLine("----->HtmlGen----->Create All Image VM Objects");

            try
            {
                int kk = 0;
                //Async operation to load single widget to canvas and update layout
                foreach (WidgetPreViewModeBase wdg in _currentItems)
                {
                    kk++;
                    PreCanvas.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                    {
                        items.Add(wdg);
                        Debug.WriteLine("----->HtmlGen----->Add Image to Canvas, No:" + kk);
                    }));
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Debug.WriteLine("----->HtmlGen-----> Exit Async Loading All Images()");
        }
        
        //Load Normal page's widgets
        private async Task UpdateHtmlPreviewPage(Guid pageGID, IDocument document)    //////////////////////////////////////////3.1
        {
             if (document == null)
            {
                _pageGID = Guid.Empty;
                return;
            }

            //Initialize the data
            _pageGID = pageGID;

            //Create and show all widgets by async-operation to avoid the UI block
            Items.Clear();
            await AsyncLoadWidgets(document);
            _pageGID = Guid.Empty;
        }
        private async Task AsyncLoadWidgets(IDocument document)                  //////////////////////////////////////////3.1.1
        {

            Debug.WriteLine("----->HtmlGen----->Start Async Loading All Images()");
            //Async operation to create all widget VM object for efficiency
            await Task.Factory.StartNew(()=>CreateImageObjs(document));
            Debug.WriteLine("----->HtmlGen----->Create All Image VM Objects");

            try
            {
                int kk=0;
                //Async operation to load single widget to canvas and update layout
                foreach (WidgetPreViewModeBase wdg in _currentItems)
                {
                    kk++;
                    PreCanvas.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() =>
                    {
                        items.Add(wdg);
                        Debug.WriteLine("----->HtmlGen----->Add Image to Canvas, No:" + kk);
                    }));
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("----->HtmlGen----->Create Canvas Image Failded:"+ex.Message);
            }

            Debug.WriteLine("----->HtmlGen-----> Exit Async Loading All Images()");
        }
        #endregion
    }
}
