﻿using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface IVehicleOverallReportBLL
    {
        List<VehicleOverallReportDto> GetVehicle(VehicleOverallReportGetByParamInput filter);
    }
}
