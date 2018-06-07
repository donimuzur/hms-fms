using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchHolidayCalendarService
    {
        void Save(ARCH_MST_HOLIDAY_CALENDAR db, Login userlogin);
    }
}
