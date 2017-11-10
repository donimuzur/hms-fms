using System;
using FMS.BLL.Mapper;
using FMS.BLL.User;
using FMS.BLL.Setting;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace FMS.BLL.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ILogger logger = LogManager.GetCurrentClassLogger();
            IUnitOfWork uow = new SqlUnitOfWork();

            var success = false;

            try
            {
                IUserBLL userBll = new UserBLL(uow, logger);
                userBll.GetAllUsers();

                success = true;
            }
            catch (Exception)
            {
                success = false;



            }

            Assert.AreEqual(true, success);
            
        

            
        }
        [TestMethod]
        public void TestChangesHistory()
        {
            ILogger logger = LogManager.GetCurrentClassLogger();
            IUnitOfWork uow = new SqlUnitOfWork();

            var success = false;
            SettingMapper.Initialize();
            try
            {
                ISettingBLL settingBLL = new SettingBLL(uow);
                settingBLL.Save(new SettingDto()
                {
                    SettingGroup = "TestGroup",
                    SettingName = "TestName",
                    SettingValue = "TestValue",
                    CreatedBy = "TestUser",
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    ModifiedBy = "TestUser",
                    ModifiedDate = DateTime.Now
                },new Login()
                {
                    FIRST_NAME = "Test",
                    LAST_NAME = "User",
                    USER_ID = "TestUser",
                    USERNAME = "TestUser"
                });

                success = true;
            }
            catch (Exception ex)
            {
                success = false;



            }

            Assert.AreEqual(true, success);
        }
    }
}
