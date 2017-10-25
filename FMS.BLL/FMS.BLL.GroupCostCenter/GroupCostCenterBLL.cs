using AutoMapper;
using FMS.BLL.GroupCostCenter;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
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
        public IUnitOfWork _uow;

        public GroupCostCenterBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _GroupCostCenterService = new GroupCostCenterService(_uow);
        }
        
        public List<GroupCostCenterDto> GetGroupCenter()
        {
            var data = _GroupCostCenterService.GetGroupCostCenter();
            var redata = Mapper.Map<List<GroupCostCenterDto>>(data);
            return redata;
        }
        public GroupCostCenterDto GetGroupCenterById(int MstGroupCostCenterId)
        {
            var data = _GroupCostCenterService.GetGroupCostCenterById(MstGroupCostCenterId);
            var redata = Mapper.Map<GroupCostCenterDto>(data);
            return redata;
        }
        public void Save(GroupCostCenterDto dto)
        {
            var dbGroupCostCenter = Mapper.Map<MST_FUNCTION_GROUP>(dto);
            _GroupCostCenterService.Save(dbGroupCostCenter);
        }
    }
}
