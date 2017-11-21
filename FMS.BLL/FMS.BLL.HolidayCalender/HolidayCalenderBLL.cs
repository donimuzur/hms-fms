using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject;
using FMS.Contract.BLL;
using FMS.BusinessObject.Business;


namespace FMS.BLL.HolidayCalender
{
    public class HolidayCalenderBLL :IHolidayCalenderBLL
    {
        private IHolidayCalenderService _holidayCalenderService;
        private IUnitOfWork _uow;
        public HolidayCalenderBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _holidayCalenderService = new HolidayCalenderService(uow);
        }

        public List<HolidayCalenderDto> GetHolidayCalender()
        {
            var data = _holidayCalenderService.GetHolidayCalender();
            var retData = Mapper.Map<List<HolidayCalenderDto>>(data);
            return retData;
        }

        public HolidayCalenderDto GetholidayCalenderById(int MstHolidayDateId)
        {
            var data = _holidayCalenderService.GetHolidayCalenderById(MstHolidayDateId);
            var retData = Mapper.Map<HolidayCalenderDto>(data);

            return retData;
        }

        public void Save(HolidayCalenderDto HolidayCalenderDto)
        {
            var dbHolidayCalender = Mapper.Map<MST_HOLIDAY_CALENDAR>(HolidayCalenderDto);
            _holidayCalenderService.save(dbHolidayCalender);
        }

        public void Save(HolidayCalenderDto Dto, Login userLogin)
        {
            var dbHolidayCalender = Mapper.Map<MST_HOLIDAY_CALENDAR>(Dto);
            _holidayCalenderService.save(dbHolidayCalender, userLogin);
        }
    }
}
