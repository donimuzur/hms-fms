using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface IUserBLL
    {
        List<SysUser> GetAllUsers();


        SysUser GetLogin(string userId);
    }
}
