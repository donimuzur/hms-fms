﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;
using FMS.BusinessObject.Inputs;
using System.Linq.Expressions;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class RptFuelService : IRptFuelService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<FUEL_REPORT_DATA> _rptFuelRepository;

        public RptFuelService(IUnitOfWork uow)
        {
            _uow = uow;

            _rptFuelRepository = _uow.GetGenericRepository<FUEL_REPORT_DATA>();
        }

        public List<FUEL_REPORT_DATA> GetRptFuel(RptFuelByParamInput filter)
        {
            Expression<Func<FUEL_REPORT_DATA, bool>> queryFilter = PredicateHelper.True<FUEL_REPORT_DATA>();

            if (filter != null)
            {
                if (filter.MonthFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_MONTH == filter.MonthFrom);
                }
                if (filter.YearFrom > 0)
                {
                    queryFilter = queryFilter.And(c => c.REPORT_YEAR == filter.YearFrom);
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => c.VEHICLE_TYPE.ToUpper() == filter.VehicleType.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => c.FUNCTION.ToUpper() == filter.Function.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.CostCenter))
                {
                    queryFilter = queryFilter.And(c => c.COST_CENTER.ToUpper() == filter.CostCenter.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.PoliceNumber))
                {
                    queryFilter = queryFilter.And(c => c.POLICE_NUMBER.ToUpper() == filter.PoliceNumber.ToUpper());
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    queryFilter = queryFilter.And(c => c.REGIONAL.ToUpper() == filter.Regional.ToUpper());
                }
            }

            return _rptFuelRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
