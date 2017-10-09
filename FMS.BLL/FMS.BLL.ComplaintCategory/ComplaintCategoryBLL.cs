using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using NLog;


namespace FMS.BLL.ComplaintCategory
{
    public class ComplaintCategoryBLL : IComplaintCategoryBLL
    {
        private ILogger _logger;
        private IComplaintCategoryService _complainService;


        public ComplaintCategoryBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _complainService = new ComplainCategoryService(uow, _logger);
            
        }

        public List<ComplaintDto> GetComplaints()
        {
            var data = _complainService.GetComplaintCategories();

            return null;
        }
    }

    
}
