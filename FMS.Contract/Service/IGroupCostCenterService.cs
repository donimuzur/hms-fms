using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IGroupCostCenterService
    {
        List<MST_FUNCTION_GROUP> GetGroupCostCenter();
        MST_FUNCTION_GROUP GetGroupCostCenterById(int MstGroupCostCenterId);
        void Save(MST_FUNCTION_GROUP dbGroupCostCenter);
    }
}
