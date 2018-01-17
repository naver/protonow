using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naver.Compass.InfoStructure
{
    public enum GroupStatus
    {
        UnSelect,
        Selected,
        Edit
    }
    public interface IGroupOperation
    {
        GroupStatus GetGroupStatus(Guid GroupID);
        void SetGroupStatus(Guid GroupID,GroupStatus status);
        void DeselectAllChildren(Guid GroupID);
        void DeselectAllGroups();
        void UpdateGroup(Guid GroupID);
        void MoveSelectedGroup(double OffsetX, double OffsetY);
        List<Guid> GetAllSelectedGroups();
    }
}
