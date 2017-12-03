using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;
using System.Data.Entity.Core.EntityClient;
using FMS.BusinessObject.Business;

namespace FMS.BLL.Delegation
{
    public class DelegationBLL : IDelegationBLL
    {
        private IDelegationService _DelegationService;
        private IUnitOfWork _uow;
        public DelegationBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _DelegationService = new DelegationService(uow);
        }

        public List<DelegationDto> GetDelegation()
        {
            //var data = _DelegationService.GetDelegation();
            //var retData = Mapper.Map<List<DelegationDto>>(data);
            //return retData;

            FMSEntities context = new FMSEntities();
            var query = (from a in context.MST_DELEGATION
                         join b in context.MST_EMPLOYEE on a.EMPLOYEE_FROM equals b.EMPLOYEE_ID
                         join c in context.MST_EMPLOYEE on a.EMPLOYEE_TO equals c.EMPLOYEE_ID
                         select new DelegationDto()
                         {
                             MstDelegationID = a.MST_DELEGATION_ID,
                             EmployeeFrom = a.EMPLOYEE_FROM,
                             EmployeeTo = a.EMPLOYEE_TO,
                             DateFrom = a.DATE_FROM,
                             LoginTo = a.LOGIN_TO,
                             LoginFrom =a.LOGIN_FROM,
                             DateTo = a.DATE_TO,
                             Attachment = a.ATTACHMENT,
                             IsComplaintFrom = (bool) a.IS_COMPLAINT_FORM,
                             CreatedBy = a.CREATED_BY,
                             CreatedDate = a.CREATED_DATE,
                             ModifiedBy = a.MODIFIED_BY,
                             NameEmployeeFrom = b.FORMAL_NAME,
                             NameEmployeeTo = c.FORMAL_NAME,
                             IsActive = (bool)a.IS_ACTIVE
                         });
            var dbResult = query.ToList();
            return dbResult;
        }

        public DelegationDto GetDelegationById(int Id)
        {
            var data = _DelegationService.GetDelegationById(Id);
            var retData = Mapper.Map<DelegationDto>(data);

            return retData;
        }

        public void Save(DelegationDto DelegationDto)
        {
            var dbDelegation = Mapper.Map<MST_DELEGATION>(DelegationDto);
            _DelegationService.save(dbDelegation);
        }


        public void Save(DelegationDto DelegationDto, Login userLogin)
        {
            var dbDelegation = Mapper.Map<MST_DELEGATION>(DelegationDto);
            _DelegationService.save(dbDelegation, userLogin);
        }
    }
}
