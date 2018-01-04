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
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.MST_SYSACCESS_ID))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.ROLE_NAME))
                .ForMember(dest => dest.RoleNameAlias, opt => opt.MapFrom(src => src.ROLE_NAME_ALIAS))
                .ForMember(dest => dest.WriteAccess, opt => opt.MapFrom(src => src.WRITE_ACCESSS))
                .ForMember(dest => dest.ReadAccess, opt => opt.MapFrom(src => src.READ_ACCESS))
                .ForMember(dest => dest.UploadAccess, opt => opt.MapFrom(src => src.UPLOAD_ACCESS))
                .ForMember(dest => dest.ModulId, opt => opt.MapFrom(src => src.MODUL_ID))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));
        }
    }
}
