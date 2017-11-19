using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;
using FMS.Core.Exceptions;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.DAL.Services;
using FMS.Utils;
using AutoMapper;
using System.Data.SqlClient;

namespace FMS.BLL.Temporary
{
    public class TemporaryBLL : ITraTemporaryBLL
    {
        private ITemporaryService _TemporaryService;
        private IUnitOfWork _uow;

        private ISettingService _settingService;

        public TemporaryBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _TemporaryService = new TemporaryService(_uow);

            _settingService = new SettingService(_uow);
        }

        public List<TemporaryDto> GetTemporary(Login userLogin, bool isCompleted)
        {
            var settingData = _settingService.GetSetting().Where(x => x.SETTING_GROUP == EnumHelper.GetDescription(Enums.SettingGroup.VehicleType));
            var benefitType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "BENEFIT").FirstOrDefault().MST_SETTING_ID.ToString();
            var wtcType = settingData.Where(x => x.SETTING_NAME.ToUpper() == "WTC").FirstOrDefault().MST_SETTING_ID.ToString();

            var data = _TemporaryService.GetTemp(userLogin, isCompleted, benefitType, wtcType);
            var retData = Mapper.Map<List<TemporaryDto>>(data);
            return retData;
        }

        public List<TemporaryDto> GetTempPersonal(Login userLogin)
        {
            var data = _TemporaryService.GetAllTemp().Where(x => (x.EMPLOYEE_ID == userLogin.EMPLOYEE_ID && x.DOCUMENT_STATUS != Enums.DocumentStatus.Draft)
                                                                || x.CREATED_BY == userLogin.USER_ID).ToList();
            var retData = Mapper.Map<List<TemporaryDto>>(data);
            return retData;
        }
    }
}
