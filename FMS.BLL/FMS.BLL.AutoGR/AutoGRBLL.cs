using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.AutoGR
{
    public class AutoGrBLL : IAutoGrBLL
    {
        private IUnitOfWork _uow;
        private IAutoGrService _grService;
        public AutoGrBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _grService = new AutoGrService(_uow);
        }
    }
}
