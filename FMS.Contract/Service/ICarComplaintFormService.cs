using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;

namespace FMS.Contract.Service
{
    public interface ICarComplaintFormService
    {
        List<TRA_CCF> GetCCF();
        List<TRA_CCF_DETAIL> GetCCFD1();
        TRA_CCF GetCCFById(int Id);
        void save(TRA_CCF dbTraCCF, TRA_CCF_DETAIL dbTraCCFD1);
    }
}
