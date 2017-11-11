using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class CtfMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TRA_CTF, TraCtfDto>().IgnoreAllNonExisting()
             .ForMember(dest => dest.TraCtfId, opt => opt.MapFrom(src => src.TRA_CTF_ID))
             .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
             .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
             .ForMember(dest => dest.EpafId, opt => opt.MapFrom(src => src.EPAF_ID))
             .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
             .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
             .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
             .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GROUP_LEVEL))
             .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON))
             .ForMember(dest => dest.SupplyMethod, opt => opt.MapFrom(src => src.SUPPLY_METHOD))
             .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
             .ForMember(dest => dest.VehicleYear, opt => opt.MapFrom(src => src.VEHICLE_YEAR))
             .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
             .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
             .ForMember(dest => dest.VehicleLocation, opt => opt.MapFrom(src => src.VEHICLE_LOCATION))
             .ForMember(dest => dest.EndRendDate, opt => opt.MapFrom(src => src.END_RENT_DATE))
             .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
             .ForMember(dest => dest.IsTransferToIdle, opt => opt.MapFrom(src => src.IS_TRANSFER_TO_IDLE))
             .ForMember(dest => dest.BuyCost, opt => opt.MapFrom(src => src.BUY_COST))
             .ForMember(dest => dest.ExtendVehicle, opt => opt.MapFrom(src => src.EXTEND_VEHICLE))
             .ForMember(dest => dest.WithdPic, opt => opt.MapFrom(src => src.WITHD_PIC))
             .ForMember(dest => dest.WithdPhone, opt => opt.MapFrom(src => src.WITHD_PHONE))
             .ForMember(dest => dest.WithdDate, opt => opt.MapFrom(src => src.WITHD_DATE))
             .ForMember(dest => dest.WithdCity, opt => opt.MapFrom(src => src.WITHD_CITY))
             .ForMember(dest => dest.WithdAddress, opt => opt.MapFrom(src => src.WITHD_ADDRESS))
             .ForMember(dest => dest.EmployeeContribution, opt => opt.MapFrom(src => src.EMPLOYEE_CONTRIBUTION))
             .ForMember(dest => dest.Penalty, opt => opt.MapFrom(src => src.PENALTY))
             .ForMember(dest => dest.RefundCost, opt => opt.MapFrom(src => src.REFUND_COST))
             .ForMember(dest => dest.BuyCostTotal, opt => opt.MapFrom(src => src.BUY_COST_TOTAL))
             .ForMember(dest => dest.UserDecision, opt => opt.MapFrom(src => src.USER_DECISION))
             .ForMember(dest => dest.PenaltyPoNumber, opt => opt.MapFrom(src => src.PENALTY_PO_NUMBER))
             .ForMember(dest => dest.PenaltyPoLine, opt => opt.MapFrom(src => src.PENALTY_PO_LINE))
             .ForMember(dest => dest.PenaltyPrice, opt => opt.MapFrom(src => src.PENALTY_PRICE))
             .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
             .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
             .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
             .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
             .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
             .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
             .ForMember(dest => dest.ReasonS, opt => opt.MapFrom(src => src.MST_REASON.REASON));

            AutoMapper.Mapper.CreateMap<TraCtfDto, TRA_CTF>().IgnoreAllNonExisting()
                .ForMember(dest => dest.TRA_CTF_ID, opt => opt.MapFrom(src => src.TraCtfId))
             .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
             .ForMember(dest => dest.DOCUMENT_STATUS, opt => opt.MapFrom(src => src.DocumentStatus))
             .ForMember(dest => dest.EPAF_ID, opt => opt.MapFrom(src => src.EpafId))
             .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
             .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
             .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
             .ForMember(dest => dest.GROUP_LEVEL, opt => opt.MapFrom(src => src.GroupLevel))
             .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.Reason))
             .ForMember(dest => dest.SUPPLY_METHOD, opt => opt.MapFrom(src => src.SupplyMethod))
             .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
             .ForMember(dest => dest.VEHICLE_YEAR, opt => opt.MapFrom(src => src.VehicleYear))
             .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
             .ForMember(dest => dest.VEHICLE_USAGE, opt => opt.MapFrom(src => src.VehicleUsage))
             .ForMember(dest => dest.VEHICLE_LOCATION, opt => opt.MapFrom(src => src.VehicleLocation))
             .ForMember(dest => dest.END_RENT_DATE, opt => opt.MapFrom(src => src.EndRendDate))
             .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EffectiveDate))
             .ForMember(dest => dest.IS_TRANSFER_TO_IDLE, opt => opt.MapFrom(src => src.IsTransferToIdle))
             .ForMember(dest => dest.BUY_COST, opt => opt.MapFrom(src => src.BuyCost))
             .ForMember(dest => dest.EXTEND_VEHICLE, opt => opt.MapFrom(src => src.ExtendVehicle))
             .ForMember(dest => dest.WITHD_PIC, opt => opt.MapFrom(src => src.WithdPic))
             .ForMember(dest => dest.WITHD_PHONE, opt => opt.MapFrom(src => src.WithdPhone))
             .ForMember(dest => dest.WITHD_DATE, opt => opt.MapFrom(src => src.WithdDate))
             .ForMember(dest => dest.WITHD_CITY, opt => opt.MapFrom(src => src.WithdCity))
             .ForMember(dest => dest.WITHD_ADDRESS, opt => opt.MapFrom(src => src.WithdAddress))
             .ForMember(dest => dest.EMPLOYEE_CONTRIBUTION, opt => opt.MapFrom(src => src.EmployeeContribution))
             .ForMember(dest => dest.PENALTY, opt => opt.MapFrom(src => src.Penalty))
             .ForMember(dest => dest.REFUND_COST, opt => opt.MapFrom(src => src.RefundCost))
             .ForMember(dest => dest.BUY_COST_TOTAL, opt => opt.MapFrom(src => src.BuyCostTotal))
             .ForMember(dest => dest.USER_DECISION, opt => opt.MapFrom(src => src.UserDecision))
             .ForMember(dest => dest.PENALTY_PO_NUMBER, opt => opt.MapFrom(src => src.PenaltyPoNumber))
             .ForMember(dest => dest.PENALTY_PO_LINE, opt => opt.MapFrom(src => src.PenaltyPoLine))
             .ForMember(dest => dest.PENALTY_PRICE, opt => opt.MapFrom(src => src.PenaltyPrice))
             .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
             .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
             .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
             .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
             .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
             .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
             .ForMember(dest => dest.MST_EMPLOYEE, opt => opt.MapFrom(src => src.MstEmployee))
             .ForMember(dest => dest.MST_EPAF, opt => opt.MapFrom(src => src.MstEpaf))
             .ForMember(dest => dest.MST_REASON, opt => opt.MapFrom(src => src.MstReason))
             .ForMember(dest => dest.MST_REMARK, opt => opt.MapFrom(src => src.MstRemark));

            AutoMapper.Mapper.CreateMap<WorkflowHistoryDto, CtfWorkflowDocumentInput>().IgnoreAllNonExisting()
             .ForMember(dest => dest.ActionType, opt => opt.MapFrom(src => src.ACTION))
             .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ACTION_BY))
             .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.FORM_ID));

            AutoMapper.Mapper.CreateMap<CtfWorkflowDocumentInput, WorkflowHistoryDto >().IgnoreAllNonExisting()
             .ForMember(dest => dest.ACTION, opt => opt.MapFrom(src => src.ActionType))
             .ForMember(dest => dest.ACTION_BY, opt => opt.MapFrom(src => src.UserId))
             .ForMember(dest => dest.FORM_ID, opt => opt.MapFrom(src => src.DocumentId));

            AutoMapper.Mapper.CreateMap<EpafDto, TraCtfDto>().IgnoreAllNonExisting()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
           .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EmployeeName))
           .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EfectiveDate))
           .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.CostCenter))
           .ForMember(dest => dest.GroupLevel, opt => opt.MapFrom(src => src.GroupLevel))
           .ForMember(dest => dest.EpafId, opt => opt.MapFrom(src => src.MstEpafId));
        }
    }
}
