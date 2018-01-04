﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using FMS.BusinessObject.Dto;
using AutoMapper;

namespace FMS.BLL.Page
{
    public class PageBLL : IPageBLL
    {
        //private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MST_MODUL> _pageRepository;
        private IGenericRepository<TRA_CHANGES_HISTORY> _changesRepository;
        private IGenericRepository<TRA_WORKFLOW_HISTORY> _workflowRepository;
        private IGenericRepository<MST_REMARK> _remarkRepository;
        private IRoleService _roleService;

        public PageBLL(IUnitOfWork uow)
        {

            _uow = uow;
            _pageRepository = _uow.GetGenericRepository<MST_MODUL>();
            _changesRepository = _uow.GetGenericRepository<TRA_CHANGES_HISTORY>();
            _workflowRepository = _uow.GetGenericRepository<TRA_WORKFLOW_HISTORY>();
            _roleService = new RoleService(_uow);
            _remarkRepository = _uow.GetGenericRepository<MST_REMARK>();
        }

        public MST_MODUL GetPageByModulName(string ModulName)
        {
            return _pageRepository.Get().Where(x => x.MODUL_NAME ==ModulName).FirstOrDefault();
        }

        public MST_MODUL GetPageByID(int id)
        {
            return _pageRepository.GetByID(id); 
        }

        public List<MST_MODUL> GetPages()
        {
            return _pageRepository.Get().ToList();
        }

        public List<MST_MODUL> GetModulePages()
        {
            throw new NotImplementedException();
        }

        public void DeletePageMap(int id)
        {
            throw new NotImplementedException();
        }

        public List<MST_MODUL> GetParentPages()
        {
            throw new NotImplementedException();
        }

        public List<RoleDto> GetAuthPages(Login user)
        {
            var data = _roleService.GetRoles().Where(x => x.ROLE_NAME_ALIAS == user.UserRole.ToString()).ToList();
            var redata = Mapper.Map< List<RoleDto>>(data);
            var pages = redata.ToList();
            return pages;
        }

        public List<ChangesHistoryDto> GetChangesHistory(int modulId, long formId)
        {
            var data = _changesRepository.Get(x => x.MODUL_ID == modulId && x.FORM_ID == formId).ToList();
            return Mapper.Map<List<ChangesHistoryDto>>(data);
        }

        public List<WorkflowHistoryDto> GetWorkflowHistory(int modulId, long formId)
        {
            var data = _workflowRepository.Get(x => x.MODUL_ID == modulId && x.FORM_ID == formId).ToList();
            
            foreach (var traWorkflowHistory in data)
            {
                
            }
            return Mapper.Map<List<WorkflowHistoryDto>>(data);
        }

        public void Save(MST_MODUL pageMap)
        {
            throw new NotImplementedException();
        }

        public List<int?> GetAuthPages(string userid)
        {
            //var pages = new List<int?>();
            //var broleMaps = _repositoryRoleMap.Get(x => x.MSACCT == userid);
            //foreach (var broleMap in broleMaps)
            //{
            //    var brole = GetById(broleMap.BROLE);
            //    foreach (var page in brole.PageMaps)
            //    {
            //        pages.Add(page.Page.Id);
            //    }
            //}

            //var refService = new CustomService.Services.SystemReferenceService();
            //if (refService.IsAdminApprover(userid))
            //{
            //    var additionalPages = refService.GetAuthorizedPages(userid).ToList();
            //    foreach (var page in additionalPages)
            //    {
            //        if (pages.IndexOf(page.PAGE_ID) <= 0)
            //            pages.Add(page.PAGE_ID);
            //    }
            //}
            //return pages;
            return new List<int?>();
        }




        public List<RemarkDto> GetAllRemark()
        {
            var data = _remarkRepository.Get(x => x.IS_ACTIVE).ToList();
            return Mapper.Map<List<RemarkDto>>(data);
        }
    }
}
