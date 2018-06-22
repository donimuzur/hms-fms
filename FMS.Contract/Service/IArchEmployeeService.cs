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
    public interface IArchEmployeeService
    {
        void Save(ARCH_MST_EMPLOYEE db, Login userlogin);
        List<ARCH_MST_EMPLOYEE> GetEmployeeByParam(EmployeeParamInput input);
        ARCH_MST_EMPLOYEE GetEmployeeById(string MstEmployeeId);
    }
}
