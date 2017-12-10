using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class SysAccessMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_SYSACCESS, SysAccessDto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MstSysAccessId, opt => opt.MapFrom(src => src.MST_SYSACCESS_ID))
              .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.ROLE_NAME))
              .ForMember(dest => dest.RoleNameAlias, opt => opt.MapFrom(src => src.ROLE_NAME_ALIAS))
              .ForMember(dest => dest.ModulId, opt => opt.MapFrom(src => src.MODUL_ID))
              .ForMember(dest => dest.ReadAccessData, opt => opt.MapFrom(src => src.READ_ACCESS))
              .ForMember(dest => dest.WriteAccessData, opt => opt.MapFrom(src => src.WRITE_ACCESSS))
              .ForMember(dest => dest.UploadAccess, opt => opt.MapFrom(src => src.UPLOAD_ACCESS))
              .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
              .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
              .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
              .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
              .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
              .ForMember(dest => dest.MstModul, opt => opt.MapFrom(src => src.MST_MODUL))
              .ForMember(dest => dest.ModulName, opt => opt.MapFrom(src => src.MST_MODUL.MODUL_NAME));

            AutoMapper.Mapper.CreateMap<SysAccessDto, MST_SYSACCESS>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MST_SYSACCESS_ID, opt => opt.MapFrom(src => src.MstSysAccessId))
              .ForMember(dest => dest.ROLE_NAME, opt => opt.MapFrom(src => src.RoleName))
              .ForMember(dest => dest.ROLE_NAME_ALIAS, opt => opt.MapFrom(src => src.RoleNameAlias))
              .ForMember(dest => dest.MODUL_ID, opt => opt.MapFrom(src => src.ModulId))
              .ForMember(dest => dest.READ_ACCESS, opt => opt.MapFrom(src => src.ReadAccessData))
              .ForMember(dest => dest.WRITE_ACCESSS, opt => opt.MapFrom(src => src.WriteAccessData))
              .ForMember(dest => dest.UPLOAD_ACCESS, opt => opt.MapFrom(src => src.UploadAccess))
              .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
              .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
              .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
              .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
              .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
              .ForMember(dest => dest.MST_MODUL, opt => opt.MapFrom(src => src.MstModul));
        }
    }
}
