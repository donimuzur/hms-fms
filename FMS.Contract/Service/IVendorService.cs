using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IVendorService
    {
        List<MST_VENDOR> GetVendor();
        MST_VENDOR GetVendorById(int MstVendorId);
        MST_VENDOR GetExist(string NamaVendor);
        void save(MST_VENDOR dbVendor);
        void save(MST_VENDOR dbVendor, Login userLogin);
    }
}
