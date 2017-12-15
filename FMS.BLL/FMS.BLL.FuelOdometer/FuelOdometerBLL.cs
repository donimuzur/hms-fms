using FMS.Contract.BLL;
using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.Service;
using FMS.Contract;
using FMS.DAL.Services;
using FMS.BusinessObject.Dto;
using AutoMapper;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.FuelOdometer
{
    public class FuelOdometerBLL :IFuelOdometerBLL
    {
        private IFuelOdometerService _fuelodometerService;
        private IUnitOfWork _uow;
        public FuelOdometerBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _fuelodometerService = new FuelOdometerService(uow);
        }

        public List<FuelOdometerDto> GetFuelOdometer()
        {
            var data = _fuelodometerService.GetFuelOdometer();
            var retData = Mapper.Map<List<FuelOdometerDto>>(data);
            return retData;
        }

        public List<FuelOdometerDto> GetFuelOdometerByParam(FuelOdometerParamInput param)
        {
            var data = _fuelodometerService.GetFuelOdometerByParam(param);
            return Mapper.Map<List<FuelOdometerDto>>(data);
        }

        public FuelOdometerDto GetByID(long MstFuelOdometerID)
        {
            var data = _fuelodometerService.GetByID(MstFuelOdometerID);
            return Mapper.Map<FuelOdometerDto>(data);
        }
    }
}
