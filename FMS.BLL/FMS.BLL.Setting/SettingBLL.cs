using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;

namespace FMS.BLL.Setting
{

    public class SettingBLL : ISettingBLL
    {
        //private ILogger _logger;
        private ISettingService _SettingService;
        private IUnitOfWork _uow;
        public SettingBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _SettingService = new SettingService(uow);
        }

        public List<SettingDto> GetSetting()
        {
            var data = _SettingService.GetSetting();
            var retData = Mapper.Map<List<SettingDto>>(data);
            return retData;
        }

        public SettingDto GetExist(string SettingGroup, string SettingName)
        {
            var data = _SettingService.GetExist(SettingGroup, SettingName);
            var retData = Mapper.Map<SettingDto>(data);

            return retData;
        }
        public void Save(SettingDto SettingDto)
        {
            var dbSetting = Mapper.Map<MST_SETTING>(SettingDto);
            _SettingService.save(dbSetting);
        }

        public void Save(SettingDto SettingDto, Login userLogin)
        {
            var dbSetting = Mapper.Map<MST_SETTING>(SettingDto);
            _SettingService.save(dbSetting,userLogin);
        }

        public SettingDto GetByID(int Id)
        {
            var data = _SettingService.GetSettingById(Id);
            var retData = Mapper.Map<SettingDto>(data);

            return retData;
        }


    }
}

