using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.AutoGR
{
    public class AutoGrBLL : IAutoGrBLL
    {
        private IUnitOfWork _uow;
        private IAutoGrService _grService;
        private IFleetService _fleetService;
        public AutoGrBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _grService = new AutoGrService(_uow);
            _fleetService = new FleetService(_uow);
        }

        public List<RptAutoGrDto> GetAutoGR(RptAutoGrInput rptAutoGrInput)
        {
            List<AUTO_GR> data = _grService.GetAutoGr(rptAutoGrInput);
            //_grService.get

            //var poAndLineList = data.Select(x=> x.PO_NUMBER + "_" + x.)
            var dataFleet = _fleetService.GetFleet();

            return Mapper.Map<List<RptAutoGrDto>>(data);
        }
    }
}
