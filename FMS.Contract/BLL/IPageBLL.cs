using System.Collections.Generic;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IPageBLL
    {
        MST_SYSACCESS GetPageByID(int id);
        List<MST_SYSACCESS> GetPages();

        List<MST_SYSACCESS> GetModulePages();

        void Save(MST_SYSACCESS pageMap);

        void DeletePageMap(int id);

        List<MST_SYSACCESS> GetParentPages();

        List<int?> GetAuthPages(Login user);
        List<int?> GetAuthPages(System.String userId);
        List<string> getAuthPagess(Login User);
        MST_MODUL GetPageByName(string pagename);

    }
}
