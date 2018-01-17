using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.ComponentModel;
using System.IO;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Naver.Compass.Common.CommonBase;
using Naver.Compass.Common.Helper;
using System.Xml;
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.AccessControl;
using System.Security.Principal;
using Naver.Compass.InfoStructure;


namespace Security.WinTrust
{
    using System;
    using System.Runtime.InteropServices;

    #region WinTrustData struct field enums
    enum WinTrustDataUIChoice : uint
    {
        All = 1,
        None = 2,
        NoBad = 3,
        NoGood = 4
    }

    enum WinTrustDataRevocationChecks : uint
    {
        None = 0x00000000,
        WholeChain = 0x00000001
    }

    enum WinTrustDataChoice : uint
    {
        File = 1,
        Catalog = 2,
        Blob = 3,
        Signer = 4,
        Certificate = 5
    }

    enum WinTrustDataStateAction : uint
    {
        Ignore = 0x00000000,
        Verify = 0x00000001,
        Close = 0x00000002,
        AutoCache = 0x00000003,
        AutoCacheFlush = 0x00000004
    }

    [FlagsAttribute]
    enum WinTrustDataProvFlags : uint
    {
        UseIe4TrustFlag = 0x00000001,
        NoIe4ChainFlag = 0x00000002,
        NoPolicyUsageFlag = 0x00000004,
        RevocationCheckNone = 0x00000010,
        RevocationCheckEndCert = 0x00000020,
        RevocationCheckChain = 0x00000040,
        RevocationCheckChainExcludeRoot = 0x00000080,
        SaferFlag = 0x00000100,        // Used by software restriction policies. Should not be used.
        HashOnlyFlag = 0x00000200,
        UseDefaultOsverCheck = 0x00000400,
        LifetimeSigningFlag = 0x00000800,
        CacheOnlyUrlRetrieval = 0x00001000,      // affects CRL retrieval and AIA retrieval
        DisableMD2andMD4 = 0x00002000      // Win7 SP1+: Disallows use of MD2 or MD4 in the chain except for the root
    }

    enum WinTrustDataUIContext : uint
    {
        Execute = 0,
        Install = 1
    }
    #endregion

    #region WinTrust structures
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    class WinTrustFileInfo
    {
        UInt32 StructSize = (UInt32)Marshal.SizeOf(typeof(WinTrustFileInfo));
        IntPtr pszFilePath;                     // required, file name to be verified
        IntPtr hFile = IntPtr.Zero;             // optional, open handle to FilePath
        IntPtr pgKnownSubject = IntPtr.Zero;    // optional, subject type if it is known

        public WinTrustFileInfo(String _filePath)
        {
            pszFilePath = Marshal.StringToCoTaskMemAuto(_filePath);
        }
        ~WinTrustFileInfo()
        {
            Marshal.FreeCoTaskMem(pszFilePath);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    class WinTrustData
    {
        UInt32 StructSize = (UInt32)Marshal.SizeOf(typeof(WinTrustData));
        IntPtr PolicyCallbackData = IntPtr.Zero;
        IntPtr SIPClientData = IntPtr.Zero;
        // required: UI choice
        WinTrustDataUIChoice UIChoice = WinTrustDataUIChoice.None;
        // required: certificate revocation check options
        WinTrustDataRevocationChecks RevocationChecks = WinTrustDataRevocationChecks.None;
        // required: which structure is being passed in?
        WinTrustDataChoice UnionChoice = WinTrustDataChoice.File;
        // individual file
        IntPtr FileInfoPtr;
        WinTrustDataStateAction StateAction = WinTrustDataStateAction.Ignore;
        IntPtr StateData = IntPtr.Zero;
        String URLReference = null;
        WinTrustDataProvFlags ProvFlags = WinTrustDataProvFlags.RevocationCheckChainExcludeRoot;
        WinTrustDataUIContext UIContext = WinTrustDataUIContext.Execute;

        // constructor for silent WinTrustDataChoice.File check
        public WinTrustData(String _fileName)
        {
            // On Win7SP1+, don't allow MD2 or MD4 signatures
            if ((Environment.OSVersion.Version.Major > 6) ||
                ((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor > 1)) ||
                ((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1) && !String.IsNullOrEmpty(Environment.OSVersion.ServicePack)))
            {
                ProvFlags |= WinTrustDataProvFlags.DisableMD2andMD4;
            }

            WinTrustFileInfo wtfiData = new WinTrustFileInfo(_fileName);
            FileInfoPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(WinTrustFileInfo)));
            Marshal.StructureToPtr(wtfiData, FileInfoPtr, false);
        }
        ~WinTrustData()
        {
            Marshal.FreeCoTaskMem(FileInfoPtr);
        }
    }
    #endregion

    enum WinVerifyTrustResult : uint
    {
        Success = 0,
        ProviderUnknown = 0x800b0001,           // Trust provider is not recognized on this system
        ActionUnknown = 0x800b0002,         // Trust provider does not support the specified action
        SubjectFormUnknown = 0x800b0003,        // Trust provider does not support the form specified for the subject
        SubjectNotTrusted = 0x800b0004,         // Subject failed the specified verification action
        FileNotSigned = 0x800B0100,         // TRUST_E_NOSIGNATURE - File was not signed
        SubjectExplicitlyDistrusted = 0x800B0111,   // Signer's certificate is in the Untrusted Publishers store
        SignatureOrFileCorrupt = 0x80096010,    // TRUST_E_BAD_DIGEST - file was probably corrupt
        SubjectCertExpired = 0x800B0101,        // CERT_E_EXPIRED - Signer's certificate was expired
        SubjectCertificateRevoked = 0x800B010C,     // CERT_E_REVOKED Subject's certificate was revoked
        UntrustedRoot = 0x800B0109          // CERT_E_UNTRUSTEDROOT - A certification chain processed correctly but terminated in a root certificate that is not trusted by the trust provider.
    }

    sealed class WinTrust
    {
        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        // GUID of the action to perform
        private const string WINTRUST_ACTION_GENERIC_VERIFY_V2 = "{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}";

        [DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
        static extern WinVerifyTrustResult WinVerifyTrust(
            [In] IntPtr hwnd,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID,
            [In] WinTrustData pWVTData
        );

        // call WinTrust.WinVerifyTrust() to check embedded file signature
        public static bool VerifyEmbeddedSignature(string fileName)
        {
            WinTrustData wtd = new WinTrustData(fileName);
            Guid guidAction = new Guid(WINTRUST_ACTION_GENERIC_VERIFY_V2);
            WinVerifyTrustResult result = WinVerifyTrust(INVALID_HANDLE_VALUE, guidAction, wtd);
            bool ret = (result == WinVerifyTrustResult.Success);
            return ret;
        }
        private WinTrust() { }
    }
}

namespace Naver.Compass.Service.Update
{
    class UpdateInfo : IUpdateInfo
    {
        public bool NeedToUpdate { get; internal set; }

        public bool HasError { get; internal set; }
        public string Message { get; internal set; }

        public string TargetVersion { get; internal set; }
        public string DownloadUrl { get; internal set; }
        public string TargetTitle { get; internal set; }
        public string TargetDescription { get; internal set; }

        public string UpdatePackageLocalLocation { get; internal set; }

        public void Clear()
        {
            NeedToUpdate = false;
            HasError = false;

            Message = "";
            TargetVersion = "";
            TargetTitle = "";
            TargetDescription = "";
            DownloadUrl = "";
            UpdatePackageLocalLocation = "";
        }
    }

    public class UpdateService : IUpdateService
    {
        #region Constructor

        public UpdateService()
        {
            _ListEventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            // Create update local working directory.
            _updateWorkingDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Design Studio\Update";
            if (Directory.Exists(_updateWorkingDir) == false)
            {
                Directory.CreateDirectory(_updateWorkingDir);
            }

            _webClient = new WebClient();

            _exitEvent = new ManualResetEvent(false);

            _checkUpdateEvent = new AutoResetEvent(false);
            _checkUpdateThread = new Thread(new ThreadStart(CheckUpdateProc));
            _checkUpdateThread.Start();

            _downloadPackageEvent = new AutoResetEvent(false);
            _downloadPackageThread = new Thread(new ThreadStart(DownloadPackageProc));
            _downloadPackageThread.Priority = ThreadPriority.Lowest;
            _downloadPackageThread.Start();

            // Try to create named system mutex, other application instance was already launched if the specified named system 
            // mutex already exists, do not do auto check update.
            try
            {
                bool createdNew = false;
                _autoCheckUpdateMutex = new Mutex(true, "Naver_Compass_UpdateService_AutoCheckUpdate_Mutex", out createdNew);
                if (createdNew == false)
                {
                    // Another Application already run, 
                    _autoCheckUpdateMutex = null;
                }
            }
            catch
            {
                _autoCheckUpdateMutex = null;
            }
        }

        #endregion

        #region IUpdateService

        public event EventHandler<EventArgs> UpdateProgressChanging;

        public bool IsBusy 
        {
            get { return (_state == UpdateServiceState.UpdateInfoXmlFileDownloading) || (_state == UpdateServiceState.UpdatePackageDownloading); } 
        }

        public bool IsAutoCheckUpdate 
        {
            get { return _isAutoCheckUpdate; }
        }

        public string CurrentVersion 
        {
            get
            {
                RegistryKey compassKey = Registry.LocalMachine.OpenSubKey(@"Software\Design Studio");

                if (compassKey == null)
                {
                    return String.Empty;
                }

                return compassKey.GetValue("CurrentVersion").ToString();
            }
        }

        public bool CheckUpdateAtStart 
        {
            get
            {
                try
                {
                    RegistryKey checkUpdateKey = Registry.CurrentUser.OpenSubKey(@"Software\Design Studio");
                    if (checkUpdateKey != null)
                    {
                        int check = Int32.Parse(checkUpdateKey.GetValue("CheckUpdateAtStart").ToString());
                        if(check == 0)
                        {
                            return false;
                        }
                    }
                }
                catch
                {
                }

                return true;
            }

            set
            {
                try
                {
                    RegistryKey checkUpdateKey = Registry.CurrentUser.CreateSubKey(@"Software\Design Studio");
                    if (checkUpdateKey != null)
                    {
                        if (value)
                        {
                            checkUpdateKey.SetValue("CheckUpdateAtStart", 1);
                        }
                        else
                        {
                            checkUpdateKey.SetValue("CheckUpdateAtStart", 0);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public UpdateServiceState State 
        { 
            get { return _state; } 
        }

        public IUpdateInfo UpdateInfo
        {
            get { return _updateInfo; }
        }

        public double UpdateProcess
        {
            get
            {
                return _updateProgress;
            }
            set
            {
                if (_updateProgress != value)
                {
                    _updateProgress = value;
                    OnUpdateProgressChanging(value);
                }
            }
        }

        public void CheckUpdate(bool isAntoCheck = true)
        {
            // Get update setting from config file, including UpdateInfo.xml file download Url.
            GetUpdateInfoXmlUrlFromConfig();

            if (IsBusy)
            {
                WriteDebugLog("CheckUpdate() - WebClient is busy. Update Service State is {0}. ", _state);
                return;
            }

            WriteDebugLog("CheckUpdate() - _isAutoCheckUpdate is {0}.", _isAutoCheckUpdate);

            _isAutoCheckUpdate = isAntoCheck;
            _updateInfo.Clear();
            _state = UpdateServiceState.Idle;

            // It is auto check update and another application already run, do nothing.
            if (_isAutoCheckUpdate && _autoCheckUpdateMutex == null)
            {
                WriteDebugLog("CheckUpdate() - _isAutoCheckUpdate is true and _autoCheckUpdateMutex is null.");
                return;
            }
            
            _state = UpdateServiceState.UpdateInfoXmlFileDownloading;

            // Download UpdateInfo.xml
            _checkUpdateEvent.Set();
        }

        public void Update()
        {
            if (IsBusy)
            {
                WriteDebugLog("Update() - WebClient is busy. Update Service State is {0}. ", _state);
                return;
            }

            if (String.IsNullOrEmpty(_updateInfo.DownloadUrl))
            {
                WriteDebugLog("Update() - _updateInfo.DownloadUrl is invalid.");
                return;
            }

            if(SomeoneElseDownloadingPackage())
            {
                WriteDebugLog("Update() - Someone else is downloading package.");
                return;
            }

            UpdateProcess = 3;

            _state = UpdateServiceState.UpdatePackageDownloading;

            // Download package
            try
            {
                if (File.Exists(_updateInfo.UpdatePackageLocalLocation))
                {
                    File.Delete(_updateInfo.UpdatePackageLocalLocation);
                }

                _downloadPackageEvent.Set();
            }
            catch(Exception exp)
            {
                UpdateProcess = -1;

                WriteDebugLog("Update() - Exception Raised : {0} .", exp.Message);

                if (_downloadPackageMutex != null)
                {
                    _downloadPackageMutex.Dispose();
                    _downloadPackageMutex = null;
                }

                if (_isAutoCheckUpdate == false)
                {
                    _updateInfo.Clear();
                    _updateInfo.Message = exp.Message;
                    _updateInfo.HasError = true;
                    _ListEventAggregator.GetEvent<UpdateProcessEvent>().Publish(null);
                }
            }
        }

        public void Dispose()
        {
            try
            {
                WriteDebugLog("Dispose()");

                _exitEvent.Set();
                if (_checkUpdateThread.Join(1000) == false)
                {
                    _checkUpdateThread.Abort();
                }

                if (_downloadPackageThread.Join(1000) == false)
                {
                    _downloadPackageThread.Abort();
                }

                _exitEvent.Dispose();
                _exitEvent = null;

                _checkUpdateEvent.Dispose();
                _checkUpdateEvent = null;

                _downloadPackageEvent.Dispose();
                _downloadPackageEvent = null;

                _webClient.Dispose();
                _webClient = null;

                if (_downloadPackageMutex != null)
                {
                    _downloadPackageMutex.Dispose();
                    _downloadPackageMutex = null;
                }

                if (_autoCheckUpdateMutex != null)
                {
                    _autoCheckUpdateMutex.Dispose();
                    _autoCheckUpdateMutex = null;
                }

                if (_log != null)
                {
                    _log.Dispose();
                    _log = null;
                }
            }
            catch
            {
                if (_checkUpdateThread.IsAlive)
                {
                    _checkUpdateThread.Abort();
                }

                if (_downloadPackageThread.IsAlive)
                {
                    _downloadPackageThread.Abort();
                }
            }
        }

        #endregion

        #region Helper

        private void CheckUpdateProc()
        {
            WaitHandle[] handles = new WaitHandle[2];
            handles[0] = _exitEvent;
            handles[1] = _checkUpdateEvent;

            while (true)
            {
                int index = WaitHandle.WaitAny(handles);
                
                if(index == 0)
                {
                    return;
                }

                try
                {
                    WriteDebugLog("CheckUpdateProc() - UpdateInfo.xml Url is {0}.", _updateInfoXmlUrl);
                    WriteDebugLog("CheckUpdateProc() - Start downloading UpdateInfo.xml");

                    SetNetworkCredentials();
                    _webClient.DownloadFile(_updateInfoXmlUrl, _updateWorkingDir + @"\" + UPDATE_INFO_XML_FILE_DEFAULT_NAME);

                    GrantAccess(_updateWorkingDir + @"\" + UPDATE_INFO_XML_FILE_DEFAULT_NAME);

                    DownloadUpdateInfoXmlCompleted();

                    if (_updateInfo.NeedToUpdate)
                    {
                        WriteDebugLog("CheckUpdateProc() - Need to update.");
                    }
                    else
                    {
                        WriteDebugLog("CheckUpdateProc() - No update.");
                    }

                    PublishCheckUpdateCompletedEvent(false, null);
                }
                catch(WebException webExp)
                {
                    WriteDebugLog("CheckUpdateProc() - Exception Raised : {0} .", webExp.Message);

                    _state = UpdateServiceState.Idle;

                    HttpWebResponse response = webExp.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    {
                        WriteDebugLog("CheckUpdateProc() - Not found UpdateInfo.xml, there is no avaliable update.");

                        if (_isAutoCheckUpdate == false)
                        {
                            _updateInfo.NeedToUpdate = false;
                            PublishCheckUpdateCompletedEvent(false, null);
                        }
                    }
                    else
                    {
                        if (_isAutoCheckUpdate == false)
                        {
                            PublishCheckUpdateCompletedEvent(true, webExp);
                        }
                    }
                }
                catch (Exception exp)
                {
                    WriteDebugLog("CheckUpdateProc() - Exception Raised : {0} .", exp.Message);

                    _state = UpdateServiceState.Idle;

                    if (_isAutoCheckUpdate == false)
                    {
                        PublishCheckUpdateCompletedEvent(true, exp);
                    }
                }
            }
        }

        private void DownloadUpdateInfoXmlCompleted()
        {
            WriteDebugLog("DownloadUpdateInfoXmlCompleted() - UpdateInfo.xml downloaded.");

            _state = UpdateServiceState.UpdateInfoXmlFileDownloaded;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( _updateWorkingDir + @"\" + UPDATE_INFO_XML_FILE_DEFAULT_NAME);

            XmlElement documentElement = xmlDoc.DocumentElement;
            XmlElement packageElement = documentElement["UpdatePackage"];
            if (packageElement != null)
            {
                WriteDebugLog("DownloadUpdateInfoXmlCompleted() - packageElement is not null.");

                XmlElement targetVersionElement = packageElement["TargetVersion"];
                XmlElement downloadUrlElement = packageElement["DownloadUrl"];
                XmlElement localPackageNameElement = packageElement["LocalPackageName"];
                XmlElement targetTitleElement = packageElement["TargetTitle"];
                XmlElement targetDescriptionElement = packageElement["TargetDescription"];

                _updateInfo.TargetTitle = "protoNow Update";
                if (targetTitleElement != null && String.IsNullOrEmpty(targetTitleElement.InnerText) == false)
                {
                    _updateInfo.TargetTitle = targetTitleElement.InnerText;
                }

                // Get package name on local machine.
                _updateInfo.UpdatePackageLocalLocation = _updateWorkingDir + @"\" + UPDATE_PACKAGE_FILE_DEFAULT_NAME;
                if (localPackageNameElement != null && String.IsNullOrEmpty(localPackageNameElement.InnerText) == false)
                {
                    _updateInfo.UpdatePackageLocalLocation = _updateWorkingDir + @"\" + localPackageNameElement.InnerText;
                }

                // Get current language description
                if (targetDescriptionElement != null)
                {
                    XmlElement langElement = targetDescriptionElement[GlobalData.Culture];
                    if (langElement == null)
                    {
                        WriteDebugLog("DownloadUpdateInfoXmlCompleted() - langElement is null, set culture to en-US.");
                        langElement = targetDescriptionElement["en-US"];
                    }

                    if (langElement != null)
                    {
                        WriteDebugLog("DownloadUpdateInfoXmlCompleted() - langElement name is {0}.", langElement.Name);

                        if (langElement.FirstChild != null && langElement.FirstChild.NodeType == XmlNodeType.CDATA)
                        {
                            _updateInfo.TargetDescription = langElement.FirstChild.Value;
                        }
                        else
                        {
                            _updateInfo.TargetDescription = langElement.InnerText;
                        }
                    }
                }

                if (targetVersionElement != null && downloadUrlElement != null)
                {
                    _updateInfo.TargetVersion = targetVersionElement.InnerText;
                    _updateInfo.DownloadUrl = downloadUrlElement.InnerText;
                                        
                    _updateInfo.NeedToUpdate = NeedToUpdate();

                    WriteDebugLog("DownloadUpdateInfoXmlCompleted() - TargetVersion is {0}.", _updateInfo.TargetVersion);
                    WriteDebugLog("DownloadUpdateInfoXmlCompleted() - DownloadUrl is {0}.", _updateInfo.DownloadUrl);
                    WriteDebugLog("DownloadUpdateInfoXmlCompleted() - NeedToUpdate is {0}.", _updateInfo.NeedToUpdate);
                }
            }
        }

        private void DownloadPackageProc()
        {
            WaitHandle[] handles = new WaitHandle[2];
            handles[0] = _exitEvent;
            handles[1] = _downloadPackageEvent;

            while (true)
            {
                int index = WaitHandle.WaitAny(handles);

                if (index == 0)
                {
                    return;
                }

                try
                {
                    WriteDebugLog("DownloadPackageProc() - Update package download Url is {0}.", _updateInfo.DownloadUrl);
                    WriteDebugLog("DownloadPackageProc() - Start downloading update package.");

                    SetNetworkCredentials();

                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        while (_state == UpdateServiceState.UpdatePackageDownloading && _updateProgress < 80)
                        {
                            UpdateProcess = _updateProgress + 5;
                            Thread.Sleep(1500);
                        }
                    });

                    _webClient.DownloadFile(_updateInfo.DownloadUrl, _updateInfo.UpdatePackageLocalLocation);

                    GrantAccess(_updateInfo.UpdatePackageLocalLocation);

                    if (_downloadPackageMutex != null)
                    {
                        _downloadPackageMutex.Dispose();
                        _downloadPackageMutex = null;                       
                    }                   

                    _state = UpdateServiceState.UpdatePackageDownloaded;

                    UpdateProcess = 100;

                    // RunUpdatePackage();
                }
                catch (Exception exp)
                {
                    UpdateProcess = -1;
                    WriteDebugLog("DownloadPackageProc() - Exception Raised : {0} .", exp.Message);

                    if (_downloadPackageMutex != null)
                    {
                        _downloadPackageMutex.Dispose();
                        _downloadPackageMutex = null;
                    }

                    _state = UpdateServiceState.Idle;

                    if (_isAutoCheckUpdate == false)
                    {
                        _updateInfo.Clear();
                        _updateInfo.Message = exp.Message;
                        _updateInfo.HasError = true;
                        _ListEventAggregator.GetEvent<UpdateProcessEvent>().Publish(null);
                    }
                }
            }
        }

        public void RunUpdatePackage()
        {
            if (File.Exists( _updateInfo.UpdatePackageLocalLocation))
            {
                WriteDebugLog("RunUpdatePackage() - Launch the update package {0} .", _updateInfo.UpdatePackageLocalLocation);

                try
                {
                    bool valid = Security.WinTrust.WinTrust.VerifyEmbeddedSignature(_updateInfo.UpdatePackageLocalLocation);
                    if(valid==false)
                    {
                        _state = UpdateServiceState.Idle;
                        this.UpdateProcess = -1;
                        WriteDebugLog("RunUpdatePackage() - Risk Raised: the illegal install package");
                        File.Delete(_updateInfo.UpdatePackageLocalLocation);
                        return;
                    }

                    X509Certificate cert = X509Certificate.CreateFromSignedFile(_updateInfo.UpdatePackageLocalLocation);
                    //var cert2 = new X509Certificate2(cert.Handle);
                    //bool valid = cert2.Verify();
                    if(cert.Subject.StartsWith("CN=NHN Technology Services Corp,") ==false)
                    {
                        _state = UpdateServiceState.Idle;
                        this.UpdateProcess = -1;
                        WriteDebugLog("RunUpdatePackage() - Risk Raised: the publisher is not correct");
                        File.Delete(_updateInfo.UpdatePackageLocalLocation);
                        return;
                    }


                    Process newProcess = new Process();
                    newProcess.StartInfo.FileName = _updateInfo.UpdatePackageLocalLocation;
                    newProcess.Start();
                }
                catch(Exception exp)
                {
                    _state = UpdateServiceState.Idle;
                    this.UpdateProcess = -1;
                    WriteDebugLog("RunUpdatePackage() - Exception Raised : {0} .", exp.Message);
                }

                _state = UpdateServiceState.UpdatePackageLaunched;
            }
        }

        private void GetUpdateInfoXmlUrlFromConfig()
        {
            _debug = false;
            _updateInfoXmlUrl = UPDATE_INFO_XML_DEFAULT_URL;
            _userName = UPDATE_DOWNLOAD_DEFAULT_USER_NAME;
            _password = UPDATE_DOWNLOAD_DEFAULT_USER_PASSWORD;

            string configFile = _updateWorkingDir + @"\" + UPDATE_CONFIG_FILE_DEFAULT_NAME;
            if (File.Exists(configFile))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(configFile);

                    XmlElement configElement = xmlDoc.DocumentElement;
                    if (configElement != null)
                    {
                        XmlElement updateInfoXmlUrlElement = configElement["UpdateInfoXmlUrl"];
                        if (updateInfoXmlUrlElement != null)
                        {
                            if (String.IsNullOrEmpty(updateInfoXmlUrlElement.InnerText) == false)
                            {
                                _updateInfoXmlUrl = updateInfoXmlUrlElement.InnerText;
                            }
                        }

                        XmlElement debugElement = configElement["Debug"];
                        if (debugElement != null)
                        {
                            _debug = true;
                        }

                        XmlElement userNameElement = configElement["UserName"];
                        if (userNameElement != null)
                        {
                            _userName = userNameElement.InnerText;
                        }

                        XmlElement passwordElement = configElement["Password"];
                        if (passwordElement != null)
                        {
                            _password = passwordElement.InnerText;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private bool SomeoneElseDownloadingPackage()
        {
            try
            {
                bool createdNew = false;
                _downloadPackageMutex = new Mutex(true, "Naver_Compass_UpdateService_DownloadPackage_Mutex", out createdNew);
                if (createdNew == false)
                {
                    // Another Application already downloading package, do nothing.
                    _downloadPackageMutex = null;
                    return true;
                }
            }
            catch(Exception exp)
            {
                WriteDebugLog("SomeoneElseDownloadingPackage() - Exception Raised : {0} .", exp.Message);
            }

            return false;
        }

        private bool NeedToUpdate()
        {
            try
            {
                WriteDebugLog("NeedToUpdate() - Current version is {0}, target version is {1} .", CurrentVersion, _updateInfo.TargetVersion);

                /*
                 * The version parameter can contain only the components major, minor, build, and revision, in that order, 
                 * and all separated by periods. There must be at least two components, and at most four. 
                 * The first two components are assumed to be major and minor. The value of unspecified components is undefined.
                 * The format of the version number is as follows. Optional components are shown in square brackets ('[' and ']'):
                 * major.minor[.build[.revision]]
                 * */

                Version current = new Version(CurrentVersion);
                Version target = new Version(_updateInfo.TargetVersion);

                if (current.CompareTo(target) < 0)
                {
                    return true;
                }
            }
            catch(Exception exp)
            {
                WriteDebugLog("NeedToUpdate() - Exception Raised : {0} .", exp.Message);
            }
            
            return false;
        }

        private void WriteDebugLog(string format, params object[] args)
        {
            try
            {
                if (_debug)
                {
                    if (_log == null)
                    {
                        _log = new Log(_updateWorkingDir + @"\" + UPDATE_LOG_FILE_DEFAULT_NAME);
                    }

                    _log.LogTrace(format, args);
                    _log.Flush();
                }
                else
                {
                    if (_log != null)
                    {
                        _log.Dispose();
                        _log = null;
                    }
                }
            }
            catch
            {
            }
        }

        private void SetNetworkCredentials()
        {
            if (_webClient == null)
            {
                return;
            }

            if (String.IsNullOrEmpty(_userName) || String.IsNullOrEmpty(_password))
            {
                _webClient.Credentials = CredentialCache.DefaultCredentials;
            }
            else
            {
                try
                {
                    _webClient.Credentials = new NetworkCredential(_userName, _password);
                }
                catch (Exception exp)
                {
                    WriteDebugLog("SetNetworkCredentials() - Exception Raised : {0} .", exp.Message);
                }
            }
        }

        private void PublishCheckUpdateCompletedEvent(bool hasError, Exception exp)
        {
            if (hasError)
            {
                _updateInfo.Clear();
                _updateInfo.HasError = true;
                if(exp != null)
                {
                    _updateInfo.Message = exp.Message;
                }
            }
            else
            {
                _updateInfo.HasError = false;
            }

            _ListEventAggregator.GetEvent<CheckUpdateCompletedEvent>().Publish(null);
        }

        private bool GrantAccess(string fullPath)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(fullPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
            }
            catch
            {
                return false;
            }
            return true;
        }


        private void OnUpdateProgressChanging(double progress)
        {
            if (UpdateProgressChanging != null)
            {
                UpdateProgressChanging(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Private

        private IEventAggregator _ListEventAggregator;
        private string _updateWorkingDir;
        
        private Mutex _autoCheckUpdateMutex;
        private Mutex _downloadPackageMutex;

        private UpdateServiceState _state = UpdateServiceState.Idle;
        private UpdateInfo _updateInfo = new UpdateInfo();
        private bool _isAutoCheckUpdate = true;

        private ManualResetEvent _exitEvent;

        private Thread _checkUpdateThread;
        private AutoResetEvent _checkUpdateEvent;
        private Thread _downloadPackageThread;
        private AutoResetEvent _downloadPackageEvent;
        private WebClient _webClient;

        private bool _debug;
        private Log _log;
        private string _updateInfoXmlUrl = UPDATE_INFO_XML_DEFAULT_URL;

        private string _userName;
        private string _password;

        private double _updateProgress;

        #endregion

        #region Constants

        //configure your own url
        private static readonly string UPDATE_INFO_XML_DEFAULT_URL = @"http://XXX/UpdateInfo.xml";
        private static readonly string UPDATE_INFO_XML_FILE_DEFAULT_NAME = "UpdateInfo.xml";
        private static readonly string UPDATE_PACKAGE_FILE_DEFAULT_NAME = "UpdatePackage.exe";
        private static readonly string UPDATE_CONFIG_FILE_DEFAULT_NAME = "Update.config";
        private static readonly string UPDATE_LOG_FILE_DEFAULT_NAME = "Update.log";

        private static readonly string UPDATE_DOWNLOAD_DEFAULT_USER_NAME = "";
        private static readonly string UPDATE_DOWNLOAD_DEFAULT_USER_PASSWORD = "";

        #endregion

    }
}
