using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface IArchSalesVolumeService
    {
        void Save(ARCH_MST_SALES_VOLUME db, Login userlogin);
        List<ARCH_MST_SALES_VOLUME> GetSalesVolume(SalesVolumeParamInput filter);
        List<ARCH_MST_SALES_VOLUME> GetAllSalesVolume();
        ARCH_MST_SALES_VOLUME GetSalesVolumeById(int MstSalesVolumeId);

    }
}
