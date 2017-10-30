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
    public class SalesVolumeService : ISalesVolumeService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_SALES_VOLUME> _SalesVolumeRepository;
        public SalesVolumeService(IUnitOfWork uow)
        {
            _uow = uow;
            _SalesVolumeRepository = _uow.GetGenericRepository<MST_SALES_VOLUME>();
        }

        public List<MST_SALES_VOLUME> GetSalesVolume()
        {
            return _SalesVolumeRepository.Get().ToList();
        }

        public void save(MST_SALES_VOLUME dbSalesVolume)
        {
            _uow.GetGenericRepository<MST_SALES_VOLUME>().InsertOrUpdate(dbSalesVolume);
            _uow.SaveChanges();
        }
    }
}
