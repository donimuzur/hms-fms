using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;

namespace FMS.Contract.BLL
{
    public interface ISalesVolumeBLL
    {
        List<SalesVolumeDto> GetSalesVolume();
        SalesVolumeDto GetSalesVolumeById(int MstSalesVolumeId);
        void Save(SalesVolumeDto SalesVolumeDto);
    }
}
