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
        List<MST_EMPLOYEE> GetAllUsers();


        MST_EMPLOYEE GetLogin(string userId);
    }
}
