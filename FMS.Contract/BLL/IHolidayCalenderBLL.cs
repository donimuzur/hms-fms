using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IHolidayCalenderBLL
    {
        List<HolidayCalenderDto> GetHolidayCalender();
        HolidayCalenderDto GetholidayCalenderById(int MstHolidayDateId);
        void Save(HolidayCalenderDto HolidayCalenderDto);
        void Save(HolidayCalenderDto data, Login currentUser);
        List<HolidayCalenderDto> GetHolidayCalender(HolidayCalenderParamInput holidayCalenderParamInput);
    }
}
