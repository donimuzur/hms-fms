using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using System.Linq.Expressions;
using FMS.Utils;

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

        public List<AUTO_GR> GetAutoGr(RptAutoGrInput input)
        {
            Expression<Func<AUTO_GR, bool>> queryFilter = c => c.IS_POSTED.HasValue && c.IS_POSTED.Value;

            if (input != null)
            {
                if (input.PeriodStart.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.PO_DATE >= input.PeriodStart);
                }

                if (input.PeriodEnd.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.PO_DATE <= input.PeriodEnd);
                }
                if (!String.IsNullOrEmpty(input.PONumber))
                {
                    queryFilter = queryFilter.And(x => x.PO_NUMBER == input.PONumber);
                }
                if (input.POLine.HasValue)
                {
                    queryFilter = queryFilter.And(x => x.LINE_ITEM == input.POLine);
                }


            }

            return _autoGrRepository.Get(queryFilter, null, "").ToList();
        }


        public List<AUTO_GR_DETAIL> GetAutoGrDetails(List<int> autoGrIds)
        {
            return _autoGrDetailRepository.Get(x => autoGrIds.Contains(x.AUTO_GR_ID)).ToList();
        }
    }
}
