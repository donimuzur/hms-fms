using System;
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

namespace FMS.BLL.Page
{
    public class PageBLL : IPageBLL
    {
        //private ILogger _logger;
        private IUnitOfWork _uow;
        private IRoleService _roleService;
        private IModulService _modulService;

        public PageBLL(IUnitOfWork uow)
        {
            
            _uow = uow;
            _roleService = new RoleService(uow);
            _modulService = new ModulService(uow);
        }
        
        public MST_MODUL GetPageByName(string pagename)
        {
            return _modulService.GetModul().Where(x => x.MODUL_NAME == pagename).FirstOrDefault();
        }
        public MST_SYSACCESS GetPageByID(int id)
        {
            return _roleService.GetRoleById(id);
        }

        public List<MST_SYSACCESS> GetPages()
        {
            return _roleService.GetRoles();
        }

        public List<MST_SYSACCESS> GetModulePages()
        {
            throw new NotImplementedException();
        }

        public void DeletePageMap(int id)
        {
            throw new NotImplementedException();
        }

        public List<MST_SYSACCESS> GetParentPages()
        {
            throw new NotImplementedException();
        }

        public List<int?> GetAuthPages(Login user)
        {

            return new List<int?>();
            throw new NotImplementedException();
        }
        public List<string> getAuthPagess(Login User)
        {
            var pages = new List<string>();
            pages = _roleService.GetRoles().Where(x => x.ROLE_NAME_ALIAS == User.UserRole.ToString()).Select(x => x.MODUL_NAME).ToList();
            return pages;
        }
        public void Save(MST_SYSACCESS pageMap)
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

        
    }
}
