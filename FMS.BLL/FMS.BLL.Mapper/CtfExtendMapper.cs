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
    public class CtfExtendMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CTF_EXTEND, CtfExtendDto>().IgnoreAllNonExisting()
             .ForMember(dest => dest.CtfExtendId, opt => opt.MapFrom(src => src.TRA_CTF_EXTEND_ID))
             .ForMember(dest => dest.TraCtfId, opt => opt.MapFrom(src => src.TRA_CTF_ID))
             .ForMember(dest => dest.NewProposedDate, opt => opt.MapFrom(src => src.NEW_PROPOSED_DATE))
             .ForMember(dest => dest.ExtendPoNumber, opt => opt.MapFrom(src => src.EXTEND_PO_NUMBER))
             .ForMember(dest => dest.ExtedPoLine, opt => opt.MapFrom(src => src.EXTEND_PO_LINE))
             .ForMember(dest => dest.ExtendPrice, opt => opt.MapFrom(src => src.EXTEND_PRICE))
             .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON))
             .ForMember(dest => dest.MstReason, opt => opt.MapFrom(src => src.MST_REASON))
             .ForMember(dest => dest.ExtendPoliceNumber, opt => opt.MapFrom(src => src.EXTEND_POLICE_NUMBER));

            AutoMapper.Mapper.CreateMap<CtfExtendDto, TRA_CTF_EXTEND>().IgnoreAllNonExisting()
             .ForMember(dest => dest.TRA_CTF_EXTEND_ID, opt => opt.MapFrom(src => src.CtfExtendId))
             .ForMember(dest => dest.TRA_CTF_ID, opt => opt.MapFrom(src => src.TraCtfId))
             .ForMember(dest => dest.NEW_PROPOSED_DATE, opt => opt.MapFrom(src => src.NewProposedDate))
             .ForMember(dest => dest.EXTEND_PO_NUMBER, opt => opt.MapFrom(src => src.ExtendPoNumber))
             .ForMember(dest => dest.EXTEND_PO_LINE, opt => opt.MapFrom(src => src.ExtedPoLine))
             .ForMember(dest => dest.EXTEND_PRICE, opt => opt.MapFrom(src => src.ExtendPrice))
             .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.Reason))
             .ForMember(dest => dest.MST_REASON, opt => opt.MapFrom(src => src.MstReason))
             .ForMember(dest => dest.EXTEND_POLICE_NUMBER, opt => opt.MapFrom(src => src.ExtendPoliceNumber));
        }
    }
}
