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
        private IWorkflowHistoryService _workflowHistory;

        public KpiMonitoringBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _kpiMonitoringService =new  KpiMonitoringService(_uow);
        }

        public List<KpiMonitoringDto> GetTransaction(KpiMonitoringGetByParamInput filter)
        {
            var data = _kpiMonitoringService.GetTransaction(filter);
            var redata = Mapper.Map<List<KpiMonitoringDto>>(data);
            return redata;
        }
    }
}
