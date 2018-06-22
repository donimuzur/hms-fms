using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.Service
{
    public interface IArchPricelistService
    {
        void Save(ARCH_MST_PRICELIST db, Login userlogin);
        List<ARCH_MST_PRICELIST> GetPriceList(PricelistParamInput filter);
        ARCH_MST_PRICELIST GetPriceListById(int id);
    }
}
