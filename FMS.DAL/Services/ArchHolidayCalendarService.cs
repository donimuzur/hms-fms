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
    public class ArchHolidayCalendarService : IArchHolidayCalendarService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_HOLIDAY_CALENDAR> _archHolidayCalendarRepository;
        public ArchHolidayCalendarService(IUnitOfWork uow)
        {
            _uow = uow;
            _archHolidayCalendarRepository = uow.GetGenericRepository<ARCH_MST_HOLIDAY_CALENDAR>();
        }
        public void Save(ARCH_MST_HOLIDAY_CALENDAR db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_HOLIDAY_CALENDAR>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_MST_HOLIDAY_CALENDAR> GetHolidayCalender(HolidayCalenderParamInput filter)
        {
            Expression<Func<ARCH_MST_HOLIDAY_CALENDAR, bool>> queryFilter = c => c.IS_ACTIVE == true;

            if (filter != null)
            {

                if (filter.DateFrom != null)
                {
                    queryFilter = queryFilter.And(c => c.MST_HOLIDAY_DATE >= filter.DateFrom);
                }
                if (filter.DateTo != null)
                {
                    queryFilter = queryFilter.And(c => c.MST_HOLIDAY_DATE <= filter.DateTo);
                }
                if (!string.IsNullOrEmpty(filter.Description))
                {
                    queryFilter = queryFilter.And(c => c.DESCRIPTION.Contains(filter.Description));
                }
            }

            return _archHolidayCalendarRepository.Get(queryFilter, null, "").ToList();
        }
        public ARCH_MST_HOLIDAY_CALENDAR GetHolidayCalenderById(int mstHolidayDateId)
        {
            return _archHolidayCalendarRepository.GetByID(mstHolidayDateId);
        }
    }
}
