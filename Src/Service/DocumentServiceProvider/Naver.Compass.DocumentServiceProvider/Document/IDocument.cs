using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public enum DocumentType
    {
        Standard,
        Library
    }

    public interface IDocument : IUniqueObject, INamedObject, IDisposable
    {
        DocumentType DocumentType { get; }

        string TimeStamp { get; }

        string FileVersion { get; }

        string CreatedAssemblyVersion { get; }

        string CreatedProductName { get; }

        string CreatedProductVersion { get; }

        string ThisVersion { get; }

        string Title { get; }

        bool IsDirty { get; set; }

        bool IsOpened { get; }

        IHostService HostService { get; }

        IPages Pages { get; }

        IMasterPages MasterPages { get; }

        IGuides GlobalGuides { get; }

        IAnnotationFieldSet PageAnnotationFieldSet { get; }

        IAnnotationFieldSet WidgetAnnotationFieldSet { get; }

        IWidgetDefaultStyleSet WidgetDefaultStyleSet { get; }

        IGeneratorConfigurationSet GeneratorConfigurationSet { get; }

        IAdaptiveViewSet AdaptiveViewSet { get; }

        IDeviceSet DeviceSet { get; }

        IDocumentSettings DocumentSettings { get; }

        IHashStreamManager ImagesStreamManager { get; }

        IDocumentPage CreatePage(string pageName);
        void DeletePage(Guid pageGuid);
        void AddPage(IDocumentPage page);
        IDocumentPage DuplicatePage(Guid pageGuid);  // Create a duplicate page.
        IObjectContainer AddPages(Stream stream);

        IMasterPage CreateMasterPage(string pageName);
        IMasterPage CreateMasterPage(ISerializeWriter writer, string pageName, Stream thumbnail);
        void DeleteMasterPage(Guid pageGuid);
        void AddMasterPage(IMasterPage page);
        IMasterPage DuplicateMasterPage(Guid pageGuid);
        IObjectContainer AddMasterPages(Stream stream);

        IGuide CreateGlobalGuide(Orientation orientation, double x = 0, double y = 0);
        void DeleteGlobalGuide(Guid guideGuid);
        void AddGlobalGuide(IGuide guide);

        ISerializeWriter CreateSerializeWriter(Guid currentViewGuid);
        ISerializeReader CreateSerializeReader(Stream stream);
    }
}
