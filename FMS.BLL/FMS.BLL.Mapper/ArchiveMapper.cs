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

        }
    }
}
