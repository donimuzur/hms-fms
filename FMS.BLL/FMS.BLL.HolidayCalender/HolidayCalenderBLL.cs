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
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.HolidayCalender
{
    public class HolidayCalenderBLL :IHolidayCalenderBLL
    {
        private IHolidayCalenderService _holidayCalenderService;
        private IArchHolidayCalendarService _archHolidayCalenderService;
        private IUnitOfWork _uow;
        public HolidayCalenderBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _holidayCalenderService = new HolidayCalenderService(uow);
            _archHolidayCalenderService = new ArchHolidayCalendarService(uow);
        }

        public List<HolidayCalenderDto> GetHolidayCalender()
        {
            var data = _holidayCalenderService.GetHolidayCalender();
            var retData = Mapper.Map<List<HolidayCalenderDto>>(data);
            return retData;
        }

        public HolidayCalenderDto GetholidayCalenderById(int MstHolidayDateId, bool? Archive = null)
        {
            if (Archive.HasValue)
            {
                var archData = _archHolidayCalenderService.GetHolidayCalenderById(MstHolidayDateId);
                return Mapper.Map<HolidayCalenderDto>(archData);
            }
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

        public List<HolidayCalenderDto> GetHolidayCalender(HolidayCalenderParamInput filter)
        {
            if(filter.Table == "2")
            {
                var archData = _archHolidayCalenderService.GetHolidayCalender(filter);
                return Mapper.Map<List<HolidayCalenderDto>>(archData);
            }

            var data = _holidayCalenderService.GetHolidayCalender(filter);
            var retData = Mapper.Map<List<HolidayCalenderDto>>(data);
            return retData;
        }
    }
}
