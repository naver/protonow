using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{
    class JsWidgetStyle
    {
        public JsWidgetStyle(JsWidget parentJsWidget, IWidgetStyle widgetStyle, IPageView pageView, bool bIsSetMD5)
        {
            _parentJsWidget = parentJsWidget;
            _widgetStyle = widgetStyle;
            _pageView = pageView;
            IsSetMD5 = bIsSetMD5;
        }
        public bool IsSetMD5 { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");

            // Don't add adaptiveViewID if it is style in base.
            if (_pageView.Guid != _widgetStyle.OwnerWidget.ParentDocument.AdaptiveViewSet.Base.Guid)
            {
                builder.AppendFormat("\"adaptiveViewID\":\"{0}\",", _pageView.Guid);
            }
            if (IsSetMD5 == true)
            {
                builder.AppendFormat("\"MD5\":\"{0}\",", _widgetStyle.MD5);
            }
            builder.AppendFormat("\"width\":\"{0}px\",", Math.Round(_widgetStyle.Width));
            builder.AppendFormat("\"height\":\"{0}px\",", Math.Round(_widgetStyle.Height));

            IToast toast = _widgetStyle.OwnerWidget as IToast;
            if (toast != null && toast.DisplayPosition == ToastDisplayPosition.Top)
            {
                builder.AppendFormat("\"top\":\"0px\",");
            }
            else
            {
                builder.AppendFormat("\"top\":\"{0}px\",", Math.Round(_widgetStyle.Y));
            }

            builder.AppendFormat("\"left\":\"{0}px\",", Math.Round(_widgetStyle.X));
            builder.AppendFormat("\"rotate\":\"{0}deg\",", Math.Round(_widgetStyle.WidgetRotate));
            builder.AppendFormat("\"textrot\":\"{0}deg\",", Math.Round(_widgetStyle.TextRotate));

            //  In CSS(style) opacity, not use %. From 0.0 (fully transparent) to 1.0 (fully opaque)
            double opacity = _widgetStyle.Opacity;
            builder.AppendFormat("\"opacity\":{0},", opacity / 100); 
            
            if(_widgetStyle.IsFixed)
            {
                builder.AppendFormat("\"position\":\"fixed\",");
            }
            else
            {
                if (toast != null && toast.DisplayPosition != ToastDisplayPosition.UserSetting)
                {
                    builder.AppendFormat("\"position\":\"fixed\",");
                }
                else
                {
                    builder.AppendFormat("\"position\":\"absolute\",");
                }
            }

            // isPlaced flag
            bool isPlaced = true;
            if (_pageView != null)
            {
                if (!_pageView.Widgets.Contains(_widgetStyle.OwnerWidget.Guid))
                {
                    isPlaced = false;
                }
            }
            else
            {
                // Check base vew
                Guid baseViewGuid = _widgetStyle.OwnerWidget.ParentPage.ParentDocument.AdaptiveViewSet.Base.Guid;
                IPageView baseView = _widgetStyle.OwnerWidget.ParentPage.PageViews[baseViewGuid];
                if (baseView != null && !baseView.Widgets.Contains(_widgetStyle.OwnerWidget.Guid))
                {
                    isPlaced = false;
                }
            }
            builder.AppendFormat("\"isPlaced\":{0},", isPlaced.ToString().ToLower());

            // CSS value : visible is "block" and hidden is "none".
            string visibility = "block"; 
            if (!_widgetStyle.IsVisible)
            {
                visibility = "none";
            }
            else
            {
                // If widget is not placed in this view, set to be hidden.
                if (!isPlaced)
                {
                    visibility = "none";
                }

                // The menu of hamburger are always invisible as it will show once button is clicked.
                if (_widgetStyle.OwnerWidget is IHamburgerMenu)
                {
                    visibility = "none";
                }
            }

            builder.AppendFormat("\"display\":\"{0}\",", visibility);

            // In current design, Z-Order doesn't support adaptive view.
            // All hamburger menu's z-index value is starting from 200000. menu button's z-index value is starting with 100000
            // because this menu should be on top.
            if (_widgetStyle.OwnerWidget is IHamburgerMenuButton)
            {
                builder.AppendFormat("\"z-index\":{0},", (_widgetStyle.OwnerWidget.WidgetStyle.Z + 100000));
            }
            else if (_widgetStyle.OwnerWidget is IHamburgerMenu)
            {
                builder.AppendFormat("\"z-index\":{0},", (_widgetStyle.OwnerWidget.WidgetStyle.Z + 200000));
            }
            else
            {
                builder.AppendFormat("\"z-index\":{0},", _widgetStyle.OwnerWidget.WidgetStyle.Z);
            }

            // Only following types have hand cursor when they have actions
            AppendActionCursor(builder);

            _parentJsWidget.AppendSpecificTypeStyle(builder, _widgetStyle);

            JsHelper.RemoveLastComma(builder);

            builder.Append("}");

            return builder.ToString();
        }

        private void AppendActionCursor(StringBuilder builder)
        {
            if (_widgetStyle.OwnerWidget.Events[EventType.OnClick] != null)
            {
                foreach (IInteractionCase interactionCase in _widgetStyle.OwnerWidget.Events[EventType.OnClick].Cases)
                {
                    foreach (IInteractionAction action in interactionCase.Actions)
                    {
                        if (action.ActionType == ActionType.OpenAction)
                        {
                            IInteractionOpenAction openAction = action as IInteractionOpenAction;
                            if (openAction != null && openAction.LinkType != LinkType.None)
                            {
                                builder.Append("\"cursor\":\"pointer\",");
                            }
                        }
                        else if (action.ActionType == ActionType.CloseAction)
                        {
                            builder.Append("\"cursor\":\"pointer\",");
                        }
                        else if (action.ActionType == ActionType.ShowHideAction)
                        {
                            IInteractionShowHideAction showAction = action as IInteractionShowHideAction;
                            if (showAction != null && showAction.TargetObjects.Count > 0)
                            {
                                // Only check the first item as all targets has the same setting for now.
                                IShowHideActionTarget target = showAction.TargetObjects[0];
                                if (target != null && target.VisibilityType != VisibilityType.None)
                                {
                                    builder.Append("\"cursor\":\"pointer\",");
                                }
                            }
                        }
                    }
                }
            }
        }

        private JsWidget _parentJsWidget;
        private IWidgetStyle _widgetStyle;
        private IPageView _pageView;
    }
}
