using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using NLog;


namespace FMS.BLL.ComplaintCategory
{
    public class ComplaintCategoryBLL : IComplaintCategoryBLL
    {
        //private ILogger _logger;
        private IComplaintCategoryService _complainService;
        private IRoleService _roleService;

        public ComplaintCategoryBLL(IUnitOfWork uow)
        {
            //_logger = logger;
            _complainService = new ComplainCategoryService(uow);
            _roleService = new RoleService(uow);
        }

        public List<ComplaintDto> GetComplaints()
        {
            var data = _complainService.GetComplaintCategories();
            var roles = _roleService.GetRoles();
            var rolesDto = Mapper.Map<List<RoleDto>>(roles);
            var retData = Mapper.Map<List<ComplaintDto>>(data);
            foreach (var complaintDto in retData)
            {
                complaintDto.Role = rolesDto.FirstOrDefault(x => x.RoleId == complaintDto.RoleId);
            }
            return retData;

        }

        
    }

    
}
