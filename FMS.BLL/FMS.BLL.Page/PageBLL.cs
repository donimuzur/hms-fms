using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.BLL;


namespace FMS.BLL.Page
{
    public class PageBLL : IPageBLL
    {
        //private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<MST_MODUL> _pageRepository;

        public PageBLL(IUnitOfWork uow)
        {
            
            _uow = uow;
            _pageRepository = _uow.GetGenericRepository<MST_MODUL>();
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

        public List<int?> GetAuthPages(Login user)
        {
            return new List<int?>();
            throw new NotImplementedException();
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

        
    }
}
