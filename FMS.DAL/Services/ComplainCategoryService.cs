using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;

namespace FMS.DAL.Services
{
    public class ComplainCategoryService : IComplaintCategoryService
    {
        private IUnitOfWork _uow;
        
        private IGenericRepository<complaint_category> _complainCatRepository;
        private string IncludeTables = "";

        public ComplainCategoryService(IUnitOfWork uow)
        {
            _uow = uow;
            
            _complainCatRepository = _uow.GetGenericRepository<complaint_category>();
        }

        public List<complaint_category> GetComplaintCategories()
        {
            return _complainCatRepository.Get(x => x.is_active.HasValue && x.is_active.Value).ToList();
        }
    }
}
