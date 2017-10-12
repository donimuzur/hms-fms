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
            AutoMapper.Mapper.CreateMap<MST_SYSACCESS, RoleDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.ROLE_NAME))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
                ;
        }
    }
}
