using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Update
{
    public enum UpdateServiceState
    {
        Idle,
        UpdateInfoXmlFileDownloading,
        UpdateInfoXmlFileDownloaded,
        UpdatePackageDownloading,
        UpdatePackageDownloaded,
        UpdatePackageLaunched
    }

    public interface IUpdateInfo
    {
        bool NeedToUpdate { get; }

        bool HasError { get; }
        string Message { get; }

        string TargetVersion { get; }
        string DownloadUrl { get; }
        string TargetTitle { get; }
        string TargetDescription { get; }

        string UpdatePackageLocalLocation { get; }
    }

    public interface IUpdateService : IDisposable
    {
        bool IsBusy { get; }

        bool IsAutoCheckUpdate { get; }

        string CurrentVersion { get; }

        bool CheckUpdateAtStart { get; set; }

        UpdateServiceState State { get; }

        IUpdateInfo UpdateInfo { get; }

        void CheckUpdate(bool isAutoCheck = true);

        void Update();

        void RunUpdatePackage();

        double UpdateProcess { get; set; }

        event EventHandler<EventArgs> UpdateProgressChanging;
    }
}
