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
        TRA_CCF GetCCFById(int Id);
        void save(TRA_CCF dbTraCCF);
    }
}
