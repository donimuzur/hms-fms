using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface ISettingBLL
    {
        List<SettingDto> GetSetting();
        SettingDto GetExist(string FunctionGroup);
        SettingDto GetByID(int Id);
        void Save(SettingDto SettingDto);
        void Save(SettingDto SettingDto,Login userLogin);
    }
}
