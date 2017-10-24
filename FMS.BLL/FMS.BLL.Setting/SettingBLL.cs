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

        public SettingDto GetExist(string FunctionGroup)
        {
            var data = _SettingService.GetExist(FunctionGroup);
            var retData = Mapper.Map<SettingDto>(data);

            return retData;
        }
        public void Save(SettingDto SettingDto)
        {
            var dbSetting = Mapper.Map<MST_SETTING>(SettingDto);
            _SettingService.save(dbSetting);
        }

        public SettingDto GetByID(int Id)
        {
            var data = _SettingService.GetSettingById(Id);
            var retData = Mapper.Map<SettingDto>(data);

            return retData;
        }


    }
}

