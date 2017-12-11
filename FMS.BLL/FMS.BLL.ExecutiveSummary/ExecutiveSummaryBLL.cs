﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;

namespace FMS.BLL.ExecutiveSummary
{
    public class ExecutiveSummaryBLL : IExecutiveSummaryBLL
    {
        private IExecutiveSummaryService _ExecSummService;
        private IUnitOfWork _uow;

        public ExecutiveSummaryBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _ExecSummService = new ExecutiveSummaryService(_uow);
        }

        public List<NoVehicleDto> GetNoOfVehicleData(VehicleGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicle(filter);

            return Mapper.Map<List<NoVehicleDto>>(data);
        }

        public List<NoVehicleWtcDto> GetNoOfVehicleWtcData(VehicleWtcGetByParamInput filter)
        {
            var data = _ExecSummService.GetAllNoVehicleWtc(filter);

            return Mapper.Map<List<NoVehicleWtcDto>>(data);
        }
    }
}
