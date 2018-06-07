using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{   
    public class ArchCostObService : IArchCostObService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_COST_OB> _archCostObRepository;

        public ArchCostObService(IUnitOfWork uow)
        {
            _uow = uow;
            _archCostObRepository = _uow.GetGenericRepository<ARCH_MST_COST_OB>();
        }

        public void Save(ARCH_MST_COST_OB db, Login userlogin)
        {
            try
            {
                _archCostObRepository.InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
            }
            catch (Exception exp)
            {

                throw new Exception(exp.Message);
            }
        }
    }
}
