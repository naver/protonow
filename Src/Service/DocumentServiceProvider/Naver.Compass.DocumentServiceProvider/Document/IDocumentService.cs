using System;
using System.Collections.Generic;

namespace Naver.Compass.Service.Document
{
    public interface IDocumentService : IHostService
    {
        /// <summary>
        /// Create an empty document. It could be a standard or library document depends on DocumentType.
        /// </summary>
        void NewDocument(DocumentType type);

        /// <summary>
        /// Opens a document from the specified path.
        /// </summary>
        /// <param name="fileName">Path of the file to load.</param>
        void Open(string fileName);

        /// <summary>
        /// Saves the document to the specified file. The current document is changed to the input file.
        /// </summary>
        /// <param name="fileName">The location of the file where you want to save the document.</param>
        /// <param name="saveCopy">If the file name specify a new file, saveCopy is true means don't generate new document Guid for the new file.</param>
        void Save(string fileName, bool saveCopy = false);

        /// <summary>
        /// Saves a copy of current document to the specified file. The current document is NOT changed to the input file.
        /// </summary>
        /// <param name="fileName">The location of the file where you want to save the document.</param>
        void SaveCopyTo(string fileName);

        /// <summary>
        /// Clear all content in this document.  
        /// </summary>
        void Close();

        /// <summary>
        /// Get the document which is loaded via Load() or new created via New(); 
        /// </summary>
        IDocument Document { get; }

        /// <summary>
        /// Get the library manager.
        /// </summary>
        ILibraryManager LibraryManager { get; }
    }
}
