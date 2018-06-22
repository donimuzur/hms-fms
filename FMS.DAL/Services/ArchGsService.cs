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
    public class ArchGsService : IArchGsService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_GS> _archGsRepository;

        public ArchGsService(IUnitOfWork uow)
        {
            _uow = uow;
            _archGsRepository = uow.GetGenericRepository<ARCH_MST_GS>();
        }

        public List<ARCH_MST_GS> GetGsByParam(GSParamInput input)
        {
            Expression<Func<ARCH_MST_GS, bool>> queryFilter = PredicateHelper.True<ARCH_MST_GS>();

            if (input != null)
            {

                if (!string.IsNullOrEmpty(input.VehicleUsage))
                {
                    var listFunction = input.VehicleUsage.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VEHICLE_USAGE == null ? "" : c.VEHICLE_USAGE.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.PoliceNumber))
                {
                    var listFunction = input.PoliceNumber.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.POLICE_NUMBER == null ? "" : c.POLICE_NUMBER.ToUpper())));
                }
                if (!string.IsNullOrEmpty(input.EmployeeName))
                {
                    var listFunction = input.EmployeeName.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.EMPLOYEE_NAME == null ? "" : c.EMPLOYEE_NAME.ToUpper())));
                }
            }
            return _archGsRepository.Get(queryFilter, null, "").ToList();
        }
        public void Save(ARCH_MST_GS db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_GS>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
