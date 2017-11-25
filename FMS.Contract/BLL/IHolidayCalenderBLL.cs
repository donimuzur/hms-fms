using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IHolidayCalenderBLL
    {
        List<HolidayCalenderDto> GetHolidayCalender();
        HolidayCalenderDto GetholidayCalenderById(int MstHolidayDateId);
        void Save(HolidayCalenderDto HolidayCalenderDto);
        void Save(HolidayCalenderDto data, Login currentUser);
    }
}
