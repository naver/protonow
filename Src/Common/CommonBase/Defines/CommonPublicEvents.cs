using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Naver.Compass.Service.Document;
namespace Naver.Compass.Common.CommonBase
{
    public class UpdateThemeEvent : CompositePresentationEvent<string>
    {

    }
    public class UpdateLanguageEvent : CompositePresentationEvent<string>
    {

    }
    public class UpdateFontEvent : CompositePresentationEvent<string>
    {

    }

    public class SynUploadEvent : CompositePresentationEvent<object>
    {

    }

    //change DockingLayout themes event
    public class ThemesEvent : CompositePresentationEvent<string>
    {
    }

    //Open or active document 
    public class OpenNormalPageEvent : CompositePresentationEvent<Guid>
    {
    }

    public class OpenMasterPageEvent: CompositePresentationEvent<Guid>
    {

    }

    //Open or active Dynamic/Hamburger Page 
    public class OpenWidgetPageEvent : CompositePresentationEvent<IWidget>
    {
    }

    //Open or active Dynamic/Hamburger Page 
    public class CloseWidgetPageEvent : CompositePresentationEvent<IWidget>
    {
    }

    public class RenamePageEvent : CompositePresentationEvent<Guid>
    {
    }
    //Open or active document 
    public class ClosePageEvent : CompositePresentationEvent<Guid>
    {
    }

    public class FocusSitemapEvent : CompositePresentationEvent<object>
    {
    }

    public class ChangeLayoutEvent : CompositePresentationEvent<string>
    {
    }

    //File Operation Event
    public class FileOperationEvent : CompositePresentationEvent<FileOperationType>
    {
    }

    public class AutoSaveSettingChangedEvent : CompositePresentationEvent<object>
    {
    }

    public class RecoveryDocumentOpenEvent : CompositePresentationEvent<object>
    {
    }

    //OpenFile Operation Event
    public class OpenFileEvent : CompositePresentationEvent<string>
    {
    }

    public class RecoveryFileEvent : CompositePresentationEvent<object>
    {
    }

    //File Operation Event
    public class DomLoadedEvent : CompositePresentationEvent<FileOperationType>
    {
    }

    //Flash Recent List
    public class FlashRecentList : CompositePresentationEvent<object>
    {

    }

    public class OpenPanesEvent : CompositePresentationEvent<ActivityPane>
    {

    }

    //Open setting language dialog
    public class OpenDialogEvent : CompositePresentationEvent<DialogType>
    {

    }

    public class UpdateNoteEvent : CompositePresentationEvent<string>
    {

    }

    //Open or active document 
    public class PagePreviewEvent : CompositePresentationEvent<Guid>
    {

    }

    //Selection: Widget Collection Change
    public class SelectionChangeEvent : CompositePresentationEvent<string>
    {

    }

    //Selection: Widget Property Change
    public class SelectionPropertyChangeEvent : CompositePresentationEvent<string>
    {

    }

    //Selection: Widget Property Change
    public class SelectionPageChangeEvent : CompositePresentationEvent<Guid>
    {

    }

    //Selection: Widget Property Change
    public class SelectionPagePropertyChangeEvent : CompositePresentationEvent<string>
    {

    }

    // geretate HTML, string: output Path
    public class GenerateHTMLEvent : CompositePresentationEvent<object>
    {

    }
    // geretate MD5 HTML, string: output Path
    public class GenerateMD5HTMLEvent : CompositePresentationEvent<object>
    {

    }
    // geretate HTML, string: output Path
    //true: normal process
    //false: differ process
    public class PublishHTMLEvent : CompositePresentationEvent<bool>
    {

    }
    public class PublishMD5HTMLEvent : CompositePresentationEvent<object>
    {

    }
    // Pass page selected. use in PagelistTreeView in InteractionPanel
    public class PageNameSelectedEvent : CompositePresentationEvent<PageInfo>
    {

    }

    //Load adaptive views from Document.
    public class LoadAdaptiveViewEvent : CompositePresentationEvent<AdaptiveLoadType>
    {

    }

    //Load adaptive views from Document.
    public class UpdateAdaptiveView : CompositePresentationEvent<Guid>
    {

    }

    public class UpdatePageTreeEvent : CompositePresentationEvent<string>
    {

    }

    //send event from contextmenu
    public class EditTooltipEvent : CompositePresentationEvent<string>
    {

    }

    public class CheckUpdateCompletedEvent : CompositePresentationEvent<string>
    {

    }

    public class UpdateProcessEvent : CompositePresentationEvent<string>
    {

    }

    //Guid: delete all if empty
    public class DeleteGuideEvent : CompositePresentationEvent<GuideInfo>
    {

    }

    public class NewWidgetEvent : CompositePresentationEvent<object>
    {

    }
    public class UpdateGridGuide : CompositePresentationEvent<GridGuideType>
    {

    }

    public class GlobalLockGuides : CompositePresentationEvent<object>
    {

    }

    public class FormatPaintEvent : CompositePresentationEvent<object>
    {

    }

    public class CancelFormatPaintEvent : CompositePresentationEvent<object>
    {

    }

    public class TBUpdateEvent : CompositePresentationEvent<TBUpdateType>
    {

    }

    //Selection: Widget Property Change
    public class RefreshWidgetChildPageEvent : CompositePresentationEvent<Guid>
    {

    }

    public class AddNewPageEvent : CompositePresentationEvent<Guid>
    {

    }

    //Mouse over Objects list in interaction
    public class MouseOverInteractionObject : CompositePresentationEvent<IUniqueObject>
    {

    }

    //Enable to fire left/top in ribbon when move widgets.
    public class EnableFirePositionInRibbonEvent : CompositePresentationEvent<bool>
    {

    }

    //Open Sitemap page Event
    public class SiteMapEvent : CompositePresentationEvent<SiteMapEventEnum>
    {
    }

    //This event post from widget manager to Page VM
    //public class SelectionChangedByWidgetManager : CompositePresentationEvent<object>
    //{
    //}
    //This event post from widget manager to change selection
    public class WdgMgrChangeSelectionEvent: CompositePresentationEvent<object>
    {

    }

    //This event post from widget manager to change selection
    public class WdgMgrEditSelectionEvent : CompositePresentationEvent<object>
    {

    }

    //This event post from widget manager to change selection
    public class WdgMgrHideSelectionEvent : CompositePresentationEvent<object>
    {

    }

    //This event post from widget manager to delete selection
    public class WdgMgrDeleteSelectionEvent: CompositePresentationEvent<object>
    {

    }
    //This event post from widget manager to change z-order by Up and Down
    public class WdgMgrZorderChangedEvent : CompositePresentationEvent<object>
    {

    }
    //This event post from widget manager to Place widget
    public class WdgMgrPlacewidgetEvent : CompositePresentationEvent<object>
    {

    }

    //This event post from widget manager to re-Order widget by drag and Drop
    public class WdgMgrOrderwidgetEvent : CompositePresentationEvent<object>
    {

    }

    //This event post from widget manager to re-Order widget by drag and Drop
    public class WdgMgrOpenChildWidgetPage : CompositePresentationEvent<Guid>
    {

    }

    //This event post from widget manager to Page VM
    public class SelectionChangedByItemNotify : CompositePresentationEvent<object>
    {
    }

    //This event is accepted by widget manager to resort all items
    public class ZorderChangedEvent : CompositePresentationEvent<object>
    {

    }

    //This event is accepted by widget manager to change group status
    //Parameter: true/group, fale/ungroup.
    public class GroupChangedEvent : CompositePresentationEvent<bool>
    {

    }
    //This event post from widget manager to change selection
    public class SwipePanelHidddenEvent : CompositePresentationEvent<object>
    {

    }


    public class ExportToMYLibraryEvent : CompositePresentationEvent<object>
    {

    }
    public class RefreshCustomLibraryEvent : CompositePresentationEvent<string>
    {

    }

    public class DisplayAppLoadingEvent : CompositePresentationEvent<bool>
    {

    }
    public class AddConvertedMasterEvent : CompositePresentationEvent<object>
    {

    }

    public class AddMasterEvent : CompositePresentationEvent<IMasterPage>
    {

    }

    public class DeleteMasterPageEvent : CompositePresentationEvent<IMasterPage>
    {

    }
    public class EditorScaleChangeEvent:CompositePresentationEvent<bool>
    {

    }
    public enum CompositeEventType
    {
        ZorderChange,
        GroupChange
    }
    //Example case        
    //public void ttsfl()
    //{
    //    _ListEventAggregator.GetEvent<UpdateThemeEvent>().Subscribe(cccc);
    //    _ListEventAggregator.GetEvent<UpdateThemeEvent>().Publish(@"fadsfd");
    //    _ListEventAggregator.GetEvent<UpdateThemeEvent>().Subscribe(cccc, ThreadOption.UIThread);
    //    _ListEventAggregator.GetEvent<UpdateThemeEvent>().Unsubscribe(cccc);
    //}
    //public void cccc(string sz)
    //{
    //}

    //TODO: all event will be added here
    #region used by widgetGalleryViewModel
    public class WidgetFavouriteEvent : CompositePresentationEvent<object>
    {
    }

    public class ResetFavouriteEvent : CompositePresentationEvent<object>
    {
    }

    public class CustomWidgetChangedEvent : CompositePresentationEvent<object>
    {
    }

    public class DeleteLibraryWidgetEvent : CompositePresentationEvent<object>
    {
    }

    public class LibraryExpandChangedEvent : CompositePresentationEvent<object>
    {
    }
    #endregion


    #region
    public class WidgetsNumberChangedEvent : CompositePresentationEvent<Guid>
    {

    }
    #endregion
}
