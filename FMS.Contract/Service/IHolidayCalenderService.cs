using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IHolidayCalenderService
    {
        List<MST_HOLIDAY_CALENDAR> GetHolidayCalender();
        MST_HOLIDAY_CALENDAR GetHolidayCalenderById(int MstHolidayCalenderId);
        void save(MST_HOLIDAY_CALENDAR dbHolidayCaleder);
        void save(MST_HOLIDAY_CALENDAR dbHolidayCaleder, Login userLogin);
        List<MST_HOLIDAY_CALENDAR> GetHolidayCalender(HolidayCalenderParamInput filter);
    }
}
