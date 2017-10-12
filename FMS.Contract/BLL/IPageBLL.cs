using System.Collections.Generic;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;

namespace FMS.Contract.BLL
{
    public interface IPageBLL
    {
        SysMenu GetPageByID(int id);
        List<SysMenu> GetPages();

        List<SysMenu> GetModulePages();

        void Save(SysMenuAccess pageMap);

        void DeletePageMap(int id);

        List<SysMenu> GetParentPages();

        List<int?> GetAuthPages(Login user);
        List<int?> GetAuthPages(System.String userId);


    }
}
