using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using FMS.BusinessObject.Inputs;
using System.Linq.Expressions;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class EpafService : IEpafService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_EPAF> _epafRepository;

        private string includeTables = "TRA_CSF";

        public EpafService(IUnitOfWork uow)
        {
            _uow = uow;
            _epafRepository = _uow.GetGenericRepository<MST_EPAF>();
        }

        public List<MST_EPAF> GetEpaf()
        {
            return _epafRepository.Get().ToList();
        }
        public List<MST_EPAF> GetEpaf(EpafParamInput input)
        {
            Expression<Func<MST_EPAF, bool>> queryFilter = PredicateHelper.True<MST_EPAF>();

            if (input != null)
            {
                if (input.DateFrom.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.EFFECTIVE_DATE >= input.DateFrom);
                }
                if (input.DateTo.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.EFFECTIVE_DATE <= input.DateFrom);
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

            return _epafRepository.Get(queryFilter, null, "").ToList();
        }
        public List<MST_EPAF> GetEpafByDocumentType(Enums.DocumentType docType)
        {
            return _epafRepository.Get(x => x.DOCUMENT_TYPE == (int)docType && x.IS_ACTIVE && x.REMARK == null, null, includeTables).ToList();
        }

        public void DeactivateEpaf(long epafId, int Remark, string user)
        {
            var data = _epafRepository.GetByID(epafId);

            if (data != null)
            {
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = user;
                data.IS_ACTIVE = false;
                data.REMARK = Remark;
                _uow.SaveChanges();
            }
        }


        public MST_EPAF GetEpafById(long? epafId)
        {
            return _epafRepository.GetByID(epafId);
        }
    }
}
