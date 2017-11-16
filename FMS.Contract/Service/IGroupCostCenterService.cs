using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IGroupCostCenterService
    {
        List<MST_FUNCTION_GROUP> GetGroupCostCenter();
        MST_FUNCTION_GROUP GetGroupCostCenterById(int MstGroupCostCenterId);
        void Save(MST_FUNCTION_GROUP dbGroupCostCenter);
        void Save(MST_FUNCTION_GROUP dbGroupCostCenter, Login userLogin);
    }
}
