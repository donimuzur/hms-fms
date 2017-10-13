using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;

namespace FMS.DAL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork _uow;
        private ILogger _logger;
        private IGenericRepository<MST_EMPLOYEE> _userRepository;
        private string IncludeTables = "";

        public UserService(IUnitOfWork uow, ILogger logger)
        {
            _uow = uow;
            _logger = logger;
            _userRepository = _uow.GetGenericRepository<MST_EMPLOYEE>();
        }

        public List<MST_EMPLOYEE> GetAllUser()
        {
            return  _userRepository.Get(null, null, IncludeTables).ToList();
        }
    }
}
