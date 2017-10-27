using FMS.Contract;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.SysAccessBLL
{
    public class SysAccessBLL
    {
        private ISysAccessService _sysAccessService;
        private IUnitOfWork _uow;

        public SysAccessBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _sysAccessService = new SysAccessService(uow);
        }
        
        public 
    }
}
