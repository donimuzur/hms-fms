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
    public class ArchTraCtfService : IArchTraCtfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CTF> _archTraCtfRepository;
        public ArchTraCtfService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCtfRepository = _uow.GetGenericRepository<ARCH_TRA_CTF>();
        }
        public void Save(ARCH_TRA_CTF db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_CTF>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_TRA_CTF> GetCtfDashboard(Login userLogin, bool isCompleted, string benefitType, string wtcType)
        {
            Expression<Func<ARCH_TRA_CTF, bool>> queryFilter = PredicateHelper.True<ARCH_TRA_CTF>();

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
                queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == wtcType || c.VEHICLE_TYPE == benefitType);

                if (userLogin.UserRole == Enums.UserRole.FleetManager)
                {
                    queryFilter = queryFilter.And(c => c.DOCUMENT_STATUS != Enums.DocumentStatus.Draft && c.CREATED_BY != userLogin.USER_ID && c.EMPLOYEE_ID_CREATOR != userLogin.EMPLOYEE_ID);
                }
            }

            return _archTraCtfRepository.Get(queryFilter, null, "").ToList();
        }
        public ARCH_TRA_CTF GetCtfById(long TraCtfId)
        {
            return _archTraCtfRepository.GetByID(TraCtfId);
        }
    }
}
