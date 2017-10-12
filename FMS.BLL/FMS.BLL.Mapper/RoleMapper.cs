using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class RoleMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<SysRole, RoleDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleID))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                ;
        }
    }
}
