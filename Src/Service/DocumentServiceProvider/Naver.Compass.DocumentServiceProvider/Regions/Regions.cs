using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Naver.Compass.Service.Document
{
    public class Regions : IRegions
    {
        public IRegion GetRegion(Guid regionGuid)
        {
            if (_regions.ContainsKey(regionGuid))
            {
                return _regions[regionGuid];
            }

            return null;
        }

        public IEnumerator<IRegion> GetEnumerator()
        {
            return _regions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Contains(Guid regionGuid)
        {
            return _regions.ContainsKey(regionGuid);
        }

        public int Count
        {
            get { return _regions.Count; }
        }

        public IRegion this[Guid regionGuid]
        {
            get
            {
                return GetRegion(regionGuid);
            }
        }

        internal void Add(IRegion region)
        {
            if (region != null && !_regions.ContainsKey(region.Guid))
            {
                _regions[region.Guid] = region;
            }
        }

        internal void AddRange(List<IRegion> regionList)
        {
            if(regionList != null)
            {
                foreach(IRegion region in regionList)
                {
                    if(region != null && !_regions.ContainsKey(region.Guid))
                    {
                        _regions[region.Guid] = region;
                    }
                }
            }
        }

        internal bool Remove(Guid regionGuid)
        {
            return _regions.Remove(regionGuid);
        }

        internal void clear()
        {
            _regions.Clear();
        }

        private Dictionary<Guid, IRegion> _regions = new Dictionary<Guid, IRegion>();
    }
}
