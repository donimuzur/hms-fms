using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.Service
{
    public interface ICtfService 
    {
        List<TRA_CTF> GetCtf();
        List<TRA_CTF> GetCtfDashboard(Login userLogin, bool isCompleted, string benefitType, string wtcType);
        void Save(TRA_CTF dbCtf, Login userlogin);
        TRA_CTF GetCtfById(long TraCtfId);
        void CancelCtf(long id, int Remark, Login userlogin);
    }
}
