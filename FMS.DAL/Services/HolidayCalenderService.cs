using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
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

        public MST_HOLIDAY_CALENDAR GetHolidayCalenderById(DateTime MstHolidayCalender)
        {
            return _holidaycalenderRepository.GetByID(MstHolidayCalender);
        }

        public void save(MST_HOLIDAY_CALENDAR dbHolidayCaleder)
        {
            _uow.GetGenericRepository<MST_HOLIDAY_CALENDAR>().InsertOrUpdate(dbHolidayCaleder);
            _uow.SaveChanges();
        }
    }
}
