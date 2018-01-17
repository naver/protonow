using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IInteractiveObject
    {
        IPage ContextPage { get; }

        /*
         * Events hierarchy :
         * 
         * Events -> Event -> Cases -> Case +-> Actions -> Action
         *                                  +-> Conditions -> Condition 
         * */
        IInteractionEvents Events { get; }
    }
}
