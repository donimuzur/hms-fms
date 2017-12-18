using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.RptCcf
{
    public class RptCcfBLL : IRptCcfBLL
    {
        private IRptCCFService _RptCcfService;
        private IUnitOfWork _uow;

        public RptCcfBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _RptCcfService = new RptCcfService(uow);
        }

        public List<RptCCFDto> GetRptCcf(RptCCFInput filter)
        {
            var data = _RptCcfService.GetRptCcf(filter);
            return Mapper.Map<List<RptCCFDto>>(data);
        }

        public List<RptCCFDto> GetRptCcfData()
        {
            var data = _RptCcfService.GetRptCcfData();
            var redata = Mapper.Map<List<RptCCFDto>>(data);
            return redata;
        }

    }
}
