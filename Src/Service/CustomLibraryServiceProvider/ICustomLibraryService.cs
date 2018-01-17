using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.CustomLibrary
{
    public interface ICustomLibraryService
    {
        void RegisterLibraies(IEnumerable<ICustomLibrary> MyLibraries);
        void RegisterMyLibrary(IMyLibrary myLibrary);
        IEnumerable<ICustomLibrary> GetAllCustomLibraies();        
        IMyLibrary GetMyLibrary();
    }
}
