using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IGroupCostCenterBLL
    {
        List<GroupCostCenterDto> GetGroupCenter(bool? Archived = null);
        GroupCostCenterDto GetGroupCenterById(int MstGroupCostCenterId, bool? Archived = null);
        void Save(GroupCostCenterDto dto);
        void Save(GroupCostCenterDto data, Login currentUser);
        List<GroupCostCenterDto> GetGroupCenter(GroupCostCenterParamInput groupCostCenterParamInput);
    }
}
