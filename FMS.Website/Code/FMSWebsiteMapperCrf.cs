﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeCRF()
        {
            Mapper.CreateMap<TraCrfDto, TraCrfItemDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.TraCrfId, opt => opt.MapFrom(src => src.TRA_CRF_ID))
                .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
                .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.CostCenterNew, opt => opt.MapFrom(src => src.COST_CENTER_NEW))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DELIV_ADDRESS))
                .ForMember(dest => dest.DeliveryCity, opt => opt.MapFrom(src => src.DELIV_CITY))
                .ForMember(dest => dest.DeliveryPhone, opt => opt.MapFrom(src => src.DELIV_PHONE))
                .ForMember(dest => dest.DeliveryPic, opt => opt.MapFrom(src => src.DELIV_PIC))
                .ForMember(dest => dest.EndPeriod, opt => opt.MapFrom(src => src.END_PERIOD))
                .ForMember(dest => dest.EpafId, opt => opt.MapFrom(src => src.EPAF_ID))
                .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => src.EXPECTED_DATE))
                .ForMember(dest => dest.LocationCity, opt => opt.MapFrom(src => src.LOCATION_CITY))
                .ForMember(dest => dest.LocationCityNew, opt => opt.MapFrom(src => src.LOCATION_CITY_NEW))
                .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.MODEL))
                .ForMember(dest => dest.PoLine, opt => opt.MapFrom(src => src.PO_LINE))
                .ForMember(dest => dest.PoNumber, opt => opt.MapFrom(src => src.PO_NUMBER))
                .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PRICE))
                .ForMember(dest => dest.RelocationType, opt => opt.MapFrom(src => src.RELOCATION_TYPE))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.StartPeriod, opt => opt.MapFrom(src => src.START_PERIOD))
                .ForMember(dest => dest.TemporaryDeliverableDate, opt => opt.MapFrom(src => src.TEMPORARY_DELIVERABLE_DATE))
                .ForMember(dest => dest.VendorId, opt => opt.MapFrom(src => src.VENDOR_ID))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.VENDOR_NAME))
                .ForMember(dest => dest.WithdAddress, opt => opt.MapFrom(src => src.WITHD_ADDRESS))
                .ForMember(dest => dest.WithdCity, opt => opt.MapFrom(src => src.WITHD_CITY))
                .ForMember(dest => dest.WithdDateTime, opt => opt.MapFrom(src => src.WITHD_DATETIME))
                .ForMember(dest => dest.WithdPhone, opt => opt.MapFrom(src => src.WITHD_PHONE))
                .ForMember(dest => dest.WithdPic, opt => opt.MapFrom(src => src.WITHD_PIC))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.ChangePoliceNumber, opt => opt.MapFrom(src => src.CHANGE_POLICE_NUMBER))
                ;

            Mapper.CreateMap<TraCrfItemDetails, TraCrfDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.TRA_CRF_ID, opt => opt.MapFrom(src => src.TraCrfId))
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.DocumentNumber))
                .ForMember(dest => dest.DOCUMENT_STATUS, opt => opt.MapFrom(src => src.DocumentStatus))
                .ForMember(dest => dest.EMPLOYEE_ID, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EMPLOYEE_NAME, opt => opt.MapFrom(src => src.EmployeeName))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.EFFECTIVE_DATE, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.VEHICLE_USAGE, opt => opt.MapFrom(src => src.VehicleUsage))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.COST_CENTER_NEW, opt => opt.MapFrom(src => src.CostCenterNew))
                .ForMember(dest => dest.DELIV_ADDRESS, opt => opt.MapFrom(src => src.DeliveryAddress))
                .ForMember(dest => dest.DELIV_CITY, opt => opt.MapFrom(src => src.DeliveryCity))
                .ForMember(dest => dest.DELIV_PHONE, opt => opt.MapFrom(src => src.DeliveryPhone))
                .ForMember(dest => dest.DELIV_PIC, opt => opt.MapFrom(src => src.DeliveryPic))
                .ForMember(dest => dest.END_PERIOD, opt => opt.MapFrom(src => src.EndPeriod))
                .ForMember(dest => dest.EPAF_ID, opt => opt.MapFrom(src => src.EpafId))
                .ForMember(dest => dest.EXPECTED_DATE, opt => opt.MapFrom(src => src.ExpectedDate))
                .ForMember(dest => dest.LOCATION_CITY, opt => opt.MapFrom(src => src.LocationCity))
                .ForMember(dest => dest.LOCATION_CITY_NEW, opt => opt.MapFrom(src => src.LocationCityNew))
                .ForMember(dest => dest.MANUFACTURER, opt => opt.MapFrom(src => src.Manufacturer))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.PO_LINE, opt => opt.MapFrom(src => src.PoLine))
                .ForMember(dest => dest.PO_NUMBER, opt => opt.MapFrom(src => src.PoNumber))
                .ForMember(dest => dest.POLICE_NUMBER, opt => opt.MapFrom(src => src.PoliceNumber))
                .ForMember(dest => dest.PRICE, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.RELOCATION_TYPE, opt => opt.MapFrom(src => src.RelocationType))
                .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.SERIES))
                .ForMember(dest => dest.START_PERIOD, opt => opt.MapFrom(src => src.StartPeriod))
                .ForMember(dest => dest.TEMPORARY_DELIVERABLE_DATE, opt => opt.MapFrom(src => src.TemporaryDeliverableDate))
                .ForMember(dest => dest.VENDOR_ID, opt => opt.MapFrom(src => src.VendorId))
                .ForMember(dest => dest.VENDOR_NAME, opt => opt.MapFrom(src => src.VendorName))
                .ForMember(dest => dest.WITHD_ADDRESS, opt => opt.MapFrom(src => src.WithdAddress))
                .ForMember(dest => dest.WITHD_CITY, opt => opt.MapFrom(src => src.WithdCity))
                .ForMember(dest => dest.WITHD_DATETIME, opt => opt.MapFrom(src => src.WithdDateTime))
                .ForMember(dest => dest.WITHD_PHONE, opt => opt.MapFrom(src => src.WithdPhone))
                .ForMember(dest => dest.WITHD_PIC, opt => opt.MapFrom(src => src.WithdPic))
                .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BODY_TYPE))
                .ForMember(dest => dest.CHANGE_POLICE_NUMBER, opt => opt.MapFrom(src => src.ChangePoliceNumber))
                ;
            //Mapper.CreateMap<TraCrfItemDetails, TraCrfDto>().ReverseMap().IgnoreAllNonExisting()
            //    .ForMember(dest => dest.TraCrfId, opt => opt.MapFrom(src => src.TRA_CRF_ID))
            //    .ForMember(dest => dest.DocumentNumber, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER))
            //    .ForMember(dest => dest.DocumentStatus, opt => opt.MapFrom(src => src.DOCUMENT_STATUS))
            //    .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EMPLOYEE_ID))
            //    .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.EMPLOYEE_NAME))
            //    .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
            //    .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
            //    .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EFFECTIVE_DATE))
            //    .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
            //    .ForMember(dest => dest.VehicleUsage, opt => opt.MapFrom(src => src.VEHICLE_USAGE))
            //    .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
            //    .ForMember(dest => dest.CostCenterNew, opt => opt.MapFrom(src => src.COST_CENTER_NEW))
            //    .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DELIV_ADDRESS))
            //    .ForMember(dest => dest.DeliveryCity, opt => opt.MapFrom(src => src.DELIV_CITY))
            //    .ForMember(dest => dest.DeliveryPhone, opt => opt.MapFrom(src => src.DELIV_PHONE))
            //    .ForMember(dest => dest.DeliveryPic, opt => opt.MapFrom(src => src.DELIV_PIC))
            //    .ForMember(dest => dest.EndPeriod, opt => opt.MapFrom(src => src.END_PERIOD))
            //    .ForMember(dest => dest.EpafId, opt => opt.MapFrom(src => src.EPAF_ID))
            //    .ForMember(dest => dest.ExpectedDate, opt => opt.MapFrom(src => src.EXPECTED_DATE))
            //    .ForMember(dest => dest.LocationCity, opt => opt.MapFrom(src => src.LOCATION_CITY))
            //    .ForMember(dest => dest.LocationCityNew, opt => opt.MapFrom(src => src.LOCATION_CITY_NEW))
            //    .ForMember(dest => dest.Manufacturer, opt => opt.MapFrom(src => src.MANUFACTURER))
            //    .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.MODEL))
            //    .ForMember(dest => dest.PoLine, opt => opt.MapFrom(src => src.PO_LINE))
            //    .ForMember(dest => dest.PoNumber, opt => opt.MapFrom(src => src.PO_NUMBER))
            //    .ForMember(dest => dest.PoliceNumber, opt => opt.MapFrom(src => src.POLICE_NUMBER))
            //    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PRICE))
            //    .ForMember(dest => dest.RelocationType, opt => opt.MapFrom(src => src.RELOCATION_TYPE))
            //    .ForMember(dest => dest.SERIES, opt => opt.MapFrom(src => src.SERIES))
            //    .ForMember(dest => dest.StartPeriod, opt => opt.MapFrom(src => src.START_PERIOD))
            //    .ForMember(dest => dest.TemporaryDeliverableDate, opt => opt.MapFrom(src => src.TEMPORARY_DELIVERABLE_DATE))
            //    .ForMember(dest => dest.VendorId, opt => opt.MapFrom(src => src.VENDOR_ID))
            //    .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.VENDOR_NAME))
            //    .ForMember(dest => dest.WithdAddress, opt => opt.MapFrom(src => src.WITHD_ADDRESS))
            //    .ForMember(dest => dest.WithdCity, opt => opt.MapFrom(src => src.WITHD_CITY))
            //    .ForMember(dest => dest.WithdDateTime, opt => opt.MapFrom(src => src.WITHD_DATETIME))
            //    .ForMember(dest => dest.WithdPhone, opt => opt.MapFrom(src => src.WITHD_PHONE))
            //    .ForMember(dest => dest.WithdPic, opt => opt.MapFrom(src => src.WITHD_PIC))
            //    .ForMember(dest => dest.BODY_TYPE, opt => opt.MapFrom(src => src.BODY_TYPE))
            //    .ForMember(dest => dest.ChangePoliceNumber, opt => opt.MapFrom(src => src.CHANGE_POLICE_NUMBER))
            //    ;
            

            Mapper.CreateMap<EpafDto, TraCrfEpafItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.EpafId, opt => opt.MapFrom(src => src.MstEpafId))
                .ForMember(dest => dest.CrfNumber, opt => opt.MapFrom(src => src.CrfNumber))
                .ForMember(dest => dest.CrfId, opt => opt.MapFrom(src => src.CrfId))
                .ForMember(dest=> dest.EffectiveDate, opt => opt.MapFrom(src=> src.EfectiveDate))
                .ForMember(dest => dest.CrfModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.CrfModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CurrentLocation, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.RelocateLocation, opt => opt.MapFrom(src => src.CityNew));
            Mapper.CreateMap<TraCrfEpafItem,EpafDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstEpafId, opt => opt.MapFrom(src => src.EpafId))
                .ForMember(dest => dest.CrfNumber, opt => opt.MapFrom(src => src.CrfNumber))
                .ForMember(dest => dest.CrfId, opt => opt.MapFrom(src => src.CrfId))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.CrfModifiedBy))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.CrfModifiedDate))
                ;
        }
    }
}