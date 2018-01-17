using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Naver.Compass.InfoStructure;

namespace Naver.Compass.Module
{
    public class NodeInfo:ViewModelBase
    {

        INodeViewModel selectedNode;
        
        int count;

        public INodeViewModel SelectedNode
        {
            get { return selectedNode; }
            set
            {
                if (selectedNode != value)
                {
                    selectedNode = value;
                    OnSelectedNodeChanged();
                    FirePropertyChanged("SelectedNode");
                }
            }
        }

        public int Count
        {
            get { return count; }
            private set
            {
                if (count != value)
                {
                    count = value;
                    FirePropertyChanged("Count");
                }
            }
        }

        internal void SetCount(int newcount)
        {
            Count = newcount;
        }

        public event EventHandler SelectedNodeChanged;

        protected virtual void OnSelectedNodeChanged()
        {
            if (SelectedNodeChanged != null)
                SelectedNodeChanged(this, EventArgs.Empty);
        }

    }
}
