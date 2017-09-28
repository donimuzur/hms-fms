using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using NLog;

namespace FMS.BLL.User
{
    public class UserBLL : IUserBll
    {
        private ILogger _logger;
        private IUserService _userService;
        

        public UserBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _userService = new UserService(uow,_logger);
            
        }


        public List<SysUser> GetAllUsers()
        {
            return _userService.GetAllUser();
        }
    }
}
