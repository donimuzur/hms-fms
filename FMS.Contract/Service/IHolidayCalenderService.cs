using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IHolidayCalenderService
    {
        List<MST_HOLIDAY_CALENDAR> GetHolidayCalender();
        MST_HOLIDAY_CALENDAR GetHolidayCalenderById(DateTime MstHolidayCalender);
        void save(MST_HOLIDAY_CALENDAR dbHolidayCaleder);
    }
}
