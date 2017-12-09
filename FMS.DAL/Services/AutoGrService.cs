using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;

namespace FMS.DAL.Services
{
    public class AutoGrService : IAutoGrService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<AUTO_GR> _autoGrRepository;
        private IGenericRepository<AUTO_GR_DETAIL> _autoGrDetailRepository;

        public AutoGrService(IUnitOfWork uow)
        {
            _uow = uow;
            _autoGrRepository = _uow.GetGenericRepository<AUTO_GR>();
            _autoGrDetailRepository = _uow.GetGenericRepository<AUTO_GR_DETAIL>();
        }
    }
}
