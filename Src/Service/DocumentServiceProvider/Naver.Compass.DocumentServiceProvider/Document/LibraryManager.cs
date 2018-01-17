using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Naver.Compass.Service.Document
{
    internal class LibraryManager : ILibraryManager
    {
        public void Dispose()
        {
            lock (this)
            {
                foreach (Document library in _createdLibraries.Values)
                {
                    library.Close();
                }

                foreach (Document library in _loadedLibraries.Values)
                {
                    library.Close();
                }
            }
        }

        public ILibrary NewLibrary(ISerializeWriter writer, string objectName, Stream icon, Stream thumbnail)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            Document newLibrary = new Document(DocumentType.Library);
            CreateCustomObject(newLibrary, writer, objectName, icon, thumbnail);
            return newLibrary;
        }

        public ILibrary CreateLibrary(ISerializeWriter writer, string objectName, Stream icon, Stream thumbnail)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            Document newLibrary = new Document(DocumentType.Library);
            CreateCustomObject(newLibrary, writer, objectName, icon, thumbnail);

            lock (this)
            {
                if (_createdLibraries.Count >= MAX_CACHE_COUNT)
                {
                    // If list is over the max limitaion, remove the LRU library.
                    Guid LRUGuid = _createdLibraries.Aggregate((l, r) => l.Value.LastAccessTime < r.Value.LastAccessTime ? l : r).Key;
                    DeleteLibrary(LRUGuid);
                }

                // Add to created libraries collection.
                _createdLibraries.Add(newLibrary.Guid, newLibrary);
            }

            return newLibrary;
        }

        public ILibrary LoadLibrary(string libraryFileName)
        {
            lock (this)
            {
                Document library = new Document(DocumentType.Library);
                library.Open(libraryFileName);

                if (library.DocumentType != DocumentType.Library)
                {
                    library.Close();
                    throw new CannotLoadLibraryException(libraryFileName, "The specified file is not a library document.");
                }

                if (!_loadedLibraries.ContainsKey(library.Guid))
                {
                    if (_loadedLibraries.Count >= MAX_CACHE_COUNT)
                    {
                        // If list is over the max limitaion, remove the LRU library.
                        Guid LRUGuid = _loadedLibraries.Aggregate((l, r) => l.Value.LastAccessTime < r.Value.LastAccessTime ? l : r).Key;
                        DeleteLibrary(LRUGuid);
                    }

                    _loadedLibraries[library.Guid] = library;
                }
                else
                {
                    // Delete the old one and refresh to the new one.
                    DeleteLibrary(library.Guid);

                    _loadedLibraries[library.Guid] = library;
                }

                library.UpdateLastAccessTime();

                return _loadedLibraries[library.Guid];
            }
        }

        public void DeleteLibrary(Guid libraryGuid)
        {
            lock (this)
            {
                if (_createdLibraries.ContainsKey(libraryGuid))
                {
                    Document library = _createdLibraries[libraryGuid];
                    if (library != null)
                    {
                        library.Close();
                    }

                    _createdLibraries.Remove(libraryGuid);
                }

                if (_loadedLibraries.ContainsKey(libraryGuid))
                {
                    Document library = _loadedLibraries[libraryGuid];
                    if (library != null)
                    {
                        library.Close();
                    }

                    _loadedLibraries.Remove(libraryGuid);
                }
            }
        }

        public ILibrary GetLibrary(Guid libraryGuid)
        {
            lock (this)
            {
                if (_createdLibraries.ContainsKey(libraryGuid))
                {
                    return _createdLibraries[libraryGuid];
                }
                else if (_loadedLibraries.ContainsKey(libraryGuid))
                {
                    return _loadedLibraries[libraryGuid];
                }
                else
                {
                    return null;
                }
            }
        }

        public ILibrary GetLibrary(Guid libraryGuid, string libraryFileName)
        {
            lock (this)
            {
                ILibrary library = GetLibrary(libraryGuid);

                // If cannot get library from cache collection, load it from file.
                if (library == null && File.Exists(libraryFileName))
                {
                    library = LoadLibrary(libraryFileName);
                }

                return library;
            }
        }

        public List<ILibrary> LoadedLibraries
        {
            get
            {
                lock (this)
                {
                    return _loadedLibraries.Values.ToList<ILibrary>();
                }
            }
        }

        public List<ILibrary> CreatedLibraries
        {
            get
            {
                lock (this)
                {
                    return _createdLibraries.Values.ToList<ILibrary>();
                }
            }
        }

        public Guid PeekLibraryGuidFromFile(string libraryFileName)
        {
            if(File.Exists(libraryFileName))
            {
                DirectoryInfo info = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Document.TEMP_FOLDER_NAME));
                if (!info.Exists)
                {
                    info.Create();
                }

                DirectoryInfo workingDirectory = info.CreateSubdirectory(Guid.NewGuid().ToString());

                // Only extract Document.xml.
                /*
                 * <example>The following expression includes all name ending in '.dat' with the exception of 'dummy.dat'
                 * "+\.dat$;-^dummy\.dat$"
                 * </example>
                 * */
                _fastZip.ExtractZip(libraryFileName, workingDirectory.FullName, @"+^" + Document.DOCUMENT_FILE_NAME + @"$");

                string documentXml = Path.Combine(workingDirectory.FullName, Document.DOCUMENT_FILE_NAME);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(documentXml);
                Guid guid = new Guid(xmlDoc.DocumentElement.GetAttribute("Guid"));;
                try
                {
                    workingDirectory.Delete(true);
                }
                catch(Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }

                return guid;
            }

            return Guid.Empty;
        }

        public void CreateNewLibraryFile(string sourceFileName, string destFileName)
        {
            if (File.Exists(sourceFileName))
            {
                DirectoryInfo info = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Document.TEMP_FOLDER_NAME));
                if (!info.Exists)
                {
                    info.Create();
                }

                DirectoryInfo workingDirectory = info.CreateSubdirectory(Guid.NewGuid().ToString());

                _fastZip.ExtractZip(sourceFileName, workingDirectory.FullName, "");

                string documentXml = Path.Combine(workingDirectory.FullName, Document.DOCUMENT_FILE_NAME);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(documentXml);

                Guid oldGuid = new Guid(xmlDoc.DocumentElement.GetAttribute("Guid"));
                Guid newGuid = Guid.NewGuid();
                
                xmlDoc.DocumentElement.SetAttribute("Guid", newGuid.ToString());
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Save(documentXml);

                _fastZip.CreateZip(destFileName, workingDirectory.FullName, true, "", "");

                try
                {
                    workingDirectory.Delete(true);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }
            }
        }

        internal static CustomObjectPage CreateCustomObject(Document document, ISerializeWriter writer,
                                                    string objectName, Stream icon, Stream thumbnail)
        {
            CustomObjectPage objectPage = document.CreatePage(objectName) as CustomObjectPage;

            // Create a new node in the page tree.
            ITreeNode node = document.DocumentSettings.LayoutSetting.PageTree.AddChild(TreeNodeType.Page);
            node.AttachedObject = objectPage;

            objectPage.Open();

            // Add to base view.
            PageView baseView = objectPage.PageViews[document.AdaptiveViewSet.Base.Guid] as PageView;
            IObjectContainer container = baseView.AddObjects(writer.WriteToStream());

            // Adjust widgets position from coordinate (0,0);
            baseView.PageViewStyle.X = 0;
            baseView.PageViewStyle.Y = 0;

            if (icon != null)
            {
                objectPage.Icon = icon;
            }

            if (thumbnail != null)
            {
                objectPage.Thumbnail = thumbnail;
            }

            // Page must be closed after setting icon and thumbnail, so that they can be saved into the file.
            // Document.save() only call save() on opened pages.
            objectPage.Close(); 

            document.UpdateLastAccessTime();

            return objectPage;
        }


        private FastZip _fastZip = new FastZip();
        private Dictionary<Guid, Document> _createdLibraries = new Dictionary<Guid, Document>();
        private Dictionary<Guid, Document> _loadedLibraries = new Dictionary<Guid, Document>();
        private readonly int MAX_CACHE_COUNT = 10;
    }
}
