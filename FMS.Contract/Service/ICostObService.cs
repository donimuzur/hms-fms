using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICostObService
    {
        List<MST_COST_OB> GetCostOb();
        MST_COST_OB GetCostObById(int MstCostObId);
        MST_COST_OB GetExist(string Model);
        void save(MST_COST_OB dbCostOb);
        void save(MST_COST_OB dbCostOb, Login userlogin);
    }
}
