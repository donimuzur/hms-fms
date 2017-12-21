using FMS.BusinessObject;
using FMS.BusinessObject.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ISalesVolumeService
    {
        List<MST_SALES_VOLUME> GetSalesVolume(SalesVolumeParamInput inputs);
        List<MST_SALES_VOLUME> GetAllSalesVolume();
        void save(MST_SALES_VOLUME dbSalesVolume);
        void CheckSalesVolume(String Type, String Region, int Month, int Year, String User);
        MST_SALES_VOLUME GetSalesVolumeById(int mstSalesVolumeId);


    }
}
