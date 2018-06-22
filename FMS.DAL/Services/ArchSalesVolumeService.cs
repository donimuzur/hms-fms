using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class ArchSalesVolumeService : IArchSalesVolumeService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_SALES_VOLUME> _archSalesVolumeRepository;
        public ArchSalesVolumeService(IUnitOfWork uow)
        {
            _uow = uow;
            _archSalesVolumeRepository = uow.GetGenericRepository<ARCH_MST_SALES_VOLUME>();
        }
        public void Save(ARCH_MST_SALES_VOLUME db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_SALES_VOLUME>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_MST_SALES_VOLUME> GetSalesVolume(SalesVolumeParamInput filter)
        {
            Expression<Func<ARCH_MST_SALES_VOLUME, bool>> queryFilter = PredicateHelper.True<ARCH_MST_SALES_VOLUME>();

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

            return _archSalesVolumeRepository.Get(queryFilter, null, "").ToList();
        }
        public List<ARCH_MST_SALES_VOLUME> GetAllSalesVolume()
        {
            return _archSalesVolumeRepository.Get().ToList();
        }
        public ARCH_MST_SALES_VOLUME GetSalesVolumeById(int MstSalesVolumeId)
        {
            return _archSalesVolumeRepository.Get().Where(x => x.MST_SALES_VOLUME_ID == MstSalesVolumeId).FirstOrDefault();
        }

    }
}
