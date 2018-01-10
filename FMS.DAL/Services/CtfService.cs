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
    public class CtfService : ICtfService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_CTF> _traCtfRepository;

        public CtfService(IUnitOfWork uow)
        {
            _uow = uow;
            _traCtfRepository = _uow.GetGenericRepository<TRA_CTF>();
        }
        public List<TRA_CTF> GetCtf()
        {
            return _traCtfRepository.Get().ToList();
        }
        public List<TRA_CTF> GetCtfDashboard(Login userLogin, bool isCompleted, string benefitType, string wtcType)
        {
            Expression<Func<TRA_CTF, bool>> queryFilter = PredicateHelper.True<TRA_CTF>();

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
                queryFilter = queryFilter.And(c => c.VEHICLE_TYPE == wtcType || c.VEHICLE_TYPE == benefitType);
            }

            return _traCtfRepository.Get(queryFilter,null,"").ToList();
        }

        public void Save(TRA_CTF dbCtf, Login userlogin)
        {
            _traCtfRepository.InsertOrUpdate(dbCtf, userlogin , Enums.MenuList.TraCtf);
            _uow.SaveChanges();
        }
        public void SaveUpload(TRA_CTF dbCtf, Login userlogin)
        {
            _traCtfRepository.InsertOrUpdate(dbCtf, userlogin, Enums.MenuList.TraCtf);
        }
        public TRA_CTF GetCtfById(long TraCtfId)
        {
            return _traCtfRepository.GetByID(TraCtfId);
        }

        public void CancelCtf(long id, int Remark, Login userlogin)
        {
            var data = _traCtfRepository.GetByID(id);

            if (data != null)
            {
                data.DOCUMENT_STATUS = Enums.DocumentStatus.Cancelled;
                data.MODIFIED_DATE = DateTime.Now;
                data.MODIFIED_BY = userlogin.USER_ID;
                data.IS_ACTIVE = false;
                data.REMARK = Remark;
                _traCtfRepository.InsertOrUpdate(data, userlogin, Enums.MenuList.TraCtf);
                _uow.SaveChanges();
            }
        }

    }
}
