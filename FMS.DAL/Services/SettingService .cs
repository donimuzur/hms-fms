using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Core;

namespace FMS.DAL.Services
{
    public class SettingService : ISettingService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_SETTING> _settingRepository;

        public SettingService(IUnitOfWork uow)
        {
            _uow = uow;
            _settingRepository = _uow.GetGenericRepository<MST_SETTING>();
        }

        public List<MST_SETTING> GetSetting()
        {
            return _settingRepository.Get().ToList();
        }

        public MST_SETTING GetSettingById(int MstSettingId)
        {
            return _settingRepository.GetByID(MstSettingId);
        }

        public MST_SETTING GetExist(string SettingGroup, string SettingName)
        {
            return _settingRepository.Get(x => x.SETTING_GROUP == SettingGroup && x.SETTING_NAME == SettingName).FirstOrDefault(); ;
        }

        public void save(MST_SETTING dbSetting)
        {
            _uow.GetGenericRepository<MST_SETTING>().InsertOrUpdate(dbSetting);
            _uow.SaveChanges();
        }


        public void save(MST_SETTING dbSetting, Login userlogin)
        {
            _uow.GetGenericRepository<MST_SETTING>().InsertOrUpdate(dbSetting,userlogin,Enums.MenuList.MasterSetting);
            _uow.SaveChanges();
        }
    }
}
