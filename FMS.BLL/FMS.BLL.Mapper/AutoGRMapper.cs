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
    public class AutoGRMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<AUTO_GR, RptAutoGrDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.AutoGrId, opt => opt.MapFrom(src => src.AUTO_GR_ID))
                .ForMember(dest => dest.PoNumber, opt => opt.MapFrom(src => src.PO_NUMBER))
                .ForMember(dest => dest.GrDate, opt => opt.MapFrom(src => src.PO_DATE));

            AutoMapper.Mapper.CreateMap<AUTO_GR_DETAIL, RptAutoGrDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.AutoGrId, opt => opt.MapFrom(src => src.AUTO_GR_ID))
                .ForMember(dest => dest.PoLine, opt => opt.MapFrom(src => src.LINE_ITEM))
                .ForMember(dest => dest.QtyAutoGr, opt => opt.MapFrom(src => src.QTY_ITEM));
        }
    }
}
