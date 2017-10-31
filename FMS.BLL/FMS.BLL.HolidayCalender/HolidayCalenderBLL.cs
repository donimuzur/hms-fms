using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.HolidayCalender
{
    public class HolidayCalenderBLL :IHolidayCalenderBLL
    {
        private IHolidayCalenderService _HolidayCalenderService;
        private IUnitOfWork _uow;
        public HolidayCalenderBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _HolidayCalenderService = new HolidayCalenderService(uow);
        }

        public List<HolidayCalenderDto> GetHolidayCalender()
        {
            var data = _HolidayCalenderService.GetHolidayCalender();
            var retData = Mapper.Map<List<HolidayCalenderDto>>(data);
            return retData;
        }

        public HolidayCalenderDto GetholidayCalenderById(DateTime MstHolidayDate)
        {
            var data = _HolidayCalenderService.GetHolidayCalenderById(MstHolidayDate);
            var retData = Mapper.Map<HolidayCalenderDto>(data);

            return retData;
        }

        public void Save(HolidayCalenderDto HolidayCalenderDto)
        {
            var dbHolidayCalender = Mapper.Map<MST_HOLIDAY_CALENDAR>(HolidayCalenderDto);
            _HolidayCalenderService.save(dbHolidayCalender);
        }
    }
}
