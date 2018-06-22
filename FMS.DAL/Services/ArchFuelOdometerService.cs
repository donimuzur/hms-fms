using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class ArchFuelOdometerService : IArchFuelOdometerService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_FUEL_ODOMETER> _archFuelOdometerRepository;

        public ArchFuelOdometerService(IUnitOfWork   uow)
        {
            _uow = uow;
            _archFuelOdometerRepository = uow.GetGenericRepository<ARCH_MST_FUEL_ODOMETER>();
        }
        public void Save(ARCH_MST_FUEL_ODOMETER db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_FUEL_ODOMETER>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }

        public List<ARCH_MST_FUEL_ODOMETER> GetFuelOdometerByParam(FuelOdometerParamInput param)
        {
            Expression<Func<ARCH_MST_FUEL_ODOMETER, bool>> queryFilterFuelOdometer = null;
            queryFilterFuelOdometer = c => c.IS_ACTIVE == true;
            if (param != null)
            {

                if (!string.IsNullOrEmpty(param.Status))
                {
                    if (param.Status == "True")
                    {
                        queryFilterFuelOdometer = c => c.IS_ACTIVE == true;
                    }
                    else if (param.Status == "False")
                    {
                        queryFilterFuelOdometer = c => c.IS_ACTIVE == false;
                    }
                }

                if (!string.IsNullOrEmpty(param.EmployeeId))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.EMPLOYEE_ID == param.EmployeeId);


                }

                if (!string.IsNullOrEmpty(param.EmployeeName))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.EMPLOYEE_NAME == param.EmployeeName);


                }

                if (!string.IsNullOrEmpty(param.PoliceNumber))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.POLICE_NUMBER == param.PoliceNumber);


                }

                if (!string.IsNullOrEmpty(param.ClaimComment))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.CLAIM_COMMENT == param.ClaimComment);


                }

                if (!string.IsNullOrEmpty(param.ClaimType))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.CLAIM_TYPE == param.ClaimType);


                }

                if (!string.IsNullOrEmpty(param.CostCenter))
                {
                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.COST_CENTER == param.CostCenter);


                }

                if (!string.IsNullOrEmpty(param.EcsRmbTransId))
                {
                    var ecsRmbTransId = Convert.ToInt32(param.EcsRmbTransId);

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.ECS_RMB_TRANSID == ecsRmbTransId);


                }

                if (!string.IsNullOrEmpty(param.VehicleType))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.VEHICLE_TYPE == param.VehicleType);


                }

                if (!string.IsNullOrEmpty(param.LastKM))
                {
                    var lastKM = Convert.ToInt64(param.LastKM);
                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.LAST_KM == lastKM);

                }


                if (!string.IsNullOrEmpty(param.SeqNumber))
                {
                    var seqNumber = Convert.ToInt32(param.SeqNumber);

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.SEQ_NUMBER == seqNumber);


                }

                if (!string.IsNullOrEmpty(param.DateOfCost))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.DATE_OF_COST == Convert.ToDateTime(param.DateOfCost));

                }

                if (!string.IsNullOrEmpty(param.PostedTime))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.POSTED_TIME == Convert.ToDateTime(param.PostedTime));


                }
            }
            return _archFuelOdometerRepository.Get(queryFilterFuelOdometer, null, "").ToList();
        }
    }
}
