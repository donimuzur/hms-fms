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
    public class ExecutiveSummaryService : IExecutiveSummaryService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<NO_OF_VEHICLE_REPORT_DATA> _noVehRepository;

        public ExecutiveSummaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _noVehRepository = _uow.GetGenericRepository<NO_OF_VEHICLE_REPORT_DATA>();
        }

        public List<NO_OF_VEHICLE_REPORT_DATA> GetAllNoVehicle()
        {
            return _noVehRepository.Get().ToList();
        }
    }
}
