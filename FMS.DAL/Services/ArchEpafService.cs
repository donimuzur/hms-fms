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
    public class ArchEpafService : IArchEpafService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_EPAF> _archEpafRepository;
        public ArchEpafService (IUnitOfWork uow)
        {
            _uow = uow;
            _archEpafRepository = uow.GetGenericRepository<ARCH_MST_EPAF>();
        }
        public void Save(ARCH_MST_EPAF db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_EPAF>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_MST_EPAF> GetEpaf()
        {
            return _archEpafRepository.Get().ToList();
        }
        public List<ARCH_MST_EPAF> GetEpaf(EpafParamInput input)
        {
            Expression<Func<ARCH_MST_EPAF, bool>> queryFilter = PredicateHelper.True<ARCH_MST_EPAF>();

            if (input != null)
            {
                if (input.DateFrom.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.EFFECTIVE_DATE >= input.DateFrom);
                }
                if (input.DateTo.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.EFFECTIVE_DATE <= input.DateTo);
                }
                if (!string.IsNullOrEmpty(input.DocumentType))
                {
                    var list = input.DocumentType.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => list.Contains((c.DOCUMENT_TYPE.ToString())));
                }
                if (!string.IsNullOrEmpty(input.EmployeeId))
                {
                    var listFunction = input.EmployeeId.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.EMPLOYEE_ID == null ? "" : c.EMPLOYEE_ID.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.EmployeeName))
                {
                    var listFunction = input.EmployeeName.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.EMPLOYEE_NAME == null ? "" : c.EMPLOYEE_NAME.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.EpafAction))
                {
                    var listFunction = input.EpafAction.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.EPAF_ACTION == null ? "" : c.EPAF_ACTION.ToUpper())));
                }
            }

            return _archEpafRepository.Get(queryFilter, null, "").ToList();
        }
        public ARCH_MST_EPAF GetEpafById(long? epafId)
        {
            return _archEpafRepository.GetByID(epafId);
        }
    }
}
