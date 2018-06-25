using AutoMapper;
using FMS.BLL.GroupCostCenter;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.GroupCostCenter
{
    public class GroupCostCenterBLL : IGroupCostCenterBLL
    {
        public IGroupCostCenterService _GroupCostCenterService;
        public IArchFunctionGroupService _archGroupCostCenterService;
        public IUnitOfWork _uow;

        public GroupCostCenterBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _GroupCostCenterService = new GroupCostCenterService(_uow);
            _archGroupCostCenterService = new ArchFunctionGroupService(_uow);
        }
        
        public List<GroupCostCenterDto> GetGroupCenter(bool? Archived = null)
        {
            var retData = new List<GroupCostCenterDto>();
            if(Archived.HasValue)
            {
                var data = _archGroupCostCenterService.GetGroupCostCenter();
                retData = Mapper.Map<List<GroupCostCenterDto>>(data);
            }
            else
            {
                var data = _GroupCostCenterService.GetGroupCostCenter();
                retData = Mapper.Map<List<GroupCostCenterDto>>(data);
            }
            return retData;
        }
        public GroupCostCenterDto GetGroupCenterById(int MstGroupCostCenterId, bool? Archived= null)
        {
            var retData = new GroupCostCenterDto();
            if(Archived.HasValue)
            {
                var data = _archGroupCostCenterService.GetGroupCostCenterById(MstGroupCostCenterId);
                retData = Mapper.Map<GroupCostCenterDto>(data);
            }
            else
            {
                var data = _GroupCostCenterService.GetGroupCostCenterById(MstGroupCostCenterId);
                retData = Mapper.Map<GroupCostCenterDto>(data);
            }
            return retData;
        }
        public void Save(GroupCostCenterDto dto)
        {
            var dbGroupCostCenter = Mapper.Map<MST_FUNCTION_GROUP>(dto);
            _GroupCostCenterService.Save(dbGroupCostCenter);
        }
        public void Save(GroupCostCenterDto dto, Login userLogin)
        {
            var dbGroupCostCenter = Mapper.Map<MST_FUNCTION_GROUP>(dto);
            _GroupCostCenterService.Save(dbGroupCostCenter, userLogin);
        }

        public List<GroupCostCenterDto> GetGroupCenter(GroupCostCenterParamInput filter)
        {
            var retData =new List<GroupCostCenterDto>();
            if(filter.Table == "2")
            {
                var data = _archGroupCostCenterService.GetGroupCostCenter(filter);
                retData = Mapper.Map<List<GroupCostCenterDto>>(data);
            }
            else
            {
                var data = _GroupCostCenterService.GetGroupCostCenter(filter);
                retData = Mapper.Map<List<GroupCostCenterDto>>(data);
            }
          
            return retData;
        }
    }
}
