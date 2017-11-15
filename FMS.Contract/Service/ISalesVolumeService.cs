using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ISalesVolumeService
    {
        List<MST_SALES_VOLUME> GetSalesVolume();
        void save(MST_SALES_VOLUME dbSalesVolume);
        MST_SALES_VOLUME GetSalesVolumeById(int mstSalesVolumeId);
    }
}
