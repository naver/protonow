using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.Service.Document;

namespace Naver.Compass.Service.Html
{

    /*
            "actions" :{
                "onClick" : [
                    {
                        "action" : "openLink",
                        "url" : "http://www.naver.com",
                        "target" : "openNewWindow"
                    },
                    {
                        "action" : "openLink",
                        "url" : "http://www.daum.net",
                        "target" : "openPopup",
                        "popup" : {
                            "left" : 100,
                            "top" : 100,
                            "width" : 500,
                            "height" : 500,
                            "toolbar" : false,
                            "scrollbars":false,
                            "location":false,
                            "status":false,
                            "menubar":false,
                            "directories":false,
                            "resizable":false,
                            "centerwindow":true
                        }
                    },
                    {
                       "action" : "openLink",
                       "url" : "Page_1",
                       "target" : "openCurrentWindow"
                    }
                ]
            }
     * 
     * */

    class JsAction
    {
        public JsAction(HtmlServiceProvider service, IInteractionAction action)
        {
            _service = service;
            _action = action;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            switch (_action.ActionType)
            {
                case ActionType.OpenAction:
                    {
                        IInteractionOpenAction openAction = _action as IInteractionOpenAction;
                        if (openAction == null || openAction.LinkType == LinkType.None)
                        {
                            return "";
                        }

                        builder.Append("{");
                        builder.AppendFormat("\"action\":\"{0}\",", GetJsAction(_action.ActionType));

                        if (openAction.LinkType == LinkType.LinkToPage)
                        {
                            // http://bts1.navercorp.com/nhnbts/browse/DSTUDIO-2061
                            // If action is "Link to a page", I will always set "target" to  "openCurrentWindow" in js action.
                            builder.AppendFormat("\"target\":\"{0}\",", GetJsTarget(ActionOpenIn.CurrentWindow));

                            if (_service.RenderingDocument.Pages.Contains(openAction.LinkPageGuid))
                            {
                                builder.AppendFormat("\"url\":\"{0}\",", openAction.LinkPageGuid.ToString());
                            }
                        }
                        else if (openAction.LinkType == LinkType.LinkToUrl)
                        {
                            builder.AppendFormat("\"target\":\"{0}\",", GetJsTarget(openAction.OpenIn));

                            if (!String.IsNullOrEmpty(openAction.ExternalUrl))
                            {
                                // Add http so that browser could know this is a external url
                                string url = openAction.ExternalUrl;
                                if (!url.StartsWith(@"http://"))
                                {
                                    url = @"http://" + url;
                                }
                                builder.AppendFormat("\"url\":\"{0}\",", url);
                            }
                        }

                        JsHelper.RemoveLastComma(builder);
                        builder.Append("}");
                        break;
                    }
                case ActionType.ShowHideAction:
                    {
                        IInteractionShowHideAction showHideAction = _action as IInteractionShowHideAction;
                        if(showHideAction == null || showHideAction.TargetObjects.Count <= 0)
                        {
                            return "";
                        }

                        // If all target objects VisibilityType is none, we don't issue action as well
                        IShowHideActionTarget firstTarget = showHideAction.TargetObjects.FirstOrDefault<IShowHideActionTarget>(x => x.VisibilityType != VisibilityType.None);
                        if (firstTarget == null)
                        {
                            return "";
                        }

                        builder.Append("{");
                        
                        builder.AppendFormat("\"action\":\"{0}\",", GetJsAction(_action.ActionType));

                        builder.Append("\"objectList\":[");
                        foreach(IShowHideActionTarget target in showHideAction.TargetObjects)
                        {
                            builder.Append("{");

                            builder.AppendFormat("\"id\":\"{0}\",", target.Guid.ToString());
                            builder.AppendFormat("\"type\":\"{0}\",", target.VisibilityType.ToString());
                            builder.AppendFormat("\"animateType\":\"{0}\",", target.AnimateType.ToString());
                            builder.AppendFormat("\"animateTime\":{0},", target.AnimateTime);
                            
                            builder.Append("},");
                        }

                        JsHelper.RemoveLastComma(builder);
                        builder.Append("]");

                        builder.Append("}");
                        break;
                    }
                default:
                    return "";
            }

            return builder.ToString();
        }

        private string GetJsAction(ActionType type)
        {
            switch(type)
            {
                case ActionType.OpenAction:
                    return "openLink";
                case ActionType.CloseAction:
                    return "closeWindow";
                case ActionType.ShowHideAction:
                    return "widget";
                default:
                    return type.ToString().ToLower();
            }
        }

        private string GetJsTarget(ActionOpenIn openIn)
        {
            switch (openIn)
            {
                case ActionOpenIn.CurrentWindow:
                    return "openCurrentWindow";
                case ActionOpenIn.NewWindowOrTab:
                    return "openNewWindow";
                default:
                    return "openCurrentWindow";
            }
        }

        private HtmlServiceProvider _service;
        private IInteractionAction _action;
    }

    /*
    class JsCase
    {
        public JsCase(HtmlServiceProvider service, IInteractionCase iCase)
        {
            _service = service;
            _case = iCase;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(@" { ");

            builder.AppendFormat(@" ""name"" : ""{0}"", ", _case.Name);

            builder.AppendLine();
            builder.AppendLine(@" ""actions"":[ ");

            foreach (IInteractionAction action in _case.Actions)
            {
                JsAction jsAction = new JsAction(_service, action);
                builder.Append(jsAction.ToString());
                builder.Append(@","); 
            }

            builder.AppendLine(@" ] ");
            builder.AppendLine(@" } ");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
        private IInteractionCase _case;
    }
    */

    class JsEvent
    {
        public JsEvent(HtmlServiceProvider service, IInteractionEvent iEvent)
        {
            _service = service;
            _event = iEvent;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("\"" + GetJsEventName(_event.Name) + "\":[ ");

            //foreach (IInteractionCase iCase in _event.Cases)
            //{
            //    JsCase jsCase = new JsCase(_service, iCase);
            //    builder.Append(jsCase.ToString());
            //    builder.Append(@","); 
            //}

            // Note: currently, we only support one case, uncomments above codes when we support multiple case.
            foreach (IInteractionCase iCase in _event.Cases)
            {
                foreach (IInteractionAction action in iCase.Actions)
                {
                    JsAction jsAction = new JsAction(_service, action);
                    string actionString = jsAction.ToString();
                    if (String.IsNullOrEmpty(actionString) == false)
                    {
                        builder.Append(actionString);
                        builder.Append(",");
                    }
                }
            }

            JsHelper.RemoveLastComma(builder);

            builder.Append("]");

            return builder.ToString();
        }

        private string GetJsEventName(string eventName)
        {
            if(String.Compare(eventName, "onclick", true) == 0)
            {
                return "onClick";
            }

            return eventName;
            // else event name
        }

        private HtmlServiceProvider _service;
        private IInteractionEvent _event;
    }

    class JsEvents
    {
        public JsEvents(HtmlServiceProvider service, IInteractionEvents events)
        {
            _service = service;
            _events = events;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("\"actions\":{");

            foreach (IInteractionEvent iEvent in _events)
            {
                JsEvent jsEvent = new JsEvent(_service, iEvent);
                builder.Append(jsEvent.ToString());
                builder.Append(","); 
            }

            JsHelper.RemoveLastComma(builder);

            builder.Append("}");

            return builder.ToString();
        }

        private HtmlServiceProvider _service;
        private IInteractionEvents _events;
    }
}
