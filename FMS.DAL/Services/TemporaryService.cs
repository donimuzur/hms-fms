using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class TemporaryService : ITemporaryService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_TEMPORARY> _traTempRepository;

        public TemporaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _traTempRepository = _uow.GetGenericRepository<TRA_TEMPORARY>();
        }
    }
}
