using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class GroupCostCenterService : IGroupCostCenterService
    {
        public IGenericRepository<MST_FUNCTION_GROUP> _GroupCostCenterRepository;
        public IUnitOfWork _uow;

        public GroupCostCenterService (IUnitOfWork uow)
        {
            _uow = uow;
            _GroupCostCenterRepository = _uow.GetGenericRepository<MST_FUNCTION_GROUP>();
        }

        public List<MST_FUNCTION_GROUP> GetGroupCostCenter()
        {
            return _GroupCostCenterRepository.Get().ToList();
        }

        public void Save(MST_FUNCTION_GROUP dbGroupCostCenter)
        {
            _GroupCostCenterRepository.InsertOrUpdate(dbGroupCostCenter);
            _uow.SaveChanges();
        }
    }
}
