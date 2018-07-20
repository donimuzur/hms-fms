using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchFunctionGroupService
    {
        void Save(ARCH_MST_FUNCTION_GROUP db, Login userlogin);
        ARCH_MST_FUNCTION_GROUP GetGroupCostCenterById(int MstGroupCostCenterId);
        List<ARCH_MST_FUNCTION_GROUP> GetGroupCostCenter(GroupCostCenterParamInput filter);
        List<ARCH_MST_FUNCTION_GROUP> GetGroupCostCenter();
    }
}
