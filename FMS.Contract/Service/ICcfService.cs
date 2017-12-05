using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICcfService
    {
        List<TRA_CCF> GetCcf();
        List<TRA_CCF_DETAIL> GetCcfD1();
        void Save(TRA_CCF dbCcf, Login userlogin);
        void Save_d1(TRA_CCF_DETAIL dbCcfd1);

        void SaveDetails(TRA_CCF_DETAIL details, Login currentUser);
        TRA_CCF GetCcfById(long TraCcfId);
        void CancelCcf(long id, int Remark, string user);
    }
}
