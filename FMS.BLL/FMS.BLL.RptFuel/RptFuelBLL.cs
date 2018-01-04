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

namespace FMS.BLL.RptFuel
{
    public class RptFuelBLL : IRptFuelBLL
    {
        private IRptFuelService _RptFuelService;
        private IUnitOfWork _uow;

        public RptFuelBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _RptFuelService = new RptFuelService(uow);
        }

        public List<RptFuelDto> GetRptFuel(RptFuelByParamInput filter)
        {
            var data = _RptFuelService.GetRptFuel(filter);
            return Mapper.Map<List<RptFuelDto>>(data);
        }

        public List<RptFuelDto> GetRptFuelData()
        {
            var data = _RptFuelService.GetRptFuelData();
            return Mapper.Map<List<RptFuelDto>>(data);
        }
    }
}
