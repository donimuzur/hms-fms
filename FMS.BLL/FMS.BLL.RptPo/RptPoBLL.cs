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

namespace FMS.BLL.RptPo
{
    public class RptPoBLL : IRptPoBLL
    {
        private IRptPoService _RptPoService;
        private IUnitOfWork _uow;

        public RptPoBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _RptPoService = new RptPoService(uow);
        }

        public List<RptPODto> GetRptPo(RptPoByParamInput filter)
        {
            var data = _RptPoService.GetRptPo(filter);
            return Mapper.Map<List<RptPODto>>(data);
        }

        public List<RptPODto> GetRptPoData()
        {
            var data = _RptPoService.GetRptPoData();
            var redata = Mapper.Map<List<RptPODto>>(data);
            return redata;
        }
    }
}
