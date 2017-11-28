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

        public MST_SALES_VOLUME GetSalesVolumeById(int MstSalesVolumeId)
        {
            return _SalesVolumeRepository.Get().Where(x => x.MST_SALES_VOLUME_ID == MstSalesVolumeId).FirstOrDefault();
        }

        public void CheckSalesVolume(String Type, String Region, int Month, int Year, String User)
        {
            var data = _SalesVolumeRepository.Get().Where(x => x.TYPE == Type && x.REGION == Region && x.MONTH == Month && x.YEAR == Year && x.IS_ACTIVE == true).FirstOrDefault();
            if (data != null)
            {
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = User;
                data.IS_ACTIVE = false;
                _uow.SaveChanges();
            }

        }

        public void save(MST_SALES_VOLUME dbSalesVolume)
        {
            _uow.GetGenericRepository<MST_SALES_VOLUME>().InsertOrUpdate(dbSalesVolume);
            _uow.SaveChanges();
        }
    }
}
