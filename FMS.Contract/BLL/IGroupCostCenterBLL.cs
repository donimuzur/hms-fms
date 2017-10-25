using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface IGroupCostCenterBLL
    {
        List<GroupCostCenterDto> GetGroupCenter();
        GroupCostCenterDto GetGroupCenterById(int MstGroupCostCenterId);
        void Save(GroupCostCenterDto dto);
    }
}
