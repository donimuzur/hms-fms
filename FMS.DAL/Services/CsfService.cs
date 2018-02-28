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
    public class CsfService : ICsfService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<TRA_CSF> _csfRepository;

        private string includeTables = "MST_REASON";

        public CsfService(IUnitOfWork uow)
        {
            _uow = uow;
            _csfRepository = _uow.GetGenericRepository<TRA_CSF>();
        }

        public List<TRA_CSF> GetCsf(Login userLogin, bool isCompleted, string benefitType, string wtcType)
        {
            Expression<Func<TRA_CSF, bool>> queryFilter = PredicateHelper.True<TRA_CSF>();

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
                queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == wtcType || (c.VEHICLE_TYPE == benefitType && 
                                                                                    (c.DOCUMENT_STATUS == Enums.DocumentStatus.WaitingFleetApproval || c.DOCUMENT_STATUS == Enums.DocumentStatus.InProgress
                                                                                    || c.DOCUMENT_STATUS == Enums.DocumentStatus.Completed || c.DOCUMENT_STATUS == Enums.DocumentStatus.Cancelled)));
            }
            return _csfRepository.Get(queryFilter, null, includeTables).ToList();
        }


        public void saveCsf(TRA_CSF dbTraCsf, Login userlogin)
        {
            _uow.GetGenericRepository<TRA_CSF>().InsertOrUpdate(dbTraCsf, userlogin, Enums.MenuList.TraCsf);
            _uow.SaveChanges();
        }

        public void CancelCsf(long id, int Remark, string user)
        {
            var data = _csfRepository.GetByID(id);

            if (data != null)
            {
                data.DOCUMENT_STATUS = Enums.DocumentStatus.Cancelled;
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = user;
                data.IS_ACTIVE = false;
                data.REMARK = Remark;
                _uow.SaveChanges();
            }
        }


        public TRA_CSF GetCsfById(long id)
        {
            Expression<Func<TRA_CSF, bool>> queryFilter = PredicateHelper.True<TRA_CSF>();

            queryFilter = queryFilter.And(c => c.TRA_CSF_ID == id);

            return _csfRepository.Get(queryFilter).FirstOrDefault();
        }


        public List<TRA_CSF> GetAllCsf()
        {
            return _csfRepository.Get().ToList();
        }
    }
}
