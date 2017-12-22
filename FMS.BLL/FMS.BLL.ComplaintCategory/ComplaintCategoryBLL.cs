using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;
using FMS.BusinessObject.Business;

namespace FMS.BLL.ComplaintCategory
{
    public class ComplaintCategoryBLL : IComplaintCategoryBLL
    {
        //private ILogger _logger;
        private IComplaintCategoryService _complaint;
        private IUnitOfWork _uow;

        public ComplaintCategoryBLL(IUnitOfWork uow)
        {
            //_logger = logger;
            _uow = uow;
            _complaint = new ComplainCategoryService(uow);
        }

        public List<ComplaintDto> GetComplaints()
        {
            var data = _complaint.GetComplaintCategories();
            var retData = Mapper.Map<List<ComplaintDto>>(data);
            return retData;
        }

        public void Save(ComplaintDto ComplaintDto)
        {
            var dbComplaint = Mapper.Map<MST_COMPLAINT_CATEGORY>(ComplaintDto);
            _complaint.save(dbComplaint);
        }

        public void Save(ComplaintDto ComplaintDto, Login userLogin)
        {
            var dbComplaint = Mapper.Map<MST_COMPLAINT_CATEGORY>(ComplaintDto);
            _complaint.save(dbComplaint, userLogin);
        }

        public ComplaintDto GetByID(int Id)
        {
            var data = _complaint.GetComplaintById(Id);
            var retData = Mapper.Map<ComplaintDto>(data);

            return retData;
        }

        public void SaveChanges()
        {
            _uow.SaveChanges();
        }
    }

    
}
