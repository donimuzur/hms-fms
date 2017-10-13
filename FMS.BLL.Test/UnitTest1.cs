using System;
using FMS.BLL.User;
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
            
            Assert.AreEqual(true,success);
            
        }
    }
}
