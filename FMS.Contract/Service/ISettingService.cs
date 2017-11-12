using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface ISettingService
    {
        List<MST_SETTING> GetSetting();
        MST_SETTING GetSettingById(int MstSettingId);
        MST_SETTING GetExist(string SettingGroup, string SettingName);
        void save(MST_SETTING dbSetting);

        void save(MST_SETTING dbSetting, Login userlogin);
    }
}
