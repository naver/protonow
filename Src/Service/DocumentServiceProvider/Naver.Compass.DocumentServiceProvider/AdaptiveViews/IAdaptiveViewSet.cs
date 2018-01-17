using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.Service.Document
{
    /*
     * Set for adaptive views. The adaptive views in this set are in a tree structure, 
     * the root of the tree is Base.
     * Base always exists with set and cannot remove it or change its parent.
     * */
    public interface IAdaptiveViewSet
    {
        IDocument ParentDocument { get; }

        bool AffectAllViews { get; set; }

        IAdaptiveView Base { get; }

        // Contains all adaptive views, but Base view.
        IAdaptiveViews AdaptiveViews { get; }

        IAdaptiveView CreateAdaptiveView(string name, IAdaptiveView parent);

        void DeleteAdaptiveView(IAdaptiveView view);

        void ChangeParent(IAdaptiveView view, IAdaptiveView newParent);

        bool MoveAdaptiveView(IAdaptiveView view, int delta);

        bool MoveAdaptiveViewTo(IAdaptiveView view, int index);
    }
}
