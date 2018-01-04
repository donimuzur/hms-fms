using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class KpiMonitoringMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<KPI_REPORT_DATA, KpiMonitoringDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.TraId, opt => opt.MapFrom(src => src.TRA_ID))
            .ForMember(dest => dest.FormType, opt => opt.MapFrom(src => src.FORM_TYPE))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
            .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ADDRESS))
            .ForMember(dest => dest.PreviousBaseTown, opt => opt.MapFrom(src => src.PREVIOUS_BASE_TOWN))
            .ForMember(dest => dest.NewBaseTown, opt => opt.MapFrom(src => src.NEW_BASE_TOWN))
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
            .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
            .ForMember(dest => dest.VehicleGroup, opt => opt.MapFrom(src => src.VEHICLE_GROUP_LEVEL))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.VEHICLE_MODEL))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.COLOR))
            .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
            .ForMember(dest => dest.TemporaryRequestDate, opt => opt.MapFrom(src => src.TEMPORARY_REQUEST_DATE))
            .ForMember(dest => dest.EeReceivedTemp, opt => opt.MapFrom(src => src.EE_RECEIVED_TEMP))
            .ForMember(dest => dest.SendToEmpDate, opt => opt.MapFrom(src => src.SEND_TO_EMP_DATE))
            .ForMember(dest => dest.SendBackToHr, opt => opt.MapFrom(src => src.SEND_BACK_TO_HR))
            .ForMember(dest => dest.SendToFleetDate, opt => opt.MapFrom(src => src.SEND_TO_FLEET_DATE))
            .ForMember(dest => dest.SendToEmpBenefit, opt => opt.MapFrom(src => src.SEND_TO_EMPLOYEE_BENEFIT_DATE))
            .ForMember(dest => dest.SendSuratKuasa, opt => opt.MapFrom(src => src.SEND_SURAT_KUASA))
            .ForMember(dest => dest.SendAgreement, opt => opt.MapFrom(src => src.SEND_AGREEMENT))
            .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
            .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.REPORT_MONTH))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.REPORT_YEAR))
            .ForMember(dest => dest.CreatedDate , opt => opt.MapFrom(src => src.CREATED_DATE));

            AutoMapper.Mapper.CreateMap<KpiMonitoringDto, KPI_REPORT_DATA>()
              .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TRA_ID, opt => opt.MapFrom(src => src.TraId))
            .ForMember(dest => dest.FORM_TYPE, opt => opt.MapFrom(src => src.FormType))
            .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
            .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
            .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EffectiveDate))
            .ForMember(dest => dest.ADDRESS, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.PREVIOUS_BASE_TOWN, opt => opt.MapFrom(src => src.PreviousBaseTown))
            .ForMember(dest => dest.NEW_BASE_TOWN, opt => opt.MapFrom(src => src.NewBaseTown))
            .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
            .ForMember(dest => dest.VEHICLE_USAGE, opt => opt.MapFrom(src => src.VehicleUsage))
            .ForMember(dest => dest.VEHICLE_GROUP_LEVEL, opt => opt.MapFrom(src => src.VehicleGroup))
            .ForMember(dest => dest.VEHICLE_MODEL, opt => opt.MapFrom(src => src.Model))
            .ForMember(dest => dest.COLOR, opt => opt.MapFrom(src => src.Color))
            .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
            .ForMember(dest => dest.TEMPORARY_REQUEST_DATE, opt => opt.MapFrom(src => src.TemporaryRequestDate))
            .ForMember(dest => dest.EE_RECEIVED_TEMP, opt => opt.MapFrom(src => src.EeReceivedTemp))
            .ForMember(dest => dest.SEND_TO_EMP_DATE, opt => opt.MapFrom(src => src.SendToEmpDate))
            .ForMember(dest => dest.SEND_BACK_TO_HR, opt => opt.MapFrom(src => src.SendBackToHr))
            .ForMember(dest => dest.SEND_TO_FLEET_DATE, opt => opt.MapFrom(src => src.SendToFleetDate))
            .ForMember(dest => dest.SEND_TO_EMPLOYEE_BENEFIT_DATE, opt => opt.MapFrom(src => src.SendToEmpBenefit))
            .ForMember(dest => dest.SEND_SURAT_KUASA, opt => opt.MapFrom(src => src.SendSuratKuasa))
            .ForMember(dest => dest.SEND_AGREEMENT, opt => opt.MapFrom(src => src.SendAgreement))
            .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
            .ForMember(dest => dest.REPORT_MONTH, opt => opt.MapFrom(src => src.Month))
            .ForMember(dest => dest.REPORT_YEAR, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate));
        }
    }
}
