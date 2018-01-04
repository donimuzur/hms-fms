﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;
using FMS.BusinessObject.Business;
using FMS.Core;

namespace FMS.DAL.Services
{
    public class ComplainCategoryService : IComplaintCategoryService
    {
        private IUnitOfWork _uow;
        
        private IGenericRepository<MST_COMPLAINT_CATEGORY> _complainCatRepository;
        //private string IncludeTables = "";

        public ComplainCategoryService(IUnitOfWork uow)
        {
            _uow = uow;
            
            _complainCatRepository = _uow.GetGenericRepository<MST_COMPLAINT_CATEGORY>();
        }

        public List<MST_COMPLAINT_CATEGORY> GetComplaintCategories()
        {
            //return _complainCatRepository.Get(x => x.IS_ACTIVE == true).ToList();
            return _complainCatRepository.Get().ToList();
        }

        public MST_COMPLAINT_CATEGORY GetComplaintById(int MstComplaintID)
        {
            return _complainCatRepository.GetByID(MstComplaintID);
        }

        public void save(MST_COMPLAINT_CATEGORY dbComplaint)
        {
            _uow.GetGenericRepository<MST_COMPLAINT_CATEGORY>().InsertOrUpdate(dbComplaint);
        }

        public void save(MST_COMPLAINT_CATEGORY dbComplaint, Login userLogin)
        {
            _uow.GetGenericRepository<MST_COMPLAINT_CATEGORY>().InsertOrUpdate(dbComplaint, userLogin, Enums.MenuList.MasterComplaintCategory);
        }
    }
}
