using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Naver.Compass.Service.Document
{
    public interface ILibraryManager : IDisposable
    {
        /// <summary>
        /// Create a new library to ship the new created custom object which contains the widgets and groups in writer.
        /// The new library is NOT managed by the LibraryManager, if you don't hold the return library, you cannot get it any more.
        /// </summary>
        /// <param name="writer">Container for widgets and groups which will be added in the new custom object.</param>
        /// <param name="objectName">The name of the new created custom object.</param>
        /// <param name="icon">The icon of new created custom object.</param>
        /// <param name="thumbnail">The thumbnail of new created custom object.</param>
        /// <returns></returns>
        ILibrary NewLibrary(ISerializeWriter writer, string objectName, Stream icon, Stream thumbnail);

        /// <summary>
        /// Create a new library to ship the new created custom object which contains the widgets and groups in writer.
        /// The new library is managed by the LibraryManager.
        /// </summary>
        /// <param name="writer">Container for widgets and groups which will be added in the new custom object.</param>
        /// <param name="objectName">The name of the new created custom object.</param>
        /// <param name="icon">The icon of new created custom object.</param>
        /// <param name="thumbnail">The thumbnail of new created custom object.</param>
        /// <returns></returns>
        ILibrary CreateLibrary(ISerializeWriter writer, string objectName, Stream icon, Stream thumbnail);

        /// <summary>
        /// Load a library form the specified library file path.
        /// </summary>
        /// <param name="fileName">Path of the library file to load.</param>
        /// <returns></returns>
        ILibrary LoadLibrary(string libraryFileName);

        /// <summary>
        /// Delete a library, both from loaded and created collection.
        /// </summary>
        /// <param name="libraryGuid">Guid of the library to delete.</param>
        void DeleteLibrary(Guid libraryGuid);

        /// <summary>
        /// Get a library. Search the library guid in created libraries first.
        /// </summary>
        /// <param name="libraryGuid">Guid of the library to get.</param>
        /// <returns>Return null if cannot find the library with specified guid.</returns>
        ILibrary GetLibrary(Guid libraryGuid);

        /// <summary>
        /// Get a library. Search the library guid in created libraries first.
        /// If cannot find the library with specified guid, it will load the library from the specified path. 
        /// </summary>
        /// <param name="libraryGuid">Guid of the library to get.</param>
        /// <param name="libraryFileName">Path of the library file to load.</param>
        /// <returns></returns>
        ILibrary GetLibrary(Guid libraryGuid, string libraryFileName);

        /// <summary>
        /// Get a copy list which contains the loaded libraries. 
        /// </summary>
        List<ILibrary> LoadedLibraries { get; }

        /// <summary>
        /// Get a copy list which contains the created libraries. 
        /// </summary>
        List<ILibrary> CreatedLibraries { get; }

        /// <summary>
        /// Peek the library guid in the specific library file.
        /// </summary>
        /// <param name="libraryFileName">Path to the library file</param>
        Guid PeekLibraryGuidFromFile(string libraryFileName);

        /// <summary>
        /// Create a new library file destFileName based on the library file sourceFileName. The new library has a new Guid.
        /// </summary>
        /// <param name="sourceFileName">Source library file name.</param>
        /// <param name="destFileName">New library file name.</param>
        void CreateNewLibraryFile(string sourceFileName, string destFileName);
    }
}
