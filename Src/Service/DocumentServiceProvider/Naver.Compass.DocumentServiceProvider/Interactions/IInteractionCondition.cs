using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    public interface IInteractionCondition : IUniqueObject
    {
        IInteractionCase InteractionCase { get; }

        string Description { get; }

        bool IsTrue { get; }
    }
}
