using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.KpiMonitoring
{
    public class KpiMonitoringBLL : IKpiMonitoringBLL
    {
        private IKpiMonitoringService _kpiMonitoringService;
        private IUnitOfWork _uow;

        public KpiMonitoringBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _kpiMonitoringService = new KpiMonitoringService(_uow);
        }

        public List<KpiMonitoringDto> GetTransaction(KpiMonitoringGetByParamInput filter)
        {
            var data = _kpiMonitoringService.GetTransaction(filter);
            var redata = Mapper.Map<List<KpiMonitoringDto>>(data);
            return redata;
        }

        public int? GetDifferentDays(DateTime? Day1, DateTime? Day2)
        {
            int? DiffDays = null;
            if (Day1 != null && Day2 != null)
            {
                var Day2Days = (int)(Day2.Value.Date - new DateTime(1900, 1, 1)).TotalDays;

                var Day1Days = (int)(Day1.Value.Date - new DateTime(1900, 1, 1)).TotalDays;

                DiffDays = Day2Days - Day1Days;

            }

            return DiffDays;
        }
    }
}
