using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.InfoStructure;
using Naver.Compass.Service.Document;
using System.IO;
using Naver.Compass.Common.Helper;
using System.Windows;
using Naver.Compass.WidgetLibrary;

namespace Naver.Compass.Module
{
    public class PageEditorModel
    {
        #region Normal Page Initialize
        public PageEditorModel(Guid pageGID)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document = doc.Document;
            if (_document != null)
            {
                _page = _document.Pages.GetPage(pageGID);
                if (_page == null)
                {
                    _page = _document.MasterPages.GetPage(pageGID);
                }
            }
            else
            {
                _page = null;
            }


        }
        #endregion

        #region Frame Widget's child Page Initialize
        public PageEditorModel(IPage WidgetChildPage)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            _document = doc.Document;
            if (_document != null)
            {
                _page = WidgetChildPage;
            }
        }
        #endregion

        #region Private member
        private IPage _page;
        private IPageView _activeView;
        private IDocument _document = null;              
        #endregion private member

        #region Public member and proerty
        public Stream PreviewThumbnailImage
        {
            get
            {
                if (_page != null)
                    return _page.Thumbnail;
                return null;
            }  

            set
            {
                if (_page == null)
                    return;

                if (_document.IsDirty == true)
                {
                    _page.Thumbnail = value;
                }

                else if (_page.Thumbnail == null)
                {
                    _page.Thumbnail = value;
                    _document.IsDirty = true;
                }
            }
        }
        public bool IsDirty
        {
            get
            {
                return _document.IsDirty;
                //return _page.Isdirty;
            }
            set
            {
                if (_document.IsDirty != value)
                {
                    _document.IsDirty = value;
                }
                //_page.Isdirty = value;
            }
        }
        public List<IRegion> Widgets
        {
            get
            {
                List<IRegion> AllObjs = new List<IRegion>();               
                if (_page != null)
                {
                    AllObjs.AddRange(_page.Widgets);
                    AllObjs.AddRange(_page.Masters);
                    return AllObjs;
                }
                return null;
            }  
        }
        public IGroups Groups
        {
            get
            {
                if (_page != null)
                    return _page.Groups;
                return null;
            }  
        }
        public IPageViews PageViews
        {
            get
            {
                if (_page == null)
                    return null;
                return _page.PageViews;
            }
        }
        public void SetActivePageView(Guid gid)
        {
            _activeView = _page.PageViews.GetPageView(gid);
        }
        public IPageView ActivePageView
        {
            get {return _activeView;}
        }
        public bool IsUseThumbnailAsIcon
        {
            get
            {
                ICustomObjectPage LibPage = _page as ICustomObjectPage;
                if(LibPage==null)
                {
                    return false;
                }
                else
                {
                    return LibPage.UseThumbnailAsIcon;
                }
            }
            set
            {

                ICustomObjectPage LibPage = _page as ICustomObjectPage;
                if (LibPage != null)
                {
                    LibPage.UseThumbnailAsIcon=value;
                }
            }

        }

        public double EditorScale
        {
            get
            {
                return (_page == null) ? 1 : Convert.ToDouble(_page.Zoom) / 100;
            }
            set
            {
                if (_page != null)
                {
                    _page.Zoom = Convert.ToInt16(Math.Floor(value * 100));                    
                    IsDirty = true;
                }
            }
        }
        #endregion 

        #region Public Functions
        #region Open/Close Page
        public void OpenPage()
        {
            if (_page != null)
            {
                _page.Open();
                // Refetch active view
                if (_activeView != null)
                {
                    _activeView = _page.PageViews.GetPageView(_activeView.Guid);
                }
                else
                {
                    Guid ViewId = _document.AdaptiveViewSet.Base.Guid;
                    _activeView = _page.PageViews.GetPageView(ViewId);
                }
            }
        }
        public void ClosePage()
        {
            if (_page != null)
            {
                _page.Close();
            }
        }
        public bool IsPageOpen()
        {
            if (_page != null)
            {
                return _page.IsOpened;
            }
            return false;
        }
        /// <summary>
        /// set custom widget's thumbnail
        /// </summary>
        /// <param name="stream"></param>
        public void SetCustomPageIcon(Stream stream)
        {
            if (_page is ICustomObjectPage)
            {
                (_page as ICustomObjectPage).Icon = stream;
            }
        }
        #endregion

        #region Copy/Paset/Cut
        public void AddClonedItem2Dom(IRegion cloneItem)
        {
            if (_activeView == null)
            {
                return;
            }
            IsDirty = true;
            if(cloneItem is IWidget)
            {
                _activeView.AddWidget(cloneItem as IWidget);
            }
            else if(cloneItem is IMaster)
            {
                _activeView.AddMaster(cloneItem as IMaster);
            }
        }
        public void AddClonedGroup2Dom(IGroup cloneItem)
        {
            if (_activeView == null)
            {
                return;
            }
            IsDirty = true;
            _activeView.AddGroup(cloneItem);
        }
        public IObjectContainer DeSerializeData2Dom(Stream SrcStram)
        {
            if (_activeView == null)
            {
                return null;
            }
            return _activeView.AddObjects(SrcStram);
        }

        public IObjectContainer BreakMaster2Dom(Guid masterGuid)
        {
            if (_activeView == null)
            {
                return null;
            }
            return _activeView.BreakMaster(masterGuid);
        }

        public IObjectContainer AddCustomObject(ICustomObject customObject, double x, double y)
        {
            if (_activeView == null)
            {
                return null;
            }
            return _activeView.AddCustomObject(customObject, x, y);
        }

        public IObjectContainer AddMasterPageObject(Guid masterPageID, double x, double y)
        {
            if (_activeView == null)
            {
                return null;
            }
            return _activeView.AddMasterPageObject(masterPageID, _activeView.Guid,x, y);       
        }  
        #endregion

        #region Widget Add/Delete
        public IWidget AddLineItem2Dom(Orientation lineType, double x, double y, int w, int h)
        {
            if (_activeView == null)
                return null;

            ILine widget = _activeView.CreateWidget(WidgetType.Line) as ILine;
            if (widget == null)
                return null;

            IsDirty = true;

            // Save the initial location and size in base view style, so we can place widget in the original 
            // laction with original size in base view if the widget is created in child view.
            widget.Orientation = lineType;
            widget.WidgetStyle.X = x;
            widget.WidgetStyle.Y = y;
            widget.WidgetStyle.Width = w;
            widget.WidgetStyle.Height = h;
            return widget;
        }
        public IWidget AddWidgetItem2Dom(WidgetType type, ShapeType flowType, double x, double y, int w, int h)
        {
            if (_activeView == null)
                return null;

            IWidget widget = _activeView.CreateWidget(type);
            if (widget == null)
                return null;            
            IsDirty = true;

            // Save the initial location and size in base view style, so we can place widget in the original 
            // laction with original size in base view if the widget is created in child view.

              if (widget.WidgetType == WidgetType.HamburgerMenu)
            {
                IHamburgerMenu hamburger = widget as IHamburgerMenu;
                IWidgetStyle buttonStyle = hamburger.MenuButton.WidgetStyle;

                buttonStyle.X = x;
                buttonStyle.Y = y;
                buttonStyle.Width = w;
                buttonStyle.Height = h;

                //Init menu-page size.
                hamburger.WidgetStyle.Width = 340;
                hamburger.WidgetStyle.Height = 480;
                double left = buttonStyle.X;
                double top = buttonStyle.Y - hamburger.WidgetStyle.Height - 20;
                hamburger.WidgetStyle.X = left >= 0 ? left : 0;
                hamburger.WidgetStyle.Y = top >= 0 ? top : 0;
                return hamburger;
            }
            else
            {
                widget.WidgetStyle.X = x;
                widget.WidgetStyle.Y = y;
                widget.WidgetStyle.Width = w;
                widget.WidgetStyle.Height = h;
            }

              if (widget.WidgetType == WidgetType.Toast)
              {
                  AddDefautForToast(widget as IToast);
              }

            //Initialize the Dom new widget
            IShape shape = widget as IShape;
            if(shape!=null)
            {
                shape.ShapeType = flowType;
                if (flowType == ShapeType.Paragraph)
                {
                    widget.SetRichText(@"label");
                }
            }
            return widget;
        }
       
        //public void RemoveWidgetFromDom(Guid wdgGID)
        public void RemoveWidgetFromDom(WidgetViewModBase wdg)
        {
            if (_activeView == null)
                return ;
            Guid wdgGID = wdg.widgetGID;            
            IsDirty = true;

            if (wdg is MasterWidgetViewModel)
            {
                _activeView.DeleteMaster(wdgGID);  
            }
            else
            {
                _activeView.DeleteWidget(wdgGID);      
            }       
        }
        public void PlaceWidget2View(IRegion target)
        {
            if(target==null)
            {
                return;
            }

            Guid wdgGID = target.Guid;
            if (_activeView != null)
            {
                IsDirty = true;
                if (target is IWidget)
                {
                    _activeView.PlaceWidget(wdgGID);
                }
                else if (target is IMaster)
                {
                    _activeView.PlaceMaster(wdgGID);
                }

            }
        }
        public void UnplaceWidgetFromView(IRegion target)
        {
            if (target == null)
            {
                return;
            }

            Guid wdgGID = target.Guid;
            if (_activeView != null)
            {
                IsDirty = true;
                if (target is IWidget)
                {
                    _activeView.UnplaceWidget(wdgGID);
                }
                else if(target is IMaster)
                {
                    _activeView.UnplaceMaster(wdgGID);
                }
            }
        }
        #endregion

        #region Master Operation
        public IMaster AddWidgetItem2Dom(Guid masterPageGid, double x, double y)
        {
            if (_activeView == null)
                return null;

            IMaster master = _activeView.CreateMaster(masterPageGid);
            if (master == null)
                return null;
            IsDirty = true;

            master.MasterStyle.X = x;
            master.MasterStyle.Y = y;

            return master;
        }
        public List<IRegion> QueryMaster(Guid masterPageGid)
        {
            return _page.Masters.Where<IMaster>(x => x.MasterPageGuid==masterPageGid).ToList<IRegion>();
        }
        #endregion

        #region Group Operation
        public IGroup CreateGroup(List<Guid> guidList)
        {
            if (_activeView == null)
                return null;
            return _activeView.CreateGroup(guidList);
        }
        public void DeleteGroup(Guid guid)
        {
            if (_activeView == null)
                return;
            IsDirty = true;
            _activeView.DeleteGroup(guid);
        }
        public void UnGroup(Guid guid)
        {
            if (_activeView == null)
                return ;
            IsDirty = true;
            _activeView.Ungroup(guid);
        }
        #endregion
        #endregion

        #region Private Functions
        /// <summary>
        /// Add defaut image and text for Toast.
        /// </summary>
        private void AddDefautForToast(IToast toast)
        {
            toast.ToastPage.Open();
            IPageView curPageView = toast.ToastPage.PageViews.GetPageView(_document.AdaptiveViewSet.Base.Guid);

            //add default image for toast
            IImage image1 = curPageView.CreateWidget(WidgetType.Image) as IImage;
            Stream _imageStream = Application.GetResourceStream(new Uri(@"pack://application:,,,/Naver.Compass.Module.DiagramEditor;component/Image/toast01.png", UriKind.RelativeOrAbsolute)).Stream;
            image1.ImageStream = _imageStream;
            image1.WidgetStyle.Width = 297;
            image1.WidgetStyle.Height = 87;
            image1.WidgetStyle.Z = 0;
            image1.Name = @"Pop_Panel";

            IImage image2 = curPageView.CreateWidget(WidgetType.Image) as IImage;
            Stream _imageStream2 = Application.GetResourceStream(new Uri(@"pack://application:,,,/Naver.Compass.Module.DiagramEditor;component/Image/toast02.png", UriKind.RelativeOrAbsolute)).Stream;
            image2.ImageStream = _imageStream2;
            image2.WidgetStyle.Y = 106;
            image2.WidgetStyle.Width = 297;
            image2.WidgetStyle.Height = 40;
            image2.WidgetStyle.Z = 1;
            image2.Name = @"IconBar";

            //add default text for toast
            IShape textWidget = curPageView.CreateWidget(WidgetType.Shape) as IShape;
            textWidget.ShapeType = ShapeType.Paragraph;
            textWidget.WidgetStyle.X = 20;
            textWidget.WidgetStyle.Y = 15;
            textWidget.WidgetStyle.Width = 258;
            textWidget.WidgetStyle.Height = 47;
            textWidget.WidgetStyle.FontSize = 14;
            textWidget.WidgetStyle.FontColor = new StyleColor(ColorFillType.Solid, System.Drawing.Color.FromArgb(0xc8, 0xcb, 0xce).ToArgb());
            textWidget.WidgetStyle.FontFamily = GlobalData.FindResource("Common_Font");
            textWidget.Text = GlobalData.FindResource("widgets_Toast_Default_Msg");
            textWidget.WidgetStyle.Z = 2;
            textWidget.Name = @"Pop_Message";
        }
        #endregion
    }
}
