using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class CafService : ICAFService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_CAF> _traCafRepository;

        public CafService(IUnitOfWork uow)
        {
            _uow = uow;
            _traCafRepository = _uow.GetGenericRepository<TRA_CAF>();
        }
    }
}
