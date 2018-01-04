using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IEmployeeService
    {
        List<MST_EMPLOYEE> GetEmployee();
        MST_EMPLOYEE GetEmployeeById(string MstEmployeeId);
        MST_EMPLOYEE GetExist(string FormalName);
        List<MST_EMPLOYEE> GetEmployeeByParam(EmployeeParamInput param);
        string GetLastEmployeeId();
        void save(MST_EMPLOYEE dbEmployee);
    }
}
