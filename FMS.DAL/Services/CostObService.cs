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
    public class CostObService : ICostObService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_COST_OB> _costObRepository;

        public CostObService(IUnitOfWork uow)
        {
            _uow = uow;
            _costObRepository = _uow.GetGenericRepository<MST_COST_OB>();
        }

        public List<MST_COST_OB> GetCostOb()
        {
            return _costObRepository.Get().ToList();
        }

        public MST_COST_OB GetCostObById(int MstCostObId)
        {
            return _costObRepository.GetByID(MstCostObId);
        }

        public MST_COST_OB GetExist(string Model)
        {
            return _costObRepository.Get(x => x.MODEL == Model).FirstOrDefault(); ;
        }

        public void save(MST_COST_OB dbCostOb)
        {
            _uow.GetGenericRepository<MST_COST_OB>().InsertOrUpdate(dbCostOb);
            _uow.SaveChanges();
        }

        public void save(MST_COST_OB dbCostOb, Login userlogin)
        {
            _uow.GetGenericRepository<MST_COST_OB>().InsertOrUpdate(dbCostOb, userlogin, Enums.MenuList.MasterCostOB);
            _uow.SaveChanges();
        }

    }
}
