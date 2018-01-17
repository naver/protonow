using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Naver.Compass.Service.Document
{
    internal abstract class RegionStyle : Style, IRegionStyle
    {
        internal RegionStyle(Guid viewGuid, string tagName)
            : base(tagName)
        {
            _viewGuid = viewGuid;
        }

        public abstract IRegion OwnerRegion { get; }

        public Guid ViewGuid
        {
            get { return _viewGuid; }
            set { _viewGuid = value; }
        }

        public virtual bool IsVisible
        {
            // The region is visible by default.
            get { return true; }
            set { /* Help derived class to override setter accessor.*/ }
        }

        public virtual double X
        {
            get
            {
                // Return the left-most in child regions by default.
                double leftMost = Double.MaxValue;
                GetLeftMost(ref leftMost);

                if (leftMost == Double.MaxValue)
                {
                    return 0;
                }

                return leftMost;
            }
            set { /* Help derived class to override setter accessor.*/ }
        }

        public virtual double Y
        {
            get
            {
                // Return the top-most in child regions by default.
                double topMost = Double.MaxValue;
                GetTopMost(ref topMost);

                if (topMost == Double.MaxValue)
                {
                    return 0;
                }

                return topMost;
            }
            set { /* Help derived class to override setter accessor.*/ }
        }

        public virtual double Height
        {
            get
            {
                // Return the height of the border which contains all child regions by default.
                double topMost = Double.MaxValue;
                double buttomMost = Double.MinValue;
                GetTopMost(ref topMost);
                GetBottomMost(ref buttomMost);

                if(topMost == Double.MaxValue || buttomMost == Double.MinValue)
                {
                    return 0;
                }

                double height = buttomMost - topMost;
                return height > 0 ? height : 0;
            }
            set { /* Help derived class to override setter accessor.*/ }
        }

        public virtual double Width
        {
            get
            {
                // Return the width of the border which contains all child regions by default.
                double leftMost = Double.MaxValue;
                double rightMost = Double.MinValue;
                GetLeftMost(ref leftMost);
                GetRightMost(ref rightMost);

                if (leftMost == Double.MaxValue || rightMost == Double.MinValue)
                {
                    // There is no child regions.
                    return 0;
                }

                double width = rightMost - leftMost;
                return width > 0 ? width : 0;
            }
            set { /* Help derived class to override setter accessor.*/ }
        }

        public virtual int Z
        {
            get
            {
                // Return the max z-order in child regions by default.
                int zMost = int.MinValue;
                GetZMost(ref zMost);

                if (zMost == Double.MinValue)
                {
                    return 0;
                }

                return zMost;
            }
            set { /* Help derived class to override setter accessor.*/ }
        }

        public virtual double Rotate
        {
            // The region is not rotated by default.
            get { return 0; }
        }   

        private void GetLeftMost(ref double leftMost)
        {
            if (OwnerRegion == null)
            {
                return;
            }

            IRegions regions = OwnerRegion.GetChildRegions(_viewGuid);
            foreach (IRegion region in regions)
            {
                IRegionStyle regionStyle = region.GetRegionStyle(_viewGuid);

                // If region is hamburger menu, use the button location and size to caculate.
                if (region is IHamburgerMenu)
                {
                    IHamburgerMenu menu = region as IHamburgerMenu;
                    regionStyle = menu.MenuButton.GetRegionStyle(_viewGuid);
                }

                if (regionStyle != null)
                {
                    if (regionStyle.Rotate == 0)
                    {
                        leftMost = Math.Min(regionStyle.X, leftMost);
                    }
                    else
                    {
                        double x = regionStyle.X;
                        double y = regionStyle.Y;
                        double xc = (regionStyle.X * 2 + regionStyle.Width) / 2;
                        double yc = (regionStyle.Y * 2 + regionStyle.Height) / 2;
                        double angle = Math.Abs(regionStyle.Rotate) % 180;
                        if (angle > 90)
                        {
                            angle = 180 - angle;
                        }
                        double Kc = Math.Cos(angle * Math.PI / 180);
                        double Ks = Math.Sin(angle * Math.PI / 180);

                        double xr = xc - (Kc * (xc - x) + Ks * (yc - y));
                        leftMost = Math.Min(xr, leftMost);
                    }
                }
            }
        }

        private void GetTopMost(ref double topMost)
        {
            if (OwnerRegion == null)
            {
                return;
            }

            IRegions regions = OwnerRegion.GetChildRegions(_viewGuid);
            foreach (IRegion region in regions)
            {
                IRegionStyle regionStyle = region.GetRegionStyle(_viewGuid);

                // If region is hamburger menu, use the button location and size to caculate.
                if (region is IHamburgerMenu)
                {
                    IHamburgerMenu menu = region as IHamburgerMenu;
                    regionStyle = menu.MenuButton.GetRegionStyle(_viewGuid);
                }

                if (regionStyle != null)
                {
                    if (regionStyle.Rotate == 0)
                    {
                        topMost = Math.Min(regionStyle.Y, topMost);
                    }
                    else
                    {
                        double x = regionStyle.X;
                        double y = regionStyle.Y;
                        double xc = (regionStyle.X * 2 + regionStyle.Width) / 2;
                        double yc = (regionStyle.Y * 2 + regionStyle.Height) / 2;
                        double angle = Math.Abs(regionStyle.Rotate) % 180;
                        if (angle > 90)
                        {
                            angle = 180 - angle;
                        }
                        double Kc = Math.Cos(angle * Math.PI / 180);
                        double Ks = Math.Sin(angle * Math.PI / 180);

                        double yr = yc - (Ks * (xc - x) + Kc * (yc - y));
                        topMost = Math.Min(yr, topMost);
                    }
                }
            }
        }

        private void GetRightMost(ref double rightMost)
        {
            if (OwnerRegion == null)
            {
                return;
            }

            IRegions regions = OwnerRegion.GetChildRegions(_viewGuid);
            foreach (IRegion region in regions)
            {
                IRegionStyle regionStyle = region.GetRegionStyle(_viewGuid);

                // If region is hamburger menu, use the button location and size to caculate.
                if (region is IHamburgerMenu)
                {
                    IHamburgerMenu menu = region as IHamburgerMenu;
                    regionStyle = menu.MenuButton.GetRegionStyle(_viewGuid);
                }

                if (regionStyle != null)
                {
                    if (regionStyle.Rotate == 0)
                    {
                        rightMost = Math.Max(regionStyle.X + regionStyle.Width, rightMost);
                    }
                    else
                    {
                        double x = regionStyle.X;
                        double y = regionStyle.Y;
                        double xc = (regionStyle.X * 2 + regionStyle.Width) / 2;
                        double yc = (regionStyle.Y * 2 + regionStyle.Height) / 2;
                        double angle = Math.Abs(regionStyle.Rotate) % 180;
                        if (angle > 90)
                        {
                            angle = 180 - angle;
                        }
                        double Kc = Math.Cos(angle * Math.PI / 180);
                        double Ks = Math.Sin(angle * Math.PI / 180);

                        double xr = xc - (Kc * (xc - x) + Ks * (yc - y));
                        double width = regionStyle.Width + (x - xr) * 2;
                        rightMost = Math.Max(xr + width, rightMost);
                    }
                }
            }
        }

        private void GetBottomMost(ref double bottomMost)
        {
            if (OwnerRegion == null)
            {
                return;
            }

            IRegions regions = OwnerRegion.GetChildRegions(_viewGuid);
            foreach (IRegion region in regions)
            {
                IRegionStyle regionStyle = region.GetRegionStyle(_viewGuid);

                // If region is hamburger menu, use the button location and size to caculate.
                if (region is IHamburgerMenu)
                {
                    IHamburgerMenu menu = region as IHamburgerMenu;
                    regionStyle = menu.MenuButton.GetRegionStyle(_viewGuid);
                }

                if (regionStyle != null)
                {
                    if (regionStyle.Rotate == 0)
                    {
                        bottomMost = Math.Max(regionStyle.Y + regionStyle.Height, bottomMost);
                    }
                    else
                    {
                        double x = regionStyle.X;
                        double y = regionStyle.Y;
                        double xc = (regionStyle.X * 2 + regionStyle.Width) / 2;
                        double yc = (regionStyle.Y * 2 + regionStyle.Height) / 2;
                        double angle = Math.Abs(regionStyle.Rotate) % 180;
                        if (angle > 90)
                        {
                            angle = 180 - angle;
                        }
                        double Kc = Math.Cos(angle * Math.PI / 180);
                        double Ks = Math.Sin(angle * Math.PI / 180);

                        double yr = yc - (Ks * (xc - x) + Kc * (yc - y));
                        double height = regionStyle.Height + (y - yr) * 2;
                        bottomMost = Math.Max(yr + height, bottomMost);
                    }
                }
            }
        }

        private void GetZMost(ref int zMost)
        {
            if (OwnerRegion == null)
            {
                return;
            }

            IRegions regions = OwnerRegion.GetChildRegions(_viewGuid);
            foreach (IRegion region in regions)
            {
                IRegionStyle regionStyle = region.GetRegionStyle(_viewGuid);

                // If region is hamburger menu, use the button location and size to caculate.
                if (region is IHamburgerMenu)
                {
                    IHamburgerMenu menu = region as IHamburgerMenu;
                    regionStyle = menu.MenuButton.GetRegionStyle(_viewGuid);
                }

                if (regionStyle != null)
                {
                    zMost = Math.Max(regionStyle.Z, zMost);
                }
            }
        }

        protected IDocument ParentDocument
        {
            get
            {
                if (OwnerRegion != null && OwnerRegion.ParentPage != null)
                {
                    return OwnerRegion.ParentPage.ParentDocument;
                }
                
                return null;
            }
        }

        private Guid _viewGuid = Guid.Empty;
    }
}
