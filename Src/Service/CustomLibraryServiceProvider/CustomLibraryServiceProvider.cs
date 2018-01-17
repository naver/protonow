using Naver.Compass.InfoStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.CustomLibrary
{
    public class CustomLibraryServiceProvider : ICustomLibraryService
    {
        #region Constructor
        //private List<ICustomLibrary> _myLibraries;
        public CustomLibraryServiceProvider()
        {
            _myLibraries = new List<ICustomLibrary>(); 
        }
        #endregion

        private List<ICustomLibrary> _myLibraries;
        private IMyLibrary _myLibrary;


        public void RegisterLibraies(IEnumerable<ICustomLibrary> MyLibraries)
        {
            if(MyLibraries==null)
            { return; }

            _myLibraries.Clear();
            foreach(ICustomLibrary it in MyLibraries)
            {
                if(it.IsCustomWidget==true)
                {
                    _myLibraries.Add(it);
                }
            }
        }
        public IEnumerable<ICustomLibrary> GetAllCustomLibraies()
        {
            return _myLibraries;
        }


        public void RegisterMyLibrary(IMyLibrary myLibrary)
        {
            _myLibrary = myLibrary;
        }

        public IMyLibrary GetMyLibrary()
        {
            return _myLibrary;
        }
    }
}
