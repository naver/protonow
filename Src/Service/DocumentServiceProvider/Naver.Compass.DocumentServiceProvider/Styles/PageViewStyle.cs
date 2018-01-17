using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal class PageViewStyle : RegionStyle
    {
        internal PageViewStyle(PageView ownerPageView)
            : base(ownerPageView.Guid, "PageViewStyle")
        {
            // PageViewStyle must exist with its owner.
            Debug.Assert(ownerPageView != null);
            _ownerPageView = ownerPageView;
        }

        public override IRegion OwnerRegion
        {
            get { return _ownerPageView; }
        }

        public override double X
        {
            set
            {
                double delta = X - value;

                foreach (IRegion childRegion in _ownerPageView.GetChildRegions(_ownerPageView.Guid))
                {
                    if(childRegion is IMaster)
                    {
                        IMaster master = childRegion as IMaster;
                        if (master != null)
                        {
                            IMasterStyle style = master.GetMasterStyle(_ownerPageView.Guid);
                            if(style != null)
                            {
                                style.X -= delta;
                            }
                        }
                    }
                    else if(childRegion is IWidget)
                    {
                        IWidget widget = childRegion as IWidget;
                        if(widget != null)
                        {
                            if (widget is IHamburgerMenu)
                            {
                                IHamburgerMenu menu = widget as IHamburgerMenu;
                                IWidgetStyle style = menu.GetWidgetStyle(_ownerPageView.Guid);
                                if (style != null)
                                {
                                    style.X -= delta;
                                }

                                IHamburgerMenuButton button = menu.MenuButton;
                                style = button.GetWidgetStyle(_ownerPageView.Guid);
                                if (style != null)
                                {
                                    style.X -= delta;
                                }
                            }
                            else
                            {
                                IWidgetStyle style = widget.GetWidgetStyle(_ownerPageView.Guid);
                                if (style != null)
                                {
                                    style.X -= delta;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override double Y
        {
            set
            {
                double delta = Y - value;

                foreach (IRegion childRegion in _ownerPageView.GetChildRegions(_ownerPageView.Guid))
                {
                    if (childRegion is IMaster)
                    {
                        IMaster master = childRegion as IMaster;
                        if (master != null)
                        {
                            IMasterStyle style = master.GetMasterStyle(_ownerPageView.Guid);
                            if (style != null)
                            {
                                style.Y -= delta;
                            }
                        }
                    }
                    else if (childRegion is IWidget)
                    {
                        IWidget widget = childRegion as IWidget;
                        if (widget != null)
                        {
                            if (widget is IHamburgerMenu)
                            {
                                IHamburgerMenu menu = widget as IHamburgerMenu;
                                IWidgetStyle style = menu.GetWidgetStyle(_ownerPageView.Guid);
                                if (style != null)
                                {
                                    style.Y -= delta;
                                }

                                IHamburgerMenuButton button = menu.MenuButton;
                                style = button.GetWidgetStyle(_ownerPageView.Guid);
                                if (style != null)
                                {
                                    style.Y -= delta;
                                }
                            }
                            else
                            {
                                IWidgetStyle style = widget.GetWidgetStyle(_ownerPageView.Guid);
                                if (style != null)
                                {
                                    style.Y -= delta;
                                }
                            }
                        }
                    }
                }
            }
        }

        private PageView _ownerPageView;
    }
}
