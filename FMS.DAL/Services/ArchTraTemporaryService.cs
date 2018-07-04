using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
   
    public class ArchTraTemporaryService : IArchTraTemporaryService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_TEMPORARY> _archTraTemporaryRepository;


        public ArchTraTemporaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraTemporaryRepository = _uow.GetGenericRepository<ARCH_TRA_TEMPORARY>();
        }
        public void Save(ARCH_TRA_TEMPORARY db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_TEMPORARY>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }

        public List<ARCH_TRA_TEMPORARY> GetTemp(Login userLogin, bool isCompleted, string benefitType, string wtcType)
        {
            Expression<Func<ARCH_TRA_TEMPORARY, bool>> queryFilter = PredicateHelper.True<ARCH_TRA_TEMPORARY>();

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
            if (userLogin.UserRole == Enums.UserRole.HR || userLogin.UserRole == Enums.UserRole.HRManager)
            {
                queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == benefitType);
            }
            if (userLogin.UserRole == Enums.UserRole.Fleet || userLogin.UserRole == Enums.UserRole.FleetManager)
            {
                queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == wtcType || c.CREATED_BY == userLogin.USER_ID || (c.VEHICLE_TYPE == benefitType &&
                                                                                    (c.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval || c.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress
                                                                                    || c.DOCUMENT_STATUS == Enums.DocumentStatus.Completed
                                                                                    || c.DOCUMENT_STATUS == Enums.DocumentStatus.Cancelled)));

                if (userLogin.UserRole == Enums.UserRole.FleetManager)
                {
                    queryFilter = queryFilter.And(c => c.DOCUMENT_STATUS != Enums.DocumentStatus.Draft && c.CREATED_BY != userLogin.USER_ID && c.EMPLOYEE_ID_CREATOR != userLogin.EMPLOYEE_ID);
                }
            }

            return _archTraTemporaryRepository.Get(queryFilter).ToList();
        }

        public ARCH_TRA_TEMPORARY GetTempById(long TraTempId)
        {
            return _archTraTemporaryRepository.GetByID(TraTempId);
        }

    }
}
