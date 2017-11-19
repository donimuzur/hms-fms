using FMS.BusinessObject;
using FMS.BusinessObject.Business;
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
    public class TemporaryService : ITemporaryService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_TEMPORARY> _traTempRepository;

        private string includeTables = "MST_REASON";

        public TemporaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _traTempRepository = _uow.GetGenericRepository<TRA_TEMPORARY>();
        }

        public TRA_TEMPORARY GetTemporaryById(long id)
        {
            Expression<Func<TRA_TEMPORARY, bool>> queryFilter = PredicateHelper.True<TRA_TEMPORARY>();

            queryFilter = queryFilter.And(c => c.TRA_TEMPORARY_ID == id);

            return _traTempRepository.Get(queryFilter).FirstOrDefault();
        }

        public void saveTemporary(TRA_TEMPORARY dbTraTemporary, Login userlogin)
        {
            _uow.GetGenericRepository<TRA_TEMPORARY>().InsertOrUpdate(dbTraTemporary, userlogin, Enums.MenuList.TraTmp);
            _uow.SaveChanges();
        }

        public List<TRA_TEMPORARY> GetTemp(Login userLogin, bool isCompleted, string benefitType, string wtcType)
        {
            Expression<Func<TRA_TEMPORARY, bool>> queryFilter = PredicateHelper.True<TRA_TEMPORARY>();

            if (isCompleted)
            {
                queryFilter = queryFilter.And(c => c.DOCUMENT_STATUS == Enums.DocumentStatus.Completed || c.DOCUMENT_STATUS == Enums.DocumentStatus.Cancelled);
            }
            else
            {
                queryFilter = queryFilter.And(c => c.DOCUMENT_STATUS != Enums.DocumentStatus.Completed && c.DOCUMENT_STATUS != Enums.DocumentStatus.Cancelled);
            }

            if (userLogin.UserRole == Enums.UserRole.User)
            {
                queryFilter = queryFilter.And(c => c.EMPLOYEE_ID == userLogin.EMPLOYEE_ID);
            }
            if (userLogin.UserRole == Enums.UserRole.HR)
            {
                queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == benefitType);
            }
            if (userLogin.UserRole == Enums.UserRole.Fleet)
            {
                queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == wtcType || (c.VEHICLE_TYPE == benefitType &&
                                                                                    (c.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval || c.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress)));
            }

            return _traTempRepository.Get(queryFilter, null, includeTables).ToList();
        }

        public List<TRA_TEMPORARY> GetAllTemp()
        {
            return _traTempRepository.Get().ToList();
        }
    }
}
