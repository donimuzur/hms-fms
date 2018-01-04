using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IPriceListService
    {
        List<MST_PRICELIST> GetPriceList();
        MST_PRICELIST GetPriceListById(int MstPriceListId);
        MST_PRICELIST GetExist(string Model);
        void save(MST_PRICELIST dbPriceList);
        void save(MST_PRICELIST dbPriceList, Login userLogin);
    }
}
