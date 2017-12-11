using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;
using FMS.BusinessObject.Inputs;
using System.Linq.Expressions;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class RptFuelService : IRptFuelService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<FUEL_REPORT_DATA> _rptFuelRepository;

        public RptFuelService(IUnitOfWork uow)
        {
            _uow = uow;

            _rptFuelRepository = _uow.GetGenericRepository<FUEL_REPORT_DATA>();
        }

        public List<FUEL_REPORT_DATA> GetRptFuel()
        {
            return _rptFuelRepository.Get().ToList();
        }
    }
}
