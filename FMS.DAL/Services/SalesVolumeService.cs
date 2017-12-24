using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public List<MST_SALES_VOLUME> GetSalesVolume(SalesVolumeParamInput filter)
        {
            Expression<Func<MST_SALES_VOLUME, bool>> queryFilter = PredicateHelper.True<MST_SALES_VOLUME>();

            if (filter != null)
            {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.MONTH >= filter.MonthFrom);
                }
                if (filter.MonthTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.MONTH <= filter.MonthTo);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.YEAR >= filter.YearFrom);
                }
                if (filter.YearTo > 0)
                {
                    queryFilter = queryFilter.And(c => c.YEAR <= filter.YearTo);
                }
                if (!string.IsNullOrEmpty(filter.Type))
                {
                    queryFilter = queryFilter.And(c => c.TYPE.ToUpper() == filter.Type.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    queryFilter = queryFilter.And(c => c.REGION.ToUpper() == filter.Regional.ToUpper());
                }
            }

            return _SalesVolumeRepository.Get(queryFilter, null, "").ToList();
        }

        public List<MST_SALES_VOLUME> GetAllSalesVolume()
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
