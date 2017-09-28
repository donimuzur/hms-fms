using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface IUserBll
    {
        List<SysUser> GetAllUsers();
    }
}
