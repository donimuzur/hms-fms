using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Ctf
{
    public class CtfBLL : ITraCtfBLL
    {
        private IUnitOfWork _uow;
        private ICtfService _ctfService;

        public CtfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ctfService = new CtfService(uow);
        }
    }
}
