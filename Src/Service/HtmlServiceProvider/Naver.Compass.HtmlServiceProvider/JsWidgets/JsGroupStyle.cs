using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    internal class JsGroupStyle
    {
        public JsGroupStyle(HtmlServiceProvider service, IGroup group, IAdaptiveView adaptiveView)
        {
            _service = service;
            _group = group;
            _adaptiveView = adaptiveView;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            if (_adaptiveView != _group.ParentDocument.AdaptiveViewSet.Base)
            {
                builder.AppendFormat("\"adaptiveViewID\":\"{0}\",", _adaptiveView.Guid);
            }

            IRegionStyle groupStyle = _group.GetRegionStyle(_adaptiveView.Guid);
            if (groupStyle != null)
            {
                builder.AppendFormat("\"width\":\"{0}px\",", Math.Round(groupStyle.Width));
                builder.AppendFormat("\"height\":\"{0}px\",", Math.Round(groupStyle.Height));
            }

            // If this is top group, generate absolute location value.
            if (_group.ParentGroup == null)
            {
                builder.AppendFormat("\"top\":\"{0}px\",", Math.Round(groupStyle.Y));
                builder.AppendFormat("\"left\":\"{0}px\",", Math.Round(groupStyle.X));
            }
            else
            {
                // Child group, generate relative location value, relative to top level parent group.
                IGroup topLevelParentGroup = GetTopLevelParentGroup(_group);
                IRegionStyle topLevelParentGroupStyle = topLevelParentGroup.GetRegionStyle(_adaptiveView.Guid);

                builder.AppendFormat("\"top\":\"{0}px\",", Math.Round(groupStyle.Y - topLevelParentGroupStyle.Y));
                builder.AppendFormat("\"left\":\"{0}px\",", Math.Round(groupStyle.X - topLevelParentGroupStyle.X));
            }

            // In current design, Z-Order doesn't support adaptive view.
            //builder.AppendFormat("\"z-index\":{0},", groupStyle.Z);
            IRegionStyle groupBaseViewStyle = _group.RegionStyle;
            builder.AppendFormat("\"z-index\":{0},", groupBaseViewStyle.Z);

            // http://bts1.navercorp.com/nhnbts/browse/DSTUDIO-1357
            // The "display" of group is none only if all child widgets is hidden.
            string visibility = "block";
            if (HasVisibleChildWidget(_group, _adaptiveView, true) == false)
            {
                visibility = "none";
            }
            builder.AppendFormat("\"display\":\"{0}\",", visibility);

            string position = "absolute";
            if (HasUnfixedChildWidget(_group, _adaptiveView, true) == false)
            {
                position = "fixed";
            }
            builder.AppendFormat("\"position\":\"{0}\",", position);

            JsHelper.RemoveLastComma(builder);

            builder.Append("}");

            return builder.ToString();
        }

        private IGroup GetTopLevelParentGroup(IGroup group)
        {
            if (group.ParentGroup != null)
            {
                return GetTopLevelParentGroup(group.ParentGroup);
            }

            return group;
        }

        private bool HasVisibleChildWidget(IGroup group, IAdaptiveView adaptiveView, bool recursive)
        {
            foreach(IWidget widget in group.Widgets)
            {
                IWidgetStyle style = widget.GetWidgetStyle(adaptiveView.Guid);
                if (style != null && style.IsVisible)
                {
                    return true;
                }
            }

            if (recursive)
            {
                foreach(IGroup childGroup in group.Groups)
                {
                    return HasVisibleChildWidget(childGroup, adaptiveView, recursive);
                }
            }

            return false;
        }

        private bool HasUnfixedChildWidget(IGroup group, IAdaptiveView adaptiveView, bool recursive)
        {
            foreach (IWidget widget in group.Widgets)
            {
                IWidgetStyle style = widget.GetWidgetStyle(adaptiveView.Guid);
                if (style != null && style.IsFixed == false)
                {
                    return true;
                }
            }

            if (recursive)
            {
                foreach (IGroup childGroup in group.Groups)
                {
                    return HasUnfixedChildWidget(childGroup, adaptiveView, recursive);
                }
            }

            return false;
        }

        private HtmlServiceProvider _service;
        private IGroup _group;
        private IAdaptiveView _adaptiveView;
    }
}
