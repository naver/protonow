using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using Naver.Compass.Service.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Naver.Compass.Module.Model
{
    public class AutoSaveService
    {
        private static readonly AutoSaveService instance = new AutoSaveService();
        public readonly string Tmp;

        public static AutoSaveService Instance
        {
            get { return AutoSaveService.instance; }
        }

        private const string MutexName = "AutosaveMutex";
        private readonly Mutex SyncNamed;
        private readonly string RecoveryFileXmlPath;
        private bool _isAutoSaveEnable;
        private int _autoSaveTick;
        private Timer _timer;
        public RecoveryInfo RecoveryInfo { get; set; }
        private string _processId;
        private bool _needShowRecoveryWindow;
        public bool NeedShowRecoveryWindow
        {
            get
            {
                if (_needShowRecoveryWindow)
                {
                    _needShowRecoveryWindow = false;
                    return true;
                }

                return false;
            }
        }

        private bool _isEnable;
        private AutoSaveService()
        {
            NLogger.Info("Constrctor.");
            var mydocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
            var location = Path.Combine(mydocument, "protoNow");

            if (GlobalData.AutoSaveFileLocation != location)
            {
                try
                {
                    if (!Directory.Exists(location))
                    {
                        NLogger.Info("Make folder [{0}] for recovery files", location);
                        Directory.CreateDirectory(location);
                    }

                    GlobalData.AutoSaveFileLocation = location;
                }
                catch
                {
                }
            }

            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData, Environment.SpecialFolderOption.Create);
            var autoSaveSetting = Path.Combine(programdata, @"Design Studio\AutoSaveSetting");
            if (!Directory.Exists(autoSaveSetting))
            {
                NLogger.Info("Make folder [{0}] for AutoSaveSetting", autoSaveSetting);
                Directory.CreateDirectory(autoSaveSetting);
            }

            RecoveryFileXmlPath = Path.Combine(autoSaveSetting, "RecoveryFilesInfo.xml");
            try
            {
                SyncNamed = Mutex.OpenExisting(MutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                SyncNamed = new Mutex(false, MutexName);
            }

            this.LoadRecoveryFiles();

            Tmp = Path.Combine(programdata, @"Design Studio\AutoSaveSetting\tmp");
            _processId = Process.GetCurrentProcess().Id.ToString();
            if (RecoveryInfo.DocsClosedUnGracefully.Count > 0)
            {
                NLogger.Info("DocsClosedUnGracefully.Count greater than zero.Application was not closed gracefully.Try to show recovery list window.");
                RecoveryInfo.DocsClosedUnGracefully.Clear();
                _needShowRecoveryWindow = true;
            }

            if (!RecoveryInfo.DocsClosedUnGracefully.Contains(_processId))
            {
                NLogger.Info("Add current process Id {0} to DocsClosedUnGracefully.", _processId);
                RecoveryInfo.DocsClosedUnGracefully.Add(_processId);
            }

            SaveRecoveryFiles();
            CheckRecoveryFileExist();
        }

        public void OpenFile(IDocumentService doc)
        {
            if (!_isEnable) return;
            //if (RecoveryInfo.DocsClosedUnGracefully.Count > 0 && NeedShowRecoveryWindow)
            //{
            //    var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            //    _ListEventAggregator.GetEvent<RecoveryDocumentOpenEvent>().Publish(null);
            //    RecoveryInfo.DocsClosedUnGracefully.Clear();
            //}
        }

        public void CloseFile()
        {
            if (!_isEnable) return;
            NLogger.Info("Close file by shuting down application.");
            this.LoadRecoveryFiles();
            if (RecoveryInfo.DocsClosedUnGracefully.Contains(_processId))
            {
                RecoveryInfo.DocsClosedUnGracefully.Remove(_processId);
                NLogger.Info("Remove current process Id {0} from DocsClosedUnGracefully.", _processId);
            }

            SaveRecoveryFiles();
            try
            {
                if (Directory.Exists(Tmp))
                {
                    NLogger.Info("Try to clear tmp folder.");
                    Directory.Delete(Tmp, true);
                    NLogger.Info("Try to clear tmp folder successfully.");
                }
            }
            catch
            {
            }

            CloseFile(ServiceLocator.Current.GetInstance<IDocumentService>());
        }

        public void CloseFile(IDocumentService doc)
        {
            if (!_isEnable) return;
            NLogger.Info("Closing file.");
            this.LoadRecoveryFiles();
            if (doc != null && doc.Document != null && !doc.Document.IsDirty)
            {
                NLogger.Info("The current document isn't dirty.It has been saved before.Try to remove relevant recovery file.");
                var existFile = RecoveryInfo.RecoveryFiles.FirstOrDefault(x => x.Guid == doc.Document.Guid);
                if (existFile != null)
                {
                    NLogger.Info("Relevant recovery file is exist.");
                    RecoveryInfo.RecoveryFiles.Remove(existFile);
                    try
                    {
                        File.Delete(existFile.GetFullPath());
                        NLogger.Info("Remove Relevant recovery file successfully.");
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn("Remove Relevant recovery file failed.ex:{0}", ex.ToString());
                    }

                    SaveRecoveryFiles();
                }
            }
        }

        public void PerformSetting()
        {
            _isEnable = true;
            if (_isAutoSaveEnable != GlobalData.IsAutoSaveEnable
                || _autoSaveTick != GlobalData.AutoSaveTick)
            {
                NLogger.Info("IsAutoSaveEnable [{0}] or AutoSaveTick [{1}] is changed.", GlobalData.IsAutoSaveEnable, GlobalData.AutoSaveTick);
                _isAutoSaveEnable = GlobalData.IsAutoSaveEnable;
                _autoSaveTick = GlobalData.AutoSaveTick;

                if (_timer != null)
                {
                    _timer.Dispose();
                }

                if (_isAutoSaveEnable)
                {
                    _timer = new Timer(TimerTick, null, _autoSaveTick * 60 * 1000, _autoSaveTick * 60 * 1000);
                    //_timer = new Timer(TimerTick, null, 10000, 10000);
                    NLogger.Info("Set timer for tick {0}", _autoSaveTick);
                }
                else
                {
                    NLogger.Info("Disable auto save and remove timer.");
                }
            }
        }

        private void TimerTick(object obj)
        {
            NLogger.Info("Timer ticked.Try to create recovery file for AutoSave");
            CreateRecoveryFile(RecoveryType.AutoSave);
        }

        public void ManualSave()
        {
            if (!_isEnable) return;
            NLogger.Info("User save document by manual.Try to create recovery file for UserSave");
            CreateRecoveryFile(RecoveryType.UserSave);
        }

        private void LoadRecoveryFiles()
        {
            NLogger.Info("Try to load RecoveryFilesInfo.");
            if (File.Exists(RecoveryFileXmlPath))
            {
                try
                {
                    SyncNamed.WaitOne();
                    using (var rdr = new StreamReader(RecoveryFileXmlPath))
                    {
                        var serializer = new XmlSerializer(typeof(RecoveryInfo));
                        RecoveryInfo = (RecoveryInfo)serializer.Deserialize(rdr);
                        NLogger.Info("Load RecoveryFilesInfo successfully.");
                    }

                    SyncNamed.ReleaseMutex();
                }
                catch
                {
                    RecoveryInfo = new RecoveryInfo();
                    NLogger.Info("Load RecoveryFilesInfo failed.Create new setting file.");
                }
            }
            else
            {
                RecoveryInfo = new RecoveryInfo();
                NLogger.Info("RecoveryFilesInfo not exist.Create new setting file.");
            }
        }

        private void SaveRecoveryFiles()
        {
            try
            {
                SyncNamed.WaitOne();
                using (var rdr = new StreamWriter(RecoveryFileXmlPath))
                {

                    var serializer = new XmlSerializer(typeof(RecoveryInfo));
                    serializer.Serialize(rdr, RecoveryInfo);
                    NLogger.Info("Save RecoveryInfo successfully.");
                }

                SyncNamed.ReleaseMutex();
            }
            catch (Exception ex)
            {
                NLogger.Warn("Save RecoveryInfo failed.{0}", ex.ToString());
            }
        }

        public void CloseWithoutSave()
        {
            if (!_isEnable) return;
            NLogger.Info("User closed a document without saving before.");
            this.LoadRecoveryFiles();
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (!GlobalData.IsKeepLastAutoSaved && doc.Document != null)
            {
                NLogger.Info("IsKeepLastAutoSaved is false.Try to remove last recovery file.Guid is {0}", doc.Document.Guid);
                var existFile = RecoveryInfo.RecoveryFiles.FirstOrDefault(x => x.Guid == doc.Document.Guid);
                if (existFile != null)
                {
                    NLogger.Info("Last recovery file is exist, remove it from settings.");
                    RecoveryInfo.RecoveryFiles.Remove(existFile);
                    try
                    {
                        File.Delete(existFile.GetFullPath());
                        NLogger.Info("Remove last recovery file successfully.");
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn("Remove last recovery file failed.{0}", ex.ToString());
                    }

                    SaveRecoveryFiles();
                }
            }
        }

        private void CreateRecoveryFile(RecoveryType type)
        {
            IDocumentService doc = ServiceLocator.Current.GetInstance<IDocumentService>();
            if (doc.Document != null && (type == RecoveryType.UserSave || doc.Document.IsDirty))
            {
                NLogger.Info("Try to create recovery file for document with guid {0}", doc.Document.Guid);
                lock (doc.Document)
                {
                    ///load recoveryinfo.
                    this.LoadRecoveryFiles();
                    var existFile = RecoveryInfo.RecoveryFiles.FirstOrDefault(x => x.Guid == doc.Document.Guid);
                    var duplicateIndex = 0;
                    var title = string.IsNullOrEmpty(doc.Document.Title)
                        ? CommonDefine.Untitled
                        : doc.Document.Title;

                    if (existFile != null)
                    {
                        NLogger.Info("Last recovery file is exist, remove it from settings.");
                        duplicateIndex = existFile.DuplicateNameIndex;
                        RecoveryInfo.RecoveryFiles.Remove(existFile);
                        try
                        {
                            File.Delete(existFile.GetFullPath());
                            NLogger.Info("Remove last recovery file successfully.");
                        }
                        catch (Exception ex)
                        {
                            NLogger.Warn("Remove last recovery file failed.{0}", ex.ToString());
                        }
                    }
                    else
                    {
                        var topDuplicateIndex = RecoveryInfo.RecoveryFiles
                            .Where(x => x.Filename == title)
                            .OrderByDescending(x => x.DuplicateNameIndex)
                            .FirstOrDefault();
                        if (topDuplicateIndex != null)
                        {
                            duplicateIndex = topDuplicateIndex.DuplicateNameIndex + 1;
                        }
                    }

                    var recoveryFile = new RecoveryFile
                    {
                        Guid = doc.Document.Guid,
                        CreateTime = DateTime.Now,
                        Type = type,
                        Filename = title,
                        DuplicateNameIndex = duplicateIndex,
                        Location = GlobalData.AutoSaveFileLocation,
                        FileType = (doc.Document.DocumentType == DocumentType.Library) ? ".libpn" : ".pn"
                    };

                    NLogger.Info("Make a new instance for Type RecoveryFile. Guid:{0} Type :{1} Filename:{2} Location:{3}",
                        doc.Document.Guid,
                        type,
                        title,
                        GlobalData.AutoSaveFileLocation);
                    RecoveryInfo.RecoveryFiles.Add(recoveryFile);
                    NLogger.Info("Add the new instance to RecoveryFiles.And try to save recovery files setting.");
                    SaveRecoveryFiles();
                    var fname = Path.Combine(GlobalData.AutoSaveFileLocation, recoveryFile.FullFilename);
                    try
                    {
                        doc.SaveCopyTo(fname);
                        NLogger.Info("SaveCopyTo {0} successfully.", fname);
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn("SaveCopyTo {0} failed.ex:{1}", fname, ex.ToString());
                    }
                }
            }
        }

        public void FileSaveAs(Guid beforeGuid, Guid afterGuid)
        {
            if (!_isEnable) return;
            NLogger.Info("Try to saveas.beforeGuid:{0} afterGuid:{1}", beforeGuid, afterGuid);
            var existFile = RecoveryInfo.RecoveryFiles.FirstOrDefault(x => x.Guid == beforeGuid);
            if (existFile != null)
            {
                NLogger.Info("Recovery file with before guid {0} is exist.Try to remove it.", beforeGuid);
                RecoveryInfo.RecoveryFiles.Remove(existFile);
                try
                {
                    File.Delete(existFile.GetFullPath());
                    NLogger.Info("Remove recovery file with beforeguid successfully.");
                }
                catch (Exception ex)
                {
                    NLogger.Info("Remove recovery file with beforeguid failed.ex:{0}", ex.ToString());
                }
            }

            ManualSave();
        }

        public void CheckRecoveryFileExist()
        {
            if (!_isEnable) return;
            this.LoadRecoveryFiles();
            if (RecoveryInfo != null && RecoveryInfo.RecoveryFiles != null)
            {
                try
                {
                    NLogger.Info("Check files in recovery setting exist.");
                    var recoveryfiles = RecoveryInfo.RecoveryFiles.Where(x => File.Exists(x.GetFullPath())).ToList();
                    RecoveryInfo.RecoveryFiles = recoveryfiles;
                    this.SaveRecoveryFiles();
                }
                catch (Exception ex)
                {
                    NLogger.Warn("CheckRecoveryFileExist failed.{0}", ex.ToString());
                }
            }
        }

        /// <summary>
        /// Check if need to show recovery window.
        /// </summary>
        /// <param name="islibrary"></param>
        /// <returns>show recovery window if true</returns>
        public bool AfterSplashScreen()
        {
            if (!_isEnable) return false;
            //Show welcome screen after check update
            var needShowRecoveryWindow = Naver.Compass.Module.Model.AutoSaveService.Instance.NeedShowRecoveryWindow;
            var recoveryFileCount = Naver.Compass.Module.Model.AutoSaveService.Instance.RecoveryInfo.RecoveryFiles.Count;
            NLogger.Debug("CheckUpdateCompletedHandler fired. needShowRecoveryWindow:{0},recoveryFileCount:{1}", needShowRecoveryWindow, recoveryFileCount);
            var _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            if (needShowRecoveryWindow && recoveryFileCount > 0)
            {
                NLogger.Debug("Publish RecoveryDocumentOpenEvent ");
                //_ListEventAggregator.GetEvent<RecoveryDocumentOpenEvent>().Publish(null);
                return true;
            }
            return false;
        }
    }
}
