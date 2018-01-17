using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace Naver.Compass.Service.Document
{
    internal class MasterStyle : RegionStyle, IMasterStyle
    {
        #region Constructors

        internal MasterStyle()
            : this(null, Guid.Empty)
        {
        }

        internal MasterStyle(Master ownerMaster, Guid viewGuid)
            : base(viewGuid, "MasterStyle")
        {
            _ownerMaster = ownerMaster;
        }

        #endregion

        #region XmlElementObject

        internal override void LoadDataFromXml(XmlElement element)
        {
            CheckTagName(element);

            Guid viewGuid = Guid.Empty;
            LoadGuidFromChildElementInnerText("AdaptiveViewGuid", element, ref viewGuid);
            ViewGuid = viewGuid;

            XmlElement propertiesElement = element["StyleProperties"];
            if (propertiesElement == null || propertiesElement.ChildNodes.Count <= 0)
            {
                return;
            }

            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.IS_VISIBLE_PROP);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.X_Prop);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.Y_Prop);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.HEIGHT_PROP);
            LoadStyleDoubleProperty(propertiesElement, StylePropertyNames.WIDTH_PROP);
            LoadStyleIntegerProperty(propertiesElement, StylePropertyNames.Z_PROP);
            LoadStyleBooleanProperty(propertiesElement, StylePropertyNames.IS_FIXED_PROP);
        }

        internal override void SaveDataToXml(XmlDocument xmlDoc, XmlElement parentElement)
        {
            XmlElement masterStyleElement = xmlDoc.CreateElement(TagName);
            parentElement.AppendChild(masterStyleElement);

            SaveStringToChildElement("AdaptiveViewGuid", ViewGuid.ToString(), xmlDoc, masterStyleElement);

            base.SaveDataToXml(xmlDoc, masterStyleElement);
        }

        #endregion

        public override IRegion OwnerRegion
        {
            get { return OwnerMaster; }
        }

        public IMaster OwnerMaster
        {
            get { return _ownerMaster; }
            set { _ownerMaster = value as Master; }
        }

        public string MD5 { get; set; }

        public override bool IsVisible
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.IS_VISIBLE_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.IsVisible;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.IS_VISIBLE_PROP, value);
            }
        }

        public override double X
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.X_Prop) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.X;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.X_Prop, value);
            }
        }

        public override double Y
        {
            get
            {
                StyleDoubleProperty property = GetStyleProperty(StylePropertyNames.Y_Prop) as StyleDoubleProperty;
                if (property != null)
                {
                    return property.DoubleValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Y;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.Y_Prop, value);
            }
        }

        public override int Z
        {
            get
            {
                StyleIntegerProperty property = GetStyleProperty(StylePropertyNames.Z_PROP) as StyleIntegerProperty;
                if (property != null)
                {
                    return property.IntegerValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.Z;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.Z_PROP, value);
            }
        }

        public bool IsFixed
        {
            get
            {
                StyleBooleanProperty property = GetStyleProperty(StylePropertyNames.IS_FIXED_PROP) as StyleBooleanProperty;
                if (property != null)
                {
                    return property.BooleanValue;
                }
                else if (ParentStyle != null)
                {
                    return ParentStyle.IsFixed;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                SetStyleProperty(StylePropertyNames.IS_FIXED_PROP, value);
            }
        }

        internal static void CopyMasterStyle(MasterStyle source, MasterStyle target)
        {
            target.IsVisible = source.IsVisible;
            target.X = source.X;
            target.Y = source.Y;
            target.Z = source.Z;
            target.IsFixed = source.IsFixed;
        }

        private MasterStyle ParentStyle
        {
            get
            {
                if (ParentDocument != null && ParentDocument.IsOpened)
                {
                    // This is base view style.
                    if (ViewGuid == ParentDocument.AdaptiveViewSet.Base.Guid)
                    {
                        return null;
                    }
                    else
                    {
                        // Return style in parent view
                        IAdaptiveView view = ParentDocument.AdaptiveViewSet.AdaptiveViews[ViewGuid];
                        if (view != null && view.ParentView != null)
                        {
                            MasterStyle style = _ownerMaster.GetMasterStyle(view.ParentView.Guid) as MasterStyle;
                            if (style == this)
                            {
                                style = null;
                            }

                            return style;
                        }
                    }
                }

                return null;
            }
        }

        private Master _ownerMaster;
    }
}
