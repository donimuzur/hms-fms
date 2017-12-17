using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeKpiMonitoring()
        {
            Mapper.CreateMap<KpiMonitoringItem, KpiMonitoringDto>().IgnoreAllNonExisting();
            Mapper.CreateMap<KpiMonitoringDto, KpiMonitoringItem>().IgnoreAllNonExisting();

            Mapper.CreateMap<KpiMonitoringGetByParamInput, KpiReportSearchView>().IgnoreAllNonExisting();
            Mapper.CreateMap<KpiReportSearchView, KpiMonitoringGetByParamInput>().IgnoreAllNonExisting();
            
        }
    }
}