using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
