using AutoMapper;
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

namespace FMS.BLL.VehicleOverallReport
{
    public class VehicleOverallReportBLL : IVehicleOverallReportBLL
    {
        private IVehicleOverallReportService _vehicleOverallReportService;
        private IUnitOfWork _uow;

        public VehicleOverallReportBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _vehicleOverallReportService = new VehicleOverallReportService(_uow);
        }

        public List<VehicleOverallReportDto> GetVehicle(VehicleOverallReportGetByParamInput filter)
        {
            var data = _vehicleOverallReportService.GetVehicle(filter);
            var redata = Mapper.Map<List<VehicleOverallReportDto>>(data);
            return redata;
        }
    }
}
