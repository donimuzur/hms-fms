using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.CfmIdleReport
{
    public class CfmIdleReportBLL : ICfmIdleReportBLL
    {
        private CfmIdleReportService _cfmIdleReportService;
        private IUnitOfWork _uow;

        public CfmIdleReportBLL(IUnitOfWork Uow)
        {
            _uow = Uow;
            _cfmIdleReportService = new CfmIdleReportService(_uow);
        }

        public List<CfmIdleReportDto> GetCfmIdle(CfmIdleGetByParamInput filter)
        {
            var data = _cfmIdleReportService.GetCfmIdle(filter);
            var redata = Mapper.Map<List<CfmIdleReportDto>>(data);
            return redata;
        }
    }

}
