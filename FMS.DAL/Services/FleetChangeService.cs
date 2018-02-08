<<<<<<< HEAD
﻿using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
=======
﻿using System.Linq.Expressions;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
>>>>>>> story/23827-CR1-Point7-Add-dashboard-change-of-master-fleet
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD

namespace FMS.DAL.Services
{
    public class FleetChangeService : IFleetChangeService
    {
        private IGenericRepository<FLEET_CHANGE> _fleetChangeRepo;
        private IUnitOfWork _uow;

        public FleetChangeService(IUnitOfWork uow)
        {
            _uow = uow;
            _fleetChangeRepo = uow.GetGenericRepository<FLEET_CHANGE>();
        }

        public List<FLEET_CHANGE> GetListFleetChange()
        {
            var data =  _fleetChangeRepo.Get().Where(x => x.DATE_SEND == null).ToList();
            return data;
        }

        public void Save(FLEET_CHANGE dbFleetChange)
        {
            _fleetChangeRepo.InsertOrUpdate(dbFleetChange);
            _uow.SaveChanges();
=======
using FMS.Utils;
using System.Data.Entity;
using System.Diagnostics;

namespace FMS.DAL.Services
{
    public class FleetChangeService
    {
        public List<FLEET_CHANGE> GetList()
        {
            List<FLEET_CHANGE> list = null;
            try
            {
                using (FMSEntities db = new FMSEntities())
                {
                    list = db.FLEET_CHANGE.ToList();
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return list;
        }
        public List<FLEET_CHANGE> GetList(FLEET_CHANGE filter)
        {
            if(filter!=null)
            {
                try
                {
                    IEnumerable<FLEET_CHANGE> resultTemporary = null;
                    using (FMSEntities db = new FMSEntities())
                    {
                        resultTemporary = db.FLEET_CHANGE;
                        if (!String.IsNullOrEmpty(filter.EMPLOYEE_ID))
                            resultTemporary = resultTemporary.Where(x => x.EMPLOYEE_ID.Equals(filter.EMPLOYEE_ID,StringComparison.InvariantCultureIgnoreCase));
                        if (!String.IsNullOrEmpty(filter.EMPLOYEE_NAME))
                            resultTemporary = resultTemporary.Where(x => x.EMPLOYEE_NAME.Equals(filter.EMPLOYEE_NAME, StringComparison.InvariantCultureIgnoreCase));
                        if (!String.IsNullOrEmpty(filter.POLICE_NUMBER))
                            resultTemporary = resultTemporary.Where(x => x.POLICE_NUMBER.Equals(filter.POLICE_NUMBER, StringComparison.InvariantCultureIgnoreCase));
                        if (filter.FLEET_CHANGE_ID != default(long))
                            resultTemporary = resultTemporary.Where(x => x.FLEET_CHANGE_ID == filter.FLEET_CHANGE_ID);
                        if (!String.IsNullOrEmpty(filter.FIELD_NAME))
                            resultTemporary = resultTemporary.Where(x => x.FIELD_NAME.Equals(filter.FIELD_NAME, StringComparison.InvariantCultureIgnoreCase));
                        if (filter.FLEET_ID != null)
                            resultTemporary = resultTemporary.Where(x => x.FLEET_ID == filter.FLEET_ID);
                        if (!String.IsNullOrEmpty(filter.DATA_BEFORE))
                            resultTemporary = resultTemporary.Where(x => x.DATA_BEFORE.Equals(filter.DATA_BEFORE, StringComparison.InvariantCultureIgnoreCase));
                        if (!String.IsNullOrEmpty(filter.DATA_AFTER))
                            resultTemporary = resultTemporary.Where(x => x.DATA_AFTER.Equals(filter.DATA_AFTER, StringComparison.InvariantCultureIgnoreCase));
                        if (!String.IsNullOrEmpty(filter.CHASIS_NUMBER))
                            resultTemporary = resultTemporary.Where(x => x.CHASIS_NUMBER.Equals(filter.CHASIS_NUMBER, StringComparison.InvariantCultureIgnoreCase));
                        if (filter.CHANGE_DATE != null)
                            resultTemporary = resultTemporary.Where(x => x.CHANGE_DATE == filter.CHANGE_DATE);
                        if (filter.DATE_SEND != null)
                            resultTemporary = resultTemporary.Where(x => x.DATE_SEND == filter.DATE_SEND);
                    }
                    return resultTemporary != null ? resultTemporary.ToList() : new List<FLEET_CHANGE>() { };
                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }
            return GetList();
>>>>>>> story/23827-CR1-Point7-Add-dashboard-change-of-master-fleet
        }
    }
}
