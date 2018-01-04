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
    public class HolidayCalenderService : IHolidayCalenderService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_HOLIDAY_CALENDAR> _holidaycalenderRepository;

        public HolidayCalenderService(IUnitOfWork uow)
        {
            _uow = uow;
            _holidaycalenderRepository = _uow.GetGenericRepository<MST_HOLIDAY_CALENDAR>();
        }

        public List<MST_HOLIDAY_CALENDAR> GetHolidayCalender()
        {
            return _holidaycalenderRepository.Get().ToList();
        }

        public MST_HOLIDAY_CALENDAR GetHolidayCalenderById(int MstHolidayCalenderId)
        {
            return _holidaycalenderRepository.GetByID(MstHolidayCalenderId);
        }

        public void save(MST_HOLIDAY_CALENDAR dbHolidayCaleder)
        {
            _uow.GetGenericRepository<MST_HOLIDAY_CALENDAR>().InsertOrUpdate(dbHolidayCaleder);
            _uow.SaveChanges();
        }

        public void save(MST_HOLIDAY_CALENDAR dbHolidayCaleder, Login userLogin)
        {
            _holidaycalenderRepository.InsertOrUpdate(dbHolidayCaleder, userLogin, Enums.MenuList.MasterHoliday);
            _uow.SaveChanges();
        }
    }
}
