using System.Collections.Generic;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IPageBLL
    {
        MST_MODUL GetPageByID(int id);
        List<MST_MODUL> GetPages();

        List<MST_MODUL> GetModulePages();

        void Save(MST_MODUL pageMap);

        void DeletePageMap(int id);

        List<MST_MODUL> GetParentPages();

        List<int?> GetAuthPages(Login user);
        List<int?> GetAuthPages(System.String userId);
        List<string> getAuthPagess(Login User);
        MST_MODUL GetPageByName(string pagename);

    }
}
