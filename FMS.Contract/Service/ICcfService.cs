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
        void Save(TRA_CCF dbCcf, Login userlogin);
        TRA_CCF GetCcfById(long TraCcfId);
        void CancelCcf(long id, int Remark, string user);
    }
}
