using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;


namespace FMS.DAL.Services
{
    public class FuelOdometerService : IFuelOdometerService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_FUEL_ODOMETER> _FuelOdometerRepository;
        public FuelOdometerService(IUnitOfWork uow)
        {
            _uow = uow;
            _FuelOdometerRepository = _uow.GetGenericRepository<MST_FUEL_ODOMETER>();
        }

        public List<MST_FUEL_ODOMETER> GetFuelOdometer()
        {
            return _FuelOdometerRepository.Get().ToList();
        }
        
        public MST_FUEL_ODOMETER GetByID(long MstFuelOdometerID)
        {
            return _FuelOdometerRepository.GetByID(MstFuelOdometerID);
        }

        public List<MST_FUEL_ODOMETER> GetFuelOdometerByParam(FuelOdometerParamInput input)
        {
            Expression<Func<MST_FUEL_ODOMETER, bool>> queryFilterFuelOdometer = null;
            queryFilterFuelOdometer = c => c.IS_ACTIVE == true;
            if (input != null)
            {

                if (!string.IsNullOrEmpty(input.Status))
                {
                    if (input.Status == "True")
                    {
                        queryFilterFuelOdometer = c => c.IS_ACTIVE == true;
                    }
                    else if (input.Status == "False")
                    {
                        queryFilterFuelOdometer = c => c.IS_ACTIVE == false;
                    }
                }

                if (!string.IsNullOrEmpty(input.EmployeeId))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.EMPLOYEE_ID == input.EmployeeId);


                }

                if (!string.IsNullOrEmpty(input.EmployeeName))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.EMPLOYEE_NAME == input.EmployeeName);


                }

                if (!string.IsNullOrEmpty(input.PoliceNumber))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.POLICE_NUMBER == input.PoliceNumber);


                }

                if (!string.IsNullOrEmpty(input.ClaimComment))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.CLAIM_COMMENT == input.ClaimComment);


                }

                if (!string.IsNullOrEmpty(input.ClaimType))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.CLAIM_TYPE == input.ClaimType);


                }

                if (!string.IsNullOrEmpty(input.CostCenter))
                {
                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.COST_CENTER == input.CostCenter);


                }

                if (!string.IsNullOrEmpty(input.EcsRmbTransId))
                {
                    var ecsRmbTransId = Convert.ToInt32(input.EcsRmbTransId);

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.ECS_RMB_TRANSID == ecsRmbTransId);


                }

                if (!string.IsNullOrEmpty(input.VehicleType))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.VEHICLE_TYPE == input.VehicleType);


                }

                if (!string.IsNullOrEmpty(input.LastKM))
                {
                    var lastKM = Convert.ToInt64(input.LastKM);
                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.LAST_KM == lastKM);
                    
                }


                if (!string.IsNullOrEmpty(input.SeqNumber))
                {
                    var seqNumber = Convert.ToInt32(input.SeqNumber);

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.SEQ_NUMBER == seqNumber);


                }

                if (!string.IsNullOrEmpty(input.DateOfCost))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.DATE_OF_COST == Convert.ToDateTime(input.DateOfCost));

                }

                if (!string.IsNullOrEmpty(input.PostedTime))
                {

                    queryFilterFuelOdometer = queryFilterFuelOdometer.And(c => c.POSTED_TIME == Convert.ToDateTime(input.PostedTime));


                }
            }
            return _FuelOdometerRepository.Get(queryFilterFuelOdometer, null, "").ToList();
        }

        public void save(MST_FUEL_ODOMETER dbSetting, Login userlogin)
        {
            _uow.GetGenericRepository<MST_FUEL_ODOMETER>().InsertOrUpdate(dbSetting, userlogin, Enums.MenuList.MasterFuelOdoMeter);
            _uow.SaveChanges();
        }
    }
}
