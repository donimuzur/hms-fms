using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ISettingService
    {
        List<MST_SETTING> GetSetting();
        MST_SETTING GetSettingById(int MstSettingId);
        MST_SETTING GetExist(string SettingGroup);
        void save(MST_SETTING dbSetting);
    }
}
