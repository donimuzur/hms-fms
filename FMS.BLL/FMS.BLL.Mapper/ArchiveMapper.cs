using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class ArchiveMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<ARCH_MST_COST_OB, MST_COST_OB>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_COST_OB, ARCH_MST_COST_OB>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_EMPLOYEE, MST_EMPLOYEE>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_EMPLOYEE, ARCH_MST_EMPLOYEE>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_FLEET, MST_FLEET>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_FLEET, ARCH_MST_FLEET>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_EPAF, MST_EPAF>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_EPAF, ARCH_MST_EPAF>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_FUEL_ODOMETER, MST_FUEL_ODOMETER>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_FUEL_ODOMETER, ARCH_MST_FUEL_ODOMETER>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_FUNCTION_GROUP, MST_FUNCTION_GROUP>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_FUNCTION_GROUP, ARCH_MST_FUNCTION_GROUP>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_GS, MST_GS>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_GS, ARCH_MST_GS>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_HOLIDAY_CALENDAR, MST_HOLIDAY_CALENDAR>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_HOLIDAY_CALENDAR, ARCH_MST_HOLIDAY_CALENDAR>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_LOCATION_MAPPING, MST_LOCATION_MAPPING>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_LOCATION_MAPPING, ARCH_MST_LOCATION_MAPPING>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_PENALTY, MST_PENALTY>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_PENALTY, ARCH_MST_PENALTY>().IgnoreAllNonExisting();
            
            AutoMapper.Mapper.CreateMap<ARCH_MST_PRICELIST, MST_PRICELIST>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_PRICELIST, ARCH_MST_PRICELIST>().IgnoreAllNonExisting();
            
            AutoMapper.Mapper.CreateMap<ARCH_MST_SALES_VOLUME, MST_SALES_VOLUME>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_SALES_VOLUME, ARCH_MST_SALES_VOLUME>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_MST_VEHICLE_SPECT, MST_VEHICLE_SPECT>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<MST_VEHICLE_SPECT, ARCH_MST_VEHICLE_SPECT>().IgnoreAllNonExisting();


            ///---Transaction Mapper---///
            AutoMapper.Mapper.CreateMap<ARCH_TRA_CSF, TRA_CSF>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CSF, ARCH_TRA_CSF>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CRF, TRA_CRF>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CRF, ARCH_TRA_CRF>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CTF, TRA_CTF>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CTF, ARCH_TRA_CTF>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CCF, TRA_CCF>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CCF, ARCH_TRA_CCF>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CAF, TRA_CAF>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CAF, ARCH_TRA_CAF>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_TEMPORARY, TRA_TEMPORARY>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_TEMPORARY, ARCH_TRA_TEMPORARY>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CTF_EXTEND, TRA_CTF_EXTEND>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CTF_EXTEND, ARCH_TRA_CTF_EXTEND>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CCF_DETAIL, TRA_CCF_DETAIL>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CCF_DETAIL, ARCH_TRA_CCF_DETAIL>().IgnoreAllNonExisting();

            AutoMapper.Mapper.CreateMap<ARCH_TRA_CAF_PROGRESS, TRA_CAF_PROGRESS>().IgnoreAllNonExisting();
            AutoMapper.Mapper.CreateMap<TRA_CAF_PROGRESS, ARCH_TRA_CAF_PROGRESS>().IgnoreAllNonExisting();

        }
    }
}
