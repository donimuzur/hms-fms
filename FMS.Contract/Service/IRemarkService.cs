using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;

namespace FMS.Contract.Service
{
    public interface IRemarkService
    {
        List<MST_REMARK> GetRemark();

        List<MST_REMARK> GetRemarkByDoc(int MstDocId);
        MST_REMARK GetRemarkById(int MstRemarkId);
        void save(MST_REMARK dbRemark);
        void save(MST_REMARK dbVRemark, Login userLogin);
    }
}
