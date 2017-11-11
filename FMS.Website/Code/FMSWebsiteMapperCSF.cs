using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;
using FMS.BusinessObject;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeCSF()
        {
            Mapper.CreateMap<TraCsfDto, CsfData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TraCsfId, opt => opt.MapFrom(src => src.TRA_CSF_ID))
                .ForMember(dest => dest.CsfNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.CsfStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON_ID))
                .ForMember(dest => dest.ReasonId, opt => opt.MapFrom(src => src.REASON_ID))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE == null ? src.CREATED_DATE : src.MODIFIED_DATE))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreateBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
                ;

            Mapper.CreateMap<CsfData, TraCsfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TRA_CSF_ID, opt => opt.MapFrom(src => src.TraCsfId))
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.CsfNumber))
                .ForMember(dest => dest.DOCUMENT_STATUS, opt => opt.MapFrom(src => src.CsfStatus))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
                .ForMember(dest => dest.REASON_ID, opt => opt.MapFrom(src => src.ReasonId))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BodyType))
                .ForMember(dest => dest.REMARK_ID, opt => opt.MapFrom(src => src.RemarkId))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                ;

            Mapper.CreateMap<EpafDto, EpafData>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate))
                .ForMember(dest => dest.EpafEffectiveDate, opt => opt.MapFrom(src => src.EfectiveDate))
                .ForMember(dest => dest.EpafApprovedDate, opt => opt.MapFrom(src => src.ApprovedDate))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.EpafAction))
                .ForMember(dest => dest.CostCentre, opt => opt.MapFrom(src => src.CostCenter));
        }
    }
}